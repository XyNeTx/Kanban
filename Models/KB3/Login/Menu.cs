using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.KB3.Login
{
    public class Menu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 _ID { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? NameTH { get; set; }
        public string? NameJP { get; set; }
        public string? Title { get; set; }
        public string? TitleTH { get; set; }
        public string? TitleJP { get; set; }
        public string? Icon { get; set; }
        public string? i18n { get; set; }
        public string? Status { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? CreateBy { get; set; }
        public int isDelete { get; set; }
    }
}
