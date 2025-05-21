using Microsoft.EntityFrameworkCore;

namespace KANBAN.Context
{
    public class CKDUSA_Context : DbContext
    {
        public CKDUSA_Context(DbContextOptions<CKDUSA_Context> options) : base(options)
        {
        }



    }
}
