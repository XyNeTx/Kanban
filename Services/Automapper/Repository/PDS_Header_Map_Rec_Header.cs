using AutoMapper;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.Automapper.Interface;

namespace KANBAN.Services.Automapper.Repository
{

    public class PDS_Header_Map_Rec_Header : IPDS_Header_Map_Rec_Header
    {
        private readonly IMapper _mapper;

        public PDS_Header_Map_Rec_Header(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TB_REC_HEADER MapTo(TB_PDS_Header source)
        {
            return _mapper.Map<TB_REC_HEADER>(source);
        }
    }

}
