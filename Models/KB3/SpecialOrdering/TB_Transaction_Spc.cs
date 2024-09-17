using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.KB3.SpecialOrdering
{

    [PrimaryKey(nameof(F_Type),nameof(F_Type_Spc),nameof(F_PDS_No_New),
        nameof(F_PDS_Issued_Date),nameof(F_Part_No),nameof(F_Ruibetsu),
        nameof(F_Kanban_No),nameof(F_Qty),nameof(F_Delivery_Date),
        nameof(F_Adv_Deli_Date),nameof(F_Supplier_CD), nameof(F_Supplier_Plant))]

    public class TB_Transaction_Spc
    {

        [StringLength(10)]
        [Required]
        public string F_Type { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Type_Spc { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Plant { get; set; }
        [StringLength(25)]
        [Required]
        public string F_PDS_No { get; set; }
        [StringLength(25)]
        [Required]
        public string F_PDS_No_New { get; set; }
        [StringLength(10)]
        [Required]
        public string F_PDS_Issued_Date { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Acc_Dr { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Acc_Cr { get; set; }
        [StringLength(30)]
        [Required]
        public string F_Work_Code { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Dept_Use { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Store_CD { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Part_No { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Ruibetsu { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Kanban_No { get; set; }
        [StringLength(50)]
        [Required]
        public string F_Part_Name { get; set; }
        public short F_Qty_Pack { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Part_Code { get; set; }
        public int F_Qty { get; set; }
        public int F_Use_StockQty { get; set; }
        [StringLength(15)]
        [Required]
        public string F_Seq_No { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Seq_Type { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Cut_Flag { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Delivery_Date { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Delivery_Date_New { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Adv_Deli_Date { get; set; }

        [StringLength(1)]
        [Required]
        public string F_OrderType { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Country { get; set; }
        [StringLength(1)]
        [Required]
        public string F_Reg_Flg { get; set; }
        [StringLength(1)]
        [Required]
        public string F_Inventory_Flg { get; set; }
        [StringLength(4)]
        [Required]
        public string F_Supplier_CD { get; set; }
        [StringLength(1)]
        [Required]
        public string F_Supplier_Plant { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Supplier_Int { get; set; }
        [StringLength(6)]
        [Required]
        public string F_Cycle_Time { get; set; }
        [Column(TypeName = "numeric(4, 2)")]
        [Required]
        public decimal F_Safty_Stock { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Part_Refer { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Ruibetsu_Refer { get; set; }
        [StringLength(50)]
        [Required]
        public string F_Survey_Doc { get; set; }
        [Required]
        public int F_Survey_ID { get; set; }
        [StringLength(1)]
        [Required]
        public string F_Survey_Flg { get; set; }
        [StringLength(50)]
        [Required]
        public string F_Process_By { get; set; }
        [Required]
        public DateTime F_Process_Date { get; set; }
        [StringLength(1)]
        [Required]
        public string F_Process_Shift { get; set; }
        [StringLength(1)]
        [Required]
        public string F_Process_Plant { get; set; }
        [StringLength(50)]
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        [StringLength(110)]
        [Required]
        public string F_Remark { get; set; }
        [StringLength(110)]
        [Required]
        public string F_Remark2 { get; set; }
        [StringLength(110)]
        [Required]
        public string F_Remark3 { get; set; }
        [StringLength(110)]
        public string? F_Remark_KB { get; set; }
        [Required]
        public byte F_Round { get; set; }
        [StringLength(25)]
        public string? F_CustomerOrder_Type { get; set; }
        [StringLength(1)]
        [Required]
        public string F_CusOrderType_CD { get; set; }
    }
}
