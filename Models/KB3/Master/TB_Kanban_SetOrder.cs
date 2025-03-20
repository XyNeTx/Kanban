using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.Master
{
    [PrimaryKey("F_Plant", "F_Supplier_Code", "F_Supplier_Plant",
        "F_Part_No", "F_Ruibetsu", "F_Kanban_No", "F_Store_Cd")]
    public class TB_Kanban_SetOrder
    {
        public string F_Plant { get; set; } = string.Empty;
        public string F_Supplier_Code { get; set; } = string.Empty;
        public string F_Supplier_Plant { get; set; } = string.Empty;
        public string F_Part_No { get; set; } = string.Empty;
        public string F_Ruibetsu { get; set; } = string.Empty;
        public string F_Kanban_No { get; set; } = string.Empty;
        public string F_Store_Cd { get; set; } = string.Empty;
        public string? F_Cycle { get; set; } // Nullable because it's checked

        public string F_Trip1 { get; set; } = string.Empty;
        public string F_Trip2 { get; set; } = string.Empty;
        public string F_Trip3 { get; set; } = string.Empty;
        public string F_Trip4 { get; set; } = string.Empty;
        public string F_Trip5 { get; set; } = string.Empty;
        public string F_Trip6 { get; set; } = string.Empty;
        public string F_Trip7 { get; set; } = string.Empty;
        public string F_Trip8 { get; set; } = string.Empty;
        public string F_Trip9 { get; set; } = string.Empty;
        public string F_Trip10 { get; set; } = string.Empty;
        public string F_Trip11 { get; set; } = string.Empty;
        public string F_Trip12 { get; set; } = string.Empty;
        public string F_Trip13 { get; set; } = string.Empty;
        public string F_Trip14 { get; set; } = string.Empty;
        public string F_Trip15 { get; set; } = string.Empty;
        public string F_Trip16 { get; set; } = string.Empty;
        public string F_Trip17 { get; set; } = string.Empty;
        public string F_Trip18 { get; set; } = string.Empty;
        public string F_Trip19 { get; set; } = string.Empty;
        public string F_Trip20 { get; set; } = string.Empty;
        public string F_Trip21 { get; set; } = string.Empty;
        public string F_Trip22 { get; set; } = string.Empty;
        public string F_Trip23 { get; set; } = string.Empty;
        public string F_Trip24 { get; set; } = string.Empty;

        public string F_Create_By { get; set; } = string.Empty;
        public DateTime F_Create_Date { get; set; }
        public string F_Update_By { get; set; } = string.Empty;
        public DateTime F_Update_Date { get; set; }

    }
}
