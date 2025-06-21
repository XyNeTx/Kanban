using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace KANBAN.Models.KB3.OtherCondition.Model
{
    [Table("RPT_KBNOC_160", Schema = "dbo")]
    [Keyless]
    public class RPT_KBNOC_160
    {
        public string F_Plant { get; set; }
        public string F_OrderNo { get; set; }
        public string F_OrderType { get; set; }
        public string F_Supplier_Cd { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_Store_Cd { get; set; }
        public string F_Kanban_NO { get; set; }
        public string F_Part_Name { get; set; }
        public string F_Delivery_Date { get; set; }
        public byte F_Delivery_Trip { get; set; }
        public int F_Qty { get; set; }
        public string F_Update_By { get; set; }
        public DateTime F_Update_Date { get; set; }
    }
}
