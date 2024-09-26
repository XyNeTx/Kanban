using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    public class VM_Save_KBNOR210_1_STC_3_1
    {
        [StringLength(25)]
        public string F_PDS_No { get; set; }
        [StringLength(25)]
        public string F_PO_Customer { get; set; }
        [StringLength(8)]
        public string F_Delivery_Date { get; set; }
        [StringLength(15)]
        public string F_Part_No { get; set; }
        [StringLength(3)]
        public string F_Store_Cd { get; set; }
        public int F_Order_Qty { get; set; }
        public int F_Use_Qty { get; set; }
        public int F_Remain_Qty { get; set; }

    }
}
