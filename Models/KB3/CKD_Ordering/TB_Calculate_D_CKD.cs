using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.NewFolder
{
    [Keyless]
    public class TB_Calculate_D_CKD
    {
        // Required fields (not nullable)
        public string F_Supplier_Code { get; set; } // char(4)
        public string F_Supplier_Plant { get; set; } // char(1)
        public string F_Part_No { get; set; } // char(10)
        public string F_Ruibetsu { get; set; } // char(2)
        public string F_Store_Code { get; set; } // char(2)
        public string F_Kanban_No { get; set; } // char(4)
        public string F_Process_Date { get; set; } // char(8)
        public string F_Process_Shift { get; set; } // char(1)
        public int F_Process_Round { get; set; } // int

        // Nullable fields
        public bool? F_Work { get; set; } // bit
        public int? F_CountRound { get; set; } // int
        public byte? F_CycleB { get; set; } // tinyint
        public byte? F_CycleC { get; set; } // tinyint
        public string? F_Delivery_Date { get; set; } // char(8)
        public string? F_Delivery_Shift { get; set; } // char(1)
        public int? F_Delivery_Round { get; set; } // int
        public int? F_TMT_FO { get; set; } // int
        public int? F_MRP { get; set; } // int
        public int? F_Lot_SizeOrder { get; set; } // int
        public int? F_Qty_Box { get; set; } // int
        public int? F_KB_CUT { get; set; } // int
        public int? F_KB_ADD { get; set; } // int
        public bool? F_NON_STOP { get; set; } // bit
        public int? F_Actual_Order { get; set; } // int
        public int? F_Urgent_Order { get; set; } // int
        public int? F_UrgentTemp_Order { get; set; } // int
        public int? F_BL_Plan { get; set; } // int
        public int? F_BL_Actual { get; set; } // int
        public int? F_BL_SET_Plan { get; set; } // int
        public int? F_BL_SET_Actual { get; set; } // int
        public bool? F_Not_Recalculate { get; set; } // bit
        public bool? Flag_Chg_BL_Stock { get; set; } // bit
        public bool? Flag_HalfChg_BL_Stock { get; set; } // bit
        public string? F_Update_By { get; set; } // varchar(25)
        public DateTime? F_Update_Date { get; set; } // smalldatetime
    }
}
