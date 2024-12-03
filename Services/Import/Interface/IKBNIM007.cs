using KANBAN.Models.KB3.SpecialData.ViewModel;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Models.KB3.UrgentOrder;

namespace KANBAN.Services.Import.Interface
{
    public interface IKBNIM007
    {
        string SetCalendar(string YM, string StoreCD);
        Task<List<TB_Transaction_TMP>> GetPO(string YM);
        string GetStoreCD(string YM, string PO, bool isNew);
        string GetPartNo(string YM, string PO, string StoreCD, bool isNew);
        string PartNoSelected(string YM, string PO, string StoreCD, string PartNo, bool isNew);
        string ListDataTable(string? PO, string? PartNo);
        Task<List<TB_Transaction_TMP>> ListCalendar(string YM, string PO, string StoreCD, string PartNo);
        Task Save(List<VM_Save_IM007> listObj, string action);
        Task Import(VM_PostFile obj);
        Task ImportSCP(VM_PostFile obj, string BackDate);
        Task ImportIPMS(VM_PostFile obj, string BackDate);
    }
}
