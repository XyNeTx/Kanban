using KANBAN.Models.KB3.LogisticCondition;

namespace KANBAN.Services.Logistical.Interface
{
    public interface IKBNLC150
    {
        string ShowRevision(string YM);
        Task<string> Import(List<VM_TB_Import_Delivery> listObj);
        int GetProcessBar();
    }
}
