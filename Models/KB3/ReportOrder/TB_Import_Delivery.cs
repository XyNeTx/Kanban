using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.ReportOrder
{
    [PrimaryKey(nameof(F_Plant), nameof(F_YM), nameof(F_Rev), nameof(F_Delivery_Trip), nameof(F_Dock_Cd), nameof(F_Supplier_Code), nameof(F_Supplier_Plant), nameof(F_Arrival_HMMT))]
    public class TB_Import_Delivery
    {
        public required char F_Plant { get; set; }
        public required string F_YM { get; set; }
        public required int F_Rev { get; set; }
        public required int F_Delivery_Trip { get; set; }
        public required string F_Dock_Cd { get; set; }
        public required string F_Truck_Card { get; set; }
        public required string F_Tran_Type { get; set; }
        public required string F_Wheel { get; set; }
        public required string F_Supplier_Code { get; set; }
        public required char F_Supplier_Plant { get; set; }
        public required string F_short_Logistic { get; set; }
        public required string F_Supplier_Name { get; set; }
        public required string F_Arrival_Sup { get; set; }
        public required string F_Depart_Sup { get; set; }
        public required string F_Arrival_HMMT { get; set; }
        public required string F_Depart_HMMT { get; set; }
        public required string F_Cycle_Time { get; set; }
        public required string F_Remark { get; set; }
        public required string F_Import_By { get; set; }
        public required DateTime F_Import_Date { get; set; }
        public required char F_Flag { get; set; }
        public DateTime? F_Confirm_Date { get; set; }
        public required string F_Remark_Maker { get; set; }
    }
}
