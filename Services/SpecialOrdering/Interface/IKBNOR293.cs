using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR293
    {
        string LoadColorTag();
        Task Confirm(List<VM_Post_Tag_Color> listObj);
    }
}
