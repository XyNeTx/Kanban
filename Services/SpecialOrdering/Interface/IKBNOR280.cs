using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR280
    {
        string GetPDSData(string FacCD, string DeliYM);
        Task Register(List<VM_Register_KBNOR280> listObj);
        Task<List<TB_REC_HEADER>> GetSupplier();
        Task<List<TB_REC_HEADER>> GetPO(string YM);
        Task<List<TB_REC_HEADER>> GetPDS(string? POFrom, string? POTo);

        string ExportData(string? PONoFrom, string? PONoTo, string? PDSNoFrom, string? PDSNoTo, string? SupplierFrom, string? SupplierTo,
                                    string? DeliveryFrom, string? DeliveryTo);
    }
}
