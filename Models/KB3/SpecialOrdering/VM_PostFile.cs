using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    public class VM_PostFile
    {
        [Required(ErrorMessage = "Please select a file.")]
        public IFormFile File { get; set; }
    }
}
