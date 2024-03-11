using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.ReportOrder
{
    [Keyless]
    public class TB_Late_Deli_Rpt_TMP
    {
        public string? F_Date { get; set; }
        public string? F_Trip { get; set; }
        public char? F_Shift { get; set; }
        public string? F_Supplier_cd { get; set; }
        public string? F_Supplier { get; set; }
        public string? F_Code { get; set; }
        public string? F_Store_Code { get; set; }
        public string? F_Part_no { get; set; }
        public string? F_Ruibetsu { get; set; }
        public string? F_Part_Name { get; set; }
        public string? F_PDS_no { get; set; }
        public string? F_Usage_line { get; set; }
        public string? F_Usage_day { get; set; }
        public string? F_Safety_stk_day { get; set; }
        public string? F_Safety_stk_pcs { get; set; }
        public string? F_Plan_qty { get; set; }
        public string? F_Plan_time { get; set; }
        public string? F_Actual_qty { get; set; }
        public string? F_Actual_time { get; set; }
        public string? F_Trip_delayed { get; set; }
        public string? F_Total_delayed { get; set; }
        public string? F_Delayed_safety { get; set; }
        public string? F_EVA { get; set; }
        public string? F_Update_By { get; set; }
        public string? F_Host_Name { get; set; }
        public char? F_Plant { get; set; }
    }
}
