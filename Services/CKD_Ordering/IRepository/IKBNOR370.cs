using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.CKD_Ordering.IRepository
{
    public interface IKBNOR370
    {
        Task<List<string>> Preview(List<VM_Post_KBNOR261> listobj);
        Task PreviewKB();
        Task<List<TB_REC_HEADER>> GetPDS();
        Task PDS_GENBARCODE(List<VM_Post_KBNOR261> listObj);
    }
}
