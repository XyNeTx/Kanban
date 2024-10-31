using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    [PrimaryKey(nameof(F_Survey_Doc), nameof(F_Revise_Rev))]
    public class TB_Survey_Header
    {
        [StringLength(30)]
        public string F_Survey_Doc { get; set; }

        public int F_Revise_Rev { get; set; }
        [StringLength(25)]
        public string? F_PO_Customer { get; set; }
        [StringLength(8)]
        public string? F_Issued_Date { get; set; }
        [StringLength(4)]
        public string F_Supplier_CD { get; set; }
        [StringLength(1)]
        public string F_Supplier_Plant { get; set; }
        [StringLength(8)]
        public string? F_Delivery_Date { get; set; }
        [StringLength(2)]
        public string? F_Delivery_Trip { get; set; }
        [StringLength(12)]
        public string? F_Cycle_Time { get; set; }
        [StringLength(10)]
        public string? F_Acc_Dr { get; set; }
        [StringLength(10)]
        public string? F_Acc_Cr { get; set; }
        [StringLength(10)]
        public string? F_Dept_Code { get; set; }
        [StringLength(20)]
        public string? F_WK_Code { get; set; }
        [StringLength(1)]
        public string? F_Factory_Code { get; set; }
        [StringLength(10)]
        public string? F_Confirm_Date { get; set; }
        [StringLength(10)]
        public string? F_Delay_Date { get; set; }
        [StringLength(1)]
        public string? F_Status { get; set; }
        [StringLength(1)]
        public string F_Status_D { get; set; }
        [StringLength(110)]
        public string F_Remark { get; set; }
        [StringLength(110)]
        public string F_Remark2 { get; set; }
        [StringLength(110)]
        public string F_Remark3 { get; set; }
        [StringLength(110)]
        public string? F_Remark_KB { get; set; }
        [StringLength(500)]
        public string F_Remark_Delivery { get; set; }
        [StringLength(1)]
        public string? F_Upload_Flg { get; set; }
        public int? F_Resend { get; set; }
        [StringLength(20)]
        public string? F_PDS_No { get; set; }
        [StringLength(1)]
        public string? F_PDS_Flg { get; set; }
        [StringLength(50)]
        public string? F_Issue_By { get; set; }
        [StringLength(50)]
        public string? F_Issue_Tel { get; set; }
        [StringLength(50)]
        public string? F_Issue_Fax { get; set; }
        [StringLength(100)]
        public string? F_Issue_Mail { get; set; }
        public DateTime? F_Approve_Date { get; set; }
        [StringLength(25)]
        public string? F_Approve_By { get; set; }
        [StringLength(15)]
        public string F_Approve_Name { get; set; }
        [StringLength(15)]
        public string F_Approve_Mobile { get; set; }
        [StringLength(15)]
        public string F_Approve_Position { get; set; }
        [StringLength(15)]
        public string F_Approve_Dept { get; set; }
        [StringLength(25)]
        public string? F_CustomerOrder_Type { get; set; }
        public DateTime? F_Create_Date { get; set; }
        [StringLength(25)]
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        [StringLength(25)]
        public string? F_Update_By { get; set; }
    }
}
