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
using NPOI.HPSF;
using Humanizer;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.Eval;
using PdfSharp.Pdf.Filters;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;
using NPOI.POIFS.Properties;
using NuGet.Protocol;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json.Nodes;
using System.Linq;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNCR110Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;   
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;

        private readonly KB3Context _KB3Context;

        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNCR110Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;

        }



        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                if (pData != null) _json = JsonConvert.DeserializeObject(pData);

                BearerClass _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                _KBCN.Plant = _JBearer.Plant;

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KBCN.executeJSON(_SQL, pUser: _JBearer, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

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

        public async Task<IActionResult> KB3ReceiveAll(string data)
        {
            string _result = "";
            if (data != null)
            {
                dynamic _json = JsonConvert.DeserializeObject(data);
                if (_json != null)
                {
                    string PDSNo = _json["F_PDS_No"];
                    if (PDSNo.Length == 13)
                    {
                        if (PDSNo.StartsWith("7Y") || PDSNo.StartsWith("7Z"))
                        {

                        }
                        var queryData = await _KB3Context.TB_REC_HEADER.Where(x => x.F_OrderNo == PDSNo).ToListAsync();

                        if (queryData.Count > 0)
                        {
                            var resultData = queryData.Select((x, index) => new
                            {
                                No = index + 1,
                                x.F_OrderNo,
                                x.F_Delivery_Date,
                                x.F_Delivery_Trip
                            }).ToList();

                            string _jsonData = JsonConvert.SerializeObject(resultData);

                            _result = @"{
                                ""status"":""200"",
                                ""response"":""OK"",
                                ""message"": ""Data Found"",
                                ""data"": " + _jsonData + @"}";
                        }
                        else
                        {
                            _result = @"{
                                ""status"":""200"",
                                ""response"":""OK"",
                                ""message"": ""Data Not Found""
                                }";
                        }
                    }
                    else
                    {
                        _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""PDS No. length is not equal 13""
                        }";
                    }
                }
            }
            return Ok(_result);
        }

        public async Task<IActionResult> KB2ReceiveAll(string data)
        {
            string _result = "";
            if (data != null)
            {
                dynamic _json = JsonConvert.DeserializeObject(data);
                if (_json != null)
                {
                    string PDSNo = _json["F_PDS_No"];
                    if (PDSNo.StartsWith("8Y") || PDSNo.StartsWith("8Z"))
                    {

                    }
                    // wait for kanban2Context
                    var queryData = await _KB3Context.TB_REC_HEADER.Where(x => x.F_OrderNo == PDSNo).ToListAsync();

                    if (queryData.Count > 0)
                    {
                        var resultData = queryData.Select((x, index) => new
                        {
                            No = index + 1,
                            x.F_OrderNo,
                            x.F_Delivery_Date,
                            x.F_Delivery_Trip
                        }).ToList();

                        string _jsonData = JsonConvert.SerializeObject(resultData);

                        _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data Found"",
                        ""data"": " + _jsonData + @"}";
                    }
                    else
                    {
                        _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data Not Found""
                        }";
                    }
                }
            }
            return Ok(_result);
        }

        [HttpPost]
        public async Task<IActionResult> SearchPDSNo([FromBody] string data)  
        {
            try
            {
                if (_KBCN.Plant == "3")
                {
                    return await KB3ReceiveAll(data);
                }
                else return BadRequest();
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString(), "application/json");
            }
        }

        //[HttpPost]
        //public IActionResult search([FromBody] string pData = null)
        //{
        //    dynamic _json = null;
        //    string _SQL = "";
        //    try
        //    {
        //        //JObject _JBearer = _BearerClass.Authorization(Request.Headers.Authorization);
        //        //if (_JBearer.GetValue("status").ToString() == "401") return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

        //        _json = JsonConvert.DeserializeObject(pData);

        //        BearerClass _JBearer = _BearerClass.Header(Request);
        //        if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

        //        _KBCN.Plant = _JBearer.Plant;

        //        _SQL = @" EXEC [exec].[spKBNMS001_SEARCH] '" + _JBearer.Plant + "' ";
        //        string _jsonData = _KBCN.executeJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

        //        string _result = @"{
        //            ""status"":""200"",
        //            ""response"":""OK"",
        //            ""message"": ""Data Found"",
        //            ""data"": " + _jsonData + @"
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
