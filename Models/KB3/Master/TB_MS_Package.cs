namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Package")]
    public class TB_MS_Package
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Plant { get; set; }
        public string F_Package_Type { get; set; }
        public string F_Package_Code { get; set; }
        public int? F_Qty { get; set; }
        public float? F_Package_Weight { get; set; }
        public int? F_Weight { get; set; }
        public int? F_Long { get; set; }
        public int? F_High { get; set; }
        public float? F_M3 { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }

    }
}


