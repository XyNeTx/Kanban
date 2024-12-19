using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.OtherCondition.ViewModel
{
    public class VM_SAVE_KBNOC121
    {
        [Required(ErrorMessage = "Supplier is required")]
        [StringLength(6, ErrorMessage = "Supplier must be less than 6 characters long")]
        public string Supplier { get; set; }
        [Required(ErrorMessage = "Store Code is required")]
        [StringLength(2, ErrorMessage = "Store Code must be less than 2 characters long")]
        public string StoreCD { get; set; }
        [Required(ErrorMessage = "Part No is required")]
        [StringLength(13, ErrorMessage = "Part No must be less than 13 characters long")]
        public string PartNo { get; set; }
        [Required(ErrorMessage = "Delivery Date is required")]
        [StringLength(8, ErrorMessage = "Delivery Date must be less than 8 characters long")]
        public string DeliveryDate { get; set; }
        [Required(ErrorMessage = "Trip is required")]
        public string Trip { get; set; }
        [Required(ErrorMessage = "Slide Order is required")]
        public bool IsSlideOrder { get; set; }
        [StringLength(8, ErrorMessage = "Slide Date To From must be less than 8 characters long")]
        public string? SlideDateTo { get; set; }
        public string? TripNext { get; set; }
    }
}
