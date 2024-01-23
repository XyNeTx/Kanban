namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_RatioAddress")]
    public class TB_MS_RatioAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Plant { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_Store_Cd { get; set; }
        public string? F_Address1 { get; set; }
        public int? F_Ratio1 { get; set; }
        public string? F_Address2 { get; set; }
        public int? F_Ratio2 { get; set; }
        public string? F_Address3 { get; set; }
        public int? F_Ratio3 { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }

    }
}


