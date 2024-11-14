using AutoMapper;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.Automapper
{
    public class PDS_Header_To_REC_Header_Profile : Profile
    {
        public PDS_Header_To_REC_Header_Profile()
        {
            CreateMap<TB_PDS_Header, TB_REC_HEADER>();
        }
    }
}
