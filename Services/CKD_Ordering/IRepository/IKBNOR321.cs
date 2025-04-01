namespace KANBAN.Services.CKD_Ordering.IRepository
{
    public interface IKBNOR321
    {
        Task<List<string>> GetSupplier();
        Task<List<string>> GetKanban(string? F_Supplier_Code);
        Task<List<string>> GetStore(string? F_Supplier_Code);
        Task<List<string>> GetPartNo(string? F_Supplier_Code, string? F_Store_Code);
    }
}
