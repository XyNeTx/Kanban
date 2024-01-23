namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Parameter")]
    public class TB_MS_Parameter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Code { get; set; }
        public string? F_Description { get; set; }
        public float? F_Value1 { get; set; }
        public int? F_Value2 { get; set; }
        public string? F_Value3 { get; set; }
        public float? F_Value4 { get; set; }
        public float? F_Value5 { get; set; }
        public string? F_Dept_Code { get; set; }
        public string? F_Cr { get; set; }
        public string? F_DR { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }

    }
}


