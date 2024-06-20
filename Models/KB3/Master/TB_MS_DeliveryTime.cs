namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [PrimaryKey(nameof(F_Plant),nameof(F_Supplier_Code),nameof(F_Supplier_Plant),nameof(F_Cycle),nameof(F_Start_Date),nameof(F_Delivery_Trip))]
    public class TB_MS_DeliveryTime
    {
        [Required]
        [StringLength(1)]
        public string F_Plant { get; set; }

        [Required]
        [StringLength(4)]
        public string F_Supplier_Code { get; set; }

        [Required]
        [StringLength(1)]
        public string F_Supplier_Plant { get; set; }

        [Required]
        [StringLength(6)]
        public string F_Cycle { get; set; }

        [Required]
        [StringLength(8)]
        public string F_Start_Date { get; set; }

        [Required]
        [StringLength(8)]
        public string F_End_Date { get; set; }

        [Required]
        public int F_Delivery_Trip { get; set; }

        [Required]
        [StringLength(5)]
        public string F_Delivery_Time { get; set; }

        [Required]
        [StringLength(8)]
        public string F_Start_Order_Date { get; set; }

        [Required]
        [StringLength(8)]
        public string F_End_Order_Date { get; set; }

        [Required]
        public int F_Start_Order_Trip { get; set; }

        [Required]
        public int F_End_Order_Trip { get; set; }

        [StringLength(25)]
        public string? F_Create_By { get; set; }

        [Required]
        public DateTime? F_Create_Date { get; set; }

        [StringLength(25)]
        public string? F_Update_By { get; set; }

        [Required]
        public DateTime? F_Update_Date { get; set; }

        [Required]
        [StringLength(5)]
        public string F_SupplierArrival_Time { get; set; }

        [StringLength(6)]
        public string? F_Logistic_YM { get; set; }

        public short? F_Logistic_Rev { get; set; }
    }
}


