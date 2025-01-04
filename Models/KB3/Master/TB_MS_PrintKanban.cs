using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Master
{
    [PrimaryKey("F_Plant", "F_Supplier_Code", "F_Supplier_Plant")]
    public class TB_MS_PrintKanban
    {
        [Required(ErrorMessage = "Plant is required")]
        [StringLength(1)]
        public string F_Plant { get; set; }
        [Required(ErrorMessage = "Supplier Code is required")]
        [StringLength(4)]
        public string F_Supplier_Code { get; set; }
        [Required(ErrorMessage = "Supplier Plant is required")]
        [StringLength(1)]
        public string F_Supplier_Plant { get; set; }
        [Required(ErrorMessage = "Short Name is required")]
        [StringLength(80)]
        public string F_Short_Name { get; set; }
        [Required(ErrorMessage = "Flag Print is required")]
        [StringLength(1)]
        public string F_Flag_Print { get; set; }
        [Required(ErrorMessage = "Create By is required")]
        [StringLength(25)]
        public string F_Create_By { get; set; }
        [Required(ErrorMessage = "Create Date is required")]
        public DateTime F_Create_Date { get; set; }
        [Required(ErrorMessage = "Update By is required")]
        [StringLength(25)]
        public string F_Update_By { get; set; }
        [Required(ErrorMessage = "Update Date is required")]
        public DateTime F_Update_Date { get; set; }
    }
}
