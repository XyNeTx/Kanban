namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Print_Replace_KB")]
    [PrimaryKey("F_Supplier_Code", "F_Supplier_Plant", "F_Store_Code", "F_Kanban_No", "F_Part_No", "F_Ruibetsu", "F_Supply_Code", "F_Update_By")]
    public class TB_MS_Print_Replace_KB
    {
        [StringLength(10,ErrorMessage = "Supplier Code cannot be longer than 10 characters.")]
        [Required(ErrorMessage = "Supplier Code is required.")]
        public string F_Supplier_Code { get; set; }
        [StringLength(10, ErrorMessage = "Supplier Plant cannot be longer than 10 characters.")]
        [Required(ErrorMessage = "Supplier Plant is required.")]
        public string F_Supplier_Plant { get; set; }
        [StringLength(2, ErrorMessage = "Store Code cannot be longer than 2 characters.")]
        [Required(ErrorMessage = "Store Code is required.")]
        public string F_Store_Code { get; set; }
        [StringLength(10, ErrorMessage = "Kanban No cannot be longer than 10 characters.")]
        [Required(ErrorMessage = "Kanban No is required.")]
        public string F_Kanban_No { get; set; }
        [StringLength(10, ErrorMessage = "Part No cannot be longer than 10 characters.")]
        [Required(ErrorMessage = "Part No is required.")]
        public string F_Part_No { get; set; }
        [StringLength(10, ErrorMessage = "Ruibetsu cannot be longer than 10 characters.")]
        [Required(ErrorMessage = "Ruibetsu is required.")]
        public string F_Ruibetsu { get; set; }
        [StringLength(10, ErrorMessage = "Supply Code cannot be longer than 10 characters.")]
        [Required(ErrorMessage = "Supply Code is required.")]
        public string F_Supply_Code { get; set; }
        public int? F_Number { get; set; }
        public DateTime? F_Update_Date { get; set; }
        [StringLength(25, ErrorMessage = "Update By cannot be longer than 25 characters.")]
        [Required(ErrorMessage = "Update By is required.")]
        public string F_Update_By { get; set; }

    }
}


