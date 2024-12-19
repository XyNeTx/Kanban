using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.OtherCondition.Model;
using KANBAN.Models.KB3.OtherCondition.ViewModel;

namespace KANBAN.Services.OtherCondition.IRepository
{
    public interface IKBNOC121
    {
        Task<List<TB_MS_PartOrder>> GetSupplier();
        Task<List<TB_MS_PartOrder>> GetStore(string? SupplierCD);
        Task<List<TB_MS_PartOrder>> GetPartNo(string? SupplierCD, string? StoreCD);
        Task Save(List<VM_SAVE_KBNOC121> ListVMObj, string action);
        Task<List<TB_Slide_Order_Part>> GetListData(string? SupplierCD, string? StoreCD, string? PartNo);

    }
}
