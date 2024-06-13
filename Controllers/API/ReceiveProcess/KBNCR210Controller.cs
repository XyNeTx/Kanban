using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.KB3.Receive_Process;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;

namespace KANBAN.Controllers.API.ReceiveProcess
{
    public class KBNCR210Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly SerilogLibs _SerilogLibs;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNCR210Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            PPMInvenContext pPMInvenContext,
            PPM3Context pPM3Context,
            KB3Context kB3Context,
            SerilogLibs serilogLibs
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _PPM3Context = pPM3Context;
            _PPMInvenContext = pPMInvenContext;
            _SerilogLibs = serilogLibs;
        }



        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                _BearerClass.Authentication(Request);
                var user = _BearerClass.UserCode.ToString();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                if (pData != null) _json = JsonConvert.DeserializeObject(pData);

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

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

        public void setConString()
        {
            try
            {
                if (_KBCN.Plant.ToString() == "3")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB3Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPM3Connection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
                else if (_KBCN.Plant.ToString() == "2")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB2Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPMConnection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
                else if (_KBCN.Plant.ToString() == "1")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB1Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPMConnection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckPDSNo([FromBody] string data)
        {
            try
            {
                setConString();
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
                            if (!PDSNo.StartsWith("7Y") && !PDSNo.StartsWith("7Z"))
                            {
                                string _result = @"{
                                    ""status"":""400"",
                                    ""response"":""OK"",
                                    ""title"": ""Check PDS No. Error"",
                                    ""message"": ""ไม่สามารถรับชิ้นส่วนประเภท Normal,Urgent ได้ กรุณารับชิ้นส่วนใน Receive Part""
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
                                        ""message"": ""Please Check Barcode Again!!! Read Data Error""
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
                setConString();
                var recHeader = await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_OrderNo == PDSNo);
                if (recHeader != null)
                {
                    if (recHeader.F_Issued_Date > DateTime.Now)
                    {
                        _result = @"{
                        ""status"":""400"",
                        ""response"":""OK"",
                        ""title"": ""Get Data from PDS No. Error"",
                        ""message"": ""Can not receive because Issued Date More Than Receive Date!""
                        }";
                    }
                    else if (recHeader.F_Status == 'D')
                    {
                        _result = @"{
                            ""status"":""400"",
                            ""response"":""OK"",
                            ""title"": ""Get Data from PDS No. Error"",
                            ""message"": ""PDS have been deleted! Please check Data again!""
                            }";
                        return Ok(_result);
                    }
                    else if (recHeader.F_MRN_Flag == "2")
                    {
                        _result = @"{
                            ""status"":""400"",
                            ""response"":""OK"",
                            ""title"": ""Get Data from PDS No. Error"",
                            ""message"": ""This PDS No : " + PDSNo + " Already Received" + @"""
                            }";
                        return Ok(_result);
                    }
                    var recDetail = await _KB3Context.TB_REC_DETAIL
                        .Where(x => x.F_OrderNo == PDSNo).ToListAsync();
                    if (recDetail != null)
                    {
                        var recDetailCustom = recDetail.Select(x => new
                        {
                            x.F_No,
                            x.F_OrderNo,
                            x.F_Part_No,
                            x.F_Part_Name,
                            F_Receive_Date = x.F_Receive_Date.Value.ToString("dd/MM/yyyy"), // เอาแค่วันเดือนปี
                            x.F_Unit_Amount,
                            x.F_Receive_amount,
                            F_Dev_Qty = x.F_Unit_Amount - x.F_Receive_amount
                        });
                        string _jsonData = JsonConvert.SerializeObject(recDetailCustom);
                        _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data Found"",
                        ""data"": " + _jsonData + @"}";
                    }
                }
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString(), "application/json");
            }
        }

        public async Task<IActionResult> Receive([FromBody] string data)
        {
            try
            {
                setConString();
                string _result = "";
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                var systemControl = await _PPMInvenContext.T_System_Control.SingleOrDefaultAsync(x => x.F_Code == "CTL");
                string isMonthEnd = systemControl.F_Value1;
                if (Int32.Parse(DateTime.Now.ToString("yyyyMM")) < Int32.Parse(isMonthEnd))
                {
                    _result = @"{
                        ""status"":""400"",
                        ""response"":""OK"",
                        ""title"": ""Get Data from PDS No. Error"",
                        ""message"": ""Can not Receive Data.Because This Receive Date less than Month-End of Inventory.""
                        }";
                    return Ok(_result);
                }
                _BearerClass.Authentication(Request);
                var user = _BearerClass.UserCode.ToString();
                char plant = _BearerClass.Plant[0];
                if (data != null)
                {
                    dynamic _json = JsonConvert.DeserializeObject(data);
                    DateTime now = DateTime.Now.Date;
                    if (_json != null)
                    {
                        var JsonData = _json["JsonData"];
                        int index = (JsonData.Count) - 1; //get index for last index to get PDSNo
                        string PDSNo = JsonData[index].PDSNo.ToString().Trim();
                        char dr = ' ';
                        if (PDSNo.Length == 14)
                        {
                            var recHead = await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_Barcode == PDSNo);
                            if (recHead != null)
                            {
                                PDSNo = PDSNo.Remove(13, 1);
                            }
                            else
                            {
                                _result = @"{
                                        ""status"":""400"",
                                        ""response"":""OK"",
                                        ""title"": ""Receive Special Error"",
                                        ""message"": ""Didn't have data for this Barcode""
                                        }";
                                return Ok(_result);
                            }
                        }
                        if (PDSNo.StartsWith("7Z"))
                        {
                            var recHead = await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_OrderNo == PDSNo);
                            if (recHead.F_DR == "1144001" || recHead.F_DR == "1144002" || recHead.F_DR == "1145007")
                            {
                                dr = '0';
                            }
                            else
                            {
                                dr = '9';
                            }
                        }
                        else
                        {
                            dr = '0';
                        }
                        JsonData.RemoveAt(index);
                        bool _isReceiveAll = true;
                        bool _isZeroRec = true;
                        foreach (var detail in JsonData)
                        {
                            string partNo = detail["Part No."].ToString();
                            int devQty = detail["Receive Qty"];
                            int alrQty = detail["Already Received"];
                            int sumQty = devQty + alrQty;
                            var pdsDetailSingle = await _KB3Context.TB_REC_DETAIL
                                .SingleOrDefaultAsync(x => x.F_OrderNo == PDSNo && x.F_Part_No == partNo
                                && x.F_Unit_Amount != sumQty);
                            if (pdsDetailSingle != null)
                            {
                                string packCode = pdsDetailSingle.F_Address;
                                if (packCode.StartsWith("Pack:"))
                                {
                                    packCode = packCode.Substring(5, 5).Trim();
                                }
                                else
                                {
                                    packCode = "";
                                }
                                if (devQty != 0)
                                {
                                    var header = await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_OrderNo == PDSNo);
                                    header.F_MRN_Flag = "1";
                                    _KB3Context.TB_REC_HEADER.Update(header);
                                    pdsDetailSingle.F_Receive_amount = sumQty;
                                    _KB3Context.TB_REC_DETAIL.Update(pdsDetailSingle);
                                    _isReceiveAll = false;
                                    _isZeroRec = false;
                                    T_Receive_Local local = new T_Receive_Local
                                    {
                                        F_Order_No = header.F_OrderNo,
                                        F_Part_No = pdsDetailSingle.F_Part_No,
                                        F_Ruibetsu = pdsDetailSingle.F_Ruibetsu,
                                        F_System_Type = "SPC",
                                        F_Cycle = "00",
                                        F_Plant_CD = header.F_Plant,
                                        F_Store_CD = header.F_Delivery_Dock,
                                        F_Receive_Qty = devQty,
                                        F_Receive_date = pdsDetailSingle.F_Receive_Date.Value.ToString("yyyyMMdd").Trim(),
                                        F_Supplier_Code = header.F_Supplier_Code,
                                        F_Supplier_Plant = header.F_Supplier_Plant,
                                        F_Inventory_Flg = dr,
                                        F_Upload_Flg = '0',
                                        F_UpdateBy = user,
                                        F_UpdateDate = DateTime.Now.Date,
                                        F_ID = 0,
                                        F_Pds_No = "",
                                        F_Pack_Code = packCode
                                    };
                                    await _PPM3Context.AddAsync(local);
                                    _SerilogLibs.WriteLog($"Receive Seperate Part Special : UPDATE TB_REC_DEtail : {PDSNo} Line 407", UserName, HostName);
                                    _SerilogLibs.WriteLog($"Receive Seperate Part Special : {PDSNo} Line 408", UserName, HostName);
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
                                    _KB3Context.TB_REC_DETAIL.Update(_singleReceiveAll);
                                    string packCode = _singleReceiveAll.F_Address;
                                    if (packCode.StartsWith("Pack:"))
                                    {
                                        packCode = packCode.Substring(5, 5).Trim();
                                    }
                                    else
                                    {
                                        packCode = "";
                                    }
                                    var header = await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_OrderNo == PDSNo);
                                    if (devQty != 0)
                                    {
                                        _isZeroRec = false;
                                        T_Receive_Local local = new T_Receive_Local
                                        {
                                            F_Order_No = header.F_OrderNo,
                                            F_Part_No = _singleReceiveAll.F_Part_No,
                                            F_Ruibetsu = _singleReceiveAll.F_Ruibetsu,
                                            F_System_Type = "SPC",
                                            F_Cycle = "00",
                                            F_Plant_CD = header.F_Plant,
                                            F_Store_CD = header.F_Delivery_Dock,
                                            F_Receive_Qty = devQty,
                                            F_Receive_date = _singleReceiveAll.F_Receive_Date.Value.ToString("yyyyMMdd").Trim(),
                                            F_Supplier_Code = header.F_Supplier_Code,
                                            F_Supplier_Plant = header.F_Supplier_Plant,
                                            F_Inventory_Flg = dr,
                                            F_Upload_Flg = '0',
                                            F_UpdateBy = user,
                                            F_UpdateDate = DateTime.Now.Date,
                                            F_ID = 0,
                                            F_Pds_No = "",
                                            F_Pack_Code = packCode
                                        };
                                        await _PPM3Context.AddAsync(local);
                                    }
                                }
                                else
                                {
                                    _result = @"{
                                        ""status"":""400"",
                                        ""response"":""OK"",
                                        ""title"": ""Receive Special Error"",
                                        ""message"": ""Receive Special Not Complete""
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
                                    ""title"": ""Receive Special Error"",
                                    ""message"": ""Can't Receive 0 Qty in all part""
                                    }";
                            return Content(_result, "application/json");
                        }

                        if (_isReceiveAll)
                        {
                            var header = await _KB3Context.TB_REC_HEADER.SingleOrDefaultAsync(x => x.F_OrderNo == PDSNo);
                            if (header != null)
                            {
                                header.F_MRN_Flag = "2";
                                _KB3Context.TB_REC_HEADER.Update(header);
                                _PPM3Context.Database.ExecuteSqlRaw(
                                    "exec [dbo].[SP_Interface_Inven_Special] @Plant, @PDS, @User",
                                    new SqlParameter("@Plant", plant),
                                    new SqlParameter("@PDS", PDSNo),
                                    new SqlParameter("@User", user)
                                );
                                _SerilogLibs.WriteLog($"Receive Seperate Part Special : UPDATE TB_REC_DEtail : {PDSNo} Line 492", UserName, HostName);
                                _SerilogLibs.WriteLog($"Receive Seperate Part Special : {PDSNo} Line 493", UserName, HostName);
                            }
                        }

                        _result = @"{
                                ""status"":""200"",
                                ""response"":""OK"",
                                ""title"": ""Receive Special Success"",
                                ""message"": ""Receive Separate Complete""
                                }";
                        await _KB3Context.SaveChangesAsync();
                        await _PPM3Context.SaveChangesAsync();
                        return Ok(_result);
                    }
                }
                else
                {
                    _result = @"{
                            ""status"":""400"",
                            ""response"":""OK"",
                            ""title"": ""Receive Special Error"",
                            ""message"": ""Receive Separate Complete""
                            }";
                    return BadRequest(_result);
                }
                return Ok();
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }

    }
}
