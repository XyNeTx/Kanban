using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialData.ViewModel
{
    public class VM_Save_KBNIM007T
    {
        [Required(ErrorMessage = "Customer Order No is required")]
        [StringLength(25, ErrorMessage = "Customer Order No must be less than 25 characters")]
        public string PO { get; set; }
        [Required(ErrorMessage = "Issued Date is required")]
        [StringLength(8, ErrorMessage = "Issued Date must be less than 8 characters")]
        public string IssuedDate { get; set; }
        [StringLength(13, ErrorMessage = "Component Part No must be less than 13 characters")]
        public string? ParentPartNo { get; set; }
        public string? ParentStore { get; set; }

        public string? CompPartNo { get; set; }
        public string? CompSebango { get; set; }
        public string? CompPartName { get; set; }
        public int? QtyBox { get; set; }
        public string? CompStoreCD { get; set; }
        public int Qty { get; set; }
        public int Trip { get; set; }
        public string DeliveryDate { get; set; }
        [StringLength(6)]
        public string? SupplierCD { get; set; }
        [Required(ErrorMessage = "Order Type is required")]
        public string TypeSpc { get; set; }
    }
}
