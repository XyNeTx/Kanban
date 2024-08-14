using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Login
{
    public class UserErp
    {
        [Key]
        public long _ID { get; set; }

        [StringLength(10)]
        public string? Code { get; set; }

   
        [StringLength(500)]
        public string? Password { get; set; }

        
        public long? Title_ID { get; set; }

        
        [StringLength(100)]
        public string? Name { get; set; }

        
        [StringLength(100)]
        public string? Surname { get; set; }

        
        public long? TitleTH_ID { get; set; }

        
        [StringLength(100)]
        public string? NameTH { get; set; }

        
        [StringLength(100)]
        public string? SurnameTH { get; set; }

        
        public long? TitleJP_ID { get; set; }

        
        [StringLength(100)]
        public string? NameJP { get; set; }

        
        [StringLength(100)]
        public string? SurnameJP { get; set; }

        
        [StringLength(50)]
        public string? Email { get; set; }

        
        [StringLength(50)]
        public string? Avatar { get; set; }

        
        [StringLength(50)]
        public string? SupplierCode { get; set; }

        
        [StringLength(3)]
        public string? UILanguage { get; set; }

        
        [StringLength(30)]
        public string? UITheme { get; set; }

        
        [StringLength(30)]
        public string? UIHeaderBrand { get; set; }

        
        [StringLength(30)]
        public string? UIHeader { get; set; }

        
        [StringLength(30)]
        public string? UILinkColor { get; set; }

        
        [StringLength(30)]
        public string? UIMenuColor { get; set; }

        
        [StringLength(30)]
        public string? UIIconColor { get; set; }

        
        [StringLength(30)]
        public string? UIExpandIcon { get; set; }

        
        [StringLength(30)]
        public string? UIMenuIcon { get; set; }

        
        [StringLength(30)]
        public string? UISideBar { get; set; }

        
        public string? Token { get; set; }

        
        public string? ResetToken { get; set; }

        
        public DateTime? LastLogin { get; set; }

        
        [StringLength(20)]
        public string? Status { get; set; }

        
        public DateTime? UpdateAt { get; set; }

        
        [StringLength(50)]
        public string? UpdateBy { get; set; }

        
        public DateTime? CreateAt { get; set; }

        
        [StringLength(50)]
        public string? CreateBy { get; set; }

        
        public int? isDelete { get; set; }
    }
}
