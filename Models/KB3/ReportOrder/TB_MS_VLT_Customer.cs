using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.ReportOrder
{
    [PrimaryKey(nameof(F_Customer_Cd), nameof(F_Seq_Type))]
    public class TB_MS_VLT_Customer
    {
        public required string F_Customer_Cd { get; set; }
        public required string F_Customer { get; set; }
        public required string F_Line_Prod { get; set; }
        public required string F_Seq_Type { get; set; }
        public required string F_Update_By { get; set; }
        public required DateTime F_Update_Date { get; set; }
    }
}
