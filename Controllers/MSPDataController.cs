using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HINOSystem.Libs;

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


        public IActionResult KBNIM003()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM003M()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM003C()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM003CP()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM003S()
        {
            return _authenGuard.guard(ControllerContext);
        }


    }

}