using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR261
    {
        string GetPDSWaitApprove();
        Task Approve(List<VM_Post_KBNOR261> listObj);
        Task Preview(VM_Post_KBNOR261 obj);
    }
}
