using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers
{
    public class DevelopMaintainController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        public DevelopMaintainController(AuthenGuard authenGuard)
        {
            _authenGuard = authenGuard;
        }

        [Authorize(Policy = "KBNMT110")]
        public IActionResult KBNMT110()
        {
            return View();
        }
    }
}
