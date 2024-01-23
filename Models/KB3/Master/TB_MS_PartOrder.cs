namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_PartOrder")]
    public class TB_MS_PartOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Plant { get; set; }
        public string F_Supplier_Cd { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_Kanban_No { get; set; }
        public string F_Store_Code { get; set; }
        public string F_Start_Date { get; set; }
        public string? F_End_Date { get; set; }
        public string? F_Type_Order { get; set; }
        public string? F_Cycle { get; set; }
        public byte? F_Flg_ClearModule { get; set; }
        public string? F_PDS_Group { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }
        public int? F_Check_Shift { get; set; }
        public string? F_Last_Check { get; set; }
        public string? F_Next_Check { get; set; }

    }
}


