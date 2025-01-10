using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.Master.ViewModel;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS025
    {
        Task<List<TB_MS_LPSupplier>> GetLogisticSupplier(string? TruckType);

        Task<string> GetTruckType(bool isNew, string? Logistic);

        Task<string> TruckTypeSelected(bool isNew, string? Logistic, string? TruckType);

        Task<string> GetListData(string? Logistic, string? TruckType);

        Task Save(List<VM_Save_KBNMS025> listObj, string action);
    }
}
