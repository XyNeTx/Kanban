using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace KANBAN.Models.KB3.ImportData.ViewModel
{
    public class VM_KBNIM012M
    {
        public IFormFile? File { get; set; }

        [Required(ErrorMessage = "ProdYM is required")]
        [StringLength(6, ErrorMessage = "ProdYM must be less than 6 characters long")]

        public string ProdYM { get; set; }
        [Required(ErrorMessage = "Version is required")]
        public string Version { get; set; }

        [Required(ErrorMessage = "Revision is required")]
        [StringLength(3, ErrorMessage = "Revision must be less than 3 characters long")]
        public string Revision { get; set; }

        [Required(ErrorMessage = "Condition is required")]
        public string Condition { get; set; }
        public string? SupplierCode { get; set; }
        public string? KanbanNo { get; set; }
        public string? StoreCode { get; set; }
        public string? PartNo { get; set; }

        public string? Txt_ProdYM { get; set; }
        public string? Txt_Rev { get; set; }
        public string? Txt_Ver { get; set; }
        public string? Txt_Date { get; set; }

        public string? Txt_DateT { get; set; }
        public string? Txt_DateF { get; set; }


    }
}
