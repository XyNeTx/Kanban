using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    public class VM_Post_KBNOR295
    {

        [StringLength(20, ErrorMessage = "UserID Tag must be less than 20 characters")]
        [Required(ErrorMessage = "UserID is required")]
        public string F_User_ID { get; set; }

        [StringLength(50, ErrorMessage = "Name Must be less than 50 characters")]
        [Required(ErrorMessage = "Name is required")]
        public string F_Name { get; set; }

        [StringLength(50, ErrorMessage = "Surname Must be less than 50 characters")]
        [Required(ErrorMessage = "Surname is required")]
        public string F_Surname { get; set; }

        [StringLength(50, ErrorMessage = "Email Must be less than 50 characters")]
        [Required(ErrorMessage = "Email is required")]
        public string F_Email { get; set; }

        [StringLength(200, ErrorMessage = "Path File Must be less than 200 characters")]
        //[Required(ErrorMessage = "Path File is required")]
        public string? F_Path_File { get; set; }

        public string? F_Sign { get; set; }

        public string? F_RecUser { get; set; }

        public DateTime? F_RecDate { get; set; }

    }
}
