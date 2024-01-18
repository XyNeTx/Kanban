using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HINOSystem.Libs;

namespace HINOSystem.Controllers
{
    public class TacomaOrderController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        public TacomaOrderController(
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


        public IActionResult KBNIM015()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM015M()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM015C()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }



    }

}