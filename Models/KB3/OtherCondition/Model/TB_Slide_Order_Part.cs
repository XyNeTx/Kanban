using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.KB3.OtherCondition.Model
{
    [Table("TB_Slide_Order_Part", Schema = "dbo")]
    [PrimaryKey("F_Plant", "F_Supplier_CD", "F_Supplier_Plant", "F_Store_CD","F_Part_No","F_Ruibetsu", "F_Delivery_Date", "F_Delivery_Trip")]
    public class TB_Slide_Order_Part
    {
        [StringLength(1, ErrorMessage = "Plant cannot be more than 1 characters")]
        [Required(ErrorMessage = "Plant is required")]
        public string F_Plant { get; set; }

        [StringLength(4, ErrorMessage = "Supplier Code cannot be more than 4 characters")]
        [Required(ErrorMessage = "Supplier Code is required")]
        public string F_Supplier_CD { get; set; }

        [StringLength(1, ErrorMessage = "Supplier Plant cannot be more than 1 characters")]
        [Required(ErrorMessage = "Supplier Plant is required")]
        public string F_Supplier_Plant { get; set; }

        [StringLength(2, ErrorMessage = "Store CD cannot be more than 2 characters")]
        [Required(ErrorMessage = "Store CD is required")]
        public string F_Store_CD { get; set; }
        
        [StringLength(10, ErrorMessage = "Part No cannot be more than 10 characters")]
        [Required(ErrorMessage = "Part No is required")]
        public string F_Part_No { get; set; }

        [StringLength(2, ErrorMessage = "Ruibetsu cannot be more than 2 characters")]
        [Required(ErrorMessage = "Ruibetsu is required")]
        public string F_Ruibetsu { get; set; }

        [StringLength(8, ErrorMessage = "Delivery Date cannot be more than 8 characters")]
        [Required(ErrorMessage = "Delivery Date is required")]
        public string F_Delivery_Date { get; set; }

        [Required(ErrorMessage = "Delivery Trip is required")]
        public byte F_Delivery_Trip { get; set; }

        [StringLength(1, ErrorMessage = "Keep Order cannot be more than 1 character")]
        [Required(ErrorMessage = "Keep Order is required")]
        public string F_Keep_Order { get; set; }

        [StringLength(10, ErrorMessage = "Slide Order Date cannot be more than 10 characters")]
        [Required(ErrorMessage = "Slide Order Date is required")]
        public string F_Slide_Date { get; set; }

        [Required(ErrorMessage = "Slide Trip is required")]
        public int F_Slide_Trip { get; set; }
        
        [Required(ErrorMessage = "Slide Qty is required")]
        public int F_Slide_Qty { get; set; }

        [StringLength(25, ErrorMessage = "Update By cannot be more than 25 characters")]
        [Required(ErrorMessage = "Update By is required")]
        public string F_Update_By { get; set; }

        [Required(ErrorMessage = "Update Date is required")]
        public DateTime F_Update_Date { get; set; }
    }
}
