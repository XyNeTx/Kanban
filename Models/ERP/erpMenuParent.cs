namespace HINOSystem.Models.ERP
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MenuParent", Schema ="erp")]
    public class erpMenuParent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "bigint")]
        public int _ID { get; set; }

        [Column(TypeName = "bigint")]
        public int System_ID { get; set; }

        [Column(TypeName = "bigint")]
        public int Menu_ID { get; set; }

        [Column(TypeName = "bigint")]
        public int? Parent_ID { get; set; }

        public string? Controller { get; set; }

        public string? Action { get; set; }

        public string? ViewType { get; set; }

        public int? Seq { get; set; }

        public string? Remark { get; set; }

        public string? Status { get; set; }

        public DateTime? UpdateAt { get; set; }

        public string? UpdateBy { get; set; }

        public DateTime? CreateAt { get; set; }

        public string? CreateBy { get; set; }

        public int? isDelete { get; set; }

    }
}


