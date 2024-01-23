using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HINOSystem.Libs;

namespace HINOSystem.Controllers
{
    public class MonthlyForcastController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        public MonthlyForcastController(
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


        public IActionResult KBNIM012()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNIM012M()
        {
            //_authenGuard.ComponentToolbar = false;
            return _authenGuard.guard(ControllerContext);
        }



    }

}