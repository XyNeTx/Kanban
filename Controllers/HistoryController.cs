using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;

namespace HINOSystem.Controllers
{
    public class HistoryController : Controller
    {
        private readonly AuthenGuard _authenGuard;
        private readonly DbConnect _dbConnect;

        public HistoryController(DbConnect dbConnect, AuthenGuard authenGuard)
        {
            _dbConnect = dbConnect;
            _authenGuard = authenGuard;

            _authenGuard.ComponentItem = "";
        }

        #region MyViews
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Login()
        {
            return _authenGuard.guard(ControllerContext, pViewPath: "UI");
        }
        #endregion
    }
}
