using HINOSystem.Libs;
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


        public IActionResult KBNIM004()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM004M()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM0043()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM0042()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM0044()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }



    }

}