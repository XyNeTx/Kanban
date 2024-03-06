using System;
using Microsoft.EntityFrameworkCore;
using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.ERP;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.Authentication.Negotiate;
using KANBAN.Context;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<KB3Context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string 'KB3Context' not found.")
        )
);

builder.Services.AddDbContext<ERPContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string 'ERPContext' not found.")
        )
);
builder.Services.AddDbContext<PPM3Context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PPM3Connection") ??
        throw new InvalidOperationException("Connection String PPM3Context not found.")
        )
);
builder.Services.AddDbContext<PPMInvenContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PPMInvenConnection") ??
        throw new InvalidOperationException("Connection String PPM3Context not found.")
        )
);
builder.Services.AddDbContext<ProcDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ProcDBConnection") ??
        throw new InvalidOperationException("Connection String ProcDBContext not found.")
        )
);

//Add Support to logging with SERILOG
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        // Allow requests from any origin, method, and header
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

//builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
//   .AddNegotiate();


//builder.Services.AddRazorPages();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

//Add DbConnect
builder.Services.AddSingleton<DefaultConnection>();
builder.Services.AddSingleton<ERPConnection>();
builder.Services.AddSingleton<KanbanConnection>();
builder.Services.AddSingleton<CloudConnection>();
builder.Services.AddSingleton<ProcWebConnection>();

builder.Services.AddSingleton<cnConnect>();
builder.Services.AddSingleton<DbConnect>();
builder.Services.AddSingleton<WarrantyClaimConnect>();
builder.Services.AddSingleton<HRConnect>();
builder.Services.AddSingleton<PPMConnect>();


//Authenity Guard
builder.Services.AddSingleton<AuthenGuard>();

//Library Class
builder.Services.AddSingleton<BearerClass>();
builder.Services.AddSingleton<EmailClass>();
builder.Services.AddSingleton<NPOIClass>();
builder.Services.AddSingleton<PdfSharpClass>();
builder.Services.AddSingleton<CookieClass>();
builder.Services.AddSingleton<ActionResultClass>();
builder.Services.AddScoped<FillDataTable>();
builder.Services.AddScoped<SerilogLibs>();


builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Operation";
    options.IdleTimeout = TimeSpan.FromSeconds(600);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors();

app.UseWebSockets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "KBN",
    pattern: "KBN/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller=Home}/{action=Index}");

app.Run();
