using System.Data;

namespace KANBAN.Services.CKD_Ordering.IRepository
{
    public interface IKBNOR310
    {
        Task Interface();
        Task<DataTable> getCKD_ProcessDateTime();
    }
}
