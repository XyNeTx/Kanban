namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_SupplierAttn")]
    public class TB_MS_SupplierAttn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Supplier_Code { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string? F_Short_Name { get; set; }
        public string? F_Attention { get; set; }
        public string? F_Telephone { get; set; }
        public string? F_Fax { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }

    }
}


