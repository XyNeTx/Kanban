using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR230
    {
        Task<string> LoadSurvey();
        Task Upload(List<VM_Upload_KBNOR230> listObj);
    }
}
