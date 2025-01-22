using KANBAN.Models.KB3.Master;

namespace KANBAN.Services.Master.IRepository
{
    public interface IKBNMS030
    {
        Task<List<TB_MS_LineControl>> GetListData(string? F_Line_ID, string? F_Description, string? F_Customer);

        Task Insert(TB_MS_LineControl data);

        Task Update(TB_MS_LineControl data);

        Task Delete(List<TB_MS_LineControl> listData);
    }
}
