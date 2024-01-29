using KANBAN.Models.PPM;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Context
{
    public class PPMContext : DbContext
    {
        public PPMContext(DbContextOptions<PPMContext> options ) : base (options) { }

        public DbSet<T_System_Control> T_System_Control { get; set; }
    }
}
