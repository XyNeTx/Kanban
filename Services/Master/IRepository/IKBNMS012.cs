using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.Master;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS012
    {
        Task<List<TB_MS_PartOrder>> GetDropDown(string? F_Supplier_Code, string? F_Kanban_No, string? F_Store_Cd, string? F_Part_No);

        Task<string[]> GetSupplierDetail(string F_Supplier_Code);

        Task<List<TB_Kanban_SetOrder>> Search(string? F_Supplier_Code, string? F_Kanban_No, string? F_Store_Cd, string? F_Part_No);

        Task<object> FindDetail(string F_Supplier_Code, string? F_Kanban_No, string? F_Store_Cd, string? F_Part_No);

        Task Save(List<TB_Kanban_SetOrder> listObj);
    }
}
