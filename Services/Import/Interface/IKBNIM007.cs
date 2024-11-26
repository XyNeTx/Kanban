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
    
    }
}
