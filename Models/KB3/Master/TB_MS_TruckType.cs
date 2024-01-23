namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_TruckType")]
    public class TB_MS_TruckType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Truck_Type { get; set; }
        public int? F_Weight { get; set; }
        public float? F_Width { get; set; }
        public float? F_High { get; set; }
        public float? F_Long { get; set; }
        public float? F_Value { get; set; }

    }
}


