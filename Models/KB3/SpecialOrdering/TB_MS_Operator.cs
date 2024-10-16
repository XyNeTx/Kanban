using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    public class TB_MS_Operator
    {
        [Key]
        [StringLength(20, ErrorMessage = "UserID Tag must be less than 20 characters")]
        [Required(ErrorMessage = "UserID Tag is required")]
        [JsonPropertyName("UserID")]
        public string F_User_ID { get; set; }

        [StringLength(100, ErrorMessage = "UserName Must be less than 100 characters")]
        [JsonPropertyName("UserName")]
        public string? F_User_Name { get; set; }

        [StringLength(100, ErrorMessage = "Telelphone Must be less than 100 characters")]
        [JsonPropertyName("Telelphone")]
        public string? F_Telephone { get; set; }

        [StringLength(100, ErrorMessage = "Fax Must be less than 100 characters")]
        [JsonPropertyName("Fax")]
        public string? F_Fax { get; set; }

        [StringLength(200, ErrorMessage = "Email Must be less than 200 characters")]
        [JsonPropertyName("Email")]
        public string? F_Email { get; set; }
        
        [StringLength(100, ErrorMessage = "RecUser Must be less than 200 characters")]
        public string? F_RecUser { get; set; }

        public DateTime? F_RecDate { get; set; }
    }
}
