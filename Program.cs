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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;

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
builder.Services.AddDbContext<ProcWebContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ProcWebConnection") ??
        throw new InvalidOperationException("Connection String ProcDBContext not found.")
        )
);


//Add Support to logging with SERILOG
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen(c=>
{

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,      // Use Http scheme for bearer
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste your JWT token here **without** the 'Bearer ' prefix."
    });

});

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
builder.Services.AddScoped<IKBNOR320, KBNOR320>();
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

List<string> menuLishPolicy = new List<string> {"KBNMS000","KBNMS001","KBNMS002","KBNMS003","KBNMS004","KBNMS005","KBNMS005S"
,"KBNMS006","KBNMS007","KBNMS008","KBNMS009","KBNMS010"
,"KBNMS011","KBNMS012","KBNMS013","KBNMS014","KBNMS015","KBNMS016","KBNMS017","KBNMS018","KBNMS019","KBNMS020"
,"KBNMS021","KBNMS022","KBNMS023","KBNMS024","KBNMS025","KBNMS026","KBNMS027","KBNMS028","KBNMS029","KBNIM000","KBNIM000S"
,"KBNIM001","KBNIM001M","KBNIM001C","KBNIM001O","KBNIM000V","KBNIM002","KBNIM002M","KBNIM002C","KBNIM002TR","KBNIM000ME"
,"KBNIM000EK","KBNIM003","KBNIM003M","KBNIM003C","KBNIM003CP","KBNIM003S","KBNIM000VL","KBNIM004","KBNIM004M","KBNIM0043"
,"KBNIM0042","KBNIM000EM","KBNIM006","KBNIM006M","KBNIM006C","KBNIM000MF","KBNIM012","KBNIM012M","KBNIM000U","KBNIM007N"
,"KBNIM014","KBNIM014C","KBNIM000SP","KBNIM007","KBNIM007T","KBNIM007C","KBNIM007TSR","KBNIM000T","KBNIM015","KBNIM015M"
,"KBNIM015C","KBNOR000","KBNOR100","KBNOR200","KBNOR400","KBNOR300","KBNOR600","KBNOR700","KBNOR710","KBNRC100","KBNRC100ST"
,"KBNRC110","KBNRC120","KBNRC210","KBNRC100SR","KBNRC130","KBNRC140","KBNRC150","KBNRC160","KBNRC220","KBNOC100","KBNOC110"
,"KBNOC120","KBNOC121","KBNOC140","KBNOC150","KBNOC160","KBNLC100","KBNLC110","KBNLC120","KBNLC130","KBNLC140","KBNLC150","KBNLC180"
,"KBNLC190","KBNLC200","KBNRT100","KBNRT110","KBNRT120","KBNRT130","KBNRT140","KBNRT150","KBNRT160","KBNRT170","KBNRT180"
,"KBNRT190","KBNRT200","KBNRT210","KBNRT220","KBNRT230","KBNRT240","KBNRT250","KBNRT026","KBNRT270"
,"KBNRT280","KBNRT296","KBNRT300","KBNRT310","KBNMS000SER","KBNMS000SP","KBNMS000M","KBNMS000LS","KBNOR110","KBNOR120"
,"KBNOR121","KBNTL000","KBNTL001","KBNOR410","KBNOR420","KBNOR440","KBNOR450","KBNOR460","KBNOR460EX"
,"KBNOR470","KBNOR310","KBNOR320","KBNOR321","KBNOR330","KBNOR360","KBNOR370","KBNDL000","KBNDL000D","KBNDL001"
,"KBNOR610","KBNOR620","KBNOR630","KBNOR640","KBNOR122","KBNOR123","KBNOR130","KBNOR140","KBNOR150","KBNOR150EX"
,"KBNOR160","KBNIM014SRV","KBNIM010","KBNOR210","KBNOR210_2","KBNOR210_3","KBNOR210_1","KBNMT100","KBNMT110","KBNOR293"
,"KBNOR294","KBNOR295","KBNOR220","KBNOR220_1","KBNOR220_2","KBNOR230","KBNOR240","KBNOR290","KBNOR250","KBNOR297","KBNOR280"
,"KBNOR260","KBNOR261","KBNOR270","KBNOR296","KBNIM0044","KBNMS030","KBNOR291","KBNOR292","KBNOR361","KBNIM017R"
,"KBNIM013_INV"}; // Menu List for Policy


builder.Services.AddAuthorization(options =>
{
    foreach (var menu in menuLishPolicy)
    {
        options.AddPolicy(menu, policy => policy.RequireClaim("Menu", menu));
    }
});


//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Home/Index";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        //options.SlidingExpiration = true;
    })
    .AddJwtBearer(options =>
    {
        var keyBytes = Convert.FromBase64String(builder.Configuration["ApplicationSettings:JWT_Secret"] ?? throw new InvalidOperationException("JWT Secret not found."));
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };

        //options.Events = new JwtBearerEvents
        //{
        //    OnMessageReceived = context =>
        //    {
        //        // Read from cookie
        //        if (context.Request.Cookies.TryGetValue("AccessToken", out var token))
        //        {
        //            context.Token = token;
        //        }

        //        // (Optional) also support query param for specific endpoints
        //        // if (string.IsNullOrEmpty(context.Token) &&
        //        //     context.Request.Query.TryGetValue("access_token", out var q))
        //        // {
        //        //     context.Token = q;
        //        // }

        //        return Task.CompletedTask;
        //    }
        //};

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
//app.UseStatusCodePagesWithRedirects("~/Home/Index");
app.UseStatusCodePagesWithRedirects("~/Home/Index");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "KBN",
    pattern: "KBN/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller=Home}/{action=Index}");

app.Run();
