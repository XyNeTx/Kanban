using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.ServiceData.ViewModel
{
    public class VM_KBNIM001C_Confirm
    {
        [Required(ErrorMessage = "PDS No can't be null")]
        public string F_PDS_No { get; set; }

        [Required(ErrorMessage = "PDS Issued Date can't be null")]
        public string F_PDS_Issued_Date { get; set; }

        [Required(ErrorMessage = "Delivery Date can't be null")]
        public string F_Delivery_Date { get; set; }
    }
}
