using DocumentFormat.OpenXml.Bibliography;
using NPOI.Util;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Web.Services.Description;

namespace KANBAN.Models.KB3.UrgentOrder
{
    public class VM_KBNIM017R_ImportData
    {
        [JsonPropertyName("O1")]
        [Required(ErrorMessage = "Delivery Qty. is required.")]
        public int DeliveryQty { get; set; }

        [JsonPropertyName("Last Print")]
        public string? LastPrint { get; set; }

        [JsonPropertyName("Lot Size")]
        //[Required(ErrorMessage = "Lot Size is required.")]
        public int? LotSize { get; set; }

        [JsonPropertyName("No")]
        public int? No { get; set; }

        [JsonPropertyName("N1")]
        [Required(ErrorMessage = "Packs is required.")]
        public int Packs { get; set; }

        [JsonPropertyName("M1")]
        [Required(ErrorMessage = "Part Name is required.")]
        public string PartName { get; set; }

        [JsonPropertyName("A1")]
        [Required(ErrorMessage = "PDS No is required.")]
        public string PDS_No { get; set; }

        [JsonPropertyName("L1")]
        [Required(ErrorMessage = "Part No is required.")]
        public string PartNo { get; set; }

        [JsonPropertyName("V1")]
        [Required(ErrorMessage = "Sebango is required.")]
        public string Sebango { get; set; }

        [JsonPropertyName("P1")]
        [Required(ErrorMessage = "Unit Price is required.")]
        public float UnitPrice { get; set; }

        [JsonPropertyName("F1")]
        [Required(ErrorMessage = "Sebango is required.")]
        public string DeliveryDate { get; set; }
    }
}