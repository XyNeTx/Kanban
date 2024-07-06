using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KANBAN.Models.KB3.Master
{
    [PrimaryKey(nameof(F_Store_cd),nameof(F_YM))]
    public class TB_Calendar
    {
        [StringLength(2)]
        [Required(ErrorMessage = "Please input Store Code")]
        public string F_Store_cd { get; set; }
        [StringLength(6)]
        [Required(ErrorMessage = "Please input Monthly")]
        public string F_YM { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D1 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N1 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_D2 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N2 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_D3 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N3 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_D4 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N4 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D5 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N5 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D6 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N6 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D7 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N7 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D8 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N8 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D9 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N9 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D10 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N10 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D11 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N11 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D12 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N12 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D13 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N13 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D14 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N14 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D15 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N15 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D16 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N16 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D17 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N17 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D18 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N18 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D19 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N19 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D20 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N20 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D21 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N21 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D22 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N22 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D23 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N23 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D24 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N24 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D25 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N25 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D26 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N26 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D27 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N27 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D28 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N28 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D29 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N29 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D30 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N30 { get; set; }

        [StringLength(1)]
        [Required]
        public string F_workCd_D31 { get; set; }
        [StringLength(1)]
        [Required]
        public string F_workCd_N31 { get; set; }

        public DateTime? F_Create_Date { get; set; }
        [StringLength(25)]
        public string? F_Create_By { get; set; }

        public DateTime? F_Update_Date { get; set; }
        [StringLength(25)]
        public string? F_Update_By { get; set; }
    }
}
