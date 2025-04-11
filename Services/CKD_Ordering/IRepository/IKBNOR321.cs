namespace KANBAN.Services.CKD_Ordering.IRepository
{
    public interface IKBNOR321
    {
        Task Onload(string _loginDate);
        Task<List<List<string>>> GetDropDownData(string? F_Supplier_Code, string? F_Store_Code);
        //Task Find_StartEnd_Date(string action, string F_Supplier_Code);
        Task<List<string>> Get_All_Data(string action, string F_Supplier_Code, string? F_KanbanFrom, string? F_KanbanTo, string? F_StoreFrom, string? F_StoreTo, string? F_PartFrom, string? F_PartTo);
    }
}
