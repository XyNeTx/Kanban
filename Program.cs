using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Automapper.MapProfile;
using KANBAN.Services.Automapper.Repository;
using KANBAN.Services.CKD_Ordering.IRepository;
using KANBAN.Services.CKD_Ordering.Repository;
using KANBAN.Services.Import.Interface;
using KANBAN.Services.Import.Repository;
using KANBAN.Services.Logistical.Interface;
using KANBAN.Services.Logistical.Repository;
using KANBAN.Services.Master.IRepository;
using KANBAN.Services.Master.Repository;
using KANBAN.Services.OtherCondition.IRepository;
using KANBAN.Services.OtherCondition.Repository;
using KANBAN.Services.SpecialOrdering.Interface;
using KANBAN.Services.SpecialOrdering.Repository;
using KANBAN.Services.UrgentOrder.IRepository;
using KANBAN.Services.UrgentOrder.Repository;
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

builder.Services.AddDbContext<CKDWH_Context>();

builder.Services.AddDbContext<CKDUSA_Context>(opt => 
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("CKDUSAConnection") ??
        throw new InvalidOperationException("Connection string 'CKDWHContext' not found.")
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
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();

//Add DbConnect
builder.Services.AddScoped<ERPConnection>();
builder.Services.AddScoped<KanbanConnection>();
builder.Services.AddScoped<CloudConnection>();
builder.Services.AddScoped<ProcWebConnection>();
builder.Services.AddScoped<PPM3Connection>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<cnConnect>();
builder.Services.AddScoped<DbConnect>();
builder.Services.AddScoped<WarrantyClaimConnect>();
builder.Services.AddScoped<PPMConnect>();


//Authenity Guard
builder.Services.AddScoped<AuthenGuard>();

//Library Class
builder.Services.AddScoped<BearerClass>();
builder.Services.AddScoped<EmailClass>();
builder.Services.AddSingleton<NPOIClass>();
builder.Services.AddScoped<ActionResultClass>();
builder.Services.AddScoped<FillDataTable>();
builder.Services.AddScoped<SerilogLibs>();
builder.Services.AddScoped<TextFileClass>();
builder.Services.AddScoped<KBNOR310>();

builder.Services.AddScoped<ISpecialLibs, SpecialLibs>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<ILogisticService, LogisticService>();
builder.Services.AddScoped<ISpecialOrderingServices, SpecialOrderingServices>();
builder.Services.AddScoped<IMasterRepo, MasterRepo>();
builder.Services.AddScoped<ICKDService, CKDService>();
builder.Services.AddScoped<IOtherConditionRepo, OtherConditionRepo>();
builder.Services.AddScoped<IUrgentRepo,UrgentRepo>();
builder.Services.AddHttpClient();

builder.Services.AddAutoMapper
    (
        typeof(PDS_Header_To_REC_Header_Profile),
        typeof(PDS_Detail_To_Rec_Detail_Profile),
        typeof(TransactionTMP_To_Transaction_Profile)
    );

builder.Services.AddTransient(typeof(IAutoMapRepo<,>), typeof(AutoMapRepo<,>));
builder.Services.AddTransient<IAutoMapService, AutoMapService>();


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
    options.IdleTimeout = TimeSpan.FromHours(12);
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
app.UseStatusCodePagesWithRedirects("~/Home/Index");

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
