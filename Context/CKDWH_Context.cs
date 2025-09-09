using Microsoft.EntityFrameworkCore;

namespace KANBAN.Context
{
    public class CKDWH_Context : DbContext
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private static string plantDev = "";

        public CKDWH_Context(DbContextOptions<CKDWH_Context> options, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
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
                    "3" => _config.GetConnectionString("CKDWH3Connection"),
                    "2" => _config.GetConnectionString("CKDWHConnection"),
                    "1" => _config.GetConnectionString("CKDWHConnection"),
                    _ => _config.GetConnectionString("CKDWH3Connection")
                };

                optionsBuilder.UseSqlServer(connectionString, option =>
                    option.CommandTimeout(600)
                );
            }
        }


    }
}
