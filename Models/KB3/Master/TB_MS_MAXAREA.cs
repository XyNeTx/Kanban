namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_MAXAREA")]
    [PrimaryKey("F_Plant", "F_Supplier_Code", "F_Supplier_Plant",
        "F_Part_No", "F_Ruibetsu", "F_Kanban_No", "F_Store_CD")]
    public class TB_MS_MAXAREA
    {

        public string F_Plant { get; set; }
        public string F_Supplier_Code { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_Kanban_No { get; set; }
        public string F_Store_CD { get; set; }
        public int F_Max_Trip { get; set; }
        public string F_Create_By { get; set; }
        public DateTime F_Create_Date { get; set; }
        public string F_Update_By { get; set; }
        public DateTime F_Update_Date { get; set; }

    }
}


