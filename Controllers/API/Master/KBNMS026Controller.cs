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
    public class KBNMS026Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        public KBNMS026Controller(
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
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

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
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                _json = JsonConvert.DeserializeObject(pData);

                _SQL = @" EXEC [exec].[spKBNMS026_SEARCH] '" + _BearerClass.Plant + "' ";
                string _jsonData = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

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
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                //TB_MS_Route
                TB_MS_OldPart _TB_MS_OldPart = new TB_MS_OldPart();
                _TB_MS_OldPart.F_Plant = _BearerClass.Plant;
                _TB_MS_OldPart.F_Parent_Part = Request.Form["F_Parent_Part"].ToString();
                _TB_MS_OldPart.F_Ruibetsu = Request.Form["F_Ruibetsu"].ToString();
                _TB_MS_OldPart.F_Part_Name = Request.Form["F_Part_Name"].ToString();
                _TB_MS_OldPart.F_Store_Cd = Request.Form["F_Store_Cd"].ToString();
                _TB_MS_OldPart.F_Start_Date = Request.Form["F_Start_Date"].ToString();
                _TB_MS_OldPart.F_End_Date = Request.Form["F_End_Date"].ToString();
                _TB_MS_OldPart.F_Update_By = _BearerClass.UserCode.ToString();
                _TB_MS_OldPart.F_Update_Date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                _KB3Context.TB_MS_OldPart.Add(_TB_MS_OldPart);
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
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                _SQL = @"
                    UPDATE [dbo].[TB_MS_OldPart]
                    SET F_Part_Name = '" + Request.Form["F_Part_Name"].ToString().Replace("-", "") + @"'
                        ,F_End_Date = '" + Request.Form["F_End_Date"].ToString().Replace("-", "") + @"'
                        ,F_Update_By = '" + _BearerClass.UserCode + @"'
                        ,F_Update_Date = '" + DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")) + @"'
                    WHERE 1=1
                    AND F_Plant = '" + Request.Form["F_Plant"].ToString() + @"'
                    AND F_Parent_Part = '" + Request.Form["F_Parent_Part"].ToString() + @"'
                    AND F_Ruibetsu = '" + Request.Form["F_Ruibetsu"].ToString().Replace("-", "") + @"'
                    AND F_Store_Cd = '" + Request.Form["F_Store_Cd"].ToString().Replace("-", "") + @"'
                    AND F_Start_Date = '" + Request.Form["F_Start_Date"].ToString().Replace("-", "") + @"'
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




        [HttpPost]
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
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                _SQL = @"
                    DELETE [dbo].[TB_MS_OldPart]
                    WHERE 1=1
                    AND F_Plant = '" + Request.Form["F_Plant"].ToString() + @"'
                    AND F_Parent_Part = '" + Request.Form["F_Parent_Part"].ToString() + @"'
                    AND F_Ruibetsu = '" + Request.Form["F_Ruibetsu"].ToString().Replace("-", "") + @"'
                    AND F_Store_Cd = '" + Request.Form["F_Store_Cd"].ToString().Replace("-", "") + @"'
                    AND F_Start_Date = '" + Request.Form["F_Start_Date"].ToString().Replace("-", "") + @"'
                ";
                _KBCN.Execute(_SQL);


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
