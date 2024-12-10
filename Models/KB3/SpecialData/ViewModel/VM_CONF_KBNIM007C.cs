using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialData.ViewModel
{
    public class VM_CONF_KBNIM007C
    {
        [Required(ErrorMessage = "PDS No is required")]
        public string F_PDS_No { get; set; }
        [Required(ErrorMessage = "Issued Date is required")]
        public string F_PDS_ISSUED_DATE { get; set; }
        [Required(ErrorMessage = "Delivery Date is required")]
        public string F_Delivery_Date { get; set; }
    }
}
