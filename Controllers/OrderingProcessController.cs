using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HINOSystem.Libs;

namespace HINOSystem.Controllers
{
    public class OrderingProcessController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        private readonly WarrantyClaimConnect _wrtConnect;


        public OrderingProcessController(ILogger<HomeController> logger, DbConnect dbConnect, AuthenGuard authenGuard, WarrantyClaimConnect wrtConnect)
        {
            _authenGuard = authenGuard;

            _authenGuard.ComponentItem = "";

            _logger = logger;
            this._dbConnect = dbConnect;
            this._conn = _dbConnect.GetConncetionString();
            _wrtConnect = wrtConnect;
        }


        public IActionResult KBNOR100()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR110()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR120()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR121()
        {
            return _authenGuard.guard(ControllerContext);
        }




        public IActionResult KBNOR200()
        {
            return _authenGuard.guard(ControllerContext);
        }




        //KBNOR400 : Urgent Ordering
        public IActionResult KBNOR400()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR410()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR420()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR440()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR450()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR460()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR460EX()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR470()
        {
            return _authenGuard.guard(ControllerContext);
        }







        //KBNOR300 : CKD In-House Ordering
        public IActionResult KBNOR300()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR310()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR320()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR321()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR330()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR360()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR370()
        {
            return _authenGuard.guard(ControllerContext);
        }












        public IActionResult KBNOR600()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR700()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR710()
        {
            return _authenGuard.guard(ControllerContext);
        }




    }

}