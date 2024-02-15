using System;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Http;
using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using HINOSystem.Libs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;


using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PdfSharp.Charting;


namespace HINOSystem.Controllers.API.common
{
    public class SSOController : Controller
    {
        private readonly ILogger _logger;
        private readonly WarrantyClaimConnect _wrtConnect;
        private readonly BearerClass _bearerClass;
        
        public SSOController(
            WarrantyClaimConnect wrtConnect,
            BearerClass bearerClass
            )
        {
            _wrtConnect = wrtConnect;
            _bearerClass = bearerClass;
        }


        [HttpPost]
        public IActionResult Authen([FromBody] string pData = null)
        {
            string _sql = "", _result = "";
            //dynamic _json = JsonConvert.DeserializeObject(pData);
            try
            {
                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found""
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }






    }
}
