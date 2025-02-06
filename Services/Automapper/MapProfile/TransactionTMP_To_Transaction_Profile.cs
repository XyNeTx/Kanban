using AutoMapper;
using KANBAN.Models.KB3.ReportOrder;
using KANBAN.Models.KB3.UrgentOrder;

namespace KANBAN.Services.Automapper.MapProfile
{
    public class TransactionTMP_To_Transaction_Profile : Profile
    {
        public TransactionTMP_To_Transaction_Profile()
        {
            CreateMap<TB_Transaction_TMP, TB_Transaction>();
        }
    }
}
