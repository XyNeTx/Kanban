using DocumentFormat.OpenXml.Office2013.Word;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.ImportData.Model
{
    [Keyless]
    public class SP_IMPORT_FORECAST
    {
        public int? ErrorNumber { get; set; }
        public int? ErrorSeverity { get; set; }
        public int? ErrorState { get; set; }
        public string? ErrorProcedure { get; set; }
        public int? ErrorLine { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
