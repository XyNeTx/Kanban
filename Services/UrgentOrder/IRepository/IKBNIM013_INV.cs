using KANBAN.Models.PPM;

namespace KANBAN.Services.UrgentOrder.IRepository
{
    public interface IKBNIM013_INV
    {
        Task<string> GetList_Header();
        Task<string> GetList_Detail(string inDeclareNo);
        Task InterfaceDataToTransactionTemp(List<VM_KBNIM013_INV> listObj,string PDS);
        Task Delete(List<VM_KBNIM013_INV> listObj);
    }
}
