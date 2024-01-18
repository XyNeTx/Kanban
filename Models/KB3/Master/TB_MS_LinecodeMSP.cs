namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_LinecodeMSP")]
    public class TB_MS_LinecodeMSP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Plant { get; set; }
        public string F_Line_Code { get; set; }
        public string? F_Type { get; set; }
        public string? F_Remark { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }

    }
}


