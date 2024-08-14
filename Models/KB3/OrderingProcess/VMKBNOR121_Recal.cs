using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.OrderingProcess
{
    public class VMKBNOR121_Recal
    {
        [Required(ErrorMessage = "Can't Get Date")]
        public string Action { get; set; }
        [Required(ErrorMessage = "Can't Get Supplier")]
        public string Supplier { get; set; }
        [Required(ErrorMessage = "Can't Get Part No")]
        public string PartNo { get; set; }
        [Required(ErrorMessage = "Can't Get Kanban No")]
        public string Kanban { get; set; }
        [Required(ErrorMessage = "Can't Get Store Code")]
        public string Store { get; set; }
    }
}
