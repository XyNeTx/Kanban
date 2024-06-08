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


        public OrderingProcessController(
            ILogger<HomeController> logger, 
            DbConnect dbConnect, 
            AuthenGuard authenGuard, 
            WarrantyClaimConnect wrtConnect
            )
        {
            _authenGuard = authenGuard;

            _authenGuard.ComponentItem = "";

            _logger = logger;
            this._dbConnect = dbConnect;
            this._conn = _dbConnect.GetConncetionString();
            _wrtConnect = wrtConnect;
        }


        //KBNOR400 : Normal Ordering
        public IActionResult KBNOR100()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR110()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR120()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR121()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR122()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR123()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR130()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR140()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR150()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR150EX()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR160()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }




        public IActionResult KBNOR200()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }




        //KBNOR400 : Urgent Ordering
        public IActionResult KBNOR400()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR410()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR420()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR440()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR450()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR460()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR460EX()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR470()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }







        //KBNOR300 : CKD In-House Ordering
        public IActionResult KBNOR300()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR310()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR320()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR321()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR330()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR360()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR370()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }












        public IActionResult KBNOR600()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR610()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR620()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR630()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR640()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }





        public IActionResult KBNOR700()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR710()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }




    }

}