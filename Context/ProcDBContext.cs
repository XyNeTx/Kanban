using HINOSystem.Context;
using KANBAN.Models.Proc_DB;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Context
{
    public class ProcDBContext : DbContext
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private static string plantDev = "";

        public ProcDBContext(DbContextOptions<ProcDBContext> options
            ,IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(options) 
        {
            _httpContextAccessor = httpContextAccessor;
            _config = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && _httpContextAccessor.HttpContext != null)
            {
                var plantCookie = _httpContextAccessor.HttpContext.Request.Cookies["plantCode"];
                var isDev = _httpContextAccessor.HttpContext.Request.Cookies["isDev"];
                string strPlant = "";
                string strIsDev = "";
                if (plantCookie != null)
                {
                    strPlant = plantCookie.ToString();
                }
                if (isDev != null)
                {
                    strIsDev = isDev.ToString() == "1" ? "Dev" : "";
                }
                plantDev = strPlant + strIsDev;
                string connectionString = plantDev switch
                {
                    "3" => _config.GetConnectionString("ProcDBConnection"),
                    "2" => _config.GetConnectionString("ProcDBConnection"),
                    "1" => _config.GetConnectionString("ProcDBConnection"),
                    "3Dev" => _config.GetConnectionString("DevProcDBConnection"),
                    "2Dev" => _config.GetConnectionString("DevProcDBConnection"),
                    "1Dev" => _config.GetConnectionString("DevProcDBConnection"),
                    "Dev" => _config.GetConnectionString("DevProcDBConnection"),
                    _ => _config.GetConnectionString("ProcDBConnection")
                };

                optionsBuilder.UseSqlServer(connectionString, option =>
                    option.CommandTimeout(600)
                );
            }
        }


        public DbSet<T_PDS692_Header> T_PDS692_Header { get; set; }

        //KBNIM012M : Maintenance Monthly Forecast Data
        public DbSet<VW_MaxVersionForecast> VW_MaxVersionForecast { get; set; }
    }
}
