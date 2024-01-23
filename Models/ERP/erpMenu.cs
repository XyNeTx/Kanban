namespace HINOSystem.Models.ERP
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Numerics;

    [Table("Menu", Schema ="erp")]
    public class erpMenu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "bigint")]
        public int _ID { get; set; }

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

        public int? isDelete { get; set; }



    }
}


