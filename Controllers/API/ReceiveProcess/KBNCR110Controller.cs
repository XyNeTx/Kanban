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
using Org.BouncyCastle.Utilities.IO.Pem;
using System.Reflection;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Context;
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
        private readonly PPM3Context _PPM3Context;
        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNCR110Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            PPM3Context pPM3Context,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _PPM3Context = pPM3Context;
        }



        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                BearerClass _JBearer = _BearerClass.Header(Request);
                var user = _JBearer.UserCode.ToString();
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                if (pData != null) _json = JsonConvert.DeserializeObject(pData);

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
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
        public async Task<IActionResult> SearchPDSNo([FromBody] string data)  
        {
            try
            {
                if (_KBCN.Plant == "3")
                {
                    if (data != null)
                    {
                        dynamic _json = JsonConvert.DeserializeObject(data);
                        if (_json != null)
                        {
                            string PDSNo = _json["F_PDS_No"];
                            if (PDSNo.StartsWith("7Y") || PDSNo.StartsWith("7Z"))
                            {
                                string _result = @"{
                                    ""status"":""400"",
                                    ""response"":""OK"",
                                    ""title"": ""KB3 Receive All Error"",
                                    ""message"": ""ไม่สามารถรับชิ้นส่วนประเภท Special ได้ กรุณารับชิ้นส่วนใน Function Receive Special Part""
                                    }";
                                return Ok(_result);
                            }
                            // when user scan barcode
                            if (PDSNo.Trim().Length == 14)
                            {
                                string pdsRemv = PDSNo.Trim().Remove(13,1);
                                return await SearchPDSData(pdsRemv);
                            }
                            // when user enter manual
                            else if (PDSNo.Trim().Length == 13)
                            {
                                return await SearchPDSData(PDSNo.Trim());
                            }
                            //No record for PDS NO.
                            else
                            {
                                string _result = @"{
                                    ""status"":""400"",
                                    ""response"":""OK"",
                                    ""title"": ""Search PDS Error"",
                                    ""message"" : ""Don't Have This PDS No.""
                                    }";
                                return Ok(_result);
                            }
                        }
                        else
                        {
                            string _result = @"{
                                ""status"":""500"",
                                ""response"":""OK"",
                                ""title"": ""Search PDS Error"",
                                ""message"" : ""JSON Parse Error""
                                }";
                            return Ok(_result);
                        }
                    }
                    else
                    {
                        string _result = @"{
                            ""status"":""400"",
                            ""response"":""OK"",
                            ""title"": ""Search PDS Error"",
                            ""message"" : ""Please Input PDS No.""
                            }";
                        return Ok(_result);
                    }
                }
                else return BadRequest();
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString(), "application/json");
            }
        }

        public async Task<IActionResult> SearchPDSData(string PDSNo)
        {
            string _result = "";
            try
            {
                var queryData = await _KB3Context.TB_REC_HEADER.Where(x => x.F_OrderNo == PDSNo)
                .Select(x => new
                {
                    x.F_OrderNo,
                    x.F_Delivery_Date,
                    x.F_Delivery_Trip,
                    x.F_Issued_Date,
                    x.F_Status,
                    x.F_MRN_Flag
                }).SingleOrDefaultAsync();

                if (queryData != null)
                {
                    if (queryData.F_Issued_Date > DateTime.Now)
                    {
                        _result = @"{
                            ""status"":""400"",
                            ""response"":""OK"",
                            ""title"": ""KB3 Receive All Error"",
                            ""message"": ""Can not receive because Issued Date More Than Receive Date!""
                            }";
                    }
                    else if (queryData.F_MRN_Flag == "1")
                    {
                        _result = @"{
                            ""status"":""400"",
                            ""response"":""OK"",
                            ""title"": ""KB3 Receive All Error"",
                            ""message"": ""PDS ฉบับนี้ Receive แบบ Seperate""
                            }";
                    }
                    else if (queryData.F_MRN_Flag == "2")
                    {
                        _result = @"{
                            ""status"":""400"",
                            ""response"":""OK"",
                            ""title"": ""KB3 Receive All Error"",
                            ""message"": ""PDS ฉบับนี้ Receive All ครบแล้ว""
                            }";
                    }
                    else if (queryData.F_Status == 'D')
                    {
                        _result = @"{
                        ""status"":""400"",
                        ""response"":""OK"",
                        ""title"": ""KB3 Receive All Error"",
                        ""message"": ""PDS have been deleted! Please check Data again!""
                        }";
                    }
                    else
                    {
                        string _jsonData = JsonConvert.SerializeObject(queryData);

                        _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data Found"",
                        ""data"": " + _jsonData + @"}";
                    }
                }
                else
                {
                    _result = @"{
                                ""status"":""400"",
                                ""response"":""OK"",
                                ""title"": ""KB3 Receive All Error"",
                                ""message"": ""Data Not Found""
                                }";
                }
                return Ok(_result);
            }
            catch (Exception ex)
            {
                _result = @"{
                    ""status"":""400"",
                    ""response"":""OK"",
                    ""title"": ""KB3 Receive All Error"",
                    ""message"": ""Unexpected Error""
                    }";
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveAllPart([FromBody] string data)
        {
            string _result = "";
            try
            {
                if (_KBCN.Plant == "3")
                {
                    if (data != null)
                    {
                        dynamic _json = JsonConvert.DeserializeObject(data);
                        foreach (var item in _json.F_PDS_No)
                        {
                            string PDSNo = item.F_OrderNo.ToString();
                            var recHeader = await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_OrderNo == PDSNo);
                            var recDetail = await _KB3Context.TB_REC_DETAIL.Where(x => x.F_OrderNo == PDSNo).ToListAsync();
                            if (recHeader != null)
                            {
                                recHeader.F_MRN_Flag = "2";
                                _KB3Context.TB_REC_HEADER.Update(recHeader);
                                if(recDetail.Count == 0)
                                {
                                    _result = @"{
                                        ""status"":""500"",
                                        ""response"":""OK"",
                                        ""title"": ""Receive All Part Error"",
                                        ""message"" : ""No Record from Receive Detail""
                                        }";
                                    return Ok(_result);
                                }
                                else
                                {
                                    foreach (var singleRecDet in recDetail)
                                    {
                                        singleRecDet.F_Receive_amount = singleRecDet.F_Unit_Amount;
                                        singleRecDet.F_Receive_Date = DateTime.Now.Date;
                                        _KB3Context.TB_REC_DETAIL.Update(singleRecDet);
                                    }
                                    await InsToRecLocal(PDSNo);
                                }
                            }
                            else
                            {
                                _result = @"{
                                    ""status"":""400"",
                                    ""response"":""OK"",
                                    ""title"": ""Receive All Part Error"",
                                    ""message"" : ""No Record from PDS No.""
                                    }";
                                return Ok(_result);
                            }
                        }
                            _result = @"{
                            ""status"":""200"",
                            ""response"":""OK"",
                            ""title"": ""Receive All Part Success"",
                            ""message"" : ""Receive All Part is Complete.!""
                            }";
                        await _KB3Context.SaveChangesAsync();
                        return Ok(_result);
                    }
                    else
                    {
                        _result = @"{
                            ""status"":""400"",
                            ""response"":""OK"",
                            ""title"": ""Receive All Part Error"",
                            ""message"" : ""Please check the checkbox to Receive All Part""
                }";
                        return Ok(_result);
                    }
                }
                else
                {
                    _result = @"{
                        ""status"":""403"",
                        ""response"":""OK"",
                        ""title"": ""Receive All Part Error"",
                        ""message"" : ""Please correct your factory""
                        }";
                    return Ok(_result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> InsToRecLocal(string PDSNo)
        {
            BearerClass _JBearer = _BearerClass.Header(Request);
            var user = _JBearer.UserCode.ToString();
            try
            {
                if (PDSNo != null)
                {
                    var recHead = await _KB3Context.TB_REC_HEADER.Where(x => x.F_OrderNo == PDSNo).SingleOrDefaultAsync();
                    var recDetail = await _KB3Context.TB_REC_DETAIL.Where(x => x.F_OrderNo == PDSNo).ToListAsync();

                    List<T_Receive_Local> _trlList = new List<T_Receive_Local>();
                    foreach (var receive in recDetail)
                    {
                        T_Receive_Local _trl = new T_Receive_Local
                        {
                            F_Order_No = recHead.F_OrderNo,
                            F_Part_No = receive.F_Part_No,
                            F_Ruibetsu = receive.F_Ruibetsu,
                            F_System_Type = "KBN",
                            F_Cycle = "00",
                            F_Plant_CD = recHead.F_Plant,
                            F_Store_CD = recHead.F_Delivery_Dock,
                            F_Receive_Qty = (decimal)receive.F_Receive_amount,
                            F_Receive_date = receive.F_Receive_Date?.ToString("yyyyMMdd").Trim(),
                            F_Supplier_Code = recHead.F_Supplier_Code,
                            F_Supplier_Plant = recHead.F_Supplier_Plant,
                            F_Inventory_Flg = '0',
                            F_Upload_Flg = '0',
                            F_UpdateBy = user,
                            F_UpdateDate = DateTime.Now,
                            F_Pds_No = "",
                            F_Pack_Code = ""
                        };
                        _trlList.Add(_trl);
                    }
                    _PPM3Context.T_Receive_Local.UpdateRange(_trlList);
                    UploadToEpro(user);
                    await _PPM3Context.SaveChangesAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        public IActionResult UploadToEpro(string user)
        {
            try
            {
                BearerClass _JBearer = _BearerClass.Header(Request);
                string dateTime = DateTime.Now.ToString("yyyyMMdd");
                string RecCd = "K" + dateTime.Substring(2, 2);
                if (dateTime.Substring(4, 2) == "10")
                {
                    RecCd = RecCd + "X";
                }
                else if (dateTime.Substring(4, 2) == "11")
                {
                    RecCd = RecCd + "Y";
                }
                else if (dateTime.Substring(4, 2) == "12")
                {
                    RecCd = RecCd + "Z";
                }
                else { RecCd = RecCd + dateTime.Substring(5, 1); }
                RecCd += dateTime.Substring(6, 2);
                _PPMConnect.ExecuteSQL($"EXEC [dbo].[SP_UploadReceiveNormal_All] '{RecCd}','{user}'", pUser: _JBearer, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);
                _KBCN.ExecuteSQL("EXEC [dbo].[SP_UploadReceivetoProcWeb_Normal]");
                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""title"": ""Upload Receive To E-Pro Success"",
                    ""message"" : ""Upload to E-Pro Complete""
                    }";
                return Ok(_result);
            }
            catch (Exception ex)
            {
                string _result = @"{
                    ""status"":""400"",
                    ""response"":""OK"",
                    ""title"": ""Upload Receive To E-Pro Error!"",
                    ""message"" : {" + ex + @"}
                    }";
                return Content(_result);
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
