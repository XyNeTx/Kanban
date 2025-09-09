namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR210
    {
        Task Interface();
        string Page_Load();
        Task<bool> Check_Error();
    }
}
