using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.ReportOrder;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS028
    {
        Task<List<TB_Import_Delivery>> GetDockCode();

        Task<List<TB_MS_Matching_Supplier>> GetShortLogistic();

        Task<IEnumerable<TB_MS_Remark_DocSheet>> GetListData(string? F_Dock_Cd);

        Task Save(TB_MS_Remark_DocSheet obj, string action);
    }
}
