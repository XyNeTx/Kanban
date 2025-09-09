using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT240Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNRT240Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            PPMInvenContext pPMInvenContext,
            PPM3Context pPM3Context,
            KB3Context kB3Context,
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
            _FillDT = fillDataTable;
        }


        public async Task<IActionResult> Onload()
        {
            try
            {
                
                string _result = "";
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect("/OrderReport/KBNRT240");
                }
                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Special_Ord_Rpt_tmp WHERE F_Update_By = {0} AND F_Host_name = {1}", UserName, HostName);

                _result = @"{
                                ""status"" : ""200"",
                                ""response"" : ""OK"",
                                ""title"" : ""Onload Success"",
                                ""message"" : ""Delete Temp Complete""
                            }";

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        public async Task<IActionResult> OnReportClick([FromBody] string data)
        {
            try
            {
                
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect("/OrderReport/KBNRT240");
                }
                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Special_Ord_Rpt_tmp WHERE F_Update_By = {0} AND F_Host_name = {1}", UserName, HostName);

                await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SP_KBNRT240_INSERT_TB_Special_Ord_Rpt_tmp] {0},{1},{2},{3}", UserName, HostName, dateFrom, dateTo);

                DataTable dt = _FillDT.ExecuteSQL($"SELECT * FROM TB_Special_Ord_Rpt_tmp WHERE F_Update_By = '{UserName}' AND F_Host_name = '{HostName}'");

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
                    string _JsonData = JsonConvert.SerializeObject(UserName);
                    string _JsonData2 = JsonConvert.SerializeObject(HostName);

                    _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _JsonData + @",
                                    ""data2"": " + _JsonData2 + @"
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
