namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS029
    {
        Task<string> GetListData(string? Plant, string? DockCode);
    }
}
