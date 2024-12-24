using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.PPM3;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS004
    {

        Task<List<string>> GetSelectList(string? kanban, string? storecd, string? partno, string? supplier, bool isNew);

        string GetListData(string? kanban, string? storecd, string? partno, string? supplier, string? type);

        Task<T_Supplier_MS> SelectedSupplier(string supplier, string? storecd);

        Task<T_Construction> SelectedPartNo(string partno, string? supplier, string? kanban, string? storecd);

        Task Save(List<VM_SAVE_KBNMS004> listObj);
    }
}
