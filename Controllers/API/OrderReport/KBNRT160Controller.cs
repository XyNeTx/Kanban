using Azure;
using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using System.Data;
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

        public async Task<IActionResult> Initial()
        {
            try
            {
                
                string _result = "";

                var supplierDB = await _KB3Context.TB_MS_PartOrder.Select(x => new
                {
                    Sup_CD = x.F_Supplier_Cd.Trim() + "-" + x.F_Supplier_Plant.ToString().Trim(),
                }).ToListAsync();

                var kanbanDB = await _KB3Context.TB_MS_PartOrder.Select(x => new
                {
                    F_Sebango = x.F_Kanban_No
                }).ToListAsync();

                var storeDB = await _KB3Context.TB_MS_PartOrder.Select(x => new
                {
                    F_Store_CD = x.F_Store_Code
                }).ToListAsync();

                var partDB = await _KB3Context.TB_MS_PartOrder.Select(x => new
                {
                    prt_no = x.F_Part_No + "-" + x.F_Ruibetsu.Trim()
                }).ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(supplierDB.OrderBy(x => x.Sup_CD).Distinct());
                string _jsonData2 = JsonConvert.SerializeObject(kanbanDB.OrderBy(x => x.F_Sebango).Distinct());
                string _jsonData3 = JsonConvert.SerializeObject(storeDB.OrderBy(x => x.F_Store_CD).Distinct());
                string _jsonData4 = JsonConvert.SerializeObject(partDB.OrderBy(x => x.prt_no).Distinct());
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

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<IActionResult> DeleteTemp()
        {
            try
            {
                
                string _result = "";
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }

                await _KB3Context.Database.ExecuteSqlRawAsync
                        ("DELETE FROM RPT_KBNRT_160 WHERE F_Update_By = @UserLogon AND F_Host_name = @Host_name",
                        new SqlParameter("@UserLogon", UserName),
                        new SqlParameter("@Host_name", HostName)
                        );

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": null
                                    }";

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
                    .Select(x => new
                    {
                        F_Supplier_Cd = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant,
                        F_Kanban_No = x.F_Kanban_No
                    })
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Select(x => new
                    {
                        F_Sebango = x.F_Kanban_No
                    }).ToListAsync();

                var storeDB = await _KB3Context.TB_MS_PartOrder
                    .Select(x => new
                    {
                        F_Supplier_Cd = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant,
                        F_Store_Code = x.F_Store_Code
                    })
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Select(x => new
                    {
                        F_Store_CD = x.F_Store_Code
                    }).ToListAsync();

                var partDB = await _KB3Context.TB_MS_PartOrder
                    .Select(x => new
                    {
                        F_Supplier_Cd = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant,
                        F_Part_No = x.F_Part_No,
                        F_Ruibetsu = x.F_Ruibetsu
                    })
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Select(x => new
                    {
                        prt_no = x.F_Part_No + "-" + x.F_Ruibetsu.Trim()
                    }).ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(kanbanDB.OrderBy(x => x.F_Sebango).Distinct());
                string _jsonData2 = JsonConvert.SerializeObject(storeDB.OrderBy(x => x.F_Store_CD).Distinct());
                string _jsonData3 = JsonConvert.SerializeObject(partDB.OrderBy(x => x.prt_no).Distinct());

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
                
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string kbnFrom = _json["kbnFrom"];
                string kbnTo = _json["kbnTo"];

                var storeDB = await _KB3Context.TB_MS_PartOrder
                    .Select(x => new
                    {
                        F_Supplier_Cd = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant,
                        F_Kanban_No = x.F_Kanban_No,
                        F_Store_Code = x.F_Store_Code
                    })
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Where(x => x.F_Kanban_No.CompareTo(kbnFrom) >= 0 && x.F_Kanban_No.CompareTo(kbnTo) <= 0)
                    .Select(x => new
                    {
                        F_Store_CD = x.F_Store_Code
                    }).ToListAsync();

                var partDB = await _KB3Context.TB_MS_PartOrder
                    .Select(x => new
                    {
                        F_Supplier_Cd = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant,
                        F_Kanban_No = x.F_Kanban_No,
                        F_Part_No = x.F_Part_No,
                        F_Ruibetsu = x.F_Ruibetsu
                    })
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Where(x => x.F_Kanban_No.CompareTo(kbnFrom) >= 0 && x.F_Kanban_No.CompareTo(kbnTo) <= 0)
                    .Select(x => new
                    {
                        prt_no = x.F_Part_No + "-" + x.F_Ruibetsu.Trim()
                    }).ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(storeDB.OrderBy(x => x.F_Store_CD).Distinct());
                string _jsonData2 = JsonConvert.SerializeObject(partDB.OrderBy(x => x.prt_no).Distinct());

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
                
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string kbnFrom = _json["kbnFrom"];
                string kbnTo = _json["kbnTo"];
                string storeFrom = _json["storeFrom"];
                string storeTo = _json["storeTo"];

                var partDB = await _KB3Context.TB_MS_PartOrder
                    .Select(x => new
                    {
                        F_Supplier_Cd = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant,
                        F_Kanban_No = x.F_Kanban_No,
                        F_Store_Code = x.F_Store_Code,
                        F_Part_No = x.F_Part_No,
                        F_Ruibetsu = x.F_Ruibetsu
                    })
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Where(x => x.F_Kanban_No.CompareTo(kbnFrom) >= 0 && x.F_Kanban_No.CompareTo(kbnTo) <= 0)
                    .Where(x => x.F_Store_Code.CompareTo(storeFrom) >= 0 && x.F_Store_Code.CompareTo(storeTo) <= 0)
                    .Select(x => new
                    {
                        prt_no = x.F_Part_No + "-" + x.F_Ruibetsu.Trim()
                    }).ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(partDB.OrderBy(x => x.prt_no).Distinct());

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
                
                string _result = "";
                string userName = HttpContext.Session.GetString("USER_NAME");
                string hostName = HttpContext.Session.GetString("USER_DEVICENAME");
                string Plant = HttpContext.Request.Cookies["plantCode"].ToString();
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

                int result = await _KB3Context.Database.ExecuteSqlRawAsync
                    ("INSERT INTO RPT_KBNRT_160 (Order_date, Order_shift, Order_No,Supplier, Store, Part_no, Kanban_no, Qty_pcs, Qty_kb, Box_qty, chk_ord_date,F_Status, F_Update_By, F_Host_Name,F_Delivery_Date) " +
                    "SELECT Order_Date,F_Issued_Shift, F_OrderNo,Sup, F_Delivery_Dock,Prt_No,F_kanban_No,F_Unit_Amount,Qty_KB,F_Box_Qty,chk_order_Date,F_Status,@UserName as f_Update_By, @HostName as F_host_Name,F_Delivery_Date " +
                    " FROM V_KBNRT_160_rpt WHERE F_Plant = @Plant AND (Sup >= @SupFrom AND Sup <= @SupTo) AND (F_Kanban_No >= @KBNFrom AND F_Kanban_No <= @KBNTo) " +
                    " AND ( F_Delivery_Dock >= @StoreFrom AND F_Delivery_Dock <= @StoreTo ) AND (Prt_no >= @PartFrom AND Prt_no <= @PartTo) AND (chk_Order_Date >= @OrderFrom AND chk_Order_Date <= @OrderTo) " +
                    " AND ( F_Issued_Shift = @ShiftFrom OR F_Issued_Shift = @ShiftTo OR F_Issued_Shift = '') ",
                    new SqlParameter("@UserName", userName),
                    new SqlParameter("@HostName", hostName),
                    new SqlParameter("@Plant", Plant),
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

                DataTable _dt = _KBCN.ExecuteSQL($"SELECT * FROM RPT_KBNRT_160 WHERE F_Update_By = '{userName}' AND F_Host_name = '{hostName}'", skipLog: true);
                if(_dt.Rows.Count == 0)
                {
                    _result = @"{
                                    ""status"":""404"",
                                    ""response"":""OK"",
                                    ""title"":""Report Data Not Found"",
                                    ""message"": ""Please Try Other Option!""
                                    }";

                    return Ok(_result);
                }

                string _jsonData = JsonConvert.SerializeObject(userName);
                string _jsonData2 = JsonConvert.SerializeObject(hostName);

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
    }
}
