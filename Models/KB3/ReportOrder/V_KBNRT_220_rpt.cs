using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.ReportOrder
{
    [Keyless]
    public class V_KBNRT_220_rpt
    {
        public string? F_Delivery_Dock { get; set; }
        public string? F_Kanban_No { get; set; }
        public string? F_Supplier_code { get; set; }
        public string? F_Part_No { get; set; }
        public string? F_Ruibetsu { get; set; }
        public string? F_Part_Name { get; set; }
        public string? F_short_name { get; set; }
        public string? F_commemt { get; set; }
        public int F_Unit_Amount { get; set; }
        public string? F_Delivery_Date { get; set; }
    }
}
