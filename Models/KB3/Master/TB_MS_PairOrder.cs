namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_PairOrder")]
    [PrimaryKey("F_Plant", "F_Group", "F_Supplier_Cd", "F_Supplier_Plant",
        "F_Part_No", "F_Ruibetsu", "F_Kanban_No", "F_Store_Cd", "F_Start_Date")]
    public class TB_MS_PairOrder
    {
        [Required]
        [StringLength(1)]
        [DisplayName("Plant : ")]
        public string F_Plant { get; set; }
        [Required]
        [StringLength(10)]
        [DisplayName("Group Name : ")]
        public string F_Group { get; set; }
        [Required]
        [StringLength(4)]
        [DisplayName("Supplier Code : ")]
        public string F_Supplier_Cd { get; set; }
        [Required]
        [StringLength(1)]
        [DisplayName("Supplier Plant : ")]
        public string F_Supplier_Plant { get; set; }
        [Required]
        [StringLength(10)]
        [DisplayName("Part No. : ")]
        public string F_Part_No { get; set; }
        [Required]
        [StringLength(2)]
        [DisplayName("Ruibetsu : ")]
        public string F_Ruibetsu { get; set; }
        [Required]
        [StringLength(4)]
        [DisplayName("Kanban No. : ")]
        public string F_Kanban_No { get; set; }
        [Required]
        [StringLength(2)]
        [DisplayName("Store Code : ")]
        public string F_Store_Cd { get; set; }
        [Required]
        [DisplayName("Q'ty : ")]
        public int F_Qty { get; set; }
        [Required]
        [StringLength(8)]
        [DisplayName("Start Date : ")]
        public string F_Start_Date { get; set; }
        [Required]
        [StringLength(8)]
        [DisplayName("End Date : ")]
        public string F_End_Date { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }

    }
}


