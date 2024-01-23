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
using NPOI.OpenXmlFormats.Dml.Chart;
using System.Security.Policy;
using NPOI.OpenXmlFormats.Wordprocessing;
using NuGet.Common;


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
        private readonly BearerClass _bearerClass;

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


        public IActionResult KBNLC110()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNLC120()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNLC130()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNLC140()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNLC150()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNLC180()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNLC190()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult KBNLC200()
        {
            return _authenGuard.guard(ControllerContext);
        }


    }
}
