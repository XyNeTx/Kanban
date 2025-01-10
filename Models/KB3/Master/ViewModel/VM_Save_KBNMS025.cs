using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Master.ViewModel
{
    public class VM_Save_KBNMS025
    {
        [DisplayName("Logistical Supplier")]
        [Required]
        public string f_Logistic { get; set; }
        [DisplayName("Truck Type")]
        [Required]
        public string f_Truck_Type { get; set; }
        [DisplayName("Weight")]
        [Required]
        public int F_Weight { get; set; }
        [DisplayName("Width")]
        [Required]
        public float F_Width { get; set; }
        [DisplayName("Height")]
        [Required]
        public float F_High { get; set; }
        [DisplayName("Long")]
        [Required]
        public float F_Long { get; set; }
        [DisplayName("M3")]
        [Required]
        public float F_M3 { get; set; }
        [DisplayName("Start Date")]
        [Required]
        public string f_Start_Date { get; set; }
        [DisplayName("End Date")]
        [Required]
        public string f_End_Date { get; set; }
    }
}
