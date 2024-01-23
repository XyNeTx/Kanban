namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Kanban")]
    public class TB_MS_Kanban
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Plant { get; set; }
        public string F_Supplier_Code { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string F_Store_Code { get; set; }
        public string F_Kanban_No { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string? F_Start_Date { get; set; }
        public string? F_End_Date { get; set; }
        public int? F_Box_Qty { get; set; }
        public string? F_Cycle { get; set; }
        public int? F_Box_Capacity { get; set; }
        public string? F_Supply_Code { get; set; }
        public string? F_Part_Name { get; set; }
        public string? F_Supplier_Name { get; set; }
        public string? F_Description { get; set; }
        public string? F_Address { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }

    }
}


