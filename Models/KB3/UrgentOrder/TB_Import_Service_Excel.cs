using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KANBAN.Models.KB3.UrgentOrder
{
    [PrimaryKey(nameof(F_PO_No), nameof(F_Part_No), nameof(F_Ruibetsu), nameof(F_Delivery_Date), nameof(F_Update_By))]
    public class TB_Import_Service_Excel
    {
        [JsonProperty("Depot_Code_:")]
        [JsonPropertyName("Depot_Code_:")]
        public string? F_Dept_Code { get; set; }

        [JsonProperty("Supplier_Code_:")]
        [JsonPropertyName("Supplier_Code_:")]
        public string? F_Supplier_Code { get; set; }

        [JsonProperty("Factory_Code_:")]
        [JsonPropertyName("Factory_Code_:")]
        public string? F_Factory_Code { get; set; }

        [JsonProperty("Supplier_Name_:")]
        [JsonPropertyName("Supplier_Name_:")]
        public string? F_Supplier_Name { get; set; }

        public string? F_Invoice_No { get; set; }

        public string? F_Invoice_Date { get; set; }

        public string? F_Shipment_Date { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "numeric(18, 2)")]
        public decimal F_Total_Amount { get; set; } = 0;

        [DataType(DataType.Currency)]
        [Column(TypeName = "numeric(10, 2)")]
        public decimal F_Vat_Amount { get; set; } = 0;

        [DataType(DataType.Currency)]
        [Column(TypeName = "numeric(18, 2)")]
        public decimal F_Grand_Total { get; set; } = 0;

        public string? F_Receive_Case_No { get; set; }

        [JsonProperty("PO_NO._/_Shift_No.")]
        [JsonPropertyName("PO_NO._/_Shift_No.")]
        public string? F_PO_No { get; set; }

        [JsonProperty("PO_Item_No.")]
        [JsonPropertyName("PO_Item_No.")]
        public string? F_Item_No { get; set; }

        [JsonProperty("PO_P/No.")]
        [JsonPropertyName("PO_P/No.")]
        public string? F_Part_No { get; set; }

        public string? F_Ruibetsu { get; set; }

        [JsonProperty("Part_Name")]
        [JsonPropertyName("Part_Name")]
        public string? F_Part_Name { get; set; }

        public string? F_Supplier_Part_No { get; set; }

        [JsonProperty("PO_Qty")]
        [JsonPropertyName("PO_Qty")]
        public int F_PO_Qty { get; set; }

        [JsonProperty("PO_Date")]
        [JsonPropertyName("PO_Date")]
        public string? F_PO_Date { get; set; }

        [JsonProperty("Delivery_Date")]
        [JsonPropertyName("Delivery_Date")]
        public string? F_Delivery_Date { get; set; }

        [JsonProperty("Order_Type")]
        [JsonPropertyName("Order_Type")]
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
        public int? F_Trip_No { get; set; }

        public string? F_Destination_Code { get; set; }

        public string? F_Destination_Name { get; set; }

        public string? F_Address1 { get; set; }

        public string? F_Address2 { get; set; }

        public string? F_Customer_Order_No { get; set; }

        public string? F_Update_By { get; set; }

    }
}
