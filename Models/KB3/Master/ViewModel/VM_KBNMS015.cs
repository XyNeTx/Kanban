using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Master.ViewModel
{
    public class VM_KBNMS015
    {
        [DisplayName("Supplier Code")]
        [Required]
        [StringLength(6)]
        public string F_Supplier_Code { get; set; }

        [DisplayName("Kanban No")]
        [Required]
        [StringLength(4)]
        public string F_Kanban_No { get; set; }

        [DisplayName("Store Code")]
        [Required]
        [StringLength(2)]
        public string F_Store_Code { get; set; }

        [DisplayName("Part No")]
        [Required]
        [StringLength(15)]
        public string F_Part_No { get; set; }

        [DisplayName("Start Date")]
        [Required]
        [StringLength(10)]
        public string F_Start_Date { get; set; }

        [DisplayName("End Date")]
        [Required]
        [StringLength(10)]
        public string F_End_Date { get; set; }

        [DisplayName("Color")]
        [StringLength(50)]
        public string? F_Color { get; set; }

        [DisplayName("Description/Colour of tag")]
        [StringLength(20)]
        public string? F_Description { get; set; }

        [DisplayName("Cycle")]
        [StringLength(6)]
        public string? F_Cycle { get; set; }
    }
}
