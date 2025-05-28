using KANBAN.Models.KB3.CKD_Ordering;
using System.Data;

namespace KANBAN.Services.CKD_Ordering.IRepository
{
    public interface IKBNOR360
    {
        Task<List<string>> Check_CKDStatus();
        Task<DataTable> List_Data();
        Task Register(List<VM_KBNOR360_Register> listObj);
        Task<List<string>> GeneratePicking_Click();
    }
}
