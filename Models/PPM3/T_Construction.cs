using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.PPM3
{
    [PrimaryKey(nameof(F_Part_no), nameof(F_Ruibetsu), nameof(F_Store_cd), nameof(F_subcontract), nameof(F_TC_Str), nameof(F_Plant_CD))]
    public class T_Construction
    {
        public required string F_Part_no { get; set; }
        public required string F_Ruibetsu { get; set; }
        public required string F_Store_cd { get; set; }
        public required char F_subcontract { get; set; }
        public required string F_TC_Str { get; set; }
        public string? F_TC_End { get; set; }
        public string? F_supplier_cd { get; set; }
        public char? F_plant { get; set; }
        public string? F_Sebango { get; set; }
        public char? F_Order_Met { get; set; }
        public char? F_Kanban_con { get; set; }
        public char? F_incre_cut { get; set; }
        public decimal? F_Safety_Stk { get; set; }
        public Int16? F_ratio { get; set; }
        public char? F_Send_class { get; set; }
        public string? F_send_supplier { get; set; }
        public char? F_send_plant { get; set; }
        public string? F_send_store { get; set; }
        public Int16? F_qty_box { get; set; }
        public Int16? F_Weight { get; set; }
        public string? F_box_cd { get; set; }
        public string? F_Part_nm { get; set; }
        public char? F_KD_Flag { get; set; }
        public decimal? F_STD_stock_ratio { get; set; }
        public char? F_Cycle_A { get; set; }
        public string? F_cycle_B { get; set; }
        public string? F_cycle_C { get; set; }
        public Int16? F_Logistic_cd { get; set; }
        public string? F_commemt { get; set; }
        public string? F_update { get; set; }
        public string? F_CKD_Str { get; set; }
        public string? F_CKD_End { get; set; }
        public string? F_Local_Str { get; set; }
        public string? F_Local_End { get; set; }
        public string? F_inputuser { get; set; }
        public required char F_Plant_CD { get; set; }
    }
}
