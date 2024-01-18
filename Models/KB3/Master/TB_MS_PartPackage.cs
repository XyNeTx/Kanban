namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_PartPackage")]
    public class TB_MS_PartPackage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Plant { get; set; }
        public string F_Supplier_Cd { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_Store_Cd { get; set; }
        public string? F_short_Logistic { get; set; }
        public string? F_Short_Name { get; set; }
        public string F_Start_Date { get; set; }
        public string? F_End_Date { get; set; }
        public string? F_Package_Type { get; set; }
        public string? F_Package_Code { get; set; }
        public float? F_Part_Weight { get; set; }
        public int? F_Qty { get; set; }
        public float? F_Total { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }

    }
}


