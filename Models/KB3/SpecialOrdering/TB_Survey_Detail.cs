using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    [PrimaryKey(nameof(F_Survey_Doc), nameof(F_Revise_Rev), nameof(F_PO_Customer),
        nameof(F_Part_No),nameof(F_Ruibetsu),nameof(F_Qty),nameof(F_Delivery_Date))]

    public class TB_Survey_Detail
    {
        [StringLength(30)]
        public string F_Survey_Doc { get; set; }
        public int F_Revise_Rev { get; set; }
        [StringLength(25)]
        public string F_PO_Customer { get; set; }
        public int? F_No { get; set; }
        [StringLength(15)]
        public string F_Part_No { get; set; }
        [StringLength(50)]
        public string? F_Part_Name { get; set; }
        [StringLength(2)]
        public string F_Ruibetsu { get; set; }
        [StringLength(4)]
        public string F_Kanban_No { get; set; }
        [StringLength(2)]
        public string F_Store_Code { get; set; }
        [StringLength(10)]
        public string? F_Package { get; set; }
        public int F_Qty { get; set; }
        public int F_Adjust_Qty { get; set; }
        [StringLength(20)]
        public string F_PDS_No { get; set; }
        public int? F_PDS_Flg { get; set; }
        [StringLength(1)]
        public string F_Status { get; set; }
        [StringLength(8)]
        public string F_Delivery_Date { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? F_Unit_Price { get; set; }
        [StringLength(1)]
        public string F_Price_Flg { get; set; }
    }
}
