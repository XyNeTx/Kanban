using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.OrderingProcess
{
    [PrimaryKey(nameof(F_Supplier_Code), nameof(F_Supplier_Plant), nameof(F_Part_No), nameof(F_Ruibetsu), nameof(F_Store_Code), nameof(F_Kanban_No), nameof(F_Process_Date), nameof(F_Process_Shift), nameof(F_Process_Round))]
    public class TB_Calculate_D
    {
        public string F_Supplier_Code { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_Store_Code { get; set; }
        public string F_Kanban_No { get; set; }
        public string F_Process_Date { get; set; }
        public string F_Process_Shift { get; set; }
        public int F_Process_Round { get; set; }
        public int F_TMT_FO { get; set; }
        public int F_HMMT_Prod { get; set; }
        public int F_HMMT_Order { get; set; }
        public int F_Cycle_Order { get; set; }
        public int F_MRP { get; set; }
        public int F_Lot_SizeOrder { get; set; }
        public int F_QTY_BOX { get; set; }
        public int F_KB_CUT { get; set; }
        public int F_KB_ADD { get; set; }
        [StringLength(1)]
        public string F_KB_STOP { get; set; }
        public int F_Actual_order { get; set; }
        public int F_Urgent_Order { get; set; }
        public int F_BL_Plan { get; set; }
        public int F_BL_Actual { get; set; }
        public int F_BL_SET_Plan { get; set; }
        public int F_BL_SET_Actual { get; set; }
        public bool F_Not_Recalculate { get; set; }
        public bool Flag_Chg_BL_Stock { get; set; }
        public bool Flag_HalfChg_BL_Stock { get; set; }
        public bool Flag_Cancel_PDS { get; set; }
        public bool Flag_Chg_MRP { get; set; }
        public bool Flag_Chg_Urgent { get; set; }
        public bool Flag_Adjust_Order { get; set; }
        public string F_Update_By { get; set; }
        public DateTime F_Update_Date { get; set; }
    }
}
