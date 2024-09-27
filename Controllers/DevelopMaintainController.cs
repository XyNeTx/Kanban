using HINOSystem.Libs;
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

        public IActionResult KBNMT110()
        {
            return _authenGuard.guard(ControllerContext);
        }
    }
}
