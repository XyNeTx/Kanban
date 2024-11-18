using KANBAN.Models.KB3.ReportOrder;
using Microsoft.AspNetCore.Routing;

namespace KANBAN.Services.Logistical.Interface
{
    public interface IKBNLC190
    {

        Task<TB_Import_Delivery> GetRev(string YM);
        Task<List<TB_Import_Delivery>> Search(string YM, int Rev);
        Task<bool> Interface(string YM, string Rev, string StartDate);

    }
}
