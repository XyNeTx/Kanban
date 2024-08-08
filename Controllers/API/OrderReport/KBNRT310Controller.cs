using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT310Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _Serilog;
        private readonly ProcDBContext _ProcDB;

        public KBNRT310Controller(
                    IConfiguration configuration,
                    BearerClass bearerClass,
                    KanbanConnection kanbanConnection,
                    PPMInvenContext pPMInvenContext,
                    PPM3Context pPM3Context,
                    KB3Context kB3Context,
                    FillDataTable fillDataTable,
                    SerilogLibs serilog,
                    ProcDBContext procDB
                    )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPM3Context = pPM3Context;
            _PPMInvenContext = pPMInvenContext;
            _Serilog = serilog;
            _FillDT = fillDataTable;
            _ProcDB = procDB;
        }

        public async Task<IActionResult> Initial([FromBody] string data)
        {
            try
            {
                
                string _result = "";
                string date = "";
                string shift = "";
                if (!string.IsNullOrWhiteSpace(data))
                {
                    dynamic _json = JsonConvert.DeserializeObject(data);
                    date = _json["date"];
                    shift = _json["shift"];
                }
                if (string.IsNullOrWhiteSpace(date))
                {
                    date = DateTime.Now.ToString("yyyyMMdd");
                }
                var supplierDB = await _KB3Context.TB_MS_PartOrder.Where(x => x.F_Start_Date.CompareTo(date) <= 0 && x.F_End_Date.CompareTo(date) >= 0)
                    .Select(x => new
                    {
                        Sup_CD = x.F_Supplier_Cd.Trim() + '-' + x.F_Supplier_Plant.ToString().Trim(),
                    }).OrderBy(x => x.Sup_CD).Distinct().ToListAsync();

                var kanbanDB = await _KB3Context.TB_MS_PartOrder.Where(x => x.F_Start_Date.CompareTo(date) <= 0 && x.F_End_Date.CompareTo(date) >= 0)
                    .Select(x => new
                    {
                        F_Sebango = x.F_Kanban_No
                    }).OrderBy(x => x.F_Sebango).Distinct().ToListAsync();

                var storeDB = await _KB3Context.TB_MS_PartOrder.Where(x => x.F_Start_Date.CompareTo(date) <= 0 && x.F_End_Date.CompareTo(date) >= 0)
                    .Select(x => new
                    {
                        F_Store_CD = x.F_Store_Code
                    }).OrderBy(x => x.F_Store_CD).Distinct().ToListAsync();

                var partDB = await _KB3Context.TB_MS_PartOrder.Where(x => x.F_Start_Date.CompareTo(date) <= 0 && x.F_End_Date.CompareTo(date) >= 0)
                    .Select(x => new
                    {
                        prt_no = x.F_Part_No + '-' + x.F_Ruibetsu.Trim()
                    }).OrderBy(x => x.prt_no).Distinct().ToListAsync();

                DataTable dockDT = _FillDT.ExecuteSQL(" Select Distinct F_Dock_Check From TB_Count_Stock " +
                    $" Where F_Process_Date ='{date}' and F_Process_Shift = '{shift}' Order by 1 ");

                List<string> dockList = new();

                for (int i = 0; i < dockDT.Rows.Count; i++)
                {
                    dockList.Add(dockDT.Rows[i].ItemArray[0].ToString().Trim());
                }

                string _jsonData = JsonConvert.SerializeObject(supplierDB);
                string _jsonData2 = JsonConvert.SerializeObject(kanbanDB);
                string _jsonData3 = JsonConvert.SerializeObject(storeDB);
                string _jsonData4 = JsonConvert.SerializeObject(partDB);
                string _jsonData5 = JsonConvert.SerializeObject(dockList);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @",
                                    ""data2"": " + _jsonData2 + @",
                                    ""data3"": " + _jsonData3 + @",
                                    ""data4"": " + _jsonData4 + @",
                                    ""data5"": " + _jsonData5 + @"
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
                string date = _json["date"];

                var kanbanDB = await _KB3Context.TB_MS_PartOrder
                    .Select(x => new
                    {
                        F_Supplier_Cd = x.F_Supplier_Cd + '-' + x.F_Supplier_Plant,
                        F_Kanban_No = x.F_Kanban_No,
                        F_Start_Date = x.F_Start_Date,
                        F_End_Date = x.F_End_Date
                    })
                    .Where(x => x.F_Start_Date.CompareTo(date) <= 0 && x.F_End_Date.CompareTo(date) >= 0)
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Select(x => new
                    {
                        F_Sebango = x.F_Kanban_No
                    }).OrderBy(x => x.F_Sebango).Distinct().ToListAsync();

                var storeDB = await _KB3Context.TB_MS_PartOrder
                    .Select(x => new
                    {
                        F_Supplier_Cd = x.F_Supplier_Cd + '-' + x.F_Supplier_Plant,
                        F_Store_Code = x.F_Store_Code,
                        F_Start_Date = x.F_Start_Date,
                        F_End_Date = x.F_End_Date
                    })
                    .Where(x => x.F_Start_Date.CompareTo(date) <= 0 && x.F_End_Date.CompareTo(date) >= 0)
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Select(x => new
                    {
                        F_Store_CD = x.F_Store_Code
                    }).OrderBy(x => x.F_Store_CD).Distinct().ToListAsync();

                var partDB = await _KB3Context.TB_MS_PartOrder
                    .Select(x => new
                    {
                        F_Supplier_Cd = x.F_Supplier_Cd + '-' + x.F_Supplier_Plant,
                        F_Part_No = x.F_Part_No,
                        F_Ruibetsu = x.F_Ruibetsu,
                        F_Start_Date = x.F_Start_Date,
                        F_End_Date = x.F_End_Date
                    })
                    .Where(x => x.F_Start_Date.CompareTo(date) <= 0 && x.F_End_Date.CompareTo(date) >= 0)
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

        public async Task<IActionResult> OnPartNoChange([FromBody] string data)
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
                string partFrom = _json["partFrom"];
                string partTo = _json["partTo"];
                string date = _json["date"];

                var storeDB = await _KB3Context.TB_MS_PartOrder
                    .Select(x => new
                    {
                        F_Supplier_Cd = x.F_Supplier_Cd + '-' + x.F_Supplier_Plant,
                        F_Part_No = x.F_Part_No + '-' + x.F_Ruibetsu,
                        F_Store_Code = x.F_Store_Code,
                        F_Start_Date = x.F_Start_Date,
                        F_End_Date = x.F_End_Date
                    })
                    .Where(x => x.F_Start_Date.CompareTo(date) <= 0 && x.F_End_Date.CompareTo(date) >= 0)
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Where(x => x.F_Part_No.CompareTo(partFrom) >= 0 && x.F_Part_No.CompareTo(partTo) <= 0)
                    .Select(x => new
                    {
                        F_Store_CD = x.F_Store_Code
                    }).OrderBy(x => x.F_Store_CD).Distinct().ToListAsync();

                var kbnDB = await _KB3Context.TB_MS_PartOrder
                    .Select(x => new
                    {
                        F_Supplier_Cd = x.F_Supplier_Cd + '-' + x.F_Supplier_Plant,
                        F_Part_No = x.F_Part_No + '-' + x.F_Ruibetsu,
                        F_Kanban_No = x.F_Kanban_No,
                        F_Start_Date = x.F_Start_Date,
                        F_End_Date = x.F_End_Date
                    })
                    .Where(x => x.F_Start_Date.CompareTo(date) <= 0 && x.F_End_Date.CompareTo(date) >= 0)
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Where(x => x.F_Part_No.CompareTo(partFrom) >= 0 && x.F_Part_No.CompareTo(partTo) <= 0)
                    .Select(x => new
                    {
                        F_Sebango = x.F_Kanban_No
                    }).OrderBy(x => x.F_Sebango).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(storeDB);
                string _jsonData2 = JsonConvert.SerializeObject(kbnDB);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""store"": " + _jsonData + @",
                                    ""kanban_no"": " + _jsonData2 + @"
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
                string partFrom = _json["partFrom"];
                string partTo = _json["partTo"];
                string date = _json["date"];

                var storeDB = await _KB3Context.TB_MS_PartOrder
                    .Select(x => new
                    {
                        F_Supplier_Cd = x.F_Supplier_Cd + '-' + x.F_Supplier_Plant,
                        F_Kanban_No = x.F_Kanban_No,
                        F_Part_No = x.F_Part_No + '-' + x.F_Ruibetsu,
                        F_Store_Code = x.F_Store_Code,
                        F_Start_Date = x.F_Start_Date,
                        F_End_Date = x.F_End_Date
                    })
                    .Where(x => x.F_Start_Date.CompareTo(date) <= 0 && x.F_End_Date.CompareTo(date) >= 0)
                    .Where(x => x.F_Supplier_Cd.CompareTo(supFrom) >= 0 && x.F_Supplier_Cd.CompareTo(supTo) <= 0)
                    .Where(x => x.F_Part_No.CompareTo(partFrom) >= 0 && x.F_Part_No.CompareTo(partTo) <= 0)
                    .Where(x => x.F_Kanban_No.CompareTo(kbnFrom) >= 0 && x.F_Kanban_No.CompareTo(kbnTo) <= 0)
                    .Select(x => new
                    {
                        F_Store_CD = x.F_Store_Code
                    }).OrderBy(x => x.F_Store_CD).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(storeDB);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""store"": " + _jsonData + @"
                                    }";

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<IActionResult> OnReportClick([FromBody] string data)
        {
            
            string _result = "";
            string date = "";
            string shift = "";
            string UserName = HttpContext.Session.GetString("USER_NAME");
            string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
            {
                return Redirect($"{Request.Path.ToString()}");
            }
            if (!string.IsNullOrWhiteSpace(data))
            {
                dynamic _json = JsonConvert.DeserializeObject(data);
                date = _json["date"];
                shift = _json["shift"];
            }

            await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC dbo.SP_Gen_Count_New '{date}','{shift}'");

            DataTable DT = _FillDT.ExecuteSQL($" Select * From TB_Count_Stock Where F_Process_Date = '{date}' and F_Process_Shift = '{shift}' ");

            if (DT.Rows.Count > 0)
            {
                string _jsondata = JsonConvert.SerializeObject(UserName);
                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsondata + @"
                                    }";

                return Ok(_result);
            }
            else
            {
                _result = @"{
                                    ""status"":""404"",
                                    ""response"":""OK"",
                                    ""title"" : ""Data Not Found"",
                                    ""message"": ""Please Try Other Option""
                                    }";

                return Ok(_result);
            }
        }
        public async Task<IActionResult> OnReportAllClick([FromBody] string data)
        {
            
            string _result = "";
            string date = "";
            string shift = "";
            string UserName = HttpContext.Session.GetString("USER_NAME");
            string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
            {
                return Redirect($"{Request.Path.ToString()}");
            }
            if (!string.IsNullOrWhiteSpace(data))
            {
                dynamic _json = JsonConvert.DeserializeObject(data);
                date = _json["date"];
                shift = _json["shift"];
            }

            await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC dbo.SP_Gen_Count_ALL '{date}','{shift}'");

            DataTable DT = _FillDT.ExecuteSQL($" Select * From TB_Count_Stock Where F_Process_Date = '{date}' and F_Process_Shift = '{shift}' ");

            if (DT.Rows.Count > 0)
            {
                string _jsondata = JsonConvert.SerializeObject(UserName);
                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsondata + @"
                                    }";

                return Ok(_result);
            }
            else
            {
                _result = @"{
                                    ""status"":""404"",
                                    ""response"":""OK"",
                                    ""title"" : ""Data Not Found"",
                                    ""message"": ""Please Try Other Option""
                                    }";

                return Ok(_result);
            }
        }
    }
}