using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.KB3.Receive_Process
{
    public class T_Receive_Local
    {
        public required string F_Order_No { get; set; }
        public required string F_Part_No { get; set; }
        public required string F_Ruibetsu { get; set; }
        public required string F_System_Type { get; set; }
        public required string F_Cycle { get; set; }
        public char? F_Plant_CD { get; set; }
        public string? F_Store_CD { get; set; }
        public required decimal F_Receive_Qty { get; set; }
        public string? F_Receive_date { get; set; }
        public string? F_Supplier_Code { get; set; }
        public char? F_Supplier_Plant { get; set; }
        public char? F_Inventory_Flg { get; set; }
        public char? F_Upload_Flg { get; set; }
        public string? F_UpdateBy { get; set; }
        public DateTime? F_UpdateDate { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int F_ID { get; set; }
        public required string? F_Pds_No { get; set; } = "";
        public required string? F_Pack_Code   { get; set; } = "";
    }
}
