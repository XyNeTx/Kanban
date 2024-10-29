using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    public class VM_Post_KBNOR220_Gen
    {
        public string F_PDS_No { get; set; }
        public string F_Issued_Date { get; set; }
        public string F_Store_CD { get; set; }
        [Required(ErrorMessage = "Please input data for User Section!")]
        public string F_Dept_Use { get; set; }
        [Required(ErrorMessage = "Please input data for Debit Account Code!")]
        public string F_Acc_Dr { get; set; }
        [Required(ErrorMessage = "Please input data for Credit Account Code!")]
        public string F_Acc_Cr { get; set; }
        public string F_Work_Code { get; set; }
        [Required(ErrorMessage = "Please input data for Remark!")]
        public string F_Remark { get; set; }
        public string F_Remark2 { get; set; }
        public string F_Remark3 { get; set; }
        public string F_Remark_KB { get; set; }
        public string F_CustomerOrder_Type { get; set; }
        public string F_CusOrderType_CD { get; set; }
    }
}
