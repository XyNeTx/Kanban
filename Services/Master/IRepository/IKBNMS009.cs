using HINOSystem.Models.KB3.Master;
using KANBAN.Models.PPM3;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS009
    {
        Task<List<T_Construction>> GetSupplier();

        Task<List<TB_MS_Print_Replace_KB>> SupplierClicked(string Supplier);

        Task Save(List<TB_MS_Print_Replace_KB> listObj);
    }
}
