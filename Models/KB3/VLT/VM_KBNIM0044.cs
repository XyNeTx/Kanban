using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.KB3.VLT
{
    public class VM_KBNIM0044
    {
        [Column("F_Customer")]
        [StringLength(10)]
        public string F_Customer { get; set; } = null!;

        [Column("F_Date")]
        [StringLength(8)]
        public string F_Date { get; set; } = null!;

        [Column("F_Line_ID")]
        [StringLength(20)]
        public string F_LineCode { get; set; } = null!;

        [Column("F_Seq")]
        [StringLength(10)]
        public string F_Seq { get; set; } = null!;

        [Column("F_PartCode")]
        [StringLength(3)]
        public string F_PartCode { get; set; } = null!;
    }
}
