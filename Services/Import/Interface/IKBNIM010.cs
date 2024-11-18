using System.Data;

namespace KANBAN.Services.Import.Interface
{
    public interface IKBNIM010
    {
        bool Check_Holiday(string date, string shift);
        List<string> ListData(string date, string shift);
        Task<bool> Confirm(string date, string shift);
    }
}