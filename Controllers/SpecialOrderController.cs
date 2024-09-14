using HINOSystem.Controllers;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers
{
    public class SpecialOrderController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        private readonly WarrantyClaimConnect _wrtConnect;


        public SpecialOrderController(
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


    }
}
