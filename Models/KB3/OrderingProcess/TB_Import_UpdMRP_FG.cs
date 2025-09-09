using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KANBAN.Models.KB3.OrderingProcess
{
    [PrimaryKey(nameof(F_Date), nameof(F_Part_NO), nameof(F_Store_Code),nameof(F_Update_By))]
    public class TB_Import_UpdMRP_FG
    {
        [Required]
        [StringLength(8)]
        [JsonPropertyName("Date")]
        public string F_Date { get; set; }

        [Required]
        [StringLength(12)]
        [JsonPropertyName("ParentPart")]
        public string F_Part_NO { get; set; }

        [Required]
        [StringLength(2)]
        [JsonPropertyName("Store")]
        public string F_Store_Code { get; set; }

        [JsonPropertyName("MRP")]
        public int? F_MRP_Qty { get; set; } = 0;

        [StringLength(25)]
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
    }
}
