using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    [PrimaryKey(nameof(F_Supplier_CD), nameof(F_Supplier_Plant),
        nameof(F_Part_no), nameof(F_Ruibetsu),nameof(F_Stock_Date))]

    public class TB_STOCK_KB_SPC_PART_REMAIN
    {
        [StringLength(4)]
        public string F_Supplier_CD { get; set; }
        [StringLength(1)]
        public string F_Supplier_Plant { get; set; }
        [StringLength(15)]
        public string F_Part_no { get; set; }
        [StringLength(2)]
        public string F_Ruibetsu { get; set; }
        [StringLength(4)]
        public string F_Kanban_no { get; set; }
        [StringLength(2)]
        public string F_Store_Code { get; set; }
        public int F_Package { get; set; }
        [StringLength(8)]
        public string F_Stock_Date { get; set; }
        public int F_Stock_Qty { get; set; }
        public int F_Prev_Stock_Qty { get; set; }
        [StringLength(500)]
        public string? F_Remark { get; set; }
        [StringLength(20)]
        public string? F_Check_By { get; set; }
        [StringLength(20)]
        public string F_Update_By { get; set; }
        public DateTime F_Update_Date { get; set; }

    }
}
