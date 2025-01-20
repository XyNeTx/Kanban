using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.PPM3;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS015
    {
        Task<List<T_Construction>> GetDropDownNew(string? Supplier, string? Kanban, string? StoreCode, string? PartNo);

        Task<List<TB_MS_Label>> GetDropDownInq(string? Supplier, string? Kanban, string? StoreCode, string? PartNo);

        Task<T_Supplier_MS> SupplierChanged(string SupplierCode, string? StoreCode);

        Task<string> PartNoSelectedInq(string PartNo, string? SupplierCode, string? Kanban, string? StoreCode);
        Task<string> PartNoSelectedNew(string PartNo, string? SupplierCode, string? Kanban, string? StoreCode);
    }
}
