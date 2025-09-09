using KANBAN.Models.KB3.OtherCondition.ViewModel;

namespace KANBAN.Services.OtherCondition.IRepository
{
    public interface IKBNOC150
    {
        Task<List<string>> Sup_DropDown();

        Task Print(VM_REPORT_KBNOC150 model);
    }
}
