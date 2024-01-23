namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_CTL")]
    public class TB_MS_CTL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string F_Plant { get; set; }
        public string F_Shift { get; set; }
        public string? F_Start_Time { get; set; }
        public string? F_End_Time { get; set; }
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
    }
}


