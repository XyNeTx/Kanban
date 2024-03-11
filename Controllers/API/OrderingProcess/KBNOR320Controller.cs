using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.Web;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.Net.Http.Headers;
using System.Collections.Specialized;
using System.Net;
using System.DirectoryServices.ActiveDirectory;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Org.BouncyCastle.Asn1.Ocsp;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading.Tasks;

using HINOSystem.Libs;
using HINOSystem.Context;
using HINOSystem.Models.KB3.Master;
using NPOI.SS.Formula.Functions;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNOR320Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        public KBNOR320Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
            _KB3Context = kB3Context;

        }



        [HttpPost]
        public IActionResult initial([FromBody] string pPostData = null)
        {
            dynamic _bearer, _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";

            _bearer = _BearerClass.Header(Request);
            if (_bearer.Status == 401 || _bearer.Status == null) return Content(JsonConvert.SerializeObject(_bearer), "application/json");

            try
            {
                _KBCN.Plant = _bearer.Plant;

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                //_SQL = @" EXEC [exec].[spKBNOR310] '"
                //    + _bearer.Plant + @"','"
                //    + _bearer.UserCode + @"','"
                //    + _data.ProcessDate.ToString().Replace("-", "") + @"','"
                //    + _data.ProcessShift.ToString() + @"','' ";

                //_resData = _KBCN.ExecuteJSON(_SQL, pUser: _bearer,
                //    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                //    pActionName: ControllerContext.ActionDescriptor.ActionName
                //    );

                //_result = @"{
                //    ""status"":""200"",
                //    ""response"":""OK"",
                //    ""message"": ""Data Found"",
                //    ""data"": " + _resData + @"
                //}";

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"": null
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        //[HttpPost]
        //public IActionResult search([FromBody] string pPostData = null)
        //{
        //    dynamic _bearer, _data = null;
        //    string _SQL, _resData;
        //    string _result = @"{
        //            ""status"":""200"",
        //            ""response"":""OK"",
        //            ""message"": ""No data found"",
        //            ""data"": null
        //        }";

        //    _bearer = _BearerClass.Header(Request);
        //    if (_bearer.Status == 401 || _bearer.Status == null) return Content(JsonConvert.SerializeObject(_bearer), "application/json");

        //    try
        //    {
        //        _KBCN.Plant = _bearer.Plant;

        //        if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

        //        _SQL = @" EXEC [exec].[spKBNOR310] '"
        //            + _bearer.Plant + @"','"
        //            + _bearer.UserCode + @"','"
        //            + _data.ProcessDate.ToString().Replace("-", "") + @"','"
        //            + _data.ProcessShift.ToString() + @"','' ";

        //        _resData = _KBCN.ExecuteJSON(_SQL, pUser: _bearer,
        //            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
        //            pActionName: ControllerContext.ActionDescriptor.ActionName
        //            );

        //        _result = @"{
        //            ""status"":""200"",
        //            ""response"":""OK"",
        //            ""message"": ""Data Found"",
        //            ""data"": " + _resData + @"
        //        }";
        //        return Content(_result, "application/json");
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(e.Message.ToString(), "application/json");
        //    }
        //}




    }
}
