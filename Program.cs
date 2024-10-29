using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services;
using KANBAN.Services.Logistical;
using KANBAN.Services.SpecialOrdering;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();

//Add DbConnect
builder.Services.AddScoped<DefaultConnection>();
builder.Services.AddScoped<ERPConnection>();
builder.Services.AddScoped<KanbanConnection>();
builder.Services.AddScoped<CloudConnection>();
builder.Services.AddScoped<ProcWebConnection>();
builder.Services.AddScoped<PPM3Connection>();
builder.Services.AddScoped<IEmailService, EmailService>();

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

builder.Services.AddScoped<ISpecialLibs, SpecialLibs>();
builder.Services.AddScoped<IImportService ,ImportService>();
builder.Services.AddScoped<ILogisticService, LogisticService>();
builder.Services.AddScoped<ISpecialOrderingServices, SpecialOrderingServices>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API Test Service",
        Description = "A Swagger Web Service for Testing API Endpoints in .NET 7",
    });

});


builder.Services.AddSession(options =>
{
    //options.Cookie.Name = "Operation";
    options.IdleTimeout = TimeSpan.FromDays(2);
    //options.IOTimeout = Timeout.InfiniteTimeSpan;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    //options.Cookie.MaxAge = TimeSpan.FromHours(36);
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve Swagger UI, specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Test Service V1");
        c.RoutePrefix = "swagger";
        //c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root (http://localhost:<port>/)
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<CustomExceptionMiddleware>();

app.UseSession(); 
app.UseRouting();

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
