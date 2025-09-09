using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize(Policy = "KBNRT110")]
        public IActionResult KBNRT110()
        {
            return View();
        }

        [Authorize(Policy = "KBNRT120")]
        public IActionResult KBNRT120()
        {
            return View();
        }

        [Authorize(Policy = "KBNRT130")]
        public IActionResult KBNRT130()
        {
            return View();
        }

        [Authorize(Policy = "KBNRT140")]
        public IActionResult KBNRT140()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT150")]
        public IActionResult KBNRT150()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT160")]
        public IActionResult KBNRT160()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT170")]
        public IActionResult KBNRT170()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT180")]
        public IActionResult KBNRT180()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT190")]
        public IActionResult KBNRT190()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT200")]
        public IActionResult KBNRT200()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT210")]
        public IActionResult KBNRT210()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT220")]
        public IActionResult KBNRT220()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT230")]
        public IActionResult KBNRT230()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT240")]
        public IActionResult KBNRT240()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT250")]
        public IActionResult KBNRT250()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT026")]
        public IActionResult KBNRT026()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT270")]
        public IActionResult KBNRT270()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT280")]
        public IActionResult KBNRT280()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT296")]
        public IActionResult KBNRT296()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT300")]
        public IActionResult KBNRT300()
        {
            return View();
        }
        [Authorize(Policy = "KBNRT310")]
        public IActionResult KBNRT310()
        {
            return View();
        }



    }

}