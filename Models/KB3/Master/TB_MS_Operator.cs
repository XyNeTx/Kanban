namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Operator")]
    public class TB_MS_Operator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_User_ID { get; set; }
        public string? F_User_Name { get; set; }
        public string? F_Telephone { get; set; }
        public string? F_Fax { get; set; }
        public string? F_Email { get; set; }
        public string? F_RecUser { get; set; }
        public DateTime? F_RecDate { get; set; }

    }
}


