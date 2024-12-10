using KANBAN.Models.KB3.SpecialData.ViewModel;

namespace KANBAN.Services.Import.Interface
{
    public interface IKBNIM007C
    {
        string GetPDS(string? DeliDateFrom, string? DeliDateTo);

        string GetUser(string? DeliDateFrom, string? DeliDateTo);

        string GetListData(string? DeliDateFrom, string? DeliDateTo, string? PDSNo, string? User);

        Task Update_Cycle(string ProcessDate);

        Task Confirm(List<VM_CONF_KBNIM007C> listObj, string? ProcessDate, string? ProcessShift);
    }
}
