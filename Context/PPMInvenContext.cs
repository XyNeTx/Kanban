using KANBAN.Models.PPM;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Context
{
    public class PPMInvenContext : DbContext
    {
        public PPMInvenContext(DbContextOptions<PPMInvenContext> options ) : base (options) { }

        public DbSet<T_System_Control> T_System_Control { get; set; }
    }
}
