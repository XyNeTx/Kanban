using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using System.Data;
using System.Diagnostics;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT260Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _Serilog;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNRT260Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            PPMInvenContext pPMInvenContext,
            PPM3Context pPM3Context,
            KB3Context kB3Context,
            FillDataTable fillDataTable,
            SerilogLibs serilog
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

        public async Task<IActionResult> Display_Detail([FromBody] string data)
        {
            try
            {
                setConString();
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string prodMonth = _json["prodMonth"];
                prodMonth = prodMonth.Replace("-", string.Empty);

                var detailList = await _KB3Context.TB_Import_Forecast.Where(x => x.F_Production_date == prodMonth).Select(x => new
                {
                    F_Version = (x.F_Version == 'C') ? "CONFIRM" : "TENTATIVE",
                    F_Revision = x.F_revision_no
                }).Distinct().ToListAsync();

                if (detailList.Count == 0)
                {
                    _result = @"{
                                    ""status"":""400"",
                                    ""response"":""OK"",
                                    ""title"":""Initial Data not Found"",
                                    ""message"": ""Data Error"",
                                    }";

                    return Ok(_result);
                }
                else
                {
                    string _JsonData = JsonConvert.SerializeObject(detailList);

                    _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _JsonData + @"
                                    }";

                    return Ok(_result);
                }
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public IActionResult OnReportClicked([FromBody] string data)
        {
            try
            {
                setConString();
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string prodMonth = _json["prodMonth"];
                prodMonth = prodMonth.Replace("-", string.Empty);
                string checkedValue = _json["checkedValue"];
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string sql = $"EXEC [dbo].[SP_KBNRT260_SELECT_VW_Forecast_Variant_RPT] '{prodMonth}','{checkedValue}'";
                DataTable dt = _FillDT.ExecuteSQL(sql);
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
                    string _jsondata = JsonConvert.SerializeObject(UserName);
                    _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsondata + @" 
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
