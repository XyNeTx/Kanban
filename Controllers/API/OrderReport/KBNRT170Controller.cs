using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Linq;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT170Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly SerilogLibs _Serilog;
        private readonly FillDataTable _FillDT;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNRT170Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            PPMInvenContext pPMInvenContext,
            PPM3Context pPM3Context,
            KB3Context kB3Context,
            SerilogLibs serilogLibs,
            FillDataTable fillDataTable
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
            _Serilog = serilogLibs;
            _FillDT = fillDataTable;
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

                var sortTrip = await _KB3Context.TB_REC_HEADER.OrderBy(x=>x.F_Delivery_Trip).ToListAsync();
                var cycleDB = sortTrip.Select(x => x.F_Delivery_Trip).Distinct();

                string _jsonData = JsonConvert.SerializeObject(supDB);
                string _jsonData2 = JsonConvert.SerializeObject(cycleDB);

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
        public async Task<IActionResult> OnChange([FromBody] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest();
            }
            try
            {
                setConString();
                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                string _result = "";

                var cycleDB = await _KB3Context.TB_REC_HEADER.Select(x => new
                {
                    Sup_Code = x.F_Supplier_Code + "-" + x.F_Supplier_Plant,
                    Deli_Date = x.F_Delivery_Date,
                    Deli_Trip = x.F_Delivery_Trip
                })
                    .Where(x => x.Sup_Code == supFrom)
                    .Where(x => x.Deli_Date.CompareTo(dateFrom) >= 0 && x.Deli_Date.CompareTo(dateTo) <= 0)
                    .OrderBy(x=>x.Deli_Trip)
                    .Select(x => x.Deli_Trip).Distinct().ToListAsync();
                string _jsonData = JsonConvert.SerializeObject(cycleDB);


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
        [HttpPost]
        public IActionResult OnClickReport([FromBody] string data)
        {
            try
            {
                setConString();
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string typeDate = _json["typeDate"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                string cycleFrom = _json["cycleFrom"];
                string cycleTo = _json["cycleTo"];
                string fromTable = "";
                DataTable dt = new();
                if (typeDate == "Delivery")
                {
                    fromTable = "V_KBNRT_170_Deli_rpt";
                    dt = _FillDT.ExecuteSQL($"SELECT * FROM {fromTable} WHERE Sup = '{supFrom}' " +
                    $"AND (chk_Deli_Date >= '{dateFrom}' AND chk_Deli_Date <= '{dateTo}') AND (F_Delivery_Trip >= '{cycleFrom}' " +
                    $"AND F_Delivery_Trip <= '{cycleTo}' ) ORDER BY F_Delivery_Dock,F_Kanban_No,Prt_no ");
                }
                else
                {
                    fromTable = "V_KBNRT_170_Ord_rpt";
                    dt = _FillDT.ExecuteSQL($"SELECT * FROM {fromTable} WHERE Sup = '{supFrom}' " +
                        $"AND (chk_Order_Date >= '{dateFrom}' AND chk_Order_Date <= '{dateTo}') ORDER BY F_Delivery_Dock,F_Kanban_No,Prt_no ");
                }


                if (dt.Rows.Count == 0)
                {
                    _result = @"{
                                    ""status"":""404"",
                                    ""response"":""OK"",
                                    ""title"":""Report Data Not Found"",
                                    ""message"": ""Please Try Other Option!""
                                    }";

                    return Ok(_result);
                }
                else
                {
                    _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""title"" : ""Check Database Complete"",
                                    ""message"": ""Data Found""
                                    }";

                    return Ok(_result);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}
