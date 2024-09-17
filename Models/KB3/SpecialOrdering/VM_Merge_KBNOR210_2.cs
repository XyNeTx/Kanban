using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    public class VM_Merge_KBNOR210_2
    {
        [StringLength(25)]
        [Required(ErrorMessage = "Can't Get Customer Order No")]
        public string F_PDS_No { get; set; }
        [StringLength(25)]
        [Required(ErrorMessage = "Please Input New Customer Order No")]
        public string F_PDS_No_New { get; set; }
        [StringLength(10)]
        [Required(ErrorMessage = "Can't Get Delivery Date")]
        public string F_Delivery_Date { get; set; }

    }
}
