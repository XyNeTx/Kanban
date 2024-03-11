using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.ReportOrder
{
    [Keyless]
    public class TB_BL
    {
        public required string F_Delivery_Date { get; set; }
        public required string F_Part_No { get; set; }
        public required string F_Kanban_No { get; set; }
        public required string F_Store_Cd { get; set; }
        public required string F_Supplier_Cd { get; set; }
        public required string F_Supplier_Plant { get; set; }
        public int F_Prod_Qty { get; set; }
        public int F_Order_Qty { get; set; }
        public int F_BL { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }
    }
}
