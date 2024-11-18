using KANBAN.Models.KB3.SpecialOrdering;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR250
    {
        DataTable GetSurveyNoPDS(string fac, string? SurveyDoc = "", string? DeliveryDT = "", string? DeliYM = "");
        DataTable CheckPriceAndPackageFlag(string SurveyDoc);
        Task Refresh();
        Task Unlock(List<VM_KBNOR250> listObj);
        Task Generate(List<VM_KBNOR250> listObj, [FromQuery] string DeliYM);
    }
}
