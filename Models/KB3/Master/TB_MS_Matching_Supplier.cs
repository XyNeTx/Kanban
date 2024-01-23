namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Matching_Supplier")]
    public class TB_MS_Matching_Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_short_Logistic { get; set; }
        public string F_short_name { get; set; }
        public string F_Supplier_CD { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string? F_name { get; set; }
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }

    }
}


