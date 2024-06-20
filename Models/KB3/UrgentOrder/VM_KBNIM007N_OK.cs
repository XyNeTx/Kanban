using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KANBAN.Models.KB3.UrgentOrder
{
    public class VM_KBNIM007N_OK
    {
        public string? F_PDS_No { get; set; }

        [JsonPropertyName("Supplier")]
        public string F_Supplier { get; set; }

        [JsonPropertyName("Part_No")]
        public string F_Part_No { get; set; }

        [JsonPropertyName("Kanban_No")]
        public string F_Kanban_No { get; set; }

        public int? F_Pack { get; set; }
        [JsonPropertyName("Qty")]
        public int F_Qty { get; set; }
        [JsonPropertyName("Delivery_Date")]
        public string F_Delivery_Date { get; set; }
        [JsonPropertyName("Delivery_Trip")]
        public int F_Delivery_Trip { get; set; }
    }
}
