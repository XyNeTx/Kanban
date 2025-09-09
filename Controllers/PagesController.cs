using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;

namespace HINOSystem.Controllers
{
    public class PagesController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly WarrantyClaimConnect _wrtConnect;

        private string _DB = "";
        private string _SQL = "";

        private string _CurrentUserName = "";

        public PagesController(IConfiguration configuration, WarrantyClaimConnect wrtConnect)
        {
            _config = configuration;
            _wrtConnect = wrtConnect;

            this._DB = _config.GetValue<string>("Application:Database");
        }

        public IActionResult Unauthorized401()
        {
            return View();
        }
    }
}
