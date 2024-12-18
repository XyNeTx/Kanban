using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.OtherCondition.Model;
using KANBAN.Models.KB3.OtherCondition.ViewModel;
using KANBAN.Models.KB3.Receive_Process;
using TB_MS_PartOrder = HINOSystem.Models.KB3.Master.TB_MS_PartOrder;

namespace KANBAN.Services.OtherCondition.IRepository
{
    public interface IKBNOC120
    {
        Task<List<TB_MS_PartOrder>> GetSupplier(string? StoreCD);
        Task<List<T_Supplier_MS>> GetStore(string? SupplierCD);
        Task Save(List<VM_SAVE_KBNOC120> ListVMObj, string action);
        Task<List<TB_Slide_Order>> GetListData(string? SupplierCD, string? StoreCD);

    }
}
