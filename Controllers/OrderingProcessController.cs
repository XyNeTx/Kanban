using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Policy = "KBNOR100")]
        public IActionResult KBNOR100()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR110")]
        public IActionResult KBNOR110()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR120")]
        public IActionResult KBNOR120()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR121")]
        public IActionResult KBNOR121()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR122")]
        public IActionResult KBNOR122()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR123")]
        public IActionResult KBNOR123()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR130")]
        public IActionResult KBNOR130()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR140")]
        public IActionResult KBNOR140()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR150")]
        public IActionResult KBNOR150()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR150EX")]
        public IActionResult KBNOR150EX()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR160")]
        public IActionResult KBNOR160()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }



        [Authorize(Policy = "KBNOR200")]
        public IActionResult KBNOR200()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }




        //KBNOR400 : Urgent Ordering
        [Authorize(Policy = "KBNOR400")]
        public IActionResult KBNOR400()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR410")]
        public IActionResult KBNOR410()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR420")]
        public IActionResult KBNOR420()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR440")]
        public IActionResult KBNOR440()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR450")]
        public IActionResult KBNOR450()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR460")]
        public IActionResult KBNOR460()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR460EX")]
        public IActionResult KBNOR460EX()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR470")]
        public IActionResult KBNOR470()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }







        //KBNOR300 : CKD In-House Ordering
        [Authorize(Policy = "KBNOR300")]
        public IActionResult KBNOR300()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR310")]
        public IActionResult KBNOR310()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR320")]
        public IActionResult KBNOR320()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR321")]
        public IActionResult KBNOR321()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR330")]
        public IActionResult KBNOR330()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR360")]
        public IActionResult KBNOR360()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR361")]
        public IActionResult KBNOR361()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR370")]
        public IActionResult KBNOR370()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }












        public IActionResult KBNOR600()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        public IActionResult KBNOR610()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        public IActionResult KBNOR620()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        public IActionResult KBNOR630()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        public IActionResult KBNOR640()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }





        public IActionResult KBNOR700()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }

        public IActionResult KBNOR710()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }




    }

}