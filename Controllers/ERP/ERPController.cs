using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;

namespace HINOSystem.Controllers.erp
{

    public class ERPController : Controller
    {
        private readonly AuthenGuard _authenGuard;
        private readonly DbConnect _dbConnect;

        public ERPController(DbConnect dbConnect, AuthenGuard authenGuard)
        {
            _dbConnect = dbConnect;
            _authenGuard = authenGuard;

            _authenGuard.ComponentItem = "";
            _authenGuard.ComponentToolbar = true;
        }

        #region MyViews
        public IActionResult ERP01M010()
        {
            return View();
        }
        public IActionResult ERP01M011()
        {
            return View();
        }
        public IActionResult ERP01M020()
        {
            return View();
        }
        public IActionResult ERP01M021()
        {
            return View();
        }
        public IActionResult ERP01M022()
        {
            return View();
        }
        public IActionResult ERP01M030()
        {
            return View();
        }
        public IActionResult ERP01M040()
        {
            return View();
        }
        public IActionResult ERP01M041()
        {
            return View();
        }


        public IActionResult UserGroups()
        {

            return View();
        }
        public IActionResult MenuGroups()
        {

            return View();
        }
        
        public IActionResult Groups()
        {

            return View();
        }

        public IActionResult Menus()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }
        #endregion

    }
}
