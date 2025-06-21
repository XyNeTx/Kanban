using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.KB3.OtherCondition.Model
{
    [Table("RPT_KBNOC_150", Schema = "dbo")]
    [Keyless]
    public class RPT_KBNOC_150
    {
        public string F_Plant { get; set; }
        public string F_OrderNo { get; set; }
        public string F_Supplier_Code { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_Kanban_No { get; set; }
        public string? F_Store_Cd { get; set; }
        public string F_Delivery_Date { get; set; }
        public int F_Delivery_Trip1 { get; set; }
        public int F_Delivery_Trip2 { get; set; }
        public int F_Delivery_Trip3 { get; set; }
        public int F_Delivery_Trip4 { get; set; }
        public int F_Delivery_Trip5 { get; set; }
        public int F_Delivery_Trip6 { get; set; }
        public int F_Delivery_Trip7 { get; set; }
        public int F_Delivery_Trip8 { get; set; }
        public int F_Delivery_Trip9 { get; set; }
        public int F_Delivery_Trip10 { get; set; }
        public int F_Delivery_Trip11 { get; set; }
        public int F_Delivery_Trip12 { get; set; }
        public int F_Delivery_Trip13 { get; set; }
        public int F_Delivery_Trip14 { get; set; }
        public int F_Delivery_Trip15 { get; set; }
        public int F_Delivery_Trip16 { get; set; }
        public int F_Delivery_Trip17 { get; set; }
        public int F_Delivery_Trip18 { get; set; }
        public int F_Delivery_Trip19 { get; set; }
        public int F_Delivery_Trip20 { get; set; }
        public int F_Delivery_Trip21 { get; set; }
        public int F_Delivery_Trip22 { get; set; }
        public int F_Delivery_Trip23 { get; set; }
        public int F_Delivery_Trip24 { get; set; }
        public int F_Delivery_Summary { get; set; }
        public string F_Cycle_Time { get; set; }
        public decimal F_Std { get; set; }
        public decimal F_Min { get; set; }
        public decimal F_Max { get; set; }
        public string F_Update_By { get; set; }
        public DateTime F_Update_Date { get; set; }
    }


    [Keyless]
    public class GetStd_KBNOC_150
    {
        public int?  F_Amount { get; set; }
    }
}


