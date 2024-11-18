using KANBAN.Models.KB3.SpecialOrdering;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR220_1
    {
        string LoadSurveyDoc(string? surveyDoc, string? mode);
        Task Save(bool isDel, List<VM_Post_KBNOR220_1> listModel);
    }

}
