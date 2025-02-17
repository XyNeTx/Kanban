using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Master.ViewModel
{
    public class VM_KBNMS019
    {
        [Required]
        [StringLength(6)]
        [DisplayName("Supplier")]
        public string F_Supplier { get; set; }
        [Required]
        [DisplayName("Total Max/Trip")]
        public int F_Total_Max_Trip { get; set; }
        [Required]
        [StringLength(13)]
        [DisplayName("Part No")]
        public string F_PartNo { get; set; }
        public string F_PartName { get; set; }
        [Required]
        [StringLength(2)]
        [DisplayName("Store Code")]
        public string F_StoreCD { get; set; }
        [Required]
        [StringLength(4)]
        [DisplayName("Kanban No.")]
        public string F_KanbanNo { get; set; }
        //[DisplayName("Q'ty")]
        //public int F_Qty { get; set; }
        [DisplayName("Max/Trip")]
        public int F_Max_Trip { get; set; }
    }
}
