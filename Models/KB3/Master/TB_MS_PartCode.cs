namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_PartCode")]
    public class TB_MS_PartCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Line { get; set; }
        public string F_Code { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string? F_name { get; set; }
        public string? F_Bridge { get; set; }
        public string? F_Detail { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }

    }
}


