using Microsoft.EntityFrameworkCore;

namespace KANBAN.Context
{
    public class ProcWebContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private static string plantDev = "";

        public ProcWebContext(
            DbContextOptions<ProcWebContext> options
            , IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(options)
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
                    "3" => _config.GetConnectionString("ProcWebConnection"),
                    "2" => _config.GetConnectionString("ProcWebConnection"),
                    "1" => _config.GetConnectionString("ProcWebConnection"),
                    "3Dev" => _config.GetConnectionString("DevProcDBConnection"),
                    "2Dev" => _config.GetConnectionString("DevProcDBConnection"),
                    "1Dev" => _config.GetConnectionString("DevProcDBConnection"),
                    "Dev" => _config.GetConnectionString("DevProcDBConnection"),
                    _ => _config.GetConnectionString("ProcWebConnection")
                };

                optionsBuilder.UseSqlServer(connectionString, option =>
                    option.CommandTimeout(600)
                );
            }
        }
    }
}
