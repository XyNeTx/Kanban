using System.Text.Json.Serialization;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    public class VM_Post_KBNOR210_STC_1
    {
        public string Action { get; set; }
        [JsonPropertyName("F_Stock_Date")]
        public string Stock_Date { get; set; }
        [JsonPropertyName("F_Supplier_code")]
        public string Supplier_Code { get; set; }
        [JsonPropertyName("F_Part_No")]
        public string Part_No { get; set; }
        [JsonPropertyName("F_Store_CD")]
        public string Store_CD { get; set; }
        [JsonPropertyName("F_Kanban_No")]
        public string Kanban_No { get; set; }
        [JsonPropertyName("F_Actual_PCS")]
        public int Actual_PCS { get; set; }
        [JsonPropertyName("F_Check_By")]
        public string? Check_Stock_By { get; set; }
        [JsonPropertyName("F_Qty_Pack")]
        public int Qty_Pack { get; set; }
        [JsonPropertyName("F_Remark")]
        public string? Remark { get; set; }

    }
}
