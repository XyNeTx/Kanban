using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Policy = "KBNIM010")]
        public IActionResult KBNIM010()
        {
            return View();
        }
    }
}
