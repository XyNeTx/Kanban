namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_Heijunka")]
    [PrimaryKey("F_Plant", "F_CycleB")]
    public class TB_MS_Heijunka
    {
        [Required]
        [DisplayName("Plant")]
        [StringLength(1)]
        public string F_Plant { get; set; }

        [Required]
        [DisplayName("Cycle B")]
        [StringLength(2)]
        public string F_CycleB { get; set; }

        [Required]
        [DisplayName("Round1 : ")]
        public byte? F_Round1 { get; set; }

        [Required]
        [DisplayName("Round2 : ")]
        public byte? F_Round2 { get; set; }

        [Required]
        [DisplayName("Round3 : ")]
        public byte? F_Round3 { get; set; }

        [Required]
        [DisplayName("Round4 : ")]
        public byte? F_Round4 { get; set; }

        [Required]
        [DisplayName("Round5 : ")]
        public byte? F_Round5 { get; set; }

        [Required]
        [DisplayName("Round6 : ")]
        public byte? F_Round6 { get; set; }

        [Required]
        [DisplayName("Round7 : ")]
        public byte? F_Round7 { get; set; }

        [Required]
        [DisplayName("Round8 : ")]
        public byte? F_Round8 { get; set; }

        [Required]
        [DisplayName("Round9 : ")]
        public byte? F_Round9 { get; set; }

        [Required]
        [DisplayName("Round10 : ")]
        public byte? F_Round10 { get; set; }

        [Required]
        [DisplayName("Round11 : ")]
        public byte? F_Round11 { get; set; }

        [Required]
        [DisplayName("Round12 : ")]
        public byte? F_Round12 { get; set; }

        [Required]
        [DisplayName("Round13 : ")]
        public byte? F_Round13 { get; set; }

        [Required]
        [DisplayName("Round14 : ")]
        public byte? F_Round14 { get; set; }

        [Required]
        [DisplayName("Round15 : ")]
        public byte? F_Round15 { get; set; }

        [Required]
        [DisplayName("Round16 : ")]
        public byte? F_Round16 { get; set; }

        [Required]
        [DisplayName("Round17 : ")]
        public byte? F_Round17 { get; set; }

        [Required]
        [DisplayName("Round18 : ")]
        public byte? F_Round18 { get; set; }

        [Required]
        [DisplayName("Round19 : ")]
        public byte? F_Round19 { get; set; }

        [Required]
        [DisplayName("Round20 : ")]
        public byte? F_Round20 { get; set; }

        [Required]
        [DisplayName("Round21 : ")]
        public byte? F_Round21 { get; set; }

        [Required]
        [DisplayName("Round22 : ")]
        public byte? F_Round22 { get; set; }

        [Required]
        [DisplayName("Round23 : ")]
        public byte? F_Round23 { get; set; }

        [Required]
        [DisplayName("Round24 : ")]
        public byte? F_Round24 { get; set; }

        [Required]
        [DisplayName("Round25 : ")]
        public byte? F_Round25 { get; set; }

        [Required]
        [DisplayName("Round26 : ")]
        public byte? F_Round26 { get; set; }

        [Required]
        [DisplayName("Round27 : ")]
        public byte? F_Round27 { get; set; }

        [Required]
        [DisplayName("Round28 : ")]
        public byte? F_Round28 { get; set; }

        [Required]
        [DisplayName("Round29 : ")]
        public byte? F_Round29 { get; set; }

        [Required]
        [DisplayName("Round30 : ")]
        public byte? F_Round30 { get; set; }

        [Required]
        [DisplayName("Round31 : ")]
        public byte? F_Round31 { get; set; }

        [Required]
        [DisplayName("Round32 : ")]
        public byte? F_Round32 { get; set; }

        public DateTime? F_Create_Date { get; set; }

        public string? F_Create_By { get; set; }

        public DateTime? F_Update_Date { get; set; }

        public string? F_Update_By { get; set; }
    }

}


