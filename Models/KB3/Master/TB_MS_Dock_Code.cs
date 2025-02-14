namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Dock_Code")]
    [PrimaryKey("F_Plant", "F_Dock_Code", "F_Start_Date")]
    public class TB_MS_Dock_Code
    {
        [DisplayName("Plant")]
        [StringLength(1)]
        public string F_Plant { get; set; }
        [DisplayName("Dock Code : ")]
        [StringLength(10)]
        public string F_Dock_Code { get; set; }
        [DisplayName("Start Date : ")]
        [StringLength(8)]
        public string F_Start_Date { get; set; }
        [DisplayName("End Date : ")]
        [StringLength(8)]
        public string F_End_Date { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }

    }
}


