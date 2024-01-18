namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Print_Replace_KB")]
    public class TB_MS_Print_Replace_KB
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Supplier_Code { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string F_Store_Code { get; set; }
        public string F_Kanban_No { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_Supply_Code { get; set; }
        public int? F_Number { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string F_Update_By { get; set; }

    }
}


