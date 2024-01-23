namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_DeliveryTime")]
    public class TB_MS_DeliveryTime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Plant { get; set; }
        public string F_Supplier_Code { get; set; }
        public string F_Supplier_Plant { get; set; }
        public string F_Cycle { get; set; }
        public string F_Start_Date { get; set; }
        public string? F_End_Date { get; set; }
        public int F_Delivery_Trip { get; set; }
        public string? F_Delivery_Time { get; set; }
        public string? F_Start_Order_Date { get; set; }
        public string? F_End_Order_Date { get; set; }
        public int? F_Start_Order_Trip { get; set; }
        public int? F_End_Order_Trip { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }
        public string? F_SupplierArrival_Time { get; set; }
        public string? F_Logistic_YM { get; set; }
        public int? F_Logistic_Rev { get; set; }
    }
}


