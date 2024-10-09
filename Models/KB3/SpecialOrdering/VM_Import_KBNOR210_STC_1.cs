using System.Text.Json.Serialization;

namespace KANBAN.Models.KB3.SpecialOrdering
{
    public class VM_Import_KBNOR210_STC_1
    {

        public string PartNo { get; set; }
        public string Ruibetsu { get; set; }
        public string StockDate { get; set; }
        public int StockQty { get; set; }
        public string StoreCd { get; set; }
        [JsonPropertyName("Supp CD")]
        public string Supp_CD { get; set; }
        [JsonPropertyName("Supp Name")]
        public string Supp_Plant { get; set; }

    }
}
