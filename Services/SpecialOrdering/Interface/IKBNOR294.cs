using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR294
    {
        string LoadContactList();
        Task Confirm(List<TB_MS_Operator> listObj);
    }
}
