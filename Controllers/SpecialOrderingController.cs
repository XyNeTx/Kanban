using HINOSystem.Controllers;
using HINOSystem.Libs;
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

        public IActionResult KBNOR210()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        } 

        public IActionResult KBNOR210_1()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        } 
        
        public IActionResult KBNOR210_2()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        } 

        public IActionResult KBNOR210_3()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR220()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR230()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR240()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR250()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR280()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR290()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR220_1()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNOR220_2()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNOR293()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        } 

        public IActionResult KBNOR294()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        } 
        
        public IActionResult KBNOR295()
        {
            _authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

    }
}
