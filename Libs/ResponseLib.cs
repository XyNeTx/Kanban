using KANBAN.Models;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Libs
{
    public class ResponseLib<T> : IResponseLib<T> where T : class
    {
        public DbSet<T> DbSet { get; }

        public ResponseLib(DbSet<T> dbSet)
        {
            DbSet = dbSet;
        }

        public T Response => DbSet.Find();

        public ResponseModel Ok(string message, T T)
        {
            ResponseModel response = new ResponseModel
            {
                status = 200,
                response = "OK",
                title = "Success",
                message = message,
                error = null,
                data = T
            };
            return response;
        }
    }
}
