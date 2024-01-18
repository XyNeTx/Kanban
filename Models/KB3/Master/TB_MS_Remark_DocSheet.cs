namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Remark_DocSheet")]
    public class TB_MS_Remark_DocSheet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Plant { get; set; }
        public string F_Dock_Cd { get; set; }
        public string? F_short_Logistic1 { get; set; }
        public string? F_Remark1 { get; set; }
        public string? F_short_Logistic2 { get; set; }
        public string? F_Remark2 { get; set; }
        public string? F_short_Logistic3 { get; set; }
        public string? F_Remark3 { get; set; }
        public string? F_short_Logistic4 { get; set; }
        public string? F_Remark4 { get; set; }
        public string? F_short_Logistic5 { get; set; }
        public string? F_Remark5 { get; set; }
        public string? F_short_Logistic6 { get; set; }
        public string? F_Remark6 { get; set; }
        public string? F_short_Logistic7 { get; set; }
        public string? F_Remark7 { get; set; }
        public string? F_short_Logistic8 { get; set; }
        public string? F_Remark8 { get; set; }
        public string? F_short_Logistic9 { get; set; }
        public string? F_Remark9 { get; set; }
        public string? F_short_Logistic10 { get; set; }
        public string? F_Remark10 { get; set; }
        public string? F_short_Logistic11 { get; set; }
        public string? F_Remark11 { get; set; }
        public string? F_short_Logistic12 { get; set; }
        public string? F_Remark12 { get; set; }
        public string? F_short_Logistic13 { get; set; }
        public string? F_Remark13 { get; set; }
        public string? F_short_Logistic14 { get; set; }
        public string? F_Remark14 { get; set; }
        public string? F_short_Logistic15 { get; set; }
        public string? F_Remark15 { get; set; }
        public string? F_short_Logistic16 { get; set; }
        public string? F_Remark16 { get; set; }
        public string? F_Update_By { get; set; }
        public DateTime F_Update_Date { get; set; }

    }
}


