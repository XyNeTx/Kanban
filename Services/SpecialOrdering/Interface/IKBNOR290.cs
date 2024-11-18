using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR290
    {
        Task<List<TB_Survey_Header>> ProdYMChanged(string ProdYM);
        Task<List<TB_Survey_Header>> GetSuppCD(string? PO);
        Task<string> GetData(string PO, string Supplier);
    }
}
