using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    [PrimaryKey(nameof(F_OrderNo),nameof(F_Plant),nameof(F_Supplier_Code),nameof(F_Supplier_Plant)
        ,nameof(F_Delivery_Date),nameof(F_Delivery_Trip),nameof(F_Delivery_Time),nameof(F_OrderType))]
    public class TB_PDS_Header
    {
        [StringLength(13)]
        public string F_OrderNo { get; set; }
        [StringLength(30)]
        public string F_PO_Customer { get; set; }
        [StringLength(15)]
        public string F_Type_Import { get; set; }
        [StringLength(1)]
        public string F_Plant { get; set; }
        [StringLength(4)]
        public string F_Supplier_Code { get; set; }
        [StringLength(1)]
        public string F_Supplier_Plant { get; set; }
        [StringLength(8)]
        public string F_Delivery_Date { get; set; }
        public int F_Delivery_Trip { get; set; }
        [StringLength(5)]
        public string F_Delivery_Time { get; set; }
        [StringLength(6)]
        public string? F_Delivery_Cycle { get; set; }
        [StringLength(2)]
        public string? F_Delivery_Dock { get; set; }
        [StringLength(1)]
        public string F_OrderType { get; set; }
        [StringLength(50)]
        public string? F_Issued_By { get; set; }
        public DateTime? F_Issued_Date { get; set; }
        [StringLength(1)]
        public string F_Issued_Shift { get; set; }
        [StringLength(10)]
        public string? F_Dept { get; set; }
        [StringLength(10)]
        public string? F_CR { get; set; }
        [StringLength(10)]
        public string? F_DR { get; set; }
        [StringLength(20)]
        public string F_WK_Code { get; set; }
        [StringLength(1)]
        public string? F_Status { get; set; }
        public int? F_Vat { get; set; }
        [StringLength(5)]
        public string? F_Dock_Code { get; set; }
        [StringLength(1)]
        public string? F_MRN_Flag { get; set; }
        public int? F_Printed { get; set; }
        [StringLength(400)]
        public string F_Remark { get; set; }
        [StringLength(400)]
        public string F_Remark2 { get; set; }
        [StringLength(400)]
        public string F_Remark3 { get; set; }
        [StringLength(200)]
        public string F_Remark_KB { get; set; }
        [StringLength(11)]
        public string F_OrderNo_Old { get; set; }
        [StringLength(15)]
        public string? F_Transportor { get; set; }
        [StringLength(10)]
        public string F_Collect_Date { get; set; }
        [StringLength(5)]
        public string F_Collect_Time { get; set; }
        [StringLength(20)]
        public string F_Barcode { get; set; }
        [StringLength(15)]
        public string F_PDS_CKD { get; set; }
        [StringLength(25)]
        public string F_Update_By { get; set; }
        public DateTime F_Update_Date { get; set; }
        [StringLength(6)]
        public string? F_Logistic_YM { get; set; }
        public short? F_Logistic_Rev { get; set; }
        [StringLength(5)]
        public string? F_PDS_Group { get; set; }
    }
}
