using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HINOSystem.Libs;

namespace HINOSystem.Controllers
{
    public class UrgentOrderController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        public UrgentOrderController(
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
        }


        public IActionResult KBNIM007N()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM014()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM014SRV()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        
        public IActionResult KBNIM014C()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM013EXP()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM013INV()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM013VAN()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }
        public IActionResult KBNIM017R()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }


    }

}