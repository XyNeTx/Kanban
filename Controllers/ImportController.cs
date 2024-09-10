using HINOSystem.Libs;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers
{
    public class ImportController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        public ImportController(AuthenGuard authenGuard)
        {
            _authenGuard = authenGuard;
            _authenGuard.ComponentItem = "";
            _authenGuard.ComponentToolbar = true;
        }

        public IActionResult KBNIM010()
        {
            return _authenGuard.guard(ControllerContext);
        }
    }
}
