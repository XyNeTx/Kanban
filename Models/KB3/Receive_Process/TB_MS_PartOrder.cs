using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.Receive_Process
{
    [PrimaryKey(nameof(F_Plant), nameof(F_Supplier_Cd), nameof(F_Supplier_Plant), nameof(F_Part_No), nameof(F_Ruibetsu), nameof(F_Kanban_No), nameof(F_Store_Code), nameof(F_Start_Date))]
    public class TB_MS_PartOrder
    {
        public required char F_Plant { get; set; }
        public required string F_Supplier_Cd { get; set; }
        public required char F_Supplier_Plant { get; set; }
        public required string F_Part_No { get; set; }
        public required string F_Ruibetsu { get; set; }
        public required string F_Kanban_No { get; set; }
        public required string F_Store_Code { get; set; }
        public required string F_Start_Date { get; set; }
        public string? F_End_Date { get; set; }
        public string? F_Type_Order { get; set; }
        public string? F_Cycle { get; set; }
        public byte? F_Flg_ClearModule { get; set; }
        public string? F_PDS_Group { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_by { get; set; }
        public required int F_Check_Shift { get; set; }
        public required string F_Last_Check { get; set; }
        public required string F_Next_Check { get; set; }

    }
}
