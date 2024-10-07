using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    public class VM_Save_KBNOR210_1_STC_3
    {
        public int F_Actual_Qty { get; set; }
        [StringLength(25)]
        public string F_OrderNo { get; set; }
        [StringLength(15)]
        public string F_Part_No { get; set; }
        public int F_Qty { get; set; }
        public int F_Remain { get; set; }
        [StringLength(2)]
        public string F_Store_Cd { get; set; }
        [StringLength(6)]
        public string F_Supplier { get; set; }
        public int F_Use_StockQty { get; set; }
        public string? Flag { get; set; }

    }
}
