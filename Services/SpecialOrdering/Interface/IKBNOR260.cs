using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR260
    {
        string GetApproverList();
        Task<List<TB_REC_HEADER>> GetPDSDataNoApprove(string fac);
        Task SendApprove(List<VM_TB_Rec_Header> listObj);
        Task<List<TB_REC_HEADER>> GetPDSWaitApprove(string? fac);
    }
}
