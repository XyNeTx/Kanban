namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_SupplierAttn")]
    [PrimaryKey(nameof(F_Supplier_Code), nameof(F_Supplier_Plant))]
    public class TB_MS_SupplierAttn
    {
        [Required(ErrorMessage = "Supplier Code is required")]
        [StringLength(4)]
        public string F_Supplier_Code { get; set; }
        [Required(ErrorMessage = "Supplier Plant is required")]
        [StringLength(1)]
        public string F_Supplier_Plant { get; set; }
        [StringLength(10)]
        public string? F_Short_Name { get; set; }
        [StringLength(50)]
        public string? F_Attention { get; set; }
        [StringLength(50)]
        public string? F_Telephone { get; set; }
        [StringLength(50)]
        public string? F_Fax { get; set; }
        [StringLength(50)]
        public string? F_Create_By { get; set; }
        public DateTime? F_Create_Date { get; set; }
        [StringLength(25)]
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }

    }
}


