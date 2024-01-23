namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_TagColor")]
    public class TB_MS_TagColor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string? F_Color_Tag { get; set; }
        public string? F_Type { get; set; }
        public string? F_RecUser { get; set; }
        public DateTime? F_RecDate { get; set; }

    }
}


