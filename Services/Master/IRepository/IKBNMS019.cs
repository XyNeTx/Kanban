using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.PPM3;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS019
    {
        Task<List<T_Supplier_MS>> GetSupplierNew();
        Task<List<TB_MS_MAXAREA>> GetSupplierInq();
        Task<TB_MS_MAXAREA> GetSupplierDetail(string SupplierCode);
        Task<List<T_Construction>> GetPartNew();
        Task<List<TB_MS_MAXAREA>> GetPartInq(string SupplierCode);
        Task<T_Construction> GetPartName(string PartNo);
        Task<TB_MS_MAXAREA> GetMaxTrip(string SupplierCode, string PartNo, string StoreCode, string KanbanNo);
    }
}
