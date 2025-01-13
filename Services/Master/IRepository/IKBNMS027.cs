using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Models.KB3.Receive_Process;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS027
    {
        Task<List<TB_MS_Matching_Supplier>> GetShortLogistic();
        Task<List<T_Supplier_MS>> GetShortName();
        Task<List<TB_MS_Matching_Supplier>> GetListData(string? F_Short_Logistic);
        Task<List<T_Supplier_MS>> SupOrderSelected(string F_Short_Name);
        Task Save(List<VM_KBNMS027> listObj, string action);
    }
}
