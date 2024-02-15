using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.Receive_Process
{
    [PrimaryKey(nameof(F_supplier_cd),nameof(F_Plant_cd),nameof(F_Store_cd),nameof(F_TC_Str))]
    public class T_Supplier_MS
    {
        public required string F_supplier_cd { get; set; }
        public required char F_Plant_cd { get; set; }
        public required string F_Store_cd { get; set; }
        public required string F_TC_Str { get; set; }
        public string? F_TC_End { get; set; }
        public string? F_Cycle_A { get; set; }
        public string? F_Cycle_B { get; set; }
        public string? F_Cycle_C { get; set; }
        public decimal? F_Safety_Stk { get; set; }
        public string? F_short_name { get; set; }
        public string? F_name { get; set; }
        public string? F_Logistic_cd { get; set; }
        public string? F_inputuser { get; set; }
        public DateTime F_inputupdate { get; set; }

    }
}
