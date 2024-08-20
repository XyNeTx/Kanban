using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.PPM3
{
    [PrimaryKey(nameof(F_Part_no), nameof(F_Ruibetsu), nameof(F_Store_cd), nameof(F_subcontract), nameof(F_TC_Str), nameof(F_Plant_CD))]
    public class T_Construction
    {
        [StringLength(12)]
        public string F_Part_no { get; set; }
        [StringLength(2)]
        public string F_Ruibetsu { get; set; }
        [StringLength(2)]
        public string F_Store_cd { get; set; }
        [StringLength(1)]
        public string F_subcontract { get; set; }
        [StringLength(8)]
        public string F_TC_Str { get; set; }
        [StringLength(8)]
        public string? F_TC_End { get; set; }
        [StringLength(4)]
        public string? F_supplier_cd { get; set; }
        public char? F_plant { get; set; }
        public string? F_Sebango { get; set; }
        public char? F_Order_Met { get; set; }
        public char? F_Kanban_con { get; set; }
        public char? F_incre_cut { get; set; }
        public decimal? F_Safety_Stk { get; set; }
        public short? F_ratio { get; set; }
        public char? F_Send_class { get; set; }
        public string? F_send_supplier { get; set; }
        public char? F_send_plant { get; set; }
        public string? F_send_store { get; set; }
        public short? F_qty_box { get; set; }
        public int? F_Weight { get; set; }
        public string? F_box_cd { get; set; }
        public string? F_Part_nm { get; set; }
        public char? F_KD_Flag { get; set; }
        public decimal? F_STD_stock_ratio { get; set; }
        public char? F_Cycle_A { get; set; }
        public string? F_cycle_B { get; set; }
        public string? F_cycle_C { get; set; }
        public short? F_Logistic_cd { get; set; }
        public string? F_commemt { get; set; }
        public string? F_update { get; set; }
        public string? F_CKD_Str { get; set; }
        public string? F_CKD_End { get; set; }
        public string? F_Local_Str { get; set; }
        public string? F_Local_End { get; set; }
        public string? F_inputuser { get; set; }
        public char F_Plant_CD { get; set; }
    }
}
