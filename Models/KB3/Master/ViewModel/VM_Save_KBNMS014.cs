using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Master.ViewModel
{
    public class VM_Save_KBNMS014
    {
        [DisplayName("Supplier Code")]
        [Required(ErrorMessage = "Supplier Code is required")]
        public string SupplierCode { get; set; }
        [DisplayName("Supplier Plant")]
        [Required(ErrorMessage = "Supplier Plant is required")]
        public string SupplierPlant { get; set; }
        [DisplayName("Supplier Name")]
        [Required(ErrorMessage = "Supplier Name is required")]
        public string SupplierName { get; set; }
        [DisplayName("Flag Print")]
        public string? FlagPrint { get; set; }
    }
}
