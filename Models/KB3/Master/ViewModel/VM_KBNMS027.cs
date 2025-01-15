using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Master.ViewModel
{
    public class VM_KBNMS027
    {
        [DisplayName("Supplier Logistic")]
        [Required]
        [StringLength(15)]
        public string F_Short_Logistic { get; set; }
        [DisplayName("Supplier Order")]
        [Required]
        [StringLength(10)]
        public string F_Short_Name { get; set; }
        [DisplayName("Supplier Code")]
        [Required]
        [StringLength(6)]
        public string F_Supplier_CD { get; set; }
        [DisplayName("Supplier Plant")]
        //[Required]
        [StringLength(2)]
        public string? F_Supplier_Plant { get; set; }
        //[DisplayName("Supplier Plant")]
        //[Required]
        //public string F_Supplier_Plant { get; set; }
        [DisplayName("Supplier Name")]
        [Required]
        [StringLength(60)]
        public string F_name { get; set; }
    }
}
