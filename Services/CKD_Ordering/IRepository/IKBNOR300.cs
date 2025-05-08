using HINOSystem.Models.KB3.Master;

namespace KANBAN.Services.CKD_Ordering.IRepository
{
    public interface IKBNOR300
    {
        Task<List<string>> GetUserAuthorizeAsync();
        Task<List<TB_MS_Parameter>> GetParameterAsync();
    }
}
