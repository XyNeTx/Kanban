using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.Automapper.Interface
{
    //public interface IPDS_Header_Map_Rec_Header : IAutoMapRepo<TB_PDS_Header, TB_REC_HEADER>
    //{
    //}

    public interface IPDS_Header_Map_Rec_Header
    {
        TB_REC_HEADER MapTo(TB_PDS_Header source);
    }

}
