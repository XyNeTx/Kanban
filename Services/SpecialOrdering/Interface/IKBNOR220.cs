using KANBAN.Models.KB3.SpecialOrdering;
using System.Data;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR220
    {

        DataTable GetTransactionSPCNOSurvey(string Fac, string? PDSNo, string? PDSDate, string Mode);
        DataTable GetDeptMS(string? ProcessDT, string? DeptCode);
        DataTable GetACCOUNTMS(string ProcessDT);

        string LoadColorofTag();
        string LoadListView();
        Task Generate(List<VM_Post_KBNOR220_Gen> dataList);
        Task Save(VM_Post_KBNOR220_Gen obj);
        bool ChkDeptCode(string DeptCode, string ProcessDate);
        bool ChkDebitCode(string DBCode, string ProcessDate);
    }
}
