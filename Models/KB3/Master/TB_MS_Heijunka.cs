namespace HINOSystem.Models.KB3.Master
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Heijunka")]
    public class TB_MS_Heijunka
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string F_Plant { get; set; }
        public string F_CycleB { get; set; }
        public int? F_Round1 { get; set; }
        public int? F_Round2 { get; set; }
        public int? F_Round3 { get; set; }
        public int? F_Round4 { get; set; }
        public int? F_Round5 { get; set; }
        public int? F_Round6 { get; set; }
        public int? F_Round7 { get; set; }
        public int? F_Round8 { get; set; }
        public int? F_Round9 { get; set; }
        public int? F_Round10 { get; set; }
        public int? F_Round11 { get; set; }
        public int? F_Round12 { get; set; }
        public int? F_Round13 { get; set; }
        public int? F_Round14 { get; set; }
        public int? F_Round15 { get; set; }
        public int? F_Round16 { get; set; }
        public int? F_Round17 { get; set; }
        public int? F_Round18 { get; set; }
        public int? F_Round19 { get; set; }
        public int? F_Round20 { get; set; }
        public int? F_Round21 { get; set; }
        public int? F_Round22 { get; set; }
        public int? F_Round23 { get; set; }
        public int? F_Round24 { get; set; }
        public int? F_Round25 { get; set; }
        public int? F_Round26 { get; set; }
        public int? F_Round27 { get; set; }
        public int? F_Round28 { get; set; }
        public int? F_Round29 { get; set; }
        public int? F_Round30 { get; set; }
        public int? F_Round31 { get; set; }
        public int? F_Round32 { get; set; }
        public DateTime? F_Create_Date { get; set; }
        public string? F_Create_By { get; set; }
        public DateTime? F_Update_Date { get; set; }
        public string? F_Update_By { get; set; }

    }
}


