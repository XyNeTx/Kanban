namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Label")]
    [PrimaryKey("F_Plant", "F_Supplier_Cd", "F_Supplier_Plant", "F_Part_No", "F_Ruibetsu", "F_Kanban_No", "F_Store_Code", "F_Start_Date")]
    public class TB_MS_Label
    {
        public string F_Plant { get; set; }
        public string F_Supplier_Cd { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_Kanban_No { get; set; }
        public string F_Store_Code { get; set; }
        public string F_Start_Date { get; set; }
        public string F_End_Date { get; set; }
        public string? F_Cycle { get; set; }
        public string? F_Type_Order { get; set; }
        public string? F_Color { get; set; }
        public string? F_Text { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }

    }
}


