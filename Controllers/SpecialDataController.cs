using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;

namespace HINOSystem.Controllers
{
    public class SpecialDataController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        public SpecialDataController(
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

        [Authorize(Policy = "KBNIM007")]
        public IActionResult KBNIM007()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNIM007T")]
        public IActionResult KBNIM007T()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNIM007C")]
        public IActionResult KBNIM007C()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNIM007TSR")]
        public IActionResult KBNIM007TSR()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }



    }

}