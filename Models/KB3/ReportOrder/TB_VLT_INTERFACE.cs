using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.ReportOrder
{
    [PrimaryKey(nameof(F_System_Flag), nameof(F_Prod_Plan_Date), nameof(F_Prod_Seq), nameof(F_Ident_No), nameof(F_Jig_In_Seq))]
    public class TB_VLT_INTERFACE
    {
        public required string F_System_Flag { get; set; }
        public required string F_Prod_Plan_Date { get; set; }
        public required string F_Prod_Seq { get; set; }
        public required string F_Ident_No { get; set; }
        public required string F_Jig_In_Seq { get; set; }
        public required string F_Part_Code { get; set; }
        public required string F_Date_Stamp { get; set; }
        public required string F_Time_Stamp { get; set; }
        public required char F_Send_Flag { get; set; }
        public required char F_Process_Flag { get; set; }
        public required string F_Line_Code { get; set; }
        public required string F_Update_By { get; set; }
        public required DateTime F_Update_Date { get; set; }
    }
}
