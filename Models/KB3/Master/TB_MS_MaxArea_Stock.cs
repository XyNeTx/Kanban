namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_MaxArea_Stock")]
    public class TB_MS_MaxArea_Stock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Plant { get; set; }
        public string F_Supplier_Cd { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_Store_Cd { get; set; }
        public string F_Kanban_No { get; set; }
        public int? F_Box_Qty { get; set; }
        public float? F_STD_Stock { get; set; }
        public int? F_Max_Area { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }

    }
}


