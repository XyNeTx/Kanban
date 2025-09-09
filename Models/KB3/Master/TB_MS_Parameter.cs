namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;


    public class TB_MS_Parameter
    {
        [Key]
        [StringLength(10)]
        public string F_Code { get; set; }
        [StringLength(50)]
        public string? F_Description { get; set; }
        public decimal? F_Value1 { get; set; }
        public short? F_Value2 { get; set; }
        [StringLength(50)]
        public string? F_Value3 { get; set; }
        public decimal? F_Value4 { get; set; }
        public decimal? F_Value5 { get; set; }
        [StringLength(10)]
        public string? F_Dept_Code { get; set; }
        [StringLength(10)]
        public string? F_Cr { get; set; }
        [StringLength(10)]
        public string? F_DR { get; set; }
        public DateTime? F_Create_Date { get; set; }
        [StringLength(25)]
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        [StringLength(25)]
        public string? F_Update_By { get; set; }

    }
}


