using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.Receive_Process
{
    [PrimaryKey(nameof(F_OrderNo), nameof(F_Part_No), nameof(F_Ruibetsu), nameof(F_Kanban_No), nameof(F_No))]
    public class TB_REC_DETAIL
    {
        public required string F_OrderNo { get; set; }
        public required string F_Part_No { get; set; }
        public required string F_Ruibetsu { get; set; }
        public required string F_Kanban_No { get; set; }
        public int? F_Box_Qty { get; set; }
        public decimal? F_Unit_price { get; set; }
        public int F_No { get; set; }
        public int? F_Unit_Amount { get; set; }
        public int? F_Receive_amount { get; set; }
        public DateTime? F_Receive_Date { get; set; }
        public required string F_Part_Name { get; set; }
        public required string F_Address { get; set; }
        public byte[]? F_Part_Pic { get; set; }
        public required string F_Inf_KB { get; set; }
        public required string F_Dock_CD { get; set; }
        public required string F_PDS_GROUP { get; set; }
    }
}
