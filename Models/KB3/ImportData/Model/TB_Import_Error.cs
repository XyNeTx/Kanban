using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.ImportData.Model
{
    [Keyless]
    public class TB_Import_Error
    {
        public string F_PDS_CD { get; set; }
        public int F_Row { get; set; }
        public string F_Field { get; set; }
        public string F_Remark { get; set; }
        public string F_Update_By { get; set; }
        public DateTime F_Update_Date { get; set; }
        public string F_Type { get; set; }

    }
}
