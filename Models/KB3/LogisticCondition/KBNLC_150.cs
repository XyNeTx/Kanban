using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.LogisticCondition
{

    [PrimaryKey(nameof(F_Plant), nameof(F_YM), nameof(F_Rev)
        , nameof(F_Delivery_Trip), nameof(F_Dock_Cd)
        , nameof(F_Supplier_Code), nameof(F_Supplier_Plant)
        , nameof(F_Arrival_HMMT), nameof(F_Import_By))]

    public class KBNLC_150
    {

        [StringLength(10)]
        public string F_Plant { get; set; }
        [StringLength(10)]
        public string F_YM { get; set; }
        public int F_Rev { get; set; }
        public int F_Delivery_Trip { get; set; }
        [StringLength(19)]
        public string F_Dock_Cd { get; set; }
        [StringLength(10)]
        public string F_Truck_Card { get; set; }
        [StringLength(10)]
        public string F_Tran_Type { get; set; }
        [StringLength(10)]
        public string F_Wheel { get; set; }
        [StringLength(10)]
        public string? F_Supplier_Code { get; set; }
        [StringLength(10)]
        public string? F_Supplier_Plant { get; set; }
        [StringLength(15)]
        public string F_Short_Logistic { get; set; }
        [StringLength(15)]
        public string? F_Supplier_Name { get; set; }
        [StringLength(10)]
        public string F_Arrival_Sup { get; set; }
        [StringLength(10)]
        public string F_Depart_Sup { get; set; }
        [StringLength(10)]
        public string F_Arrival_HMMT { get; set; }
        [StringLength(10)]
        public string F_Depart_HMMT { get; set; }
        [StringLength(15)]
        public string F_Cycle_Time { get; set; }
        [StringLength(50)]
        public string F_Remark { get; set; }
        [StringLength(50)]
        public string? F_Import_By { get; set; }
        public DateTime? F_Import_Date { get; set; }

    }
}
