using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;

namespace HINOSystem.Controllers
{
    public class UrgentOrderController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        public UrgentOrderController(
            ILogger<HomeController> logger,
            DbConnect dbConnect,
            AuthenGuard authenGuard,
            WarrantyClaimConnect wrtConnect
            )
        {
            _authenGuard = authenGuard;

            _authenGuard.ComponentItem = "";
            _authenGuard.ComponentToolbar = true;

            _logger = logger;
            this._dbConnect = dbConnect;
            this._conn = _dbConnect.GetConncetionString();
        }

        [Authorize(Policy = "KBNIM007N")]
        public IActionResult KBNIM007N()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM014")]
        public IActionResult KBNIM014()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM014SRV")]
        public IActionResult KBNIM014SRV()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM014C")]
        public IActionResult KBNIM014C()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM013EXP")]
        public IActionResult KBNIM013EXP()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM013INV")]
        public IActionResult KBNIM013INV()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM013VAN")]
        public IActionResult KBNIM013VAN()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM017R")]
        public IActionResult KBNIM017R()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM013_INV")]
        public IActionResult KBNIM013_INV()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }


    }

}