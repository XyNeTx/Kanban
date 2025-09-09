using HINOSystem.Controllers;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers
{
    public class SpecialOrderingController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        private readonly WarrantyClaimConnect _wrtConnect;


        public SpecialOrderingController(
            ILogger<HomeController> logger,
            DbConnect dbConnect,
            AuthenGuard authenGuard,
            WarrantyClaimConnect wrtConnect
            )
        {
            _authenGuard = authenGuard;

            _authenGuard.ComponentItem = "";

            _logger = logger;
            this._dbConnect = dbConnect;
            this._conn = _dbConnect.GetConncetionString();
            _wrtConnect = wrtConnect;
        }

        [Authorize(Policy = "KBNOR210")]
        public IActionResult KBNOR210()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNOR210_1")]
        public IActionResult KBNOR210_1()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNOR210_2")]
        public IActionResult KBNOR210_2()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNOR210_3")]
        public IActionResult KBNOR210_3()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR220")]
        public IActionResult KBNOR220()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR230")]
        public IActionResult KBNOR230()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR240")]
        public IActionResult KBNOR240()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR250")]
        public IActionResult KBNOR250()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR260")]
        public IActionResult KBNOR260()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR261")]
        public IActionResult KBNOR261()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR270")]
        public IActionResult KBNOR270()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR280")]
        public IActionResult KBNOR280()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR290")]
        public IActionResult KBNOR290()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR220_1")]
        public IActionResult KBNOR220_1()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR220_2")]
        public IActionResult KBNOR220_2()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR291")]
        public IActionResult KBNOR291()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR292")]
        public IActionResult KBNOR292()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR293")]
        public IActionResult KBNOR293()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR294")]
        public IActionResult KBNOR294()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNOR295")]
        public IActionResult KBNOR295()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNOR296")]
        public IActionResult KBNOR296()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNOR297")]
        public IActionResult KBNOR297()
        {
            _authenGuard.ComponentToolbar = false;
            return View();
        }

    }
}
