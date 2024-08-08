using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<KB3Context>();

builder.Services.AddDbContext<ERPContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string 'ERPContext' not found.")
        )
);
builder.Services.AddDbContext<PPM3Context>();

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

//Add DbConnect
builder.Services.AddScoped<DefaultConnection>();
builder.Services.AddScoped<ERPConnection>();
builder.Services.AddScoped<KanbanConnection>();
builder.Services.AddSingleton<CloudConnection>();
builder.Services.AddSingleton<ProcWebConnection>();
builder.Services.AddSingleton<PPM3Connection>();

builder.Services.AddScoped<cnConnect>();
builder.Services.AddScoped<DbConnect>();
builder.Services.AddScoped<WarrantyClaimConnect>();
builder.Services.AddScoped<HRConnect>();
builder.Services.AddScoped<PPMConnect>();


//Authenity Guard
builder.Services.AddScoped<AuthenGuard>();

//Library Class
builder.Services.AddScoped<BearerClass>();
builder.Services.AddScoped<EmailClass>();
builder.Services.AddSingleton<NPOIClass>();
builder.Services.AddSingleton<PdfSharpClass>();
builder.Services.AddScoped<CookieClass>();
builder.Services.AddScoped<ActionResultClass>();
builder.Services.AddScoped<FillDataTable>();
builder.Services.AddScoped<SerilogLibs>();
builder.Services.AddScoped<TextFileClass>();




builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Operation";
    options.IdleTimeout = TimeSpan.FromSeconds(1800);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//var isDevelop = builder.Environment.IsDevelopment();
//if (isDevelop)
//{
//    builder.Services.AddSession(builder =>
//    {
//        builder.Cookie.Name = "IsDevelop";
//        builder.IdleTimeout = TimeSpan.FromMinutes(30);
//        builder.Cookie.HttpOnly = true;
//        builder.Cookie.IsEssential = true;

//    });
//}

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

//app.MapControllerRoute(
//    name: "action",
//    pattern: "action/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller=Home}/{action=Index}");

app.Run();
