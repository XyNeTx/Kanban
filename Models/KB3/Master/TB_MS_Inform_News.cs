namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Inform_News")]
    public class TB_MS_Inform_News
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Supplier_Code { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_Store_Code { get; set; }
        public string F_Kanban_No { get; set; }
        public string? F_Text { get; set; }
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }

    }
}


