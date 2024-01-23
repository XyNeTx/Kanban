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
using HINOSystem.Models.KB3;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNLC180Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        public KBNLC180Controller(
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
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                BearerClass _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsMS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_MS_Factory"" : " + _jsMS_Factory + @"
                            }
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public IActionResult Period([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                BearerClass _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                _json = JsonConvert.DeserializeObject(pData);

                _SQL = @"
                    SELECT Distinct  F_Plant, F_YM, F_Rev
                    FROM  TB_Import_Delivery 
                    WHERE  F_YM='" + _json.Period + @"'  
                    AND  F_Plant='" + _json.Plant + @"'  
                    Order by F_Rev";
                _KBCN.Plant = _json.F_Plant;
                string _jsImport_Delivery = _KBCN.ExecuteJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_Import_Delivery"" : " + _jsImport_Delivery + @"
                            }
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public IActionResult Rev([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                BearerClass _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                _json = JsonConvert.DeserializeObject(pData);

                _SQL = @"
                    SELECT Distinct  F_Plant, F_YM, F_Rev, F_Dock_CD
                    FROM  TB_Import_Delivery 
                    WHERE  F_YM='" + _json.Period + @"'  
                    AND  F_Plant='" + _json.Plant + @"'  
                    AND  F_Rev='" + _json.Rev + @"'  
                    Order by F_Dock_CD";
                string _jsDock = _KBCN.ExecuteJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                _SQL = @"
                    SELECT Distinct  F_Plant, F_YM, F_Rev, F_Dock_CD, F_Truck_Card As F_Truck_Card
                    FROM  TB_Import_Delivery 
                    WHERE  F_YM='" + _json.Period + @"'  
                    AND  F_Plant='" + _json.Plant + @"'  
                    AND  F_Rev='" + _json.Rev + @"'  
                    Order by F_Truck_Card";
                string _jsRoute = _KBCN.ExecuteJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_Dock"" : " + _jsDock + @",
                                ""TB_Route"" : " + _jsRoute + @"
                            }
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
