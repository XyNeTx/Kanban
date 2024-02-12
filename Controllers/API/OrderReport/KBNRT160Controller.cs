using Azure;
using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.NetworkInformation;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT160Controller : Controller
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

        public KBNRT160Controller(
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

                var supplierDB = await _KB3Context.TB_MS_PartOrder.Select(x => new
                {
                    Sup_CD = x.F_Supplier_Cd.Trim() + '-' + x.F_Supplier_Plant.ToString().Trim(),
                }).OrderBy(x => x.Sup_CD).Distinct().ToListAsync();

                var kanbanDB = await _KB3Context.TB_MS_PartOrder.Select(x => new
                {
                    F_Sebango = x.F_Kanban_No
                }).OrderBy(x => x.F_Sebango).Distinct().ToListAsync();

                var storeDB = await _KB3Context.TB_MS_PartOrder.Select(x => new
                {
                    F_Store_CD = x.F_Store_Code
                }).OrderBy(x => x.F_Store_CD).Distinct().ToListAsync();

                var partDB = await _KB3Context.TB_MS_PartOrder.Select(x => new
                {
                    prt_no = x.F_Part_No + '-' + x.F_Ruibetsu.Trim()
                }).OrderBy(x => x.prt_no).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(supplierDB);
                string _jsonData2 = JsonConvert.SerializeObject(kanbanDB);
                string _jsonData3 = JsonConvert.SerializeObject(storeDB);
                string _jsonData4 = JsonConvert.SerializeObject(partDB);
                string userName = HttpContext.Session.GetString("USER_NAME");
                string hostName = HttpContext.Session.GetString("USER_DEVICENAME");

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @",
                                    ""data2"": " + _jsonData2 + @",
                                    ""data3"": " + _jsonData3 + @",
                                    ""data4"": " + _jsonData4 + @"
                                    }";
                await _KB3Context.Database.ExecuteSqlRawAsync
                        ("DELETE FROM RPT_KBNRT_160 WHERE F_Update_By = @UserLogon AND F_Host_name = @Host_name",
                        new SqlParameter("@UserLogon", userName),
                        new SqlParameter("@Host_name", hostName)
                        );

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<IActionResult> OnSupplierChange([FromBody] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest();
            }
            try
            {
                setConString();
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                //string kbnFrom = _json["kbnFrom"];
                //string kbnTo = _json["kbnTo"];
                //string storeFrom = _json["storeFrom"];
                //string storeTo = _json["storeTo"];
                //string partFrom = _json["partFrom"];
                //string partTo = _json["partTo"];
                //string orderFrom = _json["orderFrom"];
                //string orderTo = _json["orderTo"];
                //string shiftFrom = _json["shiftFrom"];
                //string shiftTo = _json["shiftTo"];

                var kanbanDB = await _KB3Context.TB_MS_PartOrder
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Select(x => new
                    {
                        F_Sebango = x.F_Kanban_No
                    }).OrderBy(x => x.F_Sebango).Distinct().ToListAsync();

                var storeDB = await _KB3Context.TB_MS_PartOrder
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Select(x => new
                    {
                        F_Store_CD = x.F_Store_Code
                    }).OrderBy(x => x.F_Store_CD).Distinct().ToListAsync();

                var partDB = await _KB3Context.TB_MS_PartOrder
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Select(x => new
                    {
                        prt_no = x.F_Part_No + '-' + x.F_Ruibetsu.Trim()
                    }).OrderBy(x => x.prt_no).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(kanbanDB);
                string _jsonData2 = JsonConvert.SerializeObject(storeDB);
                string _jsonData3 = JsonConvert.SerializeObject(partDB);

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

        public async Task<IActionResult> OnKANBANChange([FromBody] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest();
            }
            try
            {
                setConString();
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string kbnFrom = _json["kbnFrom"];
                string kbnTo = _json["kbnTo"];

                var storeDB = await _KB3Context.TB_MS_PartOrder
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Where(x => x.F_Kanban_No.CompareTo(kbnFrom) >= 0 && x.F_Kanban_No.CompareTo(kbnTo) <= 0)
                    .Select(x => new
                    {
                        F_Store_CD = x.F_Store_Code
                    }).OrderBy(x => x.F_Store_CD).Distinct().ToListAsync();

                var partDB = await _KB3Context.TB_MS_PartOrder
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Where(x => x.F_Kanban_No.CompareTo(kbnFrom) >= 0 && x.F_Kanban_No.CompareTo(kbnTo) <= 0)
                    .Select(x => new
                    {
                        prt_no = x.F_Part_No + '-' + x.F_Ruibetsu.Trim()
                    }).OrderBy(x => x.prt_no).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(storeDB);
                string _jsonData2 = JsonConvert.SerializeObject(partDB);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @",
                                    ""data2"": " + _jsonData2 + @"
                                    }";

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<IActionResult> OnStoreChange([FromBody] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest();
            }
            try
            {
                setConString();
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string kbnFrom = _json["kbnFrom"];
                string kbnTo = _json["kbnTo"];
                string storeFrom = _json["storeFrom"];
                string storeTo = _json["storeTo"];

                var partDB = await _KB3Context.TB_MS_PartOrder
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Where(x => x.F_Kanban_No.CompareTo(kbnFrom) >= 0 && x.F_Kanban_No.CompareTo(kbnTo) <= 0)
                    .Where(x => x.F_Store_Code.CompareTo(storeFrom) >= 0 && x.F_Store_Code.CompareTo(storeTo) <= 0)
                    .Select(x => new
                    {
                        prt_no = x.F_Part_No + '-' + x.F_Ruibetsu.Trim()
                    }).OrderBy(x => x.prt_no).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(partDB);

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
        public async Task<IActionResult> OnReportBtnClick([FromBody] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest();
            }
            try
            {
                setConString();
                string _result = "";
                string userName = HttpContext.Session.GetString("USER_NAME");
                string hostName = HttpContext.Session.GetString("USER_DEVICENAME");
                await _KB3Context.Database.ExecuteSqlRawAsync
                        ("DELETE FROM RPT_KBNRT_160 WHERE F_Update_By = @UserLogon AND F_Host_name = @Host_name",
                        new SqlParameter("@UserLogon", userName),
                        new SqlParameter("@Host_name", hostName)
                        );

                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string kbnFrom = _json["kbnFrom"];
                string kbnTo = _json["kbnTo"];
                string storeFrom = _json["storeFrom"];
                string storeTo = _json["storeTo"];
                string partFrom = _json["partFrom"];
                string partTo = _json["partTo"];
                string orderFrom = _json["orderFrom"];
                string orderTo = _json["orderTo"];
                string shiftFrom = _json["shiftFrom"];
                string shiftTo = _json["shiftTo"];

                await _KB3Context.Database.ExecuteSqlRawAsync
                    ("INSERT INTO RPT_KBNRT_160 (Order_date, Order_shift, Order_No,Supplier, Store, Part_no, Kanban_no, Qty_pcs, Qty_kb, Box_qty, chk_ord_date,F_Status, F_Update_By, F_Host_Name,F_Delivery_Date) " +
                    "SELECT Order_Date,F_Issued_Shift, F_OrderNo,Sup, F_Delivery_Dock,Prt_No,F_kanban_No,F_Unit_Amount,Qty_KB,F_Box_Qty,chk_order_Date,F_Status,@UserName as f_Update_By, @HostName as F_host_Name,F_Delivery_Date " +
                    " FROM V_KBNRT_160_rpt WHERE F_Plant = @Plant AND (Sup >= @SupFrom AND Sup <= @SupTo) AND (F_Kanban_No >= @KBNFrom AND F_Kanban_No <= @KBNTo) " +
                    " AND ( F_Delivery_Dock >= @StoreFrom AND F_Delivery_Dock <= @StoreTo ) AND (Prt_no >= @PartFrom AND Prt_no <= @PartTo) AND (chk_Order_Date >= @OrderFrom AND chk_Order_Date <= @OrderTo) " +
                    " AND ( F_Issued_Shift = @ShiftFrom OR F_Issued_Shift = @ShiftTo ) ",
                    new SqlParameter("@UserName", userName),
                    new SqlParameter("@HostName", hostName),
                    new SqlParameter("@Plant", _KBCN.Plant),
                    new SqlParameter("@SupFrom", supFrom),
                    new SqlParameter("@SupTo", supTo),
                    new SqlParameter("@KBNFrom", kbnFrom),
                    new SqlParameter("@KBNTo", kbnTo),
                    new SqlParameter("@StoreFrom", storeFrom),
                    new SqlParameter("@StoreTo", storeTo),
                    new SqlParameter("@PartFrom", partFrom),
                    new SqlParameter("@PartTo", partTo),
                    new SqlParameter("@OrderFrom", orderFrom),
                    new SqlParameter("@OrderTo", orderTo),
                    new SqlParameter("@ShiftFrom", shiftFrom),
                    new SqlParameter("@ShiftTo", shiftTo)
                    );

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + userName + @",
                                    ""data2"": " + hostName + @"
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
