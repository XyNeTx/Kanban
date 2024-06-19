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
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNIM014CController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;        
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly FillDataTable _FillDT;
        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNIM014CController(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            FillDataTable fillDT,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _FillDT = fillDT;
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

                if (pData != null) _json = JsonConvert.DeserializeObject(pData);

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

        [HttpGet]
        public async Task<IActionResult> search(string? F_PDS_NO = null,bool chkDeliveryDate = false, string? F_DeliveryFrom = null , string? F_DeliveryTo = null)
        {
            string _SQL = "";
            try
            {
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant =  HttpContext.Session.GetString("USER_PLANT");
                F_DeliveryFrom = F_DeliveryFrom == null ? "" : F_DeliveryFrom.Replace("-",string.Empty);
                F_DeliveryTo = F_DeliveryTo == null ? "" : F_DeliveryTo.Replace("-", string.Empty);

                //_SQL = @" EXEC [exec].[spKBNMS001_SEARCH] '" + _json.F_Plant + "' ";
                if (chkDeliveryDate) _SQL = $" EXEC [exec].[spKBNIM014Confirm_SEARCH] '{Plant}' , '{UserID}' , NULL , '{F_DeliveryFrom}', '{F_DeliveryTo}' ";

                else if(string.IsNullOrWhiteSpace(F_PDS_NO) && !chkDeliveryDate) _SQL = $" EXEC [exec].[spKBNIM014Confirm_SEARCH] '{Plant}' , '{UserID}' ";

                else _SQL = $" EXEC [exec].[spKBNIM014Confirm_SEARCH] '{Plant}' , '{UserID}' , '{F_PDS_NO}' ";

                DataTable dt = _FillDT.ExecuteSQL(_SQL);

                var jsonData = JsonConvert.SerializeObject(dt);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Data Found",
                    data = jsonData
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error",
                    err = e.Message.ToString()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] object _obj)
        {
            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                
                _KB3Transaction.CreateSavepoint("Start_KBNIM014Confirm");
                dynamic dynamic = JsonConvert.DeserializeObject(_obj.ToString());

                string F_Delivery_Date = dynamic["F_Delivery_Date"],
                    F_PDS_No = dynamic["F_PDS_No"],
                    F_PDS_Issued_Date = dynamic["F_PDS_Issued_Date"];

                string UserID = HttpContext.Session.GetString("USER_CODE");

                await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC [exec].[spKBNIM014Confirm_SAVE] '{F_PDS_No}','{F_PDS_Issued_Date}','{F_Delivery_Date}','{UserID}' ");

                int rowAff = await _KB3Context.TB_Transaction.Where(x=>x.F_PDS_No == F_PDS_No && x.F_PDS_Issued_Date == F_PDS_Issued_Date && x.F_Delivery_Date == F_Delivery_Date).CountAsync();

                if (rowAff == 0)
                {
                    _KB3Transaction.Rollback();
                    return StatusCode(500, new
                    {
                        status = "500",
                        response = "Internal Server Error",
                        message = "Urgent Order Didn't Confirm",
                        err = "Data Not Found in TB_Transaction"
                    });
                }

                await _KB3Transaction.CommitAsync();
                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Urgent Order Confirmed",
                });
            }
            catch (Exception ex)
            {
                await _KB3Transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error",
                    err = ex.Message.ToString()
                });
            }
        }
    }
}
