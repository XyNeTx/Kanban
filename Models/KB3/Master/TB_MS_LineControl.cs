using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Master
{
    public class TB_MS_LineControl
    {
        [StringLength(2)]
        [DisplayName("Line ID")]
        [Required]
        [Key]
        public string F_Line_ID { get; set; }
        [StringLength(100)]
        [DisplayName("Description")]
        [Required]
        public string F_Description { get; set; }
        [StringLength(10)]
        [DisplayName("Customer")]
        [Required]
        public string F_Customer { get; set; }
    }
}
