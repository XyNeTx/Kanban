namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [PrimaryKey(nameof(F_Plant),nameof(F_Shift))]
    public class TB_MS_CTL
    {
        [Required]
        [StringLength(1)]
        public string F_Plant { get; set; }
        [StringLength(1)]
        public string F_Shift { get; set; }
        public string? F_Start_Time { get; set; }
        public string? F_End_Time { get; set; }
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
    }
}


