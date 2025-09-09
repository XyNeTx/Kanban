using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;

namespace HINOSystem.Controllers
{
    public class V2VDataController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        public V2VDataController(
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

        [Authorize(Policy = "KBNIM002")]
        public IActionResult KBNIM002()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM002M")]
        public IActionResult KBNIM002M()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM002C")]
        public IActionResult KBNIM002C()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNIM002TR")]
        public IActionResult KBNIM002TR()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }



    }

}