namespace HINOSystem.Models.ERP
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Numerics;

    [Table("User", Schema ="erp")]
    public class erpUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "bigint")]
        public int _ID { get; set; }

        public string Code { get; set; }

        public string? Password { get; set; }

        public string? Name { get; set; }

        public string? Surname { get; set; }

        public string? NameTH { get; set; }

        public string? SurnameTH { get; set; }

        public string? NameJP { get; set; }

        public string? SurnameJP { get; set; }

        public string? Email { get; set; }

        public string? UILanguage { get; set; }

        public string? UITheme { get; set; }

        public string? Token { get; set; }

        public string? ResetToken { get; set; }

        public DateTime? LastLogin { get; set; }

        public string? Status { get; set; }

        public DateTime? UpdateAt { get; set; }

        public string? UpdateBy { get; set; }

        public DateTime? CreateAt { get; set; }

        public string? CreateBy { get; set; }

        public int? isDelete { get; set; }
    }
}
