using KANBAN.Models.PPM3;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.PPM3
{
    [PrimaryKey("F_parent_part", "F_Ruibetsu", "F_store_cd", "F_Partial", "F_TC_Str")]
    public class T_Parents_child
    {
        public string F_parent_part { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_store_cd { get; set; }
        public short F_Partial { get; set; }
        public string F_TC_Str { get; set; }
        public string? F_TC_End { get; set; }
        public string? F_Child_part { get; set; }
        public string? F_Ch_ruibetsu { get; set; }
        public string? F_ch_store_cd { get; set; }
        public short? F_Use_pieces { get; set; }
        public string? F_Stat_Order { get; set; }
        public string? F_in_house { get; set; }
        public string? F_grp_sel { get; set; }
        public string F_sel_part { get; set; }
        public string F_sel_ruibetsu { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? F_sel_persent { get; set; }
        public short? F_Fl { get; set; }
        public string? F_Rev { get; set; }
        public string? F_inputuser { get; set; }
        public DateTime? F_inputupdate { get; set; }
    }
}