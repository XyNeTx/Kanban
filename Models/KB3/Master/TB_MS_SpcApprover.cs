namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection.Metadata;

    [Table("TB_MS_SpcApprover")]
    public class TB_MS_SpcApprover
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string? F_User_ID { get; set; }
        public string? F_Name { get; set; }
        public string? F_Surname { get; set; }
        public string? F_Email { get; set; }
        public string? F_Path_File { get; set; }
        public Blob? F_Sign { get; set; }
        public string? F_RecUser { get; set; }
        public DateTime? F_RecDate { get; set; }

    }
}


