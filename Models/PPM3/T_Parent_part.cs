using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.PPM3
{
    [PrimaryKey("F_Parent_part", "F_Ruibetsu", "F_store_cd", "F_TC_Str", "F_Plant_CD")]
    public class T_Parent_part
    {
        public string F_Parent_part { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_store_cd { get; set; }
        public string F_TC_Str { get; set; }
        public string? F_TC_End { get; set; }
        public string? F_name { get; set; }
        public string? F_new_part { get; set; }
        public string? F_New_Ruibetsu { get; set; }
        public string? F_new_store_cd { get; set; }
        public string? F_upd_time { get; set; }
        public string? F_commemt { get; set; }
        public short? F_qty_box { get; set; }
        public string? F_part_type { get; set; }
        public string? F_grp_cd { get; set; }
        public string? F_part_cd_HMMT { get; set; }
        public string? F_part_cd_TMT { get; set; }
        public string? F_spec { get; set; }
        public string? F_plant { get; set; }
        public string? F_Rev { get; set; }
        public string? F_inputuser { get; set; }
        public DateTime? F_inputupdate { get; set; }
        public string F_Plant_CD { get; set; }
    }
}