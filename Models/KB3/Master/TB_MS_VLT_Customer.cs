namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_VLT_Customer")]
    public class TB_MS_VLT_Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Customer_Cd { get; set; }
        public string? F_Customer { get; set; }
        public string? F_Line_Prod { get; set; }
        public string F_Seq_Type { get; set; }
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }

    }
}


