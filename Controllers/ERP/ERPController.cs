using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections;
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
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult ERP01M011()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult ERP01M020()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult ERP01M021()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult ERP01M022()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult ERP01M030()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult ERP01M040()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult ERP01M041()
        {
            return _authenGuard.guard(ControllerContext);
        }


        public IActionResult UserGroups()
        {

            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult MenuGroups()
        {

            return _authenGuard.guard(ControllerContext);
        }
        
        public IActionResult Groups()
        {

            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult Menus()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult Users()
        {
            return _authenGuard.guard(ControllerContext);
        }
        #endregion

    }
}
