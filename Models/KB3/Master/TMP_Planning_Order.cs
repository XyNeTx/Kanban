using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KANBAN.Models.KB3.Master
{

    [PrimaryKey(nameof(F_Plant), nameof(F_Delivery_Date), nameof(F_Supplier_Code)
        , nameof(F_Supplier_Plant), nameof(F_Store_Code)
        , nameof(F_Kanban_No), nameof(F_Part_No), nameof(F_Ruibetsu))]
    public class TMP_Planning_Order
    {
        [StringLength(1)]
        public string F_Plant { get; set; }
        [StringLength(8)]
        public string F_Delivery_Date { get; set; }
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
        [StringLength(8)]
        public string? F_Cycle { get; set; }
        public int? F_Qty { get; set; }
        public decimal? F_Safety_Stk { get; set; }
        public int? F_Last_Order { get; set; }
        public int? F_FC_Max { get; set; }
        public int? F_Total_KB { get; set; }
        public int? F_Total_Pcs { get; set; }
        public int? F_Order_Diff { get; set; }
        public int? F_Trip1 { get; set; }
        public int? F_Trip2 { get; set; }
        public int? F_Trip3 { get; set; }
        public int? F_Trip4 { get; set; }
        public int? F_Trip5 { get; set; }
        public int? F_Trip6 { get; set; }
        public int? F_Trip7 { get; set; }
        public int? F_Trip8 { get; set; }
        public int? F_Trip9 { get; set; }
        public int? F_Trip10 { get; set; }
        public int? F_Trip11 { get; set; }
        public int? F_Trip12 { get; set; }
        public int? F_Trip13 { get; set; }
        public int? F_Trip14 { get; set; }
        public int? F_Trip15 { get; set; }
        public int? F_Trip16 { get; set; }
        public int? F_Trip17 { get; set; }
        public int? F_Trip18 { get; set; }
        public int? F_Trip19 { get; set; }
        public int? F_Trip20 { get; set; }
        public int? F_Trip21 { get; set; }
        public int? F_Trip22 { get; set; }
        public int? F_Trip23 { get; set; }
        public int? F_Trip24 { get; set; }
        public int? F_Trip25 { get; set; }
        public int? F_Trip26 { get; set; }
        public int? F_Trip27 { get; set; }
        public int? F_Trip28 { get; set; }
        public int? F_Trip29 { get; set; }
        public int? F_Trip30 { get; set; }
        public int? F_Trip31 { get; set; }
        public int? F_Trip32 { get; set; }
        public DateTime? F_Create_Date { get; set; }
        [StringLength(25)]
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        [StringLength(25)]
        public string? F_Update_By { get; set; }
    }
}
