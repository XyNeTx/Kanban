using System.Data;

namespace KANBAN.Services.CKD_Ordering.IRepository
{
    public interface IKBNOR330
    {
        Task<Tuple<DataTable, string>> Generate();
    }
}
