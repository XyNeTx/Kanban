namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_CodeOrder")]
    public class TB_MS_CodeOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string F_Plant { get; set; }
        public string F_Order { get; set; }
        public string F_Store_Cd { get; set; }
        public string F_Ch_Store_Cd { get; set; }
        public string F_Type { get; set; }
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
    }
}


