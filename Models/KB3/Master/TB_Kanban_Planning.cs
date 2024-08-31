using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KANBAN.Models.KB3.Master
{

    [PrimaryKey(nameof(F_Plant),nameof(F_Supplier_Code),nameof(F_Supplier_Plant)
        ,nameof(F_Store_Code),nameof(F_Kanban_No),nameof(F_Part_No),
        nameof(F_Ruibetsu),nameof(F_Delivery_Date),nameof(F_Delivery_Trip))]

    public class TB_Kanban_Planning
    {
        [StringLength(1)]
        public string F_Plant { get; set; }
        [StringLength(4)]
        public string F_Supplier_Code { get; set; }
        [StringLength(1)]
        public string F_Supplier_Plant { get; set; }
        [StringLength(2)]
        public string F_Store_Code { get; set; }
        [StringLength(4)]
        public string F_Kanban_No { get; set; }
        [StringLength(10)]
        public string F_Part_No { get; set; }
        [StringLength(2)]
        public string F_Ruibetsu { get; set; }
        [StringLength(1)]
        public string F_Status { get; set; }
        [StringLength(8)]
        public string F_Delivery_Date { get; set; }
        [StringLength(2)]
        public string F_Delivery_Trip { get; set; }
        public int F_Order_PCS { get; set; }
        public int F_Order_KB { get; set; }
        public int F_Qty_Pack { get; set; }
        [StringLength(8)]
        public string F_Cycle { get; set; }
        public DateTime F_Create_Date { get; set; }
        [StringLength(25)]
        public string F_Create_By { get; set; }
        public DateTime F_Update_Date { get; set; }
        [StringLength(25)]
        public string F_Update_By { get; set; }


    }
}
