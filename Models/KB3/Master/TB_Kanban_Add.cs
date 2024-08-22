using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Master
{

    [PrimaryKey(nameof(F_Plant), nameof(F_Supplier_Code), nameof(F_Supplier_Plant)
                      , nameof(F_Store_Code), nameof(F_Kanban_No), nameof(F_Part_No)
                      , nameof(F_Ruibetsu))]
    public class TB_Kanban_Add
    {
        [StringLength(1)]
        public string F_Plant { get; set; }
        [StringLength(4)]
        public string F_Supplier_Code { get; set; }
        [StringLength(1)]
        public string F_Supplier_Plant { get; set; }
        [StringLength(2)]
        public string F_Store_Code { get; set; }
        [StringLength(4)]
        public string F_Kanban_No { get; set; }
        [StringLength(10)]
        public string F_Part_No { get; set; }
        [StringLength(2)]
        public string F_Ruibetsu { get; set; }
        [StringLength(1)]
        public string? F_Status { get; set; }
        [StringLength(8)]
        public string F_Delivery_Date { get; set; }
        [StringLength(2)]
        public string F_Delivery_Trip { get; set; }
        public int? F_KB_Add { get; set; }
        public int? F_KB_Add_RN { get; set; }
        public int? F_KB_Remain { get; set; }
        [StringLength(8)]
        public string? F_Finish_Date { get; set; }
        [StringLength(2)]
        public string? F_Finish_Trip { get; set; }
        [StringLength(8)]
        public string F_Start_Date { get; set; }
        [StringLength(1)]
        public string F_Start_Shift { get; set; }
        public DateTime? F_Create_Date { get; set; }
        [StringLength(25)]
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        [StringLength(25)]
        public string? F_Update_By { get; set; }

        public int? F_Round1 { get; set; }
        public int? F_Round2 { get; set; }
        public int? F_Round3 { get; set; }
        public int? F_Round4 { get; set; }
        public int? F_Round5 { get; set; }
        public int? F_Round6 { get; set; }
        public int? F_Round7 { get; set; }
        public int? F_Round8 { get; set; }
        public int? F_Round9 { get; set; }
        public int? F_Round10 { get; set; }
        public int? F_Round11 { get; set; }
        public int? F_Round12 { get; set; }
        public int? F_Round13 { get; set; }
        public int? F_Round14 { get; set; }
        public int? F_Round15 { get; set; }
        public int? F_Round16 { get; set; }
        public int? F_Round17 { get; set; }
        public int? F_Round18 { get; set; }
        public int? F_Round19 { get; set; }
        public int? F_Round20 { get; set; }
        public int? F_Round21 { get; set; }
        public int? F_Round22 { get; set; }
        public int? F_Round23 { get; set; }
        public int? F_Round24 { get; set; }
        public int? F_Round25 { get; set; }
        public int? F_Round26 { get; set; }
        public int? F_Round27 { get; set; }
        public int? F_Round28 { get; set; }
        public int? F_Round29 { get; set; }
        public int? F_Round30 { get; set; }

    }
}
