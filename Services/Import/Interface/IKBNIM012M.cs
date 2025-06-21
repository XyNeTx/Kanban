using KANBAN.Models.KB3.ImportData.Model;
using KANBAN.Models.KB3.ImportData.ViewModel;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.ReportOrder;
using KANBAN.Models.Proc_DB;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace KANBAN.Services.Import.Interface
{
    public interface IKBNIM012M
    {
        Task<LoadMonthlyForecast> FormLoadKBNIM012M(string database);

        Task<List<string>> Supplier_DropDown();

        Task<List<string>> KanBan_DropDown(string supplierCode);

        Task<T_Supplier_MS> SupplierName(string supplierCode);

        Task<List<string>> StoreCode_DropDown(string? supplierCode ,string? kanbanNo);

        Task<List<string>> PartNoList(string supplierCode, string storeCode, string kanbanNo);

        Task<string> PartName(string partNo);

        Task<List<TB_IMPORT_FORECAST_TEMP>> Search(VM_KBNIM012M model);

        Task<string> GetLastOrder();

        Task<string> Get_AdvDelivery(string sDeliveryDate, int sAdv);

        Task Save(List<TB_IMPORT_FORECAST_TEMP> model);

        Task Transfer(VM_KBNIM012M model);

        Task Report(VM_KBNIM012M model);

        Task Import(VM_KBNIM012M model);

        Task ConvertExcelToText(IFormFile file, string sNewFile);

    }
}
