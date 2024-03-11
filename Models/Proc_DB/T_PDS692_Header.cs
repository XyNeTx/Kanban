using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.Proc_DB
{
    public class T_PDS692_Header
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public required string OrderNo { get; set; }
        public string? SupplierCode { get; set; }
        public char SupplierPlant { get; set; }
        public string? PostNo { get; set; }
        public string? PocketNo { get; set; }
        public string? IssuedOperator { get; set; }
        public DateTime IssuedDate { get; set; }
        public string? IssuedTrip { get; set; }
        public string? IssuedTime { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? DeliveryTime { get; set; }
        public string? DeliveryCycle { get; set; }
        public string? DeliveryPlace { get; set; }
        public int DeliveryTrip { get; set; }
        public char F_type_order_cd { get; set; }
        public string? F_user_create { get; set; }
        public string? F_user_update { get; set; }
        public DateTime F_date_create { get; set; }
        public DateTime F_date_update { get; set; }
        public DateTime F_dev_date { get; set; }
        public string? F_acc_dr { get; set; }
        public string? F_acc_cr { get; set; }
        public string? F_dep_cd { get; set; }
        public string? F_sta_cd { get; set; }
        public char F_Status { get; set; }
        public string? F_Pro_no { get; set; }
        public string? F_Remark { get; set; }
        public string? F_Remark2 { get; set; }
        public string? F_Remark3 { get; set; }
        public string? F_str_no { get; set; }
        public int F_vat { get; set; }
        public string? DeliveryDock { get; set; }
        public string? Barcode { get; set; }
        public int F_TIME_PRINT { get; set; }
        public string? F_FILE_ORDER { get; set; }
        public char MRN_Flag { get; set; }
        public DateTime MRN_Date { get; set; }
        public string? F_PO_Customer { get; set; }
        public string? DockCode { get; set; }
        public string? F_Remark_KB { get; set; }
        public string? F_Collect_Date { get; set; }
        public string? F_Collect_Time { get; set; }
        public string? F_Transportor { get; set; }
        public string? F_OrderNo_Old { get; set; }
        public string? F_Type_Version { get; set; }
    }
}
