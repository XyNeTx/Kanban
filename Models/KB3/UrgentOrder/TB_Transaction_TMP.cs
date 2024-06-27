using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.UrgentOrder
{
    [PrimaryKey(nameof(F_Type), nameof(F_Plant), nameof(F_PDS_No), nameof(F_Store_CD), nameof(F_Part_No), nameof(F_Ruibetsu), nameof(F_Part_Order), nameof(F_Ruibetsu_Order), nameof(F_Store_Order),
    nameof(F_Seq_No), nameof(F_Seq_Type), nameof(F_Delivery_Date), nameof(F_Adv_Deli_Date), nameof(F_OrderType), nameof(F_Parent_Level2), nameof(F_Round))]
    public class TB_Transaction_TMP
    {
        public required string F_Type { get; set; }
        public required string F_Type_Spc { get; set; }
        public required char F_Plant { get; set; }
        public required string F_PDS_No { get; set; }
        public required string F_PDS_Issued_Date { get; set; }
        public required string F_Store_CD { get; set; }
        public required string F_Part_No { get; set; }
        public required string F_Ruibetsu { get; set; }
        public required string F_Kanban_No { get; set; }
        public required string F_Part_Name { get; set; }
        public short F_Qty_Pack { get; set; }
        public required string F_Part_Code { get; set; }
        public required string F_Part_Order { get; set; }
        public required string F_Ruibetsu_Order { get; set; }
        public required string F_Store_Order { get; set; }
        public required string F_Name_Order { get; set; }
        public int F_Qty { get; set; }
        public int F_Qty_Level1 { get; set; }
        public required string F_Seq_No { get; set; }
        public required string F_Seq_Type { get; set; }
        public char F_Cut_Flag { get; set; }
        public required string F_Delivery_Date { get; set; }
        public required string F_Adv_Deli_Date { get; set; }
        public char F_OrderType { get; set; }
        public required string F_Country { get; set; }
        public char F_Reg_Flg { get; set; }
        public char F_Inventory_Flg { get; set; }
        public required string F_Supplier_CD { get; set; }
        public char F_Supplier_Plant { get; set; }
        public required string F_Cycle_Time { get; set; }
        public decimal F_Safty_Stock { get; set; }
        public required string F_Part_Refer { get; set; }
        public required string F_Ruibetsu_Refer { get; set; }
        public required string F_Update_By { get; set; }
        public DateTime F_Update_Date { get; set; }
        public string? F_Remark { get; set; }
        public required string F_Parent_Level2 { get; set; }
        public int F_Qty_Level2 { get; set; }
        public required string F_Parent_Level3 { get; set; }
        public int F_Qty_Level3 { get; set; }
        public required string F_Parent_Level4 { get; set; }
        public int F_Qty_Level4 { get; set; }
        public short F_Round { get; set; }
        public required string F_Org_Store_CD { get; set; }
        public required string F_Ratio { get; set; }
        public char F_Customer_OrderType { get; set; }
        public string F_Survey_DOC { get; set; }
    }
}
