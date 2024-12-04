namespace KANBAN.Services.Import.Interface
{
    public interface IKBNIM007C
    {
        string GetPDS(string? DeliDateFrom, string? DeliDateTo);

        string GetUser(string? DeliDateFrom, string? DeliDateTo);
    }
}
