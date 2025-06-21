using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.KB3.OtherCondition.Model
{
    [Table("VW_RPT_KBNOC_160", Schema = "dbo")]
    [Keyless]
    public class VW_RPT_KBNOC_160
    {
        public string F_Plant { get; set; }
        public string F_OrderType { get; set; }
        public string F_Supplier_Code { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string? F_Delivery_Dock { get; set; }
        public string F_Part_Name { get; set; }
        public string F_Delivery_Date { get; set; }
        public int F_Delivery_Trip { get; set; }
        public int? F_Unit_Amount { get; set; }
        public string F_OrderNo { get; set; }
        public string F_Kanban_No { get; set; }

    }
}
