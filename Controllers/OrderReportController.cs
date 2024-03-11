using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HINOSystem.Libs;

namespace HINOSystem.Controllers
{
    public class OrderReportController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        private readonly WarrantyClaimConnect _wrtConnect;


        public OrderReportController(ILogger<HomeController> logger, DbConnect dbConnect, AuthenGuard authenGuard, WarrantyClaimConnect wrtConnect)
        {
            _authenGuard = authenGuard;

            _authenGuard.ComponentItem = "";

            _logger = logger;
            this._dbConnect = dbConnect;
            this._conn = _dbConnect.GetConncetionString();
            _wrtConnect = wrtConnect;
        }


        public IActionResult KBNRT110()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT120()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT130()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT140()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT150()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT160()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT170()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT180()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT190()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT200()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT210()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT220()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT230()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT240()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT250()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT026()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT270()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT280()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT296()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNRT300()
        {
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNRT310()
        {
            return _authenGuard.guard(ControllerContext);
        }



    }

}