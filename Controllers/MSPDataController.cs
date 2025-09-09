using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;

namespace HINOSystem.Controllers
{
    public class MSPDataController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        private readonly WarrantyClaimConnect _wrtConnect;


        public MSPDataController(
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
            _wrtConnect = wrtConnect;
        }

        [Authorize(Policy = "KBNIM003")]
        public IActionResult KBNIM003()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM003M")]
        public IActionResult KBNIM003M()
        {
            return View();
        }
        [Authorize(Policy = "KBNIM003C")]
        public IActionResult KBNIM003C()
        {
            return View();
        }
        [Authorize(Policy = "KBNIM003CP")]
        public IActionResult KBNIM003CP()
        {
            return View();
        }
        [Authorize(Policy = "KBNIM003S")]
        public IActionResult KBNIM003S()
        {
            return View();
        }


    }

}