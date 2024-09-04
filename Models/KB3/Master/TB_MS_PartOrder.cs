namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_PartOrder")]

    [PrimaryKey(nameof(F_Plant), nameof(F_Supplier_Cd),
        nameof(F_Supplier_Plant), nameof(F_Part_No),
        nameof(F_Ruibetsu), nameof(F_Kanban_No),
        nameof(F_Store_Code), nameof(F_Start_Date))]

    public class TB_MS_PartOrder
    {
        [StringLength(1)]
        public string F_Plant { get; set; }
        [StringLength(4)]
        public string F_Supplier_Cd { get; set; }
        [StringLength(1)]
        public string F_Supplier_Plant { get; set; }
        [StringLength(10)]
        public string F_Part_No { get; set; }
        [StringLength(2)]
        public string F_Ruibetsu { get; set; }
        [StringLength(4)]
        public string F_Kanban_No { get; set; }
        [StringLength(2)]
        public string F_Store_Code { get; set; }
        [StringLength(8)]
        public string F_Start_Date { get; set; }
        [StringLength(8)]
        public string? F_End_Date { get; set; }
        [StringLength(10)]
        public string? F_Type_Order { get; set; }
        [StringLength(6)]
        public string? F_Cycle { get; set; }

        public bool? F_Flg_ClearModule { get; set; }
        [StringLength(5)]
        public string? F_PDS_Group { get; set; }

        public DateTime? F_Create_Date { get; set; }
        [StringLength(25)]
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        [StringLength(25)]
        public string? F_Update_By { get; set; }
        public int F_Check_Shift { get; set; }
        [StringLength(9)]
        public string F_Last_Check { get; set; }
        [StringLength(9)]
        public string F_Next_Check { get; set; }

    }
}


