using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.KB3.Master
{
    [Table("TB_MS_Print_Replace_KB_TMP")]
    [PrimaryKey("F_Running","F_Plant", "F_Supplier_Code", "F_Supplier_Plant",
        "F_Kanban_No","F_Part_No","F_Ruibetsu",
        "F_Supply_Code","F_Update_By")]
    public class TB_MS_Print_Replace_KB_TMP
    {
        public int F_Running { get; set; }
        public string? F_Order_No { get; set; }
        public string F_Plant { get; set; }
        public string F_Supplier_Code { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string? F_Supplier_Name { get; set; }
        public string? F_Short_Name { get; set; }
        public string F_Store_Code { get; set; }
        public string F_Kanban_No { get; set; }
        public string F_Part_No { get; set; }
        public string F_Ruibetsu { get; set; }
        public string? F_Part_Name { get; set; }
        public int? F_Box_Qty { get; set; }
        public string F_Supply_Code { get; set; }
        public string F_Address { get; set; }
        public string? F_Description { get; set; }
        public int? F_Page { get; set; }
        public int? F_Page_Total { get; set; }
        public byte[]? F_Barcode { get; set; }
        public DateTime? F_Update_date { get; set; }
        public string F_Update_By { get; set; }
    }
}
