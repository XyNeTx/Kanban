namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_MaxArea_Stock")]
    [PrimaryKey("F_Plant", "F_Supplier_Cd", "F_Supplier_Plant"
        , "F_Part_No", "F_Ruibetsu", "F_Store_Cd", "F_Kanban_No")]
    public class TB_MS_MaxArea_Stock
    {
        [Required]
        [StringLength(1)]
        public string F_Plant { get; set; }
        [Required]
        [StringLength(4)]
        public string F_Supplier_Cd { get; set; }
        [Required]
        [StringLength(1)]
        public string F_Supplier_Plant { get; set; }
        [Required]
        [StringLength(10)]
        public string F_Part_No { get; set; }
        [Required]
        [StringLength(2)]
        public string F_Ruibetsu { get; set; }
        [Required]
        [StringLength(2)]
        public string F_Store_Cd { get; set; }
        [Required]
        [StringLength(4)]
        public string F_Kanban_No { get; set; }
        public int? F_Box_Qty { get; set; }
        [Column(TypeName = "numeric(18, 2)")]
        public decimal? F_STD_Stock { get; set; }
        public int? F_Max_Area { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime? F_Create_Date { get; set; }
        [StringLength(25)]
        public string? F_Create_By { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime? F_Update_Date { get; set; }
        [StringLength(25)]
        public string? F_Update_By { get; set; }

    }
}


