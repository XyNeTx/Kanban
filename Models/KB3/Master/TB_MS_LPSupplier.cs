namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_LPSupplier")]
    [PrimaryKey("F_Plant", "F_Logistic", "F_Start_Date")]
    public class TB_MS_LPSupplier
    {
        [StringLength(1)]
        public string F_Plant { get; set; }
        [StringLength(10)]
        public string F_Logistic { get; set; }
        [StringLength(8)]
        public string F_Start_Date { get; set; }
        [StringLength(8)]
        public string F_End_Date { get; set; }
        [StringLength(10)]
        public string F_Truck_Type { get; set; }
        public int F_Weight { get; set; }
        [StringLength(25)]
        public string F_Create_By { get; set; }
        public DateTime F_Create_Date { get; set; }
        [StringLength(25)]
        public string F_Update_By { get; set; }
        public DateTime F_Update_Date { get; set; }

    }
}


