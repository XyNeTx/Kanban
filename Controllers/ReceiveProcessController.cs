using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize(Policy = "KBNRC110")]
        public IActionResult KBNRC110()
        {
            return View();
        }

        [Authorize(Policy = "KBNRC120")]
        public IActionResult KBNRC120()
        {
            return View();
        }

        [Authorize(Policy = "KBNRC210")]
        public IActionResult KBNRC210()
        {
            return View();
        }

        [Authorize(Policy = "KBNRC130")]
        public IActionResult KBNRC130()
        {
            return View();
        }
        [Authorize(Policy = "KBNRC140")]
        public IActionResult KBNRC140()
        {
            return View();
        }

        [Authorize(Policy = "KBNRC150")]
        public IActionResult KBNRC150()
        {
            return View();
        }

        [Authorize(Policy = "KBNRC160")]
        public IActionResult KBNRC160()
        {
            return View();
        }

        [Authorize(Policy = "KBNRC220")]
        public IActionResult KBNRC220()
        {
            return View();
        }

    }

}