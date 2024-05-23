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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PdfSharp.Charting;
using HINOSystem.Controllers.API.common;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNMS001Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        public KBNMS001Controller(
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

                _KBCN.Plant = _BearerClass.Plant;

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL
                    , pUser: _BearerClass
                    , pControllerName : ControllerContext.ActionDescriptor.ControllerName
                    , pActionName: ControllerContext.ActionDescriptor.ActionName
                    );

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

                _KBCN.Plant = _BearerClass.Plant;

                _json = JsonConvert.DeserializeObject(pData);


                _SQL = @" EXEC [exec].[spKBNMS001_SEARCH] '" + _BearerClass.Plant + "' ";
                string _jsonData = _KBCN.ExecuteJSON(_SQL
                    , pUser: _BearerClass
                    , pControllerName: ControllerContext.ActionDescriptor.ControllerName
                    , pActionName: ControllerContext.ActionDescriptor.ActionName
                    );



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


                TB_MS_OrderType _TB_MS_OrderType = new TB_MS_OrderType();
                _TB_MS_OrderType.F_Plant = Request.Form["F_Plant"].ToString();
                _TB_MS_OrderType.F_OrderType = Request.Form["F_OrderType"].ToString();
                _TB_MS_OrderType.F_Effect_Date = Request.Form["F_Effect_Date"].ToString().Replace("-", "");
                _TB_MS_OrderType.F_End_Date = Request.Form["F_End_Date"].ToString().Replace("-", "");
                _TB_MS_OrderType.F_Update_By = _BearerClass.UserCode.ToString();
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
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                _KBCN.Plant = _BearerClass.Plant;

                _SQL = @"
                    UPDATE [dbo].[TB_MS_OrderType]
                    SET F_Plant = '" + Request.Form["F_Plant"].ToString() + @"'
                        ,F_OrderType = '" + Request.Form["F_OrderType"].ToString() + @"'
                        ,F_Effect_Date = '" + Request.Form["F_Effect_Date"].ToString().Replace("-", "") + @"'
                        ,F_End_Date = '" + Request.Form["F_End_Date"].ToString().Replace("-", "") + @"'
                        ,F_Update_By = '" + _BearerClass.UserCode + @"'
                        ,F_Update_Date = '" + DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")) + @"'
                    WHERE 1=1
                    AND F_Plant = '" + _BearerClass.Records.F_Plant.ToString().Trim() + @"'
                    AND F_OrderType = '" + _BearerClass.Records.F_OrderType.ToString().Trim() + @"'
                    AND F_Effect_Date = '" + _BearerClass.Records.F_Effect_Date.ToString().Trim().Replace("-", "") + @"'
                ";
                _KBCN.Execute(_SQL
                    , pUser: _BearerClass
                    , pControllerName: ControllerContext.ActionDescriptor.ControllerName
                    , pActionName: ControllerContext.ActionDescriptor.ActionName
                    );

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
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass), "application/json");

                _KBCN.Plant = _BearerClass.Plant;

                _SQL = @"
                    DELETE FROM [dbo].[TB_MS_OrderType]
                    WHERE 1=1
                    AND F_Plant = '" + _BearerClass.Records.F_Plant.ToString().Trim() + @"'
                    AND F_OrderType = '" + _BearerClass.Records.F_OrderType.ToString().Trim() + @"'
                    AND F_Effect_Date = '" + _BearerClass.Records.F_Effect_Date.ToString().Trim().Replace("-", "") + @"'
                ";
                _KBCN.Execute(_SQL
                    , pUser: _BearerClass
                    , pControllerName: ControllerContext.ActionDescriptor.ControllerName
                    , pActionName: ControllerContext.ActionDescriptor.ActionName
                    );

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


        //[HttpPatch]
        //public IActionResult save(int id = 0)
        //{
        //    dynamic _json = null;
        //    string _SQL = "";
        //    string _result = @"{
        //                ""status"":""200"",
        //                ""response"":""OK"",
        //                ""message"": ""Data not found""
        //            }";
        //    try
        //    {
        //        _BearerClass.Authentication(Request);
        //        if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

        //        _KBCN.Plant = _BearerClass.Plant;

        //        string F_Plant = Convert.ToString(_BearerClass.Records.F_Plant).Trim();
        //        string F_OrderType = Convert.ToString(_BearerClass.Records.F_OrderType).Trim();
        //        string F_Effect_Date = Convert.ToString(_BearerClass.Records.F_Effect_Date).Trim().Replace("-", "");

        //        TB_MS_OrderType _TB_MS_OrderType = _KB3Context.TB_MS_OrderType
        //            .Where(h => h.F_Plant == F_Plant)
        //            .Where(h => h.F_OrderType == F_OrderType)
        //            .Where(h => h.F_Effect_Date == F_Effect_Date)
        //            .FirstOrDefault();

        //        if (_TB_MS_OrderType == null) return Content(_result, "application/json");

        //        _TB_MS_OrderType.F_Plant = Request.Form["F_Plant"].ToString();
        //        _TB_MS_OrderType.F_OrderType = Request.Form["F_OrderType"].ToString();
        //        _TB_MS_OrderType.F_Effect_Date = Request.Form["F_Effect_Date"].ToString().Replace("-", "");
        //        _TB_MS_OrderType.F_End_Date = Request.Form["F_End_Date"].ToString().Replace("-", "");
        //        _TB_MS_OrderType.F_Update_By = _BearerClass.UserCode.ToString();
        //        _TB_MS_OrderType.F_Update_Date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        //        _KB3Context.TB_MS_OrderType.Update(_TB_MS_OrderType);
        //        _KB3Context.SaveChanges();

        //        _result = @"{
        //            ""status"":""200"",
        //            ""response"":""OK"",
        //            ""message"": ""Data has been save""
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
