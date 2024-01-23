using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HINOSystem.Libs;

namespace HINOSystem.Controllers
{
    public class OthersConditionController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        private readonly WarrantyClaimConnect _wrtConnect;


        public OthersConditionController(ILogger<HomeController> logger, DbConnect dbConnect, AuthenGuard authenGuard, WarrantyClaimConnect wrtConnect)
        {
            _authenGuard = authenGuard;

            _authenGuard.ComponentItem = "";

            _logger = logger;
            this._dbConnect = dbConnect;
            this._conn = _dbConnect.GetConncetionString();
            _wrtConnect = wrtConnect;
        }


        public IActionResult KBNOC110()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOC120()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOC121()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOC140()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOC150()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOC160()
        {
            return _authenGuard.guard(ControllerContext);
        }



    }

}