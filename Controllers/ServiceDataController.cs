using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;

namespace HINOSystem.Controllers
{
    public class ServiceDataController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        public ServiceDataController(
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

        [Authorize(Policy = "KBNIM001")]
        public IActionResult KBNIM001()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM001M")]
        public IActionResult KBNIM001M()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNIM001C")]
        public IActionResult KBNIM001C()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNIM001O")]
        public IActionResult KBNIM001O()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }



    }

}