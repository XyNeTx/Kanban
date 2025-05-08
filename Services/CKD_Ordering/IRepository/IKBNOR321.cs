using KANBAN.Models.KB3.OrderingProcess;
using System.Data;

namespace KANBAN.Services.CKD_Ordering.IRepository
{
    public interface IKBNOR321
    {
        Task<DataTable> Onload(string _loginDate);
        Task<List<List<string>>> GetDropDownData(string? F_Supplier_Code, string? F_Store_Code);
        //Task Find_StartEnd_Date(string action, string F_Supplier_Code);
        Task<List<string>> Get_All_Data(string action, string F_Supplier_Code, int? intRow, string? F_KanbanFrom, string? F_KanbanTo, string? F_StoreFrom, string? F_StoreTo, string? F_PartFrom, string? F_PartTo);
        Task<List<string>> GetBL(string strDate, string Row_Num, int intRow);
        Task<List<string>> Detail_Data(int intRow, string F_Supplier_Code);
        Task<TB_MS_Inform_News> GetInformNews(string F_Supplier_Code, string F_Kanban, string F_Store, string F_Part);
        Task Recalculate(string action, string F_Supplier_Code, int? intRow);
    }
}
