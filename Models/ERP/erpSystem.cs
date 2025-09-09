namespace HINOSystem.Models.ERP
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("System", Schema ="erp")]
    public class erpSystem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "bigint")]
        public int _ID { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string? NameTH { get; set; }

        public string? Status { get; set; }

        public DateTime? UpdateAt { get; set; }

        public string? UpdateBy { get; set; }

        public DateTime? CreateAt { get; set; }

        public string? CreateBy { get; set; }

        public int? isDelete { get; set; }

    }
}


