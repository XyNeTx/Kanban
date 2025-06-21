using KANBAN.Models.KB3.LogisticCondition.ViewModel;

namespace KANBAN.Services.Logistical.Interface
{
    public interface IKBNLC200
    {
        Task<List<string>> Sup_DropDown();
        Task Print(VM_REPORT_KBNLC200 model);
    }
}
