using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers
{
    [Authorize]
    public class MasterController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        private readonly WarrantyClaimConnect _wrtConnect;


        public MasterController(
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

        [Authorize(Policy = "KBNMS001")]
        public IActionResult KBNMS001()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS002")]
        public IActionResult KBNMS002()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS003")]
        public IActionResult KBNMS003()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS004")]
        public IActionResult KBNMS004()
        {
            return View();
        }

        [Authorize(Policy = "KBNMS005")]
        public IActionResult KBNMS005()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS005S")]
        public IActionResult KBNMS005S()
        {
            return View();
        }

        [Authorize(Policy = "KBNMS006")]
        public IActionResult KBNMS006()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS007")]
        public IActionResult KBNMS007()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS008")]
        public IActionResult KBNMS008()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS009")]
        public IActionResult KBNMS009()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNMS010")]
        public IActionResult KBNMS010()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }
        [Authorize(Policy = "KBNMS011")]
        public IActionResult KBNMS011()
        {
            //_authenGuard.ComponentToolbar = false;
            return View();
        }

        [Authorize(Policy = "KBNMS012")]
        public IActionResult KBNMS012()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS013")]
        public IActionResult KBNMS013()
        {
            return View();
        }

        [Authorize(Policy = "KBNMS014")]
        public IActionResult KBNMS014()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS015")]
        public IActionResult KBNMS015()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS016")]
        public IActionResult KBNMS016()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS017")]
        public IActionResult KBNMS017()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS018")]
        public IActionResult KBNMS018()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS019")]
        public IActionResult KBNMS019()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS020")]
        public IActionResult KBNMS020()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS021")]
        public IActionResult KBNMS021()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS022")]
        public IActionResult KBNMS022()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS023")]
        public IActionResult KBNMS023()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS024")]
        public IActionResult KBNMS024()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS025")]
        public IActionResult KBNMS025()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS026")]
        public IActionResult KBNMS026()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS027")]
        public IActionResult KBNMS027()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS028")]
        public IActionResult KBNMS028()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS029")]
        public IActionResult KBNMS029()
        {
            return View();
        }
        [Authorize(Policy = "KBNMS030")]
        public IActionResult KBNMS030()
        {
            return View();
        }



    }

}