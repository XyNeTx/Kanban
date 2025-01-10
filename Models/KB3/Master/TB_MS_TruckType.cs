namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_TruckType")]
    public class TB_MS_TruckType
    {
        [Key]
        [StringLength(10)]
        public string F_Truck_Type { get; set; }
        public int F_Weight { get; set; }
        [Column(TypeName = "numeric(4,2)")]
        public float F_Width { get; set; }
        [Column(TypeName = "numeric(4,2)")]
        public float F_High { get; set; }
        [Column(TypeName = "numeric(4,2)")]
        public float F_Long { get; set; }
        [Column(TypeName = "numeric(4,2)")]
        public float F_Value { get; set; }

    }
}