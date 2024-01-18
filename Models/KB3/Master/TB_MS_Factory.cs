namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Factory")]
    public class TB_MS_Factory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Plant { get; set; }
        public string? F_Plant_Name { get; set; }
        public string? F_Location { get; set; }
        public string? F_Update_By { get; set; }
        public DateTime? F_Update_Date { get; set; }

    }
}


