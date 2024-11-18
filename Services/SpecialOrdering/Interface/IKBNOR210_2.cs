using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR210_2
    {
        Task<List<TB_Transaction_Spc>> GetCustomerPO(string? DeliDT, string? OrderNo);
        Task<bool> Merge(List<VM_Merge_KBNOR210_2> listObj);

    }
}