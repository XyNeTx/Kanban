using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.ReportOrder
{
    [Keyless]
    public class V_KBNRT_110_rpt
    {
        public int? F_Rev { get; set; }
        public string? Production_Month { get; set; }
        public string? Sup_Cd { get; set; }
        public string? Sup_Chk { get; set; }
        public string? F_Supplier_Name { get; set; }
        public string? F_Dock_Cd { get; set; }
        public int? F_Delivery_Trip { get; set; }
        public string? F_Arrival_Sup { get; set; }
        public string? F_Depart_Sup { get; set; }
        public string? F_Arrival_HMMT { get; set; }
        public string? F_Depart_HMMT { get; set; }
        public string? F_Supplier_Plant { get; set; }
        public string? Chk_Month { get; set; }
        public string? F_Tran_Type { get; set; }
    }
}
