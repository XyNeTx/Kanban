using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.PPM3;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS020
    {
        Task<List<T_Construction>> GetDropDownList(string? F_Supplier, string? F_KanbanNo, string? F_StoreCD, string? F_PartNo);

        Task<T_Supplier_MS> GetSupplierName(string F_Supplier, string? F_StoreCD);

        Task Save(List<VM_KBNMS020> listObj, string action);

        Task<string> GetListData(string? F_Supplier, string? F_KanbanNo, string? F_StoreCD, string? F_PartNo);
    }
}
