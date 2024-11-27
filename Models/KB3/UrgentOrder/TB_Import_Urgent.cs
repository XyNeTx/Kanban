using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.UrgentOrder
{
    [PrimaryKey(nameof(F_Supplier), nameof(F_Part_No), nameof(F_Kanban_No), nameof(F_Delivery_Trip),nameof(F_Delivery_Date), nameof(F_Update_By))]
    public class TB_Import_Urgent
    {
        [StringLength(6)]
        [Required]
        public string F_Supplier { get; set; }
        [StringLength(12)]
        [Required]
        public string F_Part_No { get; set; }
        [StringLength(4)]
        [Required]
        public string F_Kanban_No { get; set; }
        public int? F_Pack { get; set; }
        public int? F_Qty { get; set; }
        [StringLength(8)]
        [Required]
        public string F_Delivery_Date { get; set; }
        [Required]
        public int F_Delivery_Trip { get; set; }
        [StringLength(150,ErrorMessage = ("Remark must be less than 150 characters"))]
        public string? F_Remark { get; set; }
        [StringLength(25)]
        [Required]
        public string F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
    }
}
