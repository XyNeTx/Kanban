using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    public class VM_Post_Tag_Color
    {
        [StringLength(30, ErrorMessage = "Color Tag must be less than 30 characters")]
        [Required(ErrorMessage = "Color Tag is required")]
        [JsonPropertyName("COLOR")]
        public string F_Color_Tag { get; set; }

        [StringLength(10, ErrorMessage = "Type Must be less than 10 characters")]
        [JsonPropertyName("TypePart")]
        public string? F_Type { get; set; }

        [StringLength(25, ErrorMessage = "User Must be less than 25 characters")]
        public string? F_RecUser { get; set; }

        public DateTime? F_RecDate { get; set; }

    }
}
