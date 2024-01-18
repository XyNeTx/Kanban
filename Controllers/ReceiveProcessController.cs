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


        public IActionResult KBNCR110()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR120()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR210()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR130()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNCR140()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR150()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR160()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR220()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR310()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR320()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR410()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR420()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR430()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR440()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR450()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNCR460()
        {
            return _authenGuard.guard(ControllerContext);
        }



    }

}