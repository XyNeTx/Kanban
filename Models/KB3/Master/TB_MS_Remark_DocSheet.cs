namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Remark_DocSheet")]
    [PrimaryKey("F_Plant", "F_Dock_Cd")]
    public class TB_MS_Remark_DocSheet
    {
        [StringLength(1)]
        ////[Required]
        [DisplayName("Plant : ")]
        public string F_Plant { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Dock Code : ")]
        public string F_Dock_Cd { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP1 : ")]
        public string F_short_Logistic1 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark1 : ")]
        public string F_Remark1 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP2 : ")]
        public string F_short_Logistic2 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark2 : ")]
        public string F_Remark2 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP3 : ")]
        public string F_short_Logistic3 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark3 : ")]
        public string F_Remark3 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP4 : ")]
        public string F_short_Logistic4 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark4 : ")]
        public string F_Remark4 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP5 : ")]
        public string F_short_Logistic5 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark5 : ")]
        public string F_Remark5 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP6 : ")]
        public string F_short_Logistic6 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark6 : ")]
        public string F_Remark6 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP7 : ")]
        public string F_short_Logistic7 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark7 : ")]
        public string F_Remark7 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP8 : ")]
        public string F_short_Logistic8 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark8 : ")]
        public string F_Remark8 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP9 : ")]
        public string F_short_Logistic9 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark9 : ")]
        public string F_Remark9 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP10 : ")]
        public string F_short_Logistic10 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark10 : ")]
        public string F_Remark10 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP11 : ")]
        public string F_short_Logistic11 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark11 : ")]
        public string F_Remark11 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP12 : ")]
        public string F_short_Logistic12 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark12 : ")]
        public string F_Remark12 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP13 : ")]
        public string F_short_Logistic13 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark13 : ")]
        public string F_Remark13 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP14 : ")]
        public string F_short_Logistic14 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark14 : ")]
        public string F_Remark14 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP15 : ")]
        public string F_short_Logistic15 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark15 : ")]
        public string F_Remark15 { get; set; }

        [StringLength(15)]
        ////[Required]
        [DisplayName("Supplier LP16 : ")]
        public string F_short_Logistic16 { get; set; }

        [StringLength(50)]
        ////[Required]
        [DisplayName("Remark16 : ")]
        public string F_Remark16 { get; set; }

        [StringLength(25)]
        ////[Required]
        [DisplayName("Update By : ")]
        public string F_Update_By { get; set; }

        ////[Required]
        public DateTime F_Update_Date { get; set; }

    }
}


