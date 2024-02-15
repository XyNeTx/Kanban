using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.ReportOrder
{
    [Keyless]
    public class V_KBNRT_180_rpt
    {
        public string? F_Supplier { get; set; }
        public string? F_short_name { get; set; }
        public string? F_Part_no { get; set; }
        public string? F_Kanban_No { get; set; }
        public string? F_Store_Cd { get; set; }
        public string? Chk_ord_date { get; set; }
        public string? F_Order_date { get; set; }
        public char F_Order_Shift { get; set; }
        public string? Chk_deli_date { get; set; }
        public string? F_Deli_date { get; set; }
        public int F_Delivery_Trip { get; set; }
        public string? F_Status { get; set; }
        public string? F_Update_By { get; set; }
    }
}
