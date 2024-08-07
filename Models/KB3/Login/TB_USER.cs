using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Login
{
    public class TB_USER
    {
        [Key]
        [Required]
        [StringLength(15)]
        public string F_User_ID { get; set; }
        [StringLength(70)]
        public string? F_User_Name { get; set; }
        [StringLength(50)]
        public string? F_Remark { get; set; }
        [StringLength(10)]
        public string? F_Flag_Use { get; set; }
        [StringLength(10)]
        public string? F_Use_Last { get; set; }
        [StringLength(1)]
        public string F_Plant { get; set; }
        [StringLength(75)]
        public string F_EMail { get; set; }
        [StringLength(50)]
        public string? F_Update_By { get; set; }
        public DateOnly? F_Update_Date { get; set; }
    }
}
