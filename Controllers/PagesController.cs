using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.Web;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.Net.Http.Headers;
using System.Collections.Specialized;
using System.Net;
using System.DirectoryServices.ActiveDirectory;
using HINOSystem.Libs;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Org.BouncyCastle.Asn1.Ocsp;

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
