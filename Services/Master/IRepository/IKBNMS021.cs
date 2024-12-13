using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.Master.ViewModel;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS021
    {
        Task<List<TB_MS_PartCode>> GetListDataTables(string? Line, string? PartCode, string? PartNo);
        Task<List<TB_MS_PartCode>> GetLine(string? PartCode, string? PartNo);
        Task<List<TB_MS_PartCode>> GetPartCode(string? Line, string? PartNo);
        Task<List<TB_MS_PartCode>> GetPartNo(string? Line, string? PartCode);
        Task Save(List<TB_MS_PartCode> listObj, string action);
        Task<List<TB_MS_PartCode>> CheckPairPart();
    }
}
