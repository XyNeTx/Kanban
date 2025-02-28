using HINOSystem.Models.KB3.Master;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS018
    {
        Task<List<TB_MS_Heijunka>> GetListData(string? CycleB);
        Task Save(TB_MS_Heijunka obj, string action);
    }
}
