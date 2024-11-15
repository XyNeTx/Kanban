using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    [PrimaryKey(nameof(F_OrderNo), nameof(F_Part_No), nameof(F_Ruibetsu), nameof(F_Kanban_No)
        ,nameof(F_No))]
    public class TB_PDS_Detail
    {
        [StringLength(13)]
        public string F_OrderNo { get; set; }
        [StringLength(10)]
        public string F_Part_No { get; set; }
        [StringLength(2)]
        public string F_Ruibetsu { get; set; }
        [StringLength(4)]
        public string F_Kanban_No { get; set; }
        public int F_Box_Qty { get; set; }
        [Column(TypeName = "numeric(18, 2)")]
        public decimal F_Unit_price { get; set; }
        public int F_No { get; set; }
        public int F_Unit_Amount { get; set; }
        public int F_Receive_Amount { get; set; }
        public DateTime F_Receive_Date { get; set; }
        [StringLength(50)]
        public string F_Part_Name { get; set; }
        [StringLength(20)]
        public string F_Address { get; set; }
        [StringLength(50)]
        public string F_Inf_KB { get; set; }
        [StringLength(10)]
        public string F_Dock_CD { get; set; }
        [StringLength(25)]
        public string F_Update_By { get; set; }
        public DateTime F_Update_Date { get; set; }
        [StringLength(5)]
        public string? F_PDS_Group { get; set; }
    }
}
