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
using KANBAN.Models.KB3.Receive_Process;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<IActionResult> CheckPDSNo([FromBody] string data)
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
                            string getPDSNo = _json["F_PDS_No"];
                            string PDSNo = getPDSNo.Trim();
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
                            if (PDSNo.Length == 14)
                            {
                                if (await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_Barcode == PDSNo) != null)
                                {
                                    string pdsRemv = PDSNo.Remove(13, 1);
                                    return await SearchDataFromPDS(pdsRemv);
                                }
                                else
                                {
                                    string _result = @"{
                                    ""status"":""400"",
                                    ""response"":""OK"",
                                    ""title"": ""Receive Separate Error"",
                                    ""message"": ""Didn't have data for this Barcode""
                                    }";
                                    return Ok(_result);
                                }
                            }
                            // when user enter manual
                            else if (PDSNo.Length == 13)
                            {
                                return await SearchDataFromPDS(PDSNo);
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


        public async Task<IActionResult> SearchDataFromPDS(string PDSNo)
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
                        var receiveDetail = _KB3Context.TB_REC_DETAIL.Where(x => x.F_OrderNo == PDSNo).ToList(); //ดึง Data มา Add Column

                        if (receiveDetail.Count == 0)
                        {
                            _result = @"{
                            ""status"":""400"",
                            ""response"":""OK"",
                            ""title"": ""Get Data from PDS No. Error"",
                            ""message"": ""No Receive Detail from this PDS No.""
                            }";
                            return Ok(_result);
                        }

                        var recDetailCustom = receiveDetail.Select((x, Index) => new //เปลี่ยน data จาก model เพราะต้องการ No มาแสดง
                        {
                            x.F_No,
                            x.F_OrderNo,
                            x.F_Part_No,
                            F_Receive_Date = x.F_Receive_Date.Value.ToString("dd/MM/yyyy"), // เอาแค่วันเดือนปี
                            x.F_Unit_Amount,
                            x.F_Receive_amount,
                            F_Dev_Qty = x.F_Unit_Amount - x.F_Receive_amount
                        }).ToList();

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
                return Content(ex.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public async Task<IActionResult> ReceiveSeparate([FromBody] string data)
        {
            try
            {
                string _result = "";
                BearerClass _JBearer = _BearerClass.Header(Request);
                var user = _JBearer.UserCode.ToString();
                if (_KBCN.Plant.ToString() == "3")
                {
                    if (data != null)
                    {
                        dynamic _json = JsonConvert.DeserializeObject(data);
                        DateTime now = DateTime.Now.Date;
                        if (_json != null)
                        {
                            var JsonData = _json["JsonData"];
                            int index = (JsonData.Count)-1; //get index for last index to get PDSNo
                            string PDSNo = JsonData[index].PDSNo.ToString().Trim();
                            if(PDSNo.Length == 14)
                            {
                                if (await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_Barcode == PDSNo) != null)
                                {
                                    PDSNo = PDSNo.Remove(13, 1);
                                }
                                else
                                {
                                    _result = @"{
                                        ""status"":""400"",
                                        ""response"":""OK"",
                                        ""title"": ""Receive Separate Error"",
                                        ""message"": ""Didn't have data for this Barcode""
                                        }";
                                    return Ok(_result);
                                }
                            }
                            JsonData.RemoveAt(index);
                            bool _isReceiveAll = true;
                            bool _isZeroRec = true;
                            foreach (var detail in JsonData)
                            {
                                string partNo = detail["Part No."].ToString();
                                int devQty = detail["Dev. Qty"];
                                int alrQty = detail["Already Dev."];
                                int sumQty = devQty + alrQty;

                                var pdsDetailSingle = await _KB3Context.TB_REC_DETAIL
                                    .SingleOrDefaultAsync(x => x.F_OrderNo == PDSNo && x.F_Part_No == partNo
                                    && x.F_Unit_Amount != sumQty);

                                if (pdsDetailSingle != null)
                                {
                                    var header = await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_OrderNo == PDSNo);
                                    if (header != null)
                                    {
                                        header.F_MRN_Flag = "1";
                                        _KB3Context.TB_REC_HEADER.Update(header);
                                        pdsDetailSingle.F_Receive_amount = sumQty;
                                        pdsDetailSingle.F_Receive_Date = now;
                                        _KB3Context.TB_REC_DETAIL.Update(pdsDetailSingle);
                                        _isReceiveAll = false;

                                        if (devQty != 0)
                                        {
                                            _isZeroRec = false;
                                            T_Receive_Local local = new T_Receive_Local
                                            {
                                                F_Order_No = header.F_OrderNo,
                                                F_Part_No = pdsDetailSingle.F_Part_No,
                                                F_Ruibetsu = pdsDetailSingle.F_Ruibetsu,
                                                F_System_Type = "KBN",
                                                F_Cycle = header.F_Delivery_Trip.ToString(),
                                                F_Plant_CD = header.F_Plant,
                                                F_Store_CD = header.F_Delivery_Dock,
                                                F_Receive_Qty = devQty,
                                                F_Receive_date = pdsDetailSingle.F_Receive_Date.Value.ToString("yyyyMMdd").Trim(),
                                                F_Supplier_Code = header.F_Supplier_Code,
                                                F_Supplier_Plant = header.F_Supplier_Plant,
                                                F_Inventory_Flg = '0',
                                                F_Upload_Flg = '0',
                                                F_UpdateBy = "KBN",
                                                F_UpdateDate = DateTime.Now.Date,
                                                F_ID = 0,
                                                F_Pds_No = "",
                                                F_Pack_Code = ""
                                            };
                                            await _PPM3Context.AddAsync(local);
                                        }
                                    }
                                    else
                                    {
                                        _result = @"{
                                        ""status"":""400"",
                                        ""response"":""OK"",
                                        ""title"": ""Receive Separate Error"",
                                        ""message"": ""Receive Separate Not Complete""
                                        }";
                                        return Ok(_result);
                                    }
                                }
                                else
                                {
                                    var _singleReceiveAll = await _KB3Context.TB_REC_DETAIL
                                                .SingleOrDefaultAsync(
                                                x => x.F_OrderNo == PDSNo && x.F_Part_No == partNo
                                                && x.F_Unit_Amount == sumQty);

                                    if (_singleReceiveAll != null) 
                                    {
                                        _singleReceiveAll.F_Receive_amount = sumQty;
                                        _singleReceiveAll.F_Receive_Date = now;
                                        _KB3Context.TB_REC_DETAIL.Update(_singleReceiveAll);

                                        var header = await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_OrderNo == PDSNo);
                                        if (header != null)
                                        {
                                            if (devQty != 0)
                                            {
                                                _isZeroRec = false;
                                                T_Receive_Local local = new T_Receive_Local
                                                {
                                                    F_Order_No = header.F_OrderNo,
                                                    F_Part_No = _singleReceiveAll.F_Part_No,
                                                    F_Ruibetsu = _singleReceiveAll.F_Ruibetsu,
                                                    F_System_Type = "KBN",
                                                    F_Cycle = header.F_Delivery_Trip.ToString(),
                                                    F_Plant_CD = header.F_Plant,
                                                    F_Store_CD = header.F_Delivery_Dock,
                                                    F_Receive_Qty = devQty,
                                                    F_Receive_date = _singleReceiveAll.F_Receive_Date.Value.ToString("yyyyMMdd").Trim(),
                                                    F_Supplier_Code = header.F_Supplier_Code,
                                                    F_Supplier_Plant = header.F_Supplier_Plant,
                                                    F_Inventory_Flg = '0',
                                                    F_Upload_Flg = '0',
                                                    F_UpdateBy = "KBN",
                                                    F_UpdateDate = DateTime.Now.Date,
                                                    F_ID = 0,
                                                    F_Pds_No = "",
                                                    F_Pack_Code = ""
                                                };
                                                await _PPM3Context.AddAsync(local);
                                            }
                                        }
                                        else
                                        {
                                            _result = @"{
                                                ""status"":""400"",
                                                ""response"":""OK"",
                                                ""title"": ""Receive Separate Error"",
                                                ""message"": ""No Data from TB Receive Header""
                                                }";
                                            return Ok(_result);
                                        }
                                    }
                                    else 
                                    {
                                        _result = @"{
                                        ""status"":""400"",
                                        ""response"":""OK"",
                                        ""title"": ""Receive Separate Error"",
                                        ""message"": ""Receive Separate Not Complete""
                                        }";
                                        return Ok(_result);
                                    }
                                }
                                    
                            }
                            if (_isZeroRec)
                            {
                                _result = @"{
                                        ""status"":""400"",
                                        ""response"":""OK"",
                                        ""title"": ""Receive Separate Error"",
                                        ""message"": ""Did't have Any Delivery Qty to Receive""
                                        }";
                                return Ok(_result);
                            }
                            if (_isReceiveAll)
                            {
                                var header = await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_OrderNo == PDSNo);
                                if (header != null)
                                {
                                    header.F_MRN_Flag = "2";
                                    _KB3Context.TB_REC_HEADER.Update(header);
                                }
                            }

                            _result = @"{
                                ""status"":""200"",
                                ""response"":""OK"",
                                ""title"": ""Receive Special Success"",
                                ""message"": ""Receive Special Complete""
                                }";
                            await _KB3Context.SaveChangesAsync();
                            await _PPM3Context.SaveChangesAsync();
                            return Ok(_result);
                        }
                    }
                    else
                    {
                        //_result = @"{
                        //    ""status"":""400"",
                        //    ""response"":""OK"",
                        //    ""title"": ""Receive Separate Error"",
                        //    ""message"": "" " +_result+ @"}""
                        //    }";
                        _result = @"{
                            ""status"":""400"",
                            ""response"":""OK"",
                            ""title"": ""Receive Separate Error"",
                            ""message"": ""No Data""
                            }";
                        return Ok(_result);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString(), "application/json");
            }
        }
    }
}
