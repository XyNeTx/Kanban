using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.Login
{
    [PrimaryKey(nameof(User_ID), nameof(Menu_ID))]
    public class UserAuthorize
    {
        public Int64 User_ID { get; set; }
        public Int64 Menu_ID { get; set; }
        public string? Remark { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? CreateBy { get; set; }
    }
}
