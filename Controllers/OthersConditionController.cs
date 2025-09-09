using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize(Policy = "KBNOC110")]
        public IActionResult KBNOC110()
        {
            return View();
        }

        [Authorize(Policy = "KBNOC120")]
        public IActionResult KBNOC120()
        {
            return View();
        }

        [Authorize(Policy = "KBNOC121")]
        public IActionResult KBNOC121()
        {
            return View();
        }

        [Authorize(Policy = "KBNOC140")]
        public IActionResult KBNOC140()
        {
            return View();
        }

        [Authorize(Policy = "KBNOC150")]
        public IActionResult KBNOC150()
        {
            return View();
        }

        [Authorize(Policy = "KBNOC160")]
        public IActionResult KBNOC160()
        {
            return View();
        }



    }

}