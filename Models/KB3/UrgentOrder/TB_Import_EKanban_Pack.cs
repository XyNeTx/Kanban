using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.UrgentOrder
{
    [PrimaryKey(nameof(F_Type), nameof(F_ITEM_NO),nameof(F_Update_By))]
    public class TB_Import_EKanban_Pack
    {
        [Required]
        [StringLength(10)]
        public string F_Type { get; set; }

        [Required]
        public int F_ITEM_NO { get; set; }

        [StringLength(4)]
        public string? F_Supplier { get; set; }

        [StringLength(1)]
        public string? F_Supplier_Plant { get; set; }

        [StringLength(50)]
        public string? F_Supplier_Name { get; set; }

        [StringLength(1)]
        public string? F_Plant { get; set; }

        [StringLength(50)]
        public string? F_Plant_Name { get; set; }

        [StringLength(2)]
        public string? F_Receive_Place { get; set; }

        [StringLength(1)]
        public string? F_Order_Type { get; set; }

        [StringLength(15)]
        public string? F_PDS_No { get; set; }

        [StringLength(12)]
        public string? F_EKBPDS_No { get; set; }

        [StringLength(10)]
        public string? F_Collect_Date { get; set; }

        [StringLength(5)]
        public string? F_Collect_Time { get; set; }

        [StringLength(10)]
        public string? F_Arrival_Date { get; set; }

        [StringLength(5)]
        public string? F_Arrival_Time { get; set; }

        [StringLength(10)]
        public string? F_Main_route_Grp_Code { get; set; }

        [StringLength(10)]
        public string? F_Main_route_Order_Seq { get; set; }

        [StringLength(10)]
        public string? F_Sub_route_Grp_Code1 { get; set; }

        [StringLength(10)]
        public string? F_Sub_route_Order_Seq1 { get; set; }

        [StringLength(13)]
        public string? F_Crs1_route { get; set; }

        [StringLength(10)]
        public string? F_Crs1_dock { get; set; }

        [StringLength(10)]
        public string? F_Crs1_arv_Date { get; set; }

        [StringLength(5)]
        public string? F_Crs1_arv_Time { get; set; }

        [StringLength(10)]
        public string? F_Crs1_dpt_Date { get; set; }

        [StringLength(5)]
        public string? F_Crs1_dpt_Time { get; set; }

        [StringLength(13)]
        public string? F_Crs2_route { get; set; }

        [StringLength(10)]
        public string? F_Crs2_dock { get; set; }

        [StringLength(10)]
        public string? F_Crs2_arv_Date { get; set; }

        [StringLength(5)]
        public string? F_Crs2_arv_Time { get; set; }

        [StringLength(10)]
        public string? F_Crs2_dpt_Date { get; set; }

        [StringLength(5)]
        public string? F_Crs2_dpt_Time { get; set; }

        [StringLength(13)]
        public string? F_Crs3_route { get; set; }

        [StringLength(10)]
        public string? F_Crs3_dock { get; set; }

        [StringLength(10)]
        public string? F_Crs3_arv_Date { get; set; }

        [StringLength(5)]
        public string? F_Crs3_arv_Time { get; set; }

        [StringLength(10)]
        public string? F_Crs3_dpt_Date { get; set; }

        [StringLength(5)]
        public string? F_Crs3_dpt_Time { get; set; }

        [StringLength(10)]
        public string? F_Supplier_Type { get; set; }

        public int? F_No { get; set; }

        [StringLength(14)]
        public string? F_Part_No { get; set; }

        [StringLength(50)]
        public string? F_Part_Name { get; set; }

        [StringLength(10)]
        public string? F_Kanban_No { get; set; }

        [StringLength(10)]
        public string? F_Line_Addr { get; set; }

        public int? F_Pack_Qty { get; set; }

        public int? F_Qty { get; set; }

        public int? F_Pack { get; set; }

        [StringLength(20)]
        public string? F_Zero_Order { get; set; }

        [StringLength(10)]
        public string? F_Sort_Lane { get; set; }

        [StringLength(10)]
        public string? F_Shipping_Date { get; set; }

        [StringLength(5)]
        public string? F_Shipping_Time { get; set; }

        [StringLength(10)]
        public string? F_Kb_print_Date_p { get; set; }

        [StringLength(5)]
        public string? F_Kb_print_Time_p { get; set; }

        [StringLength(10)]
        public string? F_Kb_print_Date_i { get; set; }

        [StringLength(5)]
        public string? F_Kb_print_Time_i { get; set; }

        [StringLength(60)]
        public string? F_Remark { get; set; }

        [StringLength(10)]
        public string? F_Order_Release_Date { get; set; }

        [StringLength(5)]
        public string? F_Order_Release_Time { get; set; }

        [StringLength(10)]
        public string? F_Main_route_Date { get; set; }

        [StringLength(10)]
        public string? F_Bill_Out_Flag { get; set; }

        [StringLength(10)]
        public string? F_Shipping_Dock { get; set; }

        [StringLength(1)]
        public string? F_Plant_CD { get; set; }

        [StringLength(50)]
        public string? F_Pack_Code { get; set; }

        [StringLength(25)]
        public string? F_Update_By { get; set; }

        public DateTime? F_Update_Date { get; set; }
    }
}
