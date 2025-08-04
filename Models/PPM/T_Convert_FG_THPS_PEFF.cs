using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.PPM
{
    [PrimaryKey("F_Part_no_PEFF", "F_Ruibetsu_PEFF", "F_Location", "F_TC_Str")]
    public class T_Convert_FG_THPS_PEFF
    {
        public string F_Fg_Part_no { get; set; }
        public string F_Part_no_PEFF { get; set; }
        public string F_Ruibetsu_PEFF { get; set; }
        public string F_Location { get; set; }
        public string F_TC_Str { get; set; }
        public string F_TC_End { get; set; }
        public string F_Update_Date { get; set; }
        public string F_Update_By { get; set; }
    }
}
