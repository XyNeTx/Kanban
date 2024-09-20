using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    public class VM_Save_KBNOR210_1
    {
        [StringLength(25)]
        [Required]
        public string F_PDS_No { get; set; }
        [StringLength(25)]
        [Required]
        public string F_PDS_No_New { get; set; }
        public int F_Qty { get; set; }
        [StringLength(20)]
        [Required]
        public string F_Part_No { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Delivery_Date { get; set; }
        [StringLength(10)]
        [Required]
        public string F_Delivery_Date_New { get; set; }
        [StringLength(6)]
        [Required]
        public string F_Supplier_CD { get; set; }
    }
}
