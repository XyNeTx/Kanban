using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.Proc_DB
{

    [Keyless]
    public class VW_MaxVersionForecast
    {
        public string? F_PO { get; set; }
        public string F_Version { get; set; }
    }
}
