using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KANBAN.Models.KB3.Master;

[Table("TB_Import_VHD")]
public partial class TB_Import_VHD
{
    [Key]
    [Column("F_Cust_Seq")]
    [StringLength(21)]
    public string F_Cust_Seq { get; set; } = null!;

    [Column("F_Customer")]
    [StringLength(10)]
    public string F_Customer { get; set; } = null!;

    [Column("F_Date")]
    [StringLength(8)]
    public string F_Date { get; set; } = null!;

    [Column("F_Line_ID")]
    [StringLength(10)]
    public string F_Line_ID { get; set; } = null!;

    [Column("F_Seq")]
    [StringLength(10)]
    public string F_Seq { get; set; } = null!;

    [Column("F_PartCode")]
    [StringLength(3)]
    public string F_PartCode { get; set; } = null!;

    [Column("F_Parent_Part")]
    [StringLength(13)]
    public string F_Parent_Part { get; set; } = null!;

    [Column("F_Deli_Date")]
    [StringLength(8)]
    public string? F_Deli_Date { get; set; }

    [Column("F_Deli_Shift")]
    [StringLength(1)]
    public string? F_Deli_Shift { get; set; }

    [Column("F_Deli_Trip")]
    public int? F_Deli_Trip { get; set; }

    [Column("F_Flag")]
    [StringLength(1)]
    public string? F_Flag { get; set; }

    [StringLength(15)]
    public string? F_Update_By { get; set; }
    public DateTime? F_Update_Date { get; set; }
}
