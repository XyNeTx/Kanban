using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT230Controller : Controller
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

        public KBNRT230Controller(
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


        public async Task<IActionResult> F_Customer_TB_MS()
        {
            try
            {
                
                string _result = "";
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect("/OrderReport/KBNRT230");
                }

                var CustomerList = await _KB3Context.TB_MS_VLT_Customer.Select(x => x.F_Customer).Distinct().ToListAsync();

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Seq_ord_rpt_tmp WHERE F_Update_By = {0} AND F_Host_name = {1}", UserName, HostName);

                if (CustomerList.Count == 0)
                {
                    _result = @"{
                                    ""status"":""400"",
                                    ""response"":""OK"",
                                    ""title"":""Initial Data not Found"",
                                    ""message"": ""Data Error""
                                    }";

                    return Ok(_result);
                }

                string _JsonData = JsonConvert.SerializeObject(CustomerList);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _JsonData + @"
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
            try
            {
                
                string _result = "";
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                string Type = "VLT";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string cusFrom = _json["cusFrom"];
                string cusTo = _json["cusTo"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Seq_ord_rpt_tmp WHERE F_Update_By = {0} AND F_Host_name = {1}", UserName, HostName);

                await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SP_KBNRT230_RPT_TB_Seq_ord_rpt_tmp] {0},{1},{2},{3},{4},{5},{6}"
                    , UserName, HostName, Type, cusFrom, cusTo, dateFrom, dateTo);

                DataTable dt = _FillDT.ExecuteSQL($"SELECT * FROM TB_Seq_ord_rpt_tmp WHERE F_Update_By = '{UserName}' AND F_Host_name = '{HostName}'");

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
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}
