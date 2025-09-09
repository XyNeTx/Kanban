using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;

namespace HINOSystem.Controllers.KBN
{
    public class KBNController : Controller
    {
        private readonly AuthenGuard _authenGuard;
        private readonly DbConnect _dbConnect;

        public KBNController(DbConnect dbConnect, AuthenGuard authenGuard)
        {
            _dbConnect = dbConnect;
            _authenGuard = authenGuard;

            _authenGuard.ComponentItem = "";
        }

        #region MyViews
        public IActionResult Home()
        {
            return View();
        }


        public IActionResult Issue()
        {
            return _authenGuard.guard(ControllerContext, pViewPath: "UI");
        }
        #endregion
    }
}
