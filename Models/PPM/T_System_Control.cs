using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.PPM
{
    [PrimaryKey(nameof(F_Code))]
    public class T_System_Control
    {
        public required string F_Code { get; set; }
        public string? F_Desc { get; set; }
        public string? F_Value1 { get; set; }
        public string? F_Value2 { get; set; }
        public string? F_Value3 { get; set; }
        public string? F_Value4 { get; set; }
        public string? F_Value5 { get; set; }
        public string? F_UpdateBy { get; set; }
        public char? F_UpdateShift { get; set; }
        public DateTime? F_UpdateDate { get; set; }
        public string? F_WorkingDate { get; set; }
    }
}
