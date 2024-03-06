using KANBAN.Models.Proc_DB;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Context
{
    public class ProcDBContext : DbContext
    {
        public ProcDBContext(DbContextOptions<ProcDBContext> options) : base(options) { }

        public DbSet<T_PDS692_Header> T_PDS692_Header { get; set; }
    }
}
