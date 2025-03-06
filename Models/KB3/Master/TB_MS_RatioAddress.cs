namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_RatioAddress")]
    [PrimaryKey("F_Plant", "F_Part_No", "F_Ruibetsu", "F_Store_Cd")]
    public class TB_MS_RatioAddress
    {
        //[Required]
        [DisplayName("Plant")]
        [StringLength(1)]
        public string F_Plant { get; set; }
        [Required(ErrorMessage = "Please Select Part Number Before Process Data")]
        [DisplayName("Part No. : ")]
        [StringLength(13)]
        public string F_Part_No { get; set; }
        //[Required]
        [DisplayName("Ruibetsu : ")]
        [StringLength(2)]
        public string F_Ruibetsu { get; set; }
        [Required(ErrorMessage = "Please Select Store Code Before Process Data")]
        [DisplayName("Store Code : ")]
        [StringLength(2)]
        public string F_Store_Cd { get; set; }
        [Required(ErrorMessage = "Please Input Address1 Before Process Data")]
        [DisplayName("Address1 : ")]
        [StringLength(10)]
        public string F_Address1 { get; set; }
        [Required(ErrorMessage = "Please Input Ratio1 Before Process Data")]
        [DisplayName("Ratio1 : ")]
        public int F_Ratio1 { get; set; }
        [DisplayName("Address2 : ")]
        [StringLength(10)]
        public string? F_Address2 { get; set; }
        [DisplayName("Ratio2 : ")]
        public int? F_Ratio2 { get; set; }
        [DisplayName("Address3 : ")]
        [StringLength(10)]
        public string? F_Address3 { get; set; }
        [DisplayName("Ratio3 : ")]
        public int? F_Ratio3 { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }

    }
}


