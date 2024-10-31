using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    public class VM_Post_KBNOR220_1
    {
        [StringLength(10)]
        [Required(ErrorMessage = "Please select Debit Code for Update data!")]
        public string F_Acc_Dr { get; set; }
        [StringLength(10)]
        public string? F_Acc_Cr { get; set; }
        [StringLength(10)]
        [Required(ErrorMessage = "Please select User for Section for Update data!")]
        public string F_Dept_Code { get; set; }
        [StringLength(25)]
        [Required(ErrorMessage = "Please select Customer Order Type for Update data!")]
        public string F_CustomerOrder_Type { get; set; }
        [StringLength(8)]
        public string? F_Issued_Date { get; set; }
        [StringLength(25)]
        public string? F_PO_Customer { get; set; }
        [StringLength(110)]
        [Required(ErrorMessage = "Please select Remark for Update data!")]
        public string F_Remark { get; set; }
        [StringLength(110)]
        public string? F_Remark2 { get; set; }
        [StringLength(110)]
        public string? F_Remark3 { get; set; }
        [StringLength(110)]
        public string? F_Remark_KB { get; set; }
        [StringLength(30)]
        [Required(ErrorMessage = "Please Select Survey Doc")]
        public string F_Survey_Doc { get; set; }
        [StringLength(6)]
        public string? F_Supplier_Code { get; set; }
        [StringLength(20)]
        public string? F_WK_Code { get; set; }
    }
}
