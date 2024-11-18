using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.SpecialOrdering;
using Microsoft.IdentityModel.Tokens;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR220_2
    {
        Task<TB_Calendar> GetCalendar(string YM);
        Task<List<TB_Survey_Detail>> GetPOList();
        string GetSurvey(string YM);
        string GetSuppCD(string PO, string? YM);
        Task<List<TB_Survey_Detail>> GetPartNo(string PO, string SuppCD);
        Task<int> PartNoSelected(string surveyDoc, string suppCD, string partNo);
        Task<string> GetCalendarQty(string SurveyDoc, string suppCD, string YM, string partNo);
        Task Save(List<VM_Post_KBNOR220_2> listObj);
        Task<string> GetSupplierName(string SuppCD, string SuppPlant);
    }
}
