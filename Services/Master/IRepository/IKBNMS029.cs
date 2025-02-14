using HINOSystem.Models.KB3.Master;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS029
    {
        Task<string> GetListData(string? Plant, string? DockCode);
        Task<List<TB_MS_Dock_Code>> GetDockCode();
        Task Save(List<TB_MS_Dock_Code> listObj, string action);
    }
}
