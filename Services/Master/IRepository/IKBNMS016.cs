using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.PPM3;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS016
    {
        Task<string> List_Data(string? F_Supplier_Cd, string? F_Kanban_No, string? F_Part_No, string? F_Store_Cd, string? F_Supplier_Plant, string? F_Ruibetsu, string? F_Group);

        Task<List<T_Construction>> GetDropDownNew(string? F_Supplier_Cd, string? F_Kanban_No, string? F_Part_No, string? F_Store_Cd, string? F_Supplier_Plant, string? F_Ruibetsu);

        Task<List<TB_MS_PairOrder>> GetDropDownInq(string? F_Supplier_Cd, string? F_Kanban_No, string? F_Part_No, string? F_Store_Cd, string? F_Supplier_Plant, string? F_Ruibetsu, string? F_Group);

        Task Save(List<TB_MS_PairOrder> listObj, string action);

        Task<T_Construction> GetPartNoDetail(string? F_Part_No, string? F_Ruibetsu);

        Task<T_Supplier_MS> GetSupplierDetail(string F_Supplier_Cd, string F_Supplier_Plant);
    }
}
