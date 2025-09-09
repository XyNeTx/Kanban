using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;

namespace HINOSystem.Controllers
{
    public class DownloadDataController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        private readonly WarrantyClaimConnect _wrtConnect;


        public DownloadDataController(
            ILogger<HomeController> logger,
            DbConnect dbConnect,
            AuthenGuard authenGuard,
            WarrantyClaimConnect wrtConnect
            )
        {
            _authenGuard = authenGuard;

            _authenGuard.ComponentItem = "";
            _authenGuard.ComponentToolbar = false;

            _logger = logger;
            this._dbConnect = dbConnect;
            this._conn = _dbConnect.GetConncetionString();
            _wrtConnect = wrtConnect;
        }


        [Authorize(Policy = "KBNDL001")]
        public IActionResult KBNDL001()
        {
            return View();
        }



    }

}