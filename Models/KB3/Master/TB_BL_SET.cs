using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Master
{
    [PrimaryKey(nameof(F_Date), nameof(F_Shift), nameof(F_Part_No), nameof(F_Ruibetsu), nameof(F_Sebango), nameof(F_Store_Cd), nameof(F_Sup_Cd), nameof(F_Sup_Plant))]
    public class TB_BL_SET
    {
        [Required]
        [StringLength(8)]
        public string F_Date { get; set; }
        [Required]
        [StringLength(1)]
        public string F_Shift { get; set; }
        [Required]
        [StringLength(10)]
        public string F_Part_No { get; set; }
        [Required]
        [StringLength(2)]
        public string F_Ruibetsu { get; set; }
        [Required]
        [StringLength(4)]
        public string F_Sebango { get; set; }
        [Required]
        [StringLength(2)]
        public string F_Store_Cd { get; set; }
        public int F_BL { get; set; }
        [Required]
        [StringLength(4)]
        public string F_Sup_Cd { get; set; }
        [Required]
        [StringLength(1)]
        public string F_Sup_Plant { get; set; }
        public int F_Qty { get; set; }
        public int F_BL_Kanban { get; set; }
        public int F_Bl_PCS { get; set; }
        [Required]
        [StringLength(25)]
        public string F_Update_By { get; set; }
        public DateTime F_Update_Date { get; set; }
    }
}
