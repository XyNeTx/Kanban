using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Master.ViewModel
{
    public class VM_Save_KBNMS021
    {
        [StringLength(1)]
        public string F_Line { get; set; }
        [StringLength(4)]
        public string F_Code { get; set; }
        [StringLength(10)]
        public string F_Part_No { get; set; }
        [StringLength(2)]
        public string F_Ruibetsu { get; set; }
        [StringLength(40)]
        public string F_name { get; set; }
        [StringLength(1)]
        public string F_Bridge { get; set; }
        [StringLength(25)]
        public string F_Detail { get; set; }
        [StringLength(25)]
        public string F_Create_By { get; set; }
        public DateTime F_Create_Date { get; set; }
        public DateTime F_Update_Date { get; set; }
        [StringLength(40)]
        public string F_Update_By { get; set; }
        [StringLength(1)]
        public string? InpLine { get; set; }
        [StringLength(4)]
        public string? InpPartCode { get; set; }
        [StringLength(13)]
        public string? InpPartNo { get; set; }
        [StringLength(25)]
        public string? InpDetail { get; set; }

    }
}
