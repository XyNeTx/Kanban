using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.OrderingProcess
{
    [PrimaryKey(nameof(F_Supplier_Code), nameof(F_Supplier_Plant), nameof(F_Part_No), nameof(F_Ruibetsu), nameof(F_Store_Code), nameof(F_Kanban_No))]
    public class TB_MS_Inform_News
    {
        [StringLength(4)]
        [Required(ErrorMessage = "Please Input Supplier Code")]
        public string F_Supplier_Code { get; set; }
        [StringLength(1)]
        [Required(ErrorMessage = "Please Input Supplier Plant")]
        public string F_Supplier_Plant { get; set; }
        [StringLength(10)]
        [Required(ErrorMessage = "Please Input Part No")]
        public string F_Part_No { get; set; }
        [StringLength(2)]
        [Required(ErrorMessage = "Please Input Ruibetsu")]
        public string F_Ruibetsu { get; set; }
        [StringLength(2)]
        [Required(ErrorMessage = "Please Input Store Code")]
        public string F_Store_Code { get; set; }
        [StringLength(4)]
        [Required(ErrorMessage = "Please Input Kanban No")]
        public string F_Kanban_No { get; set; }
        [StringLength(1500)]
        public string? F_Text { get; set; }
        [StringLength(100)]
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
    }
}
