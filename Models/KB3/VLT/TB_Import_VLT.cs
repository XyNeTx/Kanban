using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.VLT
{
    [PrimaryKey(nameof(F_PDS_No),nameof(F_Production_Plan),nameof(F_Line_Code),nameof(F_Jig_In_Seq),nameof(F_Update_By))]
    public class TB_Import_VLT
    {
        [StringLength(25)]
        public string? F_PDS_No { get; set; }

        [StringLength(8)]
        public string F_Production_Plan { get; set; }

        [StringLength(5)]
        public string F_Line_Code { get; set; }

        [StringLength(10)]
        public string? F_ID_No { get; set; }

        [StringLength(10)]
        public string F_Jig_In_Seq { get; set; }

        [StringLength(4)]
        public string? F_Frame_Code { get; set; }

        [StringLength(1)]
        public string? F_Bridge_F { get; set; }

        [StringLength(10)]
        public string? F_EDP_Type { get; set; }

        [StringLength(20)]
        public string? F_Vehicle_Model { get; set; }

        [StringLength(10)]
        public string? F_Frame_Type { get; set; }

        [StringLength(8)]
        public string? F_Frame_VIN { get; set; }

        [StringLength(1)]
        public string? F_Frame_ChK_Vin { get; set; }

        [StringLength(1)]
        public string? F_Frame_Dummy { get; set; }

        [StringLength(1)]
        public string? F_Frame_Plant { get; set; }

        [StringLength(10)]
        public string? F_Frame_Serial { get; set; }

        [StringLength(10)]
        public string? F_Stamp_VIN { get; set; }

        [StringLength(4)]
        public string? F_Side_Panel { get; set; }

        [StringLength(1)]
        public string? F_Bridge_S { get; set; }

        [StringLength(4)]
        public string? F_Tail_Gate { get; set; }

        [StringLength(1)]
        public string? F_Bridge_T { get; set; }

        [StringLength(4)]
        public string? F_RR_Axle { get; set; }

        [StringLength(1)]
        public string? F_Bridge_R { get; set; }

        [StringLength(12)]
        public string? F_VHD_Order_No { get; set; }

        [StringLength(25)]
        public string? F_Update_By { get; set; }

        public DateTime? F_Update_Date { get; set; }
    }
}
