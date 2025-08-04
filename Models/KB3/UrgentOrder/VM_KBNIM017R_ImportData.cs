using DocumentFormat.OpenXml.Bibliography;
using NPOI.Util;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Web.Services.Description;

namespace KANBAN.Models.KB3.UrgentOrder
{
    public class VM_KBNIM017R_ImportData
    {
        [JsonPropertyName("Delivery Qty.")]
        [Required(ErrorMessage = "Delivery Qty. is required.")]
        public int DeliveryQty { get; set; }

        [JsonPropertyName("Last Print")]
        public string? LastPrint { get; set; }

        [JsonPropertyName("Lot Size")]
        [Required(ErrorMessage = "Lot Size is required.")]
        public int LotSize { get; set; }

        [JsonPropertyName("No")]
        public int? No { get; set; }

        [JsonPropertyName("Packs")]
        [Required(ErrorMessage = "Packs is required.")]
        public int Packs { get; set; }

        [JsonPropertyName("Part Name")]
        [Required(ErrorMessage = "Part Name is required.")]
        public string PartName { get; set; }

        [JsonPropertyName("Part No")]
        [Required(ErrorMessage = "Part No is required.")]
        public string PartNo { get; set; }

        [JsonPropertyName("Sebango")]
        [Required(ErrorMessage = "Sebango is required.")]
        public string Sebango { get; set; }

        [JsonPropertyName("Unit Price(รวม VAT)")]
        [Required(ErrorMessage = "Unit Price is required.")]
        public float UnitPrice { get; set; }
    }
}