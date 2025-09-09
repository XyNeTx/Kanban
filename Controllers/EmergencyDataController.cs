using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;

namespace HINOSystem.Controllers
{
    public class EmergencyDataController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        public EmergencyDataController(
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

        [Authorize(Policy = "KBNIM006")]
        public IActionResult KBNIM006()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM006M")]
        public IActionResult KBNIM006M()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM006C")]
        public IActionResult KBNIM006C()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }


    }

}