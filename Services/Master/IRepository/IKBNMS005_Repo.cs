namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS005_Repo
    {
        Task<List<List<string>>> GetListOption(string? Sup, string? Part,string? PartT,string? Store,string? StoreT);
        Task<List<string>> GetAllData(string Supplier, string? Kanban, string? KanbanTo, string? Store, string? StoreTo, string? Part, string? PartTo, string? Date, string DateTo, bool isOk, bool isNo, bool isPartShort);
    }
}
