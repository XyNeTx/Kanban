using Microsoft.EntityFrameworkCore;

namespace KANBAN.Context
{
    public class CKDWH_Context : DbContext
    {
        public CKDWH_Context(DbContextOptions<CKDWH_Context> options) : base(options)
        {
        }



    }
}
