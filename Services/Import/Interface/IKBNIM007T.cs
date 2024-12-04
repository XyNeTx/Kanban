using KANBAN.Models.KB3.SpecialData.ViewModel;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Models.KB3.UrgentOrder;

namespace KANBAN.Services.Import.Interface
{
    public interface IKBNIM007T
    {
        string SetCalendar(string YM, string? StoreCD, string TypeSpc);
        Task<List<TB_Transaction_TMP>> GetPO(string YM, string TypeSpc);
        Task<string> GetParentStore(string YM, string? PO, bool isNew);
        Task<string> GetParentPart(string YM, string? PO, string? StoreCD, bool isNew);
        Task<string> GetParentPartDetail(string YM, string? StoreCD, string PartNo, bool isNew);
        Task<string> GetComponentStore(string YM, string? PO, string? ParentPartNo, bool isNew);
        Task<string> GetComponentPartNo(string YM, string? PO, string? CompStoreCD, string? ParentPartNo, bool isNew);
        Task<List<string>> ComponentStoreSelected(string YM, string? PO, string? CompStoreCD, string? CompPartNo, string IssuedDate, bool isNew);
        Task<string> ComponentPartSelected(string YM, string? CompStoreCD, string? CompPartNo, string? ParentPartNo, bool isNew);
        Task<string> ListCalendar(string YM, string? PO, string? ParentPartNo, string? ParentStoreCD, string? CompPartNo, string? CompStoreCD);
        Task<string> ListDatatable(string YM, string? PO, string? ParentPartNo, string? CompPartNo);
        Task Save(List<VM_Save_KBNIM007T> listObj, string _command);
        Task Import(VM_PostFile obj, string TypeSpc);
    }
}
