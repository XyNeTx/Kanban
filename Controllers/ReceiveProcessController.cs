using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HINOSystem.Libs;

namespace HINOSystem.Controllers
{
    public class ReceiveProcessController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        private readonly WarrantyClaimConnect _wrtConnect;


        public ReceiveProcessController(ILogger<HomeController> logger, DbConnect dbConnect, AuthenGuard authenGuard, WarrantyClaimConnect wrtConnect)
        {
            _authenGuard = authenGuard;

            _authenGuard.ComponentItem = "";

            _logger = logger;
            this._dbConnect = dbConnect;
            this._conn = _dbConnect.GetConncetionString();
            _wrtConnect = wrtConnect;
        }


        public IActionResult KBNRC110()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRC120()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRC210()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRC130()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNRC140()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRC150()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRC160()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRC220()
        {
            return _authenGuard.guard(ControllerContext);
        }

    }

}