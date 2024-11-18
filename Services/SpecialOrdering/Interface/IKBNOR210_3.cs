using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR210_3
    {
        Task<List<TB_Transaction_Spc>> LoadOrderNo();
        Task<List<TB_Transaction_Spc>> LoadCustomPo(string NewCusPO);
        Task Unmerge(List<VM_Merge_KBNOR210_2> listObj);
    }
}
