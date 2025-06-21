using KANBAN.Models.KB3.OtherCondition.ViewModel;
using KANBAN.Models.KB3.Receive_Process;

namespace KANBAN.Services.OtherCondition.IRepository
{
    public interface IKBNOC150
    {
        Task<List<string>> Sup_DropDown();

        Task Print(VM_REPORT_KBNOC150 model);
    }
}
