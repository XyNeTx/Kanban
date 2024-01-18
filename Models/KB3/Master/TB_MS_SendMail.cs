namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_SendMail")]
    public class TB_MS_SendMail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string? F_mail { get; set; }
        public string? F_Flag { get; set; }
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }

    }
}


