using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.ReportOrder
{
    [Keyless]
    public class TB_Inquriy_KB_rpt_TMP
    {
        public string? F_Part_No { get; set; }
        public string? F_kb_no { get; set; }
        public string? F_Sup_cd { get; set; }
        public string? F_Sup_name { get; set; }
        public string? F_Prt_name { get; set; }
        public string? F_str_cd { get; set; }
        public string? F_Deli_cycle { get; set; }
        public string? F_Box_qty { get; set; }
        public string? F_ord_date { get; set; }
        public char F_Shift { get; set; }
        public string? F_PDS_no { get; set; }
        public string? F_Deli_date { get; set; }
        public string? F_Deli_trip { get; set; }
        public string? F_Deli_time { get; set; }
        public string? F_Qty_kb { get; set; }
        public string? F_unit_amount { get; set; }
        public string? F_revision_no { get; set; }
        public string? F_max_forcast { get; set; }
        public string? chk_deli_date { get; set; }
        public char F_Status { get; set; }
        public string? F_Update_by { get; set; }
        public string? F_Host_Name { get; set; }
    }
}
