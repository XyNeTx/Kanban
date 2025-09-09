namespace HINOSystem.Models.ERP
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("GroupUser", Schema ="erp")]
    public class erpGroupUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "bigint")]
        public int _ID { get; set; }

        [Column(TypeName = "bigint")]
        public int? System_ID { get; set; }

        [Column(TypeName = "bigint")]
        public int User_ID { get; set; }

        [Column(TypeName = "bigint")]
        public int Group_ID { get; set; }

        public string? Remark { get; set; }

        public string? Status { get; set; }

        public DateTime? UpdateAt { get; set; }

        public string? UpdateBy { get; set; }

        public DateTime? CreateAt { get; set; }

        public string? CreateBy { get; set; }

        public int? isDelete { get; set; }

    }
}


