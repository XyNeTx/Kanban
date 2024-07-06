using HINOSystem.Context;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.PPM3;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Context
{
    public class PPM3Context : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        public PPM3Context(DbContextOptions<PPM3Context> options, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) 
            : base(options) 
        {
            _httpContextAccessor = httpContextAccessor;
            _config = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && _httpContextAccessor.HttpContext != null)
            {
                var plantHeader = _httpContextAccessor.HttpContext.Request.Headers["Plant"].ToString();
                string connectionString = plantHeader switch
                {
                    "3" => _config.GetConnectionString("PPM3Connection"),
                    "2" => _config.GetConnectionString("PPMConnection"),
                    "1" => _config.GetConnectionString("PPMConnection"),
                    _ => _config.GetConnectionString("PPM3Connection")
                };

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public DbSet<T_Receive_Local> T_Receive_Local { get; set; }
        public DbSet<T_Supplier_MS> T_Supplier_MS { get; set; }
        public DbSet<T_Construction> T_Construction { get; set; }

    }
}
