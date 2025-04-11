using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.VLT;

namespace KANBAN.Services.Import.Interface
{
    public interface IKBNIM0044
    {
        Task<int> SaveImportData(List<VM_KBNIM0044> listData);
        Task<List<TB_Import_VHD>> GetDataList(bool isAll = false);
        Task UpdateFlag(List<TB_Import_VHD> listData, string shift);
        Task Confirm(List<TB_Import_VHD> listData);
    }
}