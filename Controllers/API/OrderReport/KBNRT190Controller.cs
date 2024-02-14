using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT190Controller : Controller
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

        public KBNRT190Controller(
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
                string _result = "";
                var tripDB = await _KB3Context.V_KBNRT_190_Rpt.Select(x => x.Deli_trip).Distinct().OrderBy(x => x).ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(tripDB);

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
                return Content(ex.ToString());
            }
        }

        public async Task<IActionResult> OnDeliDateChange([FromBody] string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return BadRequest();
            }
            try
            {
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject<dynamic>(data);
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];

                var tripDB = await _KB3Context.V_KBNRT_190_Rpt.Where(x =>
                            (x.chk_Deli_Date.CompareTo(dateFrom) >= 0
                            && x.chk_Deli_Date.CompareTo(dateTo) <= 0)
                            && x.Deli_trip > 0)
                            .Where(x => x.Sup_Code.CompareTo(supFrom) >= 0 && x.Sup_Code.CompareTo(supTo) <= 0)
                            .Select(x => x.Deli_trip).Distinct().OrderBy(x => x).ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(tripDB);

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

        public async Task<IActionResult> OnReportBtnDelivery([FromBody] string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return BadRequest();
            }
            try
            {
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string kbnFrom = _json["kbnFrom"];
                string kbnTo = _json["kbnTo"];
                string storeFrom = _json["storeFrom"];
                string storeTo = _json["storeTo"];
                string partFrom = _json["partFrom"];
                string partTo = _json["partTo"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                int tripFrom = _json["tripFrom"];
                int tripTo = _json["tripTo"];
                char plant = _KBCN.Plant.ToString()[0];
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM RPT_KBNRT_190 WHERE F_Update_By = @UserName " +
                    " AND F_Host_name = @HostName",
                    new SqlParameter("@UserName", UserName),
                    new SqlParameter("@HostName", HostName));

                var rptList = await _KB3Context.V_KBNRT_190_Rpt.Where(x => x.F_Plant == plant)
                    .Where(x => x.Sup_Code.CompareTo(supFrom) >= 0 && x.Sup_Code.CompareTo(supTo) <= 0)
                    .Where(x => x.F_Kanban_No.CompareTo(kbnFrom) >= 0 && x.F_Kanban_No.CompareTo(kbnTo) <= 0)
                    .Where(x => x.F_Delivery_Dock.CompareTo(storeFrom) >= 0 && x.F_Delivery_Dock.CompareTo(storeTo) <= 0)
                    .Where(x => x.Prt_no.CompareTo(partFrom) >= 0 && x.Prt_no.CompareTo(partTo) <= 0)
                    .Where(x => x.chk_Deli_Date.CompareTo(dateFrom) >= 0 && x.chk_Deli_Date.CompareTo(dateTo) <= 0)
                    .Where(x => x.Deli_trip >= tripFrom && x.Deli_trip <= tripTo)
                    .ToListAsync();

                foreach (var rpt in rptList)
                {
                    await _KB3Context.Database.ExecuteSqlRawAsync("INSERT INTO RPT_KBNRT_190 (Supplier_cd, Store_cd, Status, PDS_no, KB_no, " +
                        "Part_no, Order_date, Shift, Deli_date, Trip, Box_qty, Pace, Total, Receive, F_Update_By, F_Host_Name) " +
                        "VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}) ",
                        rpt.Sup_Code, rpt.F_Delivery_Dock, rpt.F_Status, rpt.F_OrderNo, rpt.F_Kanban_No, rpt.Prt_no, rpt.Order_Date, rpt.F_Issued_Shift,
                        rpt.Deli_Date, rpt.Deli_trip, rpt.F_Box_Qty, rpt.Qty_KB, rpt.F_Unit_Amount, rpt.F_Receive_amount, UserName, HostName
                        );
                }

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
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string kbnFrom = _json["kbnFrom"];
                string kbnTo = _json["kbnTo"];
                string storeFrom = _json["storeFrom"];
                string storeTo = _json["storeTo"];
                string partFrom = _json["partFrom"];
                string partTo = _json["partTo"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                char plant = _KBCN.Plant.ToString()[0];
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM RPT_KBNRT_190 WHERE F_Update_By = @UserName " +
                    " AND F_Host_name = @HostName",
                    new SqlParameter("@UserName", UserName),
                    new SqlParameter("@HostName", HostName));

                var rptList = await _KB3Context.V_KBNRT_190_Rpt.Where(x => x.F_Plant == plant)
                    .Where(x => x.Sup_Code.CompareTo(supFrom) >= 0 && x.Sup_Code.CompareTo(supTo) <= 0)
                    .Where(x => x.F_Kanban_No.CompareTo(kbnFrom) >= 0 && x.F_Kanban_No.CompareTo(kbnTo) <= 0)
                    .Where(x => x.F_Delivery_Dock.CompareTo(storeFrom) >= 0 && x.F_Delivery_Dock.CompareTo(storeTo) <= 0)
                    .Where(x => x.Prt_no.CompareTo(partFrom) >= 0 && x.Prt_no.CompareTo(partTo) <= 0)
                    .Where(x => x.chk_Order_Date.CompareTo(dateFrom) >= 0 && x.chk_Order_Date.CompareTo(dateTo) <= 0)
                    .ToListAsync();

                foreach (var rpt in rptList)
                {
                    await _KB3Context.Database.ExecuteSqlRawAsync("INSERT INTO RPT_KBNRT_190 (Supplier_cd, Store_cd, Status, PDS_no, KB_no, " +
                        "Part_no, Order_date, Shift, Deli_date, Trip, Box_qty, Pace, Total, Receive, F_Update_By, F_Host_Name) " +
                        "VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}) ",
                        rpt.Sup_Code, rpt.F_Delivery_Dock, rpt.F_Status, rpt.F_OrderNo, rpt.F_Kanban_No, rpt.Prt_no, rpt.Order_Date, rpt.F_Issued_Shift,
                        rpt.Deli_Date, rpt.Deli_trip, rpt.F_Box_Qty, rpt.Qty_KB, rpt.F_Unit_Amount, rpt.F_Receive_amount, UserName, HostName
                        );
                }

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}
