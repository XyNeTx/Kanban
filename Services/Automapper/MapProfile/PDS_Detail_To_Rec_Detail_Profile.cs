using AutoMapper;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.Automapper.MapProfile
{
    public class PDS_Detail_To_Rec_Detail_Profile : Profile
    {
        public PDS_Detail_To_Rec_Detail_Profile()
        {
            CreateMap<TB_PDS_Detail, TB_REC_DETAIL>();
        }
    }
}
