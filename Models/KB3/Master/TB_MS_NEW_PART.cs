namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_NEW_PART")]
    public class TB_MS_NEW_PART
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string Part_No { get; set; }
        public string ST { get; set; }
        public string Name { get; set; }
        public string Seban { get; set; }
        public string code { get; set; }
        public string name1 { get; set; }
        public string Type { get; set; }

    }
}


