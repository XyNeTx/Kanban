using KANBAN.Models.KB3.SpecialOrdering;
using Microsoft.AspNetCore.Http.Metadata;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR295
    {
        string LoadContactList();
        Task Confirm(List<VM_Post_KBNOR295> listObj);
        Task<string> UploadIMG(IFormFile formFile);
    }
}
