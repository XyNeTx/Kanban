namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Matching_Supplier")]
    [PrimaryKey("F_short_Logistic", "F_short_name", "F_Supplier_CD", "F_Supplier_Plant")]

    public class TB_MS_Matching_Supplier
    {
        [Required]
        [StringLength(15)]
        public string F_short_Logistic { get; set; }
        [Required]
        [StringLength(10)]
        public string F_short_name { get; set; }
        [Required]
        [StringLength(4)]
        public string F_Supplier_CD { get; set; }
        [Required]
        [StringLength(1)]
        public string F_Supplier_Plant { get; set; }
        [Required]
        [StringLength(60)]
        public string? F_name { get; set; }
        [Required]
        [StringLength(25)]
        public string? F_Update_By { get; set; }
        [Required]
        public DateTime? F_Update_Date { get; set; }

    }
}


