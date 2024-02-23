using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.Util;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT180Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNRT180Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            PPMInvenContext pPMInvenContext,
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
            _PPMInvenContext = pPMInvenContext;
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

        public async Task<IActionResult> Initial()
        {
            try
            {
                setConString();
                string _result = "";
                string Plant = _KBCN.Plant.ToString();
                string now = DateTime.Now.ToString("yyyyMMdd");

                var supDB = await _KB3Context.TB_MS_PartOrder.Where(x => x.F_Store_Code.StartsWith(Plant)
                && (x.F_Start_Date.CompareTo(now) <= 0 && x.F_End_Date.CompareTo(now) >= 0))
                    .Select(x => new
                    {
                        F_Supplier = x.F_Supplier_Cd.Trim() + '-' + x.F_Supplier_Plant
                    }).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(supDB);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @"
                                    }";


                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<IActionResult> OnReportBtnOrder([FromBody] string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return BadRequest();
            }
            try
            {
                setConString();
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject<dynamic>(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                string UserCode = HttpContext.Session.GetString("USER_CODE");

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM RPT_KBNRT_180 WHERE F_Update_By = {0} AND F_Host_name = {1}",
                    UserName, HostName);

                var reportList = await _KB3Context.V_KBNRT_180_Rpt.Where(x => x.F_Supplier.CompareTo(supFrom) >= 0 && x.F_Supplier.CompareTo(supTo) <= 0)
                    .Where(x => x.Chk_ord_date.CompareTo(dateFrom) >= 0 && x.Chk_ord_date.CompareTo(dateTo) <= 0).OrderBy(x => x.F_Supplier).ToListAsync();

                if (reportList.Count == 0)
                {
                    _result = @"{
                                    ""status"":""404"",
                                    ""response"":""OK"",
                                    ""title"":""Report Data Not Found"",
                                    ""message"": ""Please Try Other Option!""
                                    }";

                    return Ok(_result);
                }

                foreach (var report in reportList)
                {
                    await _KB3Context.Database.ExecuteSqlRawAsync("INSERT INTO RPT_KBNRT_180 (Supplier_name, Supplier_code, Part_no, Kanban_no, Store_code, " +
                        "Order_date, Order_shift, Delivery_date, Delivery_trip, Status, F_Update_By, F_Host_Name) " +
                        " VALUES ( {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11} )", report.F_short_name, report.F_Supplier, report.F_Part_no, report.F_Kanban_No,
                        report.F_Store_Cd, report.F_Order_date, report.F_Order_Shift, report.F_Deli_date, report.F_Delivery_Trip, report.F_Status,
                        UserName, HostName);
                }

                string _jsonData = JsonConvert.SerializeObject(UserName);
                string _jsonData2 = JsonConvert.SerializeObject(HostName);
                string _jsonData3 = JsonConvert.SerializeObject(UserCode);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @",
                                    ""data2"": " + _jsonData2 + @",
                                    ""data3"": " + _jsonData3 + @"
                                    }";

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        public async Task<IActionResult> OnReportBtnDelivery([FromBody] string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return BadRequest();
            }
            try
            {
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject<dynamic>(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                string UserCode = HttpContext.Session.GetString("USER_CODE");

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM RPT_KBNRT_180 WHERE F_Update_By = {0} AND F_Host_name = {1}",
                    UserName, HostName);

                var reportList = await _KB3Context.V_KBNRT_180_Rpt.Where(x => x.F_Supplier.CompareTo(supFrom) >= 0 && x.F_Supplier.CompareTo(supTo) <= 0)
                    .Where(x => x.Chk_deli_date.CompareTo(dateFrom) >= 0 && x.Chk_deli_date.CompareTo(dateTo) <= 0).OrderBy(x => x.F_Supplier).ToListAsync();

                if (reportList.Count == 0)
                {
                    _result = @"{
                                    ""status"":""404"",
                                    ""response"":""OK"",
                                    ""title"":""Report Data Not Found"",
                                    ""message"": ""Please Try Other Option!""
                                    }";

                    return Ok(_result);
                }

                foreach (var report in reportList)
                {
                    await _KB3Context.Database.ExecuteSqlRawAsync("INSERT INTO RPT_KBNRT_180 (Supplier_name, Supplier_code, Part_no, Kanban_no, Store_code, " +
                        "Order_date, Order_shift, Delivery_date, Delivery_trip, Status, F_Update_By, F_Host_Name) " +
                        " VALUES ( {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11} )", report.F_short_name, report.F_Supplier, report.F_Part_no, report.F_Kanban_No,
                        report.F_Store_Cd, report.F_Order_date, report.F_Order_Shift, report.F_Deli_date, report.F_Delivery_Trip, report.F_Status,
                        UserName, HostName);
                }

                string _jsonData = JsonConvert.SerializeObject(UserName);
                string _jsonData2 = JsonConvert.SerializeObject(HostName);
                string _jsonData3 = JsonConvert.SerializeObject(UserCode);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @",
                                    ""data2"": " + _jsonData2 + @",
                                    ""data3"": " + _jsonData3 + @"
                                    }";

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}
