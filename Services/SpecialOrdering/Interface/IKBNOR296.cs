using HINOSystem.Models.KB3.Master;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR296
    {
        string List_Data(string? Supplier);
        string SupplierDropDown(bool isNew);
        string SupplierChanged(string Supplier);
        Task Save(string Action, List<TB_MS_SupplierAttn> listModel);
    }

}
