using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.ReportOrder
{
    [PrimaryKey(nameof(F_Version), nameof(F_Production_date), nameof(F_revision_no), nameof(F_Supplier_code), nameof(F_supplier_plant),
        nameof(F_part_no), nameof(F_ruibetsu), nameof(F_Store_cd), nameof(F_sebango))]
    public class TB_Import_Forecast
    {
        public char F_Version { get; set; }
        public required string F_Production_date { get; set; }
        public required string F_revision_no { get; set; }
        public required string F_Supplier_code { get; set; }
        public char F_supplier_plant { get; set; }
        public required string F_part_no { get; set; }
        public required string F_ruibetsu { get; set; }
        public required string F_Store_cd { get; set; }
        public required string F_sebango { get; set; }
        public string? F_Delivery_qty { get; set; }
        public string? F_cycle_supply { get; set; }
        public int F_Amount_M { get; set; }
        public int F_Amount_M1 { get; set; }
        public int F_Amount_M2 { get; set; }
        public int F_Amount_M3 { get; set; }
        public int F_Amount_M4 { get; set; }
        public int F_Amount_MD1 { get; set; }
        public int F_Amount_MD2 { get; set; }
        public int F_Amount_MD3 { get; set; }
        public int F_Amount_MD4 { get; set; }
        public int F_Amount_MD5 { get; set; }
        public int F_Amount_MD6 { get; set; }
        public int F_Amount_MD7 { get; set; }
        public int F_Amount_MD8 { get; set; }
        public int F_Amount_MD9 { get; set; }
        public int F_Amount_MD10 { get; set; }
        public int F_Amount_MD11 { get; set; }
        public int F_Amount_MD12 { get; set; }
        public int F_Amount_MD13 { get; set; }
        public int F_Amount_MD14 { get; set; }
        public int F_Amount_MD15 { get; set; }
        public int F_Amount_MD16 { get; set; }
        public int F_Amount_MD17 { get; set; }
        public int F_Amount_MD18 { get; set; }
        public int F_Amount_MD19 { get; set; }
        public int F_Amount_MD20 { get; set; }
        public int F_Amount_MD21 { get; set; }
        public int F_Amount_MD22 { get; set; }
        public int F_Amount_MD23 { get; set; }
        public int F_Amount_MD24 { get; set; }
        public int F_Amount_MD25 { get; set; }
        public int F_Amount_MD26 { get; set; }
        public int F_Amount_MD27 { get; set; }
        public int F_Amount_MD28 { get; set; }
        public int F_Amount_MD29 { get; set; }
        public int F_Amount_MD30 { get; set; }
        public int F_Amount_MD31 { get; set; }
        public decimal F_unit_price { get; set; }
        public decimal F_amount { get; set; }
        public char F_Fac { get; set; }
        public char F_Flag_Update { get; set; }
        public required string F_Import_By { get; set; }
        public DateTime F_Import_Date { get; set; }
        public bool F_Already_CalCKD { get; set; }
    }
}
