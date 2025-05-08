using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.OrderingProcess
{
    public class VMKBNOR121_Preview
    {
        [Required(ErrorMessage = "Please Select Action")]
        public string Action { get; set; }
        [Required(ErrorMessage = "Please Select Supplier")]
        public string Supplier { get; set; }
        public string? Kanban { get; set; }
        public string? KanbanTo { get; set; }
        public string? Store { get; set; }
        public string? StoreTo { get; set; }
        public string? PartNo { get; set; }
        public string? PartNoTo { get; set; }
        public int? intRow { get; set; }
    }
}
