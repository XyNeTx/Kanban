using KANBAN.Models.KB3.Receive_Process;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Context
{
    public class PPM3Context : DbContext
    {
        public PPM3Context(DbContextOptions<PPM3Context> options) : base(options) { }

        public DbSet<T_Receive_Local> T_Receive_Local { get; set; }
        public DbSet<T_Supplier_MS> T_Supplier_MS { get; set; }

    }
}
