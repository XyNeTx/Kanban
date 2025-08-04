using KANBAN.Models.KB3.UrgentOrder;

namespace KANBAN.Services.UrgentOrder.IRepository
{
    public interface IKBNIM017R
    {
        Task<List<TB_Transaction_TMP>> GetUrgentOrders(List<VM_KBNIM017R_ImportData> listObj);
    }
}
