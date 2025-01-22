using HINOSystem.Libs;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers
{
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


        public IActionResult KBNMS001()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS002()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS003()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS004()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS005()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS005S()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS006()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS007()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS008()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS009()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS010()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS011()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS012()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS013()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS014()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS015()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS016()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS017()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS018()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS019()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS020()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS021()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS022()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS023()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS024()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS025()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS026()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS027()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS028()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS029()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNMS030()
        {
            return _authenGuard.guard(ControllerContext);
        }



    }

}