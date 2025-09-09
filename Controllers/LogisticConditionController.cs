using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authorization;



namespace HINOSystem.Controllers
{
    public class LogisticConditionController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        private readonly AuthenGuard _authenGuard;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        private readonly WarrantyClaimConnect _wrtConnect;
        private readonly BearerClass _BearerClass;

        public LogisticConditionController(
            ILogger<HomeController> logger,
            IConfiguration configuration,
            IHttpContextAccessor? httpContextAccessor,
            DbConnect dbConnect,
            AuthenGuard authenGuard,
            WarrantyClaimConnect wrtConnect,
            BearerClass bearerClass
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

        [Authorize(Policy = "KBNLC110")]
        public IActionResult KBNLC110()
        {
            return View();
        }

        [Authorize(Policy = "KBNLC120")]
        public IActionResult KBNLC120()
        {
            return View();
        }

        [Authorize(Policy = "KBNLC130")]
        public IActionResult KBNLC130()
        {
            return View();
        }

        [Authorize(Policy = "KBNLC140")]
        public IActionResult KBNLC140()
        {
            return View();
        }

        [Authorize(Policy = "KBNLC150")]
        public IActionResult KBNLC150()
        {
            return View();
        }

        [Authorize(Policy = "KBNLC180")]
        public IActionResult KBNLC180()
        {
            return View();
        }

        [Authorize(Policy = "KBNLC190")]
        public IActionResult KBNLC190()
        {
            return View();
        }

        [Authorize(Policy = "KBNLC200")]
        public IActionResult KBNLC200()
        {
            return View();
        }


    }
}
