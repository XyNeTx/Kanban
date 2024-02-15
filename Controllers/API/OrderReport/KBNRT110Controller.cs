using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.KB3.ReportOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NuGet.Packaging;
using NuGet.Protocol;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT110Controller : Controller
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

        public KBNRT110Controller(
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
                var result = await _KB3Context.TB_Import_Delivery.OrderByDescending(x => x.F_YM).Select(x => new
                {
                    Year_Month = x.F_YM.Substring(4, 2) + '/' + x.F_YM.Substring(0, 4),
                    Supplier_CD = x.F_Supplier_Code + '-' + x.F_Supplier_Plant + " : " + x.F_Supplier_Name
                }).ToListAsync();
                var resultYM = result.Select(x => x.Year_Month).Distinct().ToList();
                var resultSup = result.OrderBy(x => x.Supplier_CD).Select(x => x.Supplier_CD).Distinct().ToList();
                string _jsonData = JsonConvert.SerializeObject(resultYM);
                string _jsonData2 = JsonConvert.SerializeObject(resultSup);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @",
                                    ""data2"": " + _jsonData2 + @"}";
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        public async Task<IActionResult> OnReportClick([FromBody] string data)
        {
            try
            {
                if (data != null)
                {
                    BearerClass _JBearer = _BearerClass.Header(Request);
                    dynamic _json = JsonConvert.DeserializeObject(data);
                    string monthFrom = _json["monthFrom"];
                    string monthTo = _json["monthTo"];
                    string supFrom = _json["supFrom"];
                    string supTo = _json["supTo"];
                    string userName = HttpContext.Session.GetString("USER_NAME");
                    string hostName = HttpContext.Session.GetString("USER_DEVICENAME");
                    if (userName == null || hostName == null)
                    {
                        return Content("Please Re-Login");
                    }

                    await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM RPT_KBNRT_110 WHERE F_Update_By = @UserName " +
                        "AND F_Host_name = @Host_Name",
                        new SqlParameter("@UserName", userName),
                        new SqlParameter("@Host_Name", hostName));

                    await _KB3Context.Database.ExecuteSqlRawAsync("INSERT INTO RPT_KBNRT_110 (Production_Month,Sup_Cd,Sup_Chk," +
                        "F_Supplier_Name,F_Dock_Code,F_Delivery_Trip,F_Arrival_Sup," +
                        "F_Depart_Sup,F_Arrival_HMMT,F_Depart_HMMT,F_Supplier_Plant,Chk_Month,F_Tran_Type,F_Update_By,F_Host_name) " +
                        "SELECT Production_Month,Sup_Cd,Sup_Chk,F_Supplier_Name,F_Dock_Cd,F_Delivery_Trip,F_Arrival_Sup," +
                        "F_Depart_Sup,F_Arrival_HMMT,F_Depart_HMMT,F_Supplier_Plant,Chk_Month,F_Tran_Type,@UserName,@HostName FROM V_KBNRT_110_rpt  " +
                        "WHERE Chk_Month >= @MonthFrom AND Chk_Month <= @MonthTo AND Sup_Chk >= @SupFrom AND Sup_Chk <= @SupTo",
                        new SqlParameter("@UserName", userName),
                        new SqlParameter("@HostName", hostName),
                        new SqlParameter("@MonthFrom", monthFrom),
                        new SqlParameter("@MonthTo", monthTo),
                        new SqlParameter("@SupFrom", supFrom),
                        new SqlParameter("@SupTo", supTo)
                        );

                    await _KB3Context.SaveChangesAsync();
                }
                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""title"": ""Delivery Timing Round Report Success"",
                    ""message"" : ""View Report Complete""
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
