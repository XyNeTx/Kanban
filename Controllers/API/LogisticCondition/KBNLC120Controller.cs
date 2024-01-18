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

namespace HINOSystem.Controllers.API.Master
{
    public class KBNLC120Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly DefaultConnection _KB3Connect;

        private readonly KB3Context _KB3Context;

        public KBNLC120Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            DefaultConnection defaultConnection,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KB3Connect = defaultConnection;
            _KB3Context = kB3Context;

        }



        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                JObject _JBearer = _BearerClass.Authorization(Request.Headers.Authorization);
                if (_JBearer.GetValue("status").ToString() == "401") return Content(JsonConvert.SerializeObject(_JBearer), "application/json");


                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KB3Connect.executeSQLJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_MS_Factory"" : " + _jsTB_MS_Factory + @"
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
        public IActionResult search([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                JObject _JBearer = _BearerClass.Authorization(Request.Headers.Authorization);
                if (_JBearer.GetValue("status").ToString() == "401") return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                _json = JsonConvert.DeserializeObject(pData);


                _SQL = @" EXEC [exec].[spKBNMS001_SEARCH] '" + _json.F_Plant + "' ";
                _KB3Connect.Plant = _json.F_Plant;
                string _jsonData = _KB3Connect.executeSQLJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);



                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"": " + _jsonData + @"
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public IActionResult save()
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                JObject _JBearer = _BearerClass.Authorization(Request.Headers.Authorization);
                if (_JBearer.GetValue("status").ToString() == "401") return Content(JsonConvert.SerializeObject(_JBearer), "application/json");


                TB_MS_OrderType _TB_MS_OrderType = new TB_MS_OrderType();
                _TB_MS_OrderType.F_Plant = Request.Form["F_Plant"].ToString();
                _TB_MS_OrderType.F_OrderType = Request.Form["F_OrderType"].ToString();
                _TB_MS_OrderType.F_Effect_Date = Request.Form["F_Effect_Date"].ToString().Replace("-", "");
                _TB_MS_OrderType.F_End_Date = Request.Form["F_End_Date"].ToString().Replace("-", "");
                _TB_MS_OrderType.F_Update_By = _JBearer.GetValue("user")["Code"].ToString();
                _TB_MS_OrderType.F_Update_Date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                _KB3Context.TB_MS_OrderType.Add(_TB_MS_OrderType);
                _KB3Context.SaveChanges();


                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data has been save""
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }




        [HttpPatch]
        public IActionResult save(int id = 0)
        {
            dynamic _json = null;
            string _SQL = "";
            string _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data not found""
                    }";
            try
            {
                JObject _JBearer = _BearerClass.Authorization(Request.Headers.Authorization);
                if (_JBearer.GetValue("status").ToString() == "401") return Content(JsonConvert.SerializeObject(_JBearer), "application/json");


                _SQL = @"
                    UPDATE [dbo].[TB_MS_OrderType]
                    SET F_End_Date = '" + Request.Form["F_End_Date"].ToString().Replace("-", "") + @"'
                        ,F_Update_By = '" + _JBearer.GetValue("user")["Code"].ToString() + @"'
                        ,F_Update_Date = '" + DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")) + @"'
                    WHERE 1=1
                    AND F_Plant = '" + Request.Form["F_Plant"].ToString() + @"'
                    AND F_OrderType = '" + Request.Form["F_OrderType"].ToString() + @"'
                    AND F_Effect_Date = '" + Request.Form["F_Effect_Date"].ToString().Replace("-", "") + @"'
                ";
                _KB3Connect.executeNonQuery(_SQL);

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data has been save""
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }




        [HttpDelete]
        public IActionResult delete(int id = 0)
        {
            dynamic _json = null;
            string _SQL = "";
            string _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data not found""
                    }";
            try
            {
                JObject _JBearer = _BearerClass.Authorization(Request.Headers.Authorization);
                if (_JBearer.GetValue("status").ToString() == "401") return Content(JsonConvert.SerializeObject(_JBearer), "application/json");


                _SQL = @"
                    DELETE [dbo].[TB_MS_OrderType]
                    WHERE 1=1
                    AND F_Plant = '" + Request.Form["F_Plant"].ToString() + @"'
                    AND F_OrderType = '" + Request.Form["F_OrderType"].ToString() + @"'
                    AND F_Effect_Date = '" + Request.Form["F_Effect_Date"].ToString().Replace("-", "") + @"'
                ";
                _KB3Connect.executeNonQuery(_SQL);

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data has been delete""
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
