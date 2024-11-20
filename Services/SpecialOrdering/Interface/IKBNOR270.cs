using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.SpecialOrdering;
using System.Data.SqlTypes;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR270
    {
        Task Preview(List<VM_Post_KBNOR261> listObj);
        Task PreviewKB();
    }
}
