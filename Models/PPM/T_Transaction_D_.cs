using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.PPM
{
    [PrimaryKey(nameof(F_TranNo), nameof(F_Serial_No))]
    public class T_Transaction_D_
    {
        public required string F_TranNo { get; set; }
        public int F_Serial_No { get; set; }
        public string? F_Tran_CD { get; set; }
        public string? F_Part_Type { get; set; }
        public string? F_ADJ_CD { get; set; }
        public Int16 F_Code { get; set; }
        public required string F_Date { get; set; }
        public string? F_Refer_Doc { get; set; }
        public required string F_PartNo { get; set; }
        public string? F_Ruibetsu { get; set; }
        public char F_Plant_CD { get; set; }
        public string? F_Location { get; set; }
        public int F_Qty { get; set; }
        public string? F_Station { get; set; }
        public string? F_Reason { get; set; }
        public string? F_Kanban_No { get; set; }
        public Int16 F_Box_Qty { get; set; }
        public string? F_Receive_Cycle { get; set; }
        public string? F_Supplier_Code { get; set; }
        public char F_Supplier_Plant { get; set; }
        public string? F_Incharge { get; set; }
        public string? F_Customer { get; set; }
        public decimal F_Material { get; set; }
        public decimal F_Process_Cost { get; set; }
        public decimal F_Sale_Amt { get; set; }
        public string? F_Issue_Date { get; set; }
        public string? F_Part_Acc { get; set; }
        public string? F_Part_Cr_Acc { get; set; }
        public string? F_ADJ_Acc { get; set; }
        public string? F_Dept_Cd { get; set; }
        public string? F_Tran_Acc { get; set; }
        public string? F_UpdateBy { get; set; }
        public char F_UpdateShift { get; set; }
        public DateTime? F_UpdateDate { get; set; }
        public string? F_WorkingDate { get; set; }
        public required string F_Pack_Code { get; set; }
    }
}
