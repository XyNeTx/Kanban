using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KANBAN.Models.KB3.LogisticCondition
{

    public class VM_TB_Import_Delivery
    {
        [StringLength(1)]
        public string F_Plant { get; set; }
        [StringLength(6)]
        public string F_YM { get; set; }
        public int F_Rev { get; set; }
        public int F_Delivery_Trip { get; set; }
        [StringLength(5)]
        [JsonPropertyName("F_Dock_Cd 5 Digit")]
        public string F_Dock_Cd { get; set; }
        [StringLength(10)]
        [JsonPropertyName("F_Truck_Card 6 Digit")]
        public string F_Truck_Card { get; set; }
        [StringLength(10)]
        [JsonPropertyName("F_Tran_Type 8 Digit")]
        public string F_Tran_Type { get; set; }
        [StringLength(10)]
        [JsonPropertyName("F_Wheel 8 Digit")]
        public string F_Wheel { get; set; }
        public string? F_Supplier_Code { get; set; }
        public string? F_Supplier_Plant { get; set; }
        [StringLength(15)]
        [JsonPropertyName("F_short_Logistic 15 Digit")]
        public string F_Short_Logistic { get; set; }
        [StringLength(15)]
        public string? F_Supplier_Name { get; set; }
        [StringLength(5)]
        [JsonPropertyName("F_Arrival_Sup 5 Digit")]
        public string F_Arrival_Sup { get; set; }
        [StringLength(5)]
        [JsonPropertyName("F_Depart_Sup 5 Digit")]
        public string F_Depart_Sup { get; set; }
        [StringLength(5)]
        [JsonPropertyName("F_Arrival_HMMT 5 Digit")]
        public string F_Arrival_HMMT { get; set; }
        [StringLength(5)]
        [JsonPropertyName("F_Depart_HMMT 5 Digit")]
        public string F_Depart_HMMT { get; set; }
        [StringLength(15)]
        [JsonPropertyName("F_Cycle_Time 15 Digit")]
        public string F_Cycle_Time { get; set; }
        [StringLength(50)]
        [JsonPropertyName("F_Remark 50 Digit")]
        public string? F_Remark { get; set; }
        public string? F_Import_By { get; set; }
        public DateTime? F_Import_Date { get; set; }
        public string? F_Flag { get; set; }
        public DateTime? F_Confirm_Date { get; set; }
        public string? F_Remark_Maker { get; set; }

    }
}
