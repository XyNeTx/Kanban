using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.Master.ViewModel;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS014
    {
        Task<string> GetSupplierCode(bool isNew);

        Task<string> GetSupplierPlant(bool isNew, string SupplierCode);

        Task<string> GetShortName(string SupplierCode, string SupplierPlant, bool isNew);

        Task Save(List<VM_Save_KBNMS014> listObj, string action);

        Task<List<TB_MS_PrintKanban>> GetListData(string? SupplierCode, string? SupplierPlant);

    }
}
