using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Master.ViewModel
{
    public class VM_KBNMS020
    {
        [Required]
        [StringLength(6)]
        [DisplayName("Supplier Code")]
        public string F_Supplier { get; set; }
        [Required]
        [StringLength(13)]
        [DisplayName("Part No")]
        public string F_PartNo { get; set; }
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
        [Required]
        [DisplayName("Max Area")]
        public int F_Max_Area { get; set; }

        [Required]
        [DisplayName("Box Q'ty")]
        public int F_BoxQty { get; set; }
    }
}
