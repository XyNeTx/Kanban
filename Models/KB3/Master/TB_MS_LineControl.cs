using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Master
{
    public class TB_MS_LineControl
    {
        [StringLength(13)]
        [DisplayName("Line Customer")]
        [Required]
        [Key]
        public string F_Line_Customer { get; set; }
        [StringLength(2)]
        [DisplayName("Line ID")]
        [Required]
        public string F_Line_ID { get; set; }
        [StringLength(100)]
        [DisplayName("Description")]
        [Required]
        public string F_Description { get; set; }
        [StringLength(10)]
        [DisplayName("Customer")]
        [Required]
        public string F_Customer { get; set; }
        [StringLength(15)]
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
    }
}
