using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.KB3.Receive_Process
{
    [PrimaryKey(nameof(F_OrderNo))]
    public class VW_KBNRC_220_RPT
    {
        public required string F_OrderNo { get; set; }

        public required string F_Supplier { get; set; }

        public required string F_Delivery_Date { get; set; }

        public string? F_Receive_Date { get; set; }

        public required string F_PDS_Status { get; set; }

        public required string F_Receive_Status { get; set; }

        public required char F_OrderType { get; set; }

    }
}
