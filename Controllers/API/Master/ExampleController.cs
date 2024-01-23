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
    public class ExampleController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        public ExampleController(
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


                _SQL = @"
                    SELECT F_Plant
                        ,F_Plant_Name
                        ,F_Location
                        ,F_Update_By
                        ,F_Update_Date
                    FROM [dbo].[TB_MS_Factory]
                ";
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

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
                BearerClass _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                _json = JsonConvert.DeserializeObject(pData);


                _SQL = @"
                    SELECT '' AS RunningNo
                        ,F_Plant
                        ,F_OrderType
                        ,F_Effect_Date
                        ,F_End_Date
                        ,F_Update_By
                        ,F_Update_Date
                    FROM [dbo].[TB_MS_OrderType]
                    WHERE 1=1
                ";

                if ( _json != null )
                {
                    _SQL += "                    AND F_Plant = "+_json.F_Plant;
                }

                string _jsTB_MS_OrderType = _KBCN.ExecuteJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);



                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"": " + _jsTB_MS_OrderType + @"
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
                BearerClass _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                TB_MS_OrderType _TB_MS_OrderType = new TB_MS_OrderType();
                _TB_MS_OrderType.F_Plant = Request.Form["F_Plant"].ToString();
                _TB_MS_OrderType.F_OrderType = Request.Form["F_OrderType"].ToString();
                _TB_MS_OrderType.F_Effect_Date = Request.Form["F_Effect_Date"].ToString();
                _TB_MS_OrderType.F_End_Date = Request.Form["F_End_Date"].ToString();
                _TB_MS_OrderType.F_Update_By = _JBearer.UserCode.ToString();
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
                BearerClass _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");


                //var _TB_MS_OrderType = _KB3Context.TB_MS_OrderType.FirstOrDefault(b => b.F_Plant == "3" && b.F_OrderType == "T39" && b.F_Effect_Date == "20230101");

                //TB_MS_OrderType _TB_MS_OrderType = _KB3Context.TB_MS_OrderType
                //    .Where(h => h.F_Plant == "3")
                //    .Where(h => h.F_OrderType == "T39")
                //    .Where(h => h.F_Effect_Date == "20230101")
                //    .FirstOrDefault();

                //if (_TB_MS_OrderType == null) return Content(_result, "application/json");

                //_TB_MS_OrderType.F_End_Date = Request.Form["F_End_Date"].ToString();
                //_TB_MS_OrderType.F_Update_By = _JBearer.UserCode.ToString();
                ////_TB_MS_OrderType.F_Update_Date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                //_KB3Context.TB_MS_OrderType.Update(_TB_MS_OrderType);
                ////_KB3Context.Update(_TB_MS_OrderType);
                //_KB3Context.SaveChanges();

                _SQL = @"
                    UPDATE [dbo].[TB_MS_OrderType]
                    SET F_End_Date = '" + Request.Form["F_End_Date"].ToString() + @"'
                        ,F_Update_By = '" + _JBearer.UserCode.ToString() + @"'
                        ,F_Update_Date = '" + DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")) + @"'
                    WHERE 1=1
                    AND F_Plant = '" + Request.Form["F_Plant"].ToString() + @"'
                    AND F_OrderType = '" + Request.Form["F_OrderType"].ToString() + @"'
                    AND F_Effect_Date = '" + Request.Form["F_Effect_Date"].ToString() + @"'
                ";
                _KBCN.Execute(_SQL);

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
                BearerClass _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");


                var _TB_MS_OrderType = _KB3Context.TB_MS_OrderType
                    .Where(h => h.F_Plant == Request.Form["F_Plant"].ToString())
                    .Where(h => h.F_OrderType == Request.Form["F_OrderType"].ToString())
                    .Where(h => h.F_Effect_Date == Request.Form["F_Effect_Date"].ToString())
                    .Single();


                if (_TB_MS_OrderType == null) return Content(_result, "application/json");

                _KB3Context.TB_MS_OrderType.Remove(_TB_MS_OrderType);
                _KB3Context.SaveChanges();


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
