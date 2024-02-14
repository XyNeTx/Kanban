using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.ReportOrder
{
    [Keyless]
    public class V_KBNRT_190_rpt
    {
        public required char F_Plant { get; set; }
        public required string Sup_Code { get; set; }
        public string? F_Delivery_Dock { get; set; }
        public char F_Status { get; set; }
        public required string F_OrderNo { get; set; }
        public required string F_Kanban_No { get; set; }
        public required string Prt_no { get; set; }
        public required string Order_Date { get; set; }
        public required string chk_Order_Date { get; set; }
        public char? F_Issued_Shift { get; set; }
        public required string Deli_Date { get; set; }
        public required string chk_Deli_Date { get; set; }
        public int Qty_KB { get; set; }
        public int Deli_trip { get; set; }
        public int F_Box_Qty { get; set; }
        public int F_Unit_Amount { get; set; }
        public int F_Receive_amount { get; set; }
    }
}
