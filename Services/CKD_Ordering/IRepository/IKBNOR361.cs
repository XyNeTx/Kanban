using KANBAN.Models.KB3.CKD_Ordering;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.PPM3;
using System.Data;

namespace KANBAN.Services.CKD_Ordering.IRepository
{
    public interface IKBNOR361
    {
        Task<List<List<string>>> GetDataList(string? Supplier_Code, string? Kanban_No, string? Store_Code, string? Part_No, bool IsNew);
        
        Task<T_Supplier_MS> GetSupplier(string Supplier_Code, string? Store_Code);

        Task<T_Construction> GetPartNo(string Part_No, string? Supplier_Code, string? Kanban_No, string? Store_Code);

        Task<string> GetList(string? Supplier_Code, string? Kanban_No, string? Store_Code, string? Part_No);

        Task UpdateFlgClearModule(List<VM_KBNOR361_Save> listObj);
    }
}
