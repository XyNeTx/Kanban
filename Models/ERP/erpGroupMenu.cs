namespace HINOSystem.Models.ERP
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Numerics;

    [Table("GroupMenu", Schema = "erp")]
    public class erpGroupMenu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "bigint")]
        public int _ID { get; set; }

        [Column(TypeName = "bigint")]
        public int? System_ID { get; set; }

        [Column(TypeName = "bigint")]
        public int Group_ID { get; set; }

        [Column(TypeName = "bigint")]
        public int Menu_ID { get; set; }
        public int? Toolbar { get; set; }
        public int? ToolbarSearch { get; set; }
        public int? ToolbarNew { get; set; }
        public int? ToolbarSave { get; set; }
        public int? ToolbarDelete { get; set; }
        public int? ToolbarPrint { get; set; }
        public int? ToolbarExecute { get; set; }
        public int? ToolbarExport { get; set; }        
        public string? ToolbarSearchText { get; set; }
        public string? ToolbarNewText { get; set; }
        public string? ToolbarSaveText { get; set; }
        public string? ToolbarDeleteText { get; set; }
        public string? ToolbarPrintText { get; set; }
        public string? ToolbarExecuteText { get; set; }
        public string? ToolbarExportText { get; set; }
        public string? Remark { get; set; }
        public string? Status { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? CreateBy { get; set; }
        public int? isDelete { get; set; }

    }
}


