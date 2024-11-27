using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialData.ViewModel
{
    public class VM_Save_IM007
    {
        [StringLength(25, ErrorMessage = "PDS must be less than 25 characters")]
        public string PDS { get; set; }
        [StringLength(8, ErrorMessage = "Issued Date must be less than 8 characters")]
        public string IssuedDate { get; set; }
        [StringLength(13, ErrorMessage = "Part No must be less than 13 characters")]
        public string? PartNo { get; set; }
        [StringLength(2, ErrorMessage = "Store Code must be less than 2 characters")]
        public string? StoreCD { get; set; }
        [StringLength(8, ErrorMessage = "Delivery Date must be less than 8 characters")]
        public string? DeliveryDate { get; set; }
        public int? Qty { get; set; }
        public int? Trip { get; set; }
    }
}
