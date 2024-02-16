using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.ReportOrder
{
    [Keyless]
    public class V_KBNRT_210_rpt_Dev
    {
        public string F_Part_no { get; set; }
        public string F_Kanban_No { get; set; }
        public string F_Sup_cd { get; set; }
        public string F_Sup_short_name { get; set; }
        public string F_Part_Name { get; set; }
        public string F_Delivery_Dock { get; set; }
        public string F_Delivery_Cycle { get; set; }
        public int F_Box_Qty { get; set; }
        public DateTime F_Order_date { get; set; }
        public char F_Shift { get; set; }
        public string F_PDS_no { get; set; }
        public string F_Delivery_Date { get; set; }
        public int F_Delivery_Trip { get; set; }
        public string F_Delivery_Time { get; set; }
        public int F_QTY_KB { get; set; }
        public int F_Total_ord { get; set; }
        public char F_Status { get; set; }
        public float TEST { get; set; }
    }
}
