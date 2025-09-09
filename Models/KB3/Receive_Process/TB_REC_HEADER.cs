using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.KB3.Receive_Process
{
    [PrimaryKey(nameof(F_OrderNo), nameof(F_Plant), nameof(F_Supplier_Code), nameof(F_Supplier_Plant), nameof(F_Delivery_Date), nameof(F_Delivery_Trip), nameof(F_Delivery_Time), nameof(F_OrderType))]
    public class TB_REC_HEADER
    {
        [Column(TypeName = "char(13)")]
        public required string F_OrderNo { get; set; }

        [Column(TypeName = "char(25)")]
        public string? F_PO_Customer { get; set; }

        [Column(TypeName = "char(1)")]
        public required char F_Plant { get; set; }

        [Column(TypeName = "char(4)")]
        public required string F_Supplier_Code { get; set; }

        [Column(TypeName = "char(1)")]
        public required char F_Supplier_Plant { get; set; }

        [Column(TypeName = "char(8)")]
        public required string F_Delivery_Date { get; set; }

        public int F_Delivery_Trip { get; set; }

        public required string F_Delivery_Time { get; set; }

        public string? F_Delivery_Cycle { get; set; }

        public string? F_Delivery_Dock { get; set; }

        public required char F_OrderType { get; set; }

        public string? F_Issued_By { get; set; }

        public DateTime? F_Issued_Date { get; set; }

        public required string F_Issued_Shift { get; set; }

        public required string F_Dept { get; set; }

        public string? F_CR { get; set; }

        public string? F_DR { get; set; }

        public string? F_WK_CODE { get; set; }
        public char? F_Status { get; set; }
        public int? F_Vat { get; set; }
        public string? F_Dock_Code { get; set; }
        public string? F_MRN_Flag { get; set; }
        public int? F_Printed { get; set; }
        public required string F_Remark { get; set; }
        public required string F_Remark2 { get; set; }
        public required string F_Remark3 { get; set; }
        public required char F_Flg_Epro { get; set; }
        public required string F_Remark_KB { get; set; }
        public required string F_Transportor { get; set; }
        public required string F_Collect_Date { get; set; }
        public required string F_Collect_Time { get; set; }
        public required string F_OrderNo_Old { get; set; }
        public required string F_Barcode { get; set; }
        public required string F_Approver { get; set; }
        public required int F_CKD_Flag { get; set; }
        public required string F_PDS_CKD { get; set; }
        public required string F_Type_Version { get; set; }
        public required bool F_Flag_Cancel { get; set; }
        public required string F_Cancel_By { get; set; }
        public required DateTime F_Cancel_Date { get; set; }
        public bool? F_Flag_Gen_WDS { get; set; }
        public bool? F_Flag_Transfer { get; set; }
        public required string F_Delay_Invoice_Date { get; set; }
        public string? F_Logistic_YM { get; set; }
        public short? F_Logistic_Rev { get; set; }
        public string? F_PDS_Group { get; set; }


    }
}
