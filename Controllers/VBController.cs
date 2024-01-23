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
    public class VBController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly WarrantyClaimConnect _wrtConnect;

        public VBController()
        {}

        public dynamic IIf(Boolean pComparision = true, dynamic pTrueParse = null, dynamic pFalsePase = null)
        {
            if (pComparision)
            {
                return pTrueParse;
            }
            else
            {
                return pFalsePase;
            }
        }


        public string Mid(string pString = "", int pStart = 0, int pEnd = 0)
        {

            return pString;
        }


        public int InStr(int pStart = 0, string pString = "", string pFind = "")
        {

            return pStart;
        }

        
    }
}
