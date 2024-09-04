using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KANBAN.Models.KB3.UrgentOrder
{
    [PrimaryKey(nameof(F_PO_No), nameof(F_Part_No), nameof(F_Ruibetsu)
        ,nameof(F_Delivery_Date), nameof(F_Update_By),nameof(F_PO_Date),
        nameof(F_Delivery_Qty),nameof(F_Trip_No))]

    public class TB_Import_Service_Excel
    {
        [StringLength(15)]
        public string? F_Receive_Case_No { get; set; }

        [JsonProperty("PO_NO._/_Shift_No.")]
        [JsonPropertyName("PO_NO._/_Shift_No.")]
        [StringLength(10)]
        public string? F_PO_No { get; set; }

        [JsonProperty("PO_Item_No.")]
        [JsonPropertyName("PO_Item_No.")]
        [StringLength(10)]
        public string? F_Item_No { get; set; }

        [JsonProperty("PO_P/No.")]
        [JsonPropertyName("PO_P/No.")]
        [StringLength(10)]
        public string? F_Part_No { get; set; }

        [StringLength(2)]
        public string? F_Ruibetsu { get; set; }

        [JsonProperty("Part_Name")]
        [JsonPropertyName("Part_Name")]
        [StringLength(50)]
        public string? F_Part_Name { get; set; }

        [StringLength(15)]
        public string? F_Supplier_Part_No { get; set; }

        [JsonProperty("PO_Qty")]
        [JsonPropertyName("PO_Qty")]
        public int F_PO_Qty { get; set; }

        [JsonProperty("PO_Date")]
        [JsonPropertyName("PO_Date")]
        [StringLength(10)]
        public string F_PO_Date { get; set; }

        [JsonProperty("Delivery_Date")]
        [JsonPropertyName("Delivery_Date")]
        [StringLength(10)]
        public string F_Delivery_Date { get; set; }

        [JsonProperty("Order_Type")]
        [JsonPropertyName("Order_Type")]
        [StringLength(10)]
        public string? F_Order_Type { get; set; }

        public int F_Delivery_Qty { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "numeric(18, 3)")]
        [JsonProperty("Price_/_Unit")]
        [JsonPropertyName("Price_/_Unit")]
        public decimal F_Price { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "numeric(18, 3)")]
        [JsonProperty("Amount_/_Item")]
        [JsonPropertyName("Amount_/_Item")]
        public decimal F_Amount { get; set; }

        [JsonPropertyName("Trip No")]
        public int F_Trip_No { get; set; }

        [StringLength(10)]
        public string? F_Destination_Code { get; set; }

        [StringLength(75)]
        public string? F_Destination_Name { get; set; }

        [StringLength(10)]
        public string? F_Address1 { get; set; }

        [StringLength(10)]
        public string? F_Address2 { get; set; }

        [StringLength(10)]
        public string? F_Customer_Order_No { get; set; }

        [StringLength(25)]
        public string? F_Update_By { get; set; }

    }
}
