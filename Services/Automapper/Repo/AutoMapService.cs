using AutoMapper;
using KANBAN.Services.Automapper;
using KANBAN.Services.Automapper.Interface;

namespace KANBAN.Services.Automapper.Repo
{
    public class AutoMapService : IAutoMapService
    {
        private IMapper _mapper;
        public IPDS_Header_Map_Rec_Header _IPDS_Header_Map_Rec_Header { get; }

        public AutoMapService(IPDS_Header_Map_Rec_Header pdsHeaderMapRecHeader)
        {
            _IPDS_Header_Map_Rec_Header = pdsHeaderMapRecHeader;
        }

    }
}
