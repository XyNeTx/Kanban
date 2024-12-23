namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_PartSpecial", Schema = "dbo")]
    [PrimaryKey("F_Plant", "F_Supplier_Cd", "F_Supplier_Plant", "F_Part_No", "F_Ruibetsu", "F_Kanban_No", "F_Store_Code", "F_Start_Date")]

    public class TB_MS_PartSpecial
    {
        [Required(ErrorMessage = "Plant is required")]
        [StringLength(1,ErrorMessage = "Plant can't be longer than 1 characters")]
        public string F_Plant { get; set; }
        [Required(ErrorMessage = "Supplier Code is required")]
        [StringLength(4, ErrorMessage = "Supplier Code can't be longer than 4 characters")]
        public string F_Supplier_Cd { get; set; }
        [Required(ErrorMessage = "Supplier Plant is required")]
        [StringLength(1, ErrorMessage = "Supplier Plant can't be longer than 1 characters")]
        public string F_Supplier_Plant { get; set; }
        [Required(ErrorMessage = "Part No is required")]
        [StringLength(10, ErrorMessage = "Part No can't be longer than 10 characters")]
        public string F_Part_No { get; set; }
        [Required(ErrorMessage = "Ruibetsu is required")]
        [StringLength(2, ErrorMessage = "Ruibetsu can't be longer than 2 characters")]
        public string F_Ruibetsu { get; set; }
        [Required(ErrorMessage = "Kanban No is required")]
        [StringLength(4, ErrorMessage = "Kanban No can't be longer than 4 characters")]
        public string F_Kanban_No { get; set; }
        [Required(ErrorMessage = "Store Code is required")]
        [StringLength(2, ErrorMessage = "Store Code can't be longer than 2 characters")]
        public string F_Store_Code { get; set; }
        [Required(ErrorMessage = "Start Date is required")]
        [StringLength(8, ErrorMessage = "Start Date can't be longer than 8 characters")]
        public string F_Start_Date { get; set; }
        [StringLength(8, ErrorMessage = "End Date can't be longer than 8 characters")]
        public string? F_End_Date { get; set; }
        [StringLength(20,ErrorMessage = "Type Special No can't be longer than 20 characters")]
        public string? F_Type_Special { get; set; }
        [StringLength(6, ErrorMessage = "Cycle can't be longer than 6 characters")]
        public string? F_Cycle { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }

    }
}


