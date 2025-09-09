using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers
{
    public class VLTDataController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        public VLTDataController(
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

        [Authorize(Policy = "KBNIM004")]
        public IActionResult KBNIM004()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNIM004M")]
        public IActionResult KBNIM004M()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNIM0043")]
        public IActionResult KBNIM0043()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNIM0042")]
        public IActionResult KBNIM0042()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNIM0044")]
        public IActionResult KBNIM0044()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }



    }

}