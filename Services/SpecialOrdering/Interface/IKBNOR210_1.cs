using KANBAN.Models.KB3.SpecialOrdering;
using System.Data;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR210_1
    {
        Task<List<TB_Transaction_Spc>> LoadDataChangeDelivery(string? PDSNo, string? SuppCd, string? PartNo, bool? chkDeli, string? DeliFrom, string? DeliTo);
        Task<string> GetSupplierName(string SuppCd);
        Task<string> GetPartName(string PartNo);
        Task<DataTable> GetPOMergeData(string? PDSNo, string? SuppCd, string? PartNo, bool? chkDeli, string? DeliFrom, string? DeliTo);
        Task Save(VM_Save_KBNOR210_1 obj);
        DataTable LoadGridData(string OrderNo);
        Task SaveKBNOR210_1_STC_3(List<VM_Save_KBNOR210_1_STC_3> obj);
        Task<DataTable> GetDataKBNOR210_1_STC_3_1(string F_OrderNo, string F_Part_No, string F_Store_Cd, string F_Supplier, string? Delivery, int F_Use_StockQty);
        Task SaveKBNOR210_1_STC_3_1(List<VM_Save_KBNOR210_1_STC_3_1> obj);
        string ListDatatogrid(string? SupplierCD = "", string? PartNo = "", string? StoreCode = "", string? StockDate = "", string? UpdateBy = "", string? UpdateDate = "");
        string STC_1_GetSupplierCode(bool isNew, string StockDate);
        string STC_1_GetPartNo(bool isNew, string StockDate, string Supplier_Code);
        string STC_1_GetStore(bool isNew, string StockDate, string Supplier_Code, string Part_No);
        string STC_1_GetSupplierName(string Supplier_Code);
        string STC_1_GetPartName(string Supplier_Code, string Part_No);
        string STC_1_GetKB_Qty(string Supplier_Code, string Part_No, string StockDate, string Store_CD);
        Task STC_1_Save(VM_Post_KBNOR210_STC_1 obj);
        Task STC_1_Import(List<VM_Import_KBNOR210_STC_1> listObj);
        string STC_2_LoadSupplier(string Type, bool chkFlg, bool chkFlgDT, string DateFrom, string DateTo);
        string STC_2_LoadPartNo(string Type, bool chkFlg, bool chkFlgDT, string DateFrom, string DateTo, string SuppF, string SuppT);
        string Del_LoadSupplier(string DeliYM);
        string Del_LoadPartNo(string DeliYM, string Supplier);
    }
}