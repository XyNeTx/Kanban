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
using KANBAN.Context;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNCR120Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNCR120Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context,
            PPM3Context pPM3Context
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
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _JBearer, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

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
                if (_KBCN.Plant.ToString() == "3")
                {
                    if (data != null)
                    {
                        dynamic _json = JsonConvert.DeserializeObject(data);
                        if (_json != null)
                        {
                            string PDSNo = _json["F_PDS_No"];
                            if (PDSNo == null || PDSNo == "")
                            {
                                string _result = @"{
                                    ""status"":""400"",
                                    ""response"":""OK"",
                                    ""title"": ""Check PDS No. Error"",
                                    ""message"": ""Please Input PDS No.""
                                    }";

                                return Ok(_result);
                            }
                            if (PDSNo.StartsWith("7Y") || PDSNo.StartsWith("7Z"))
                            {
                                string _result = @"{
                                    ""status"":""400"",
                                    ""response"":""OK"",
                                    ""title"": ""Check PDS No. Error"",
                                    ""message"": ""ไม่สามารถรับชิ้นส่วนประเภท Special ได้ กรุณารับชิ้นส่วนใน Function Receive Special Part""
                                    }";

                                return Ok(_result);
                            }
                            // when user scan barcode
                            if (PDSNo.Trim().Length == 14)
                            {
                                string pdsRemv = PDSNo.Trim().Remove(13, 1);
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
                                    ""title"": ""Check PDS No. Error"",
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
                                ""title"": ""Check PDS No. Error"",
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
                            ""title"": ""Get Data from PDS No. Error"",
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
                            ""title"": ""Get Data from PDS No. Error"",
                            ""message"": ""Can not receive because Issued Date More Than Receive Date!""
                            }";
                    }
                    else if (queryData.F_MRN_Flag == "2")
                    {
                        _result = @"{
                            ""status"":""400"",
                            ""response"":""OK"",
                            ""title"": ""Get Data from PDS No. Error"",
                            ""message"": ""PDS ฉบับนี้ Receive All ครบแล้ว""
                            }";
                    }
                    else if (queryData.F_Status == 'D')
                    {
                        _result = @"{
                            ""status"":""400"",
                            ""response"":""OK"",
                            ""title"": ""Get Data from PDS No. Error"",
                            ""message"": ""PDS have been deleted! Please check Data again!""
                            }";
                    }
                    else
                    {
                        var receiveDetail = _KB3Context.TB_REC_DETAIL.Where(x => x.F_OrderNo == PDSNo)
                            .Select(x => new
                            {
                                x.F_OrderNo,
                                x.F_Part_No,
                                x.F_Receive_Date,
                                x.F_Unit_Amount,
                                x.F_Receive_amount,
                                F_Dev_Qty = x.F_Unit_Amount - x.F_Receive_amount
                            }).ToList(); //ดึง Data มา Add Column

                        var recDetailCustom = receiveDetail.Select((x, Index) => new //เปลี่ยน data จาก model เพราะต้องการ No มาแสดง
                        {
                            No = Index + 1,
                            x.F_OrderNo,
                            x.F_Part_No,
                            F_Receive_Date = x.F_Receive_Date.Value.ToString("dd/MM/yyyy"), // เอาแค่วันเดือนปี
                            x.F_Unit_Amount,
                            x.F_Receive_amount,
                            F_Dev_Qty = x.F_Unit_Amount - x.F_Receive_amount
                        }).ToList(); //เปลี่ยน data จาก model เพราะต้องการ No มาแสดง
                        string _jsonData = JsonConvert.SerializeObject(recDetailCustom);

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
                        ""title"": ""Get Data from PDS No. Error"",
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
                    ""title"": ""Get Data from PDS No. Error"",
                    ""message"": ""Unexpected Error""
                    }";
                return BadRequest(ex.Message);
            }
        }



        [HttpPost]
        public async Task<IActionResult> ReceiveSeparate([FromBody] string data)
        {
            try
            {
                if (_KBCN.Plant.ToString() == "3")
                {
                    if (data != null)
                    {
                        dynamic _json = JsonConvert.DeserializeObject(data);
                        DateTime now = DateTime.Now;
                        if (_json != null)
                        {
                            var JsonData = _json["JsonData"];
                            int index = (JsonData.Count)-1; //get index for last index to get PDSNo
                            string PDSNo = JsonData[index].PDSNo.ToString();
                            JsonData.Remove(index);
                            bool _isReceiveAll = true;
                            foreach (var detail in JsonData)
                            {
                                string partNo = detail["Part No."].ToString();
                                int devQty = detail["Dev. Qty"];
                                int alrQty = detail["Already Dev."];
                                int sumQty = devQty + alrQty;
                                var pdsDetailSingle = await _KB3Context.TB_REC_DETAIL.Where(
                                    x => x.F_OrderNo == PDSNo && x.F_Part_No == partNo
                                    && x.F_Unit_Amount != sumQty)
                                    .SingleOrDefaultAsync();
                                if( pdsDetailSingle != null)
                                {
                                    var header = await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_OrderNo == PDSNo);
                                    if( header != null )
                                    {
                                        header.F_MRN_Flag = "1";
                                        _KB3Context.TB_REC_HEADER.Update(header);
                                        pdsDetailSingle.F_Receive_amount = sumQty;
                                        pdsDetailSingle.F_Receive_Date = DateTime.Now.Date;
                                        _KB3Context.TB_REC_DETAIL.Update(pdsDetailSingle);
                                        _isReceiveAll = false;
                                    }
                                    else
                                    {
                                        return BadRequest();
                                    }
                                }
                                else
                                {
                                    pdsDetailSingle.F_Receive_amount = sumQty;
                                    pdsDetailSingle.F_Receive_Date = DateTime.Now.Date;
                                    _KB3Context.TB_REC_DETAIL.Update(pdsDetailSingle);
                                }
                            }
                            if (_isReceiveAll)
                            {
                                var header = await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_OrderNo == PDSNo);
                                if (header != null)
                                {
                                    header.F_MRN_Flag = "2";
                                    _KB3Context.TB_REC_HEADER.Update(header);
                                    await _KB3Context.SaveChangesAsync();
                                }
                            }
                            return Ok();
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //public async Task<IActionResult> InsToRecLocal(string PDSNo)
        //{

        //}
    }
}
