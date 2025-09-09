using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT270Controller : Controller
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

        public KBNRT270Controller(
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

        public IActionResult NormalReportClick([FromBody] string data)
        {
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
                string prodMonth = _json["prodMonth"];
                prodMonth = prodMonth.Replace("-", string.Empty);
                string revision = "0.0";
                string UserName = HttpContext.Session.GetString("USER_NAME");
                if (string.IsNullOrWhiteSpace(UserName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }

                DataTable revisionDT = _FillDT.ExecuteSQL($"EXEC [dbo].[SP_KBNRT270_MaxRevision] '{prodMonth}','{supFrom}','{supTo}','{kbnFrom}','{kbnTo}','{partFrom}','{partTo}','{storeFrom}','{storeTo}'");
                if (revisionDT.Rows.Count > 0)
                {
                    revision = revisionDT.Rows[0]["F_revision_no"].ToString();
                }
                DataTable rptDT = _FillDT.ExecuteSQL($"EXEC [dbo].[SP_KBNRT270_Normal_RPT] '{prodMonth}','{revision}','{supFrom}','{supTo}','{kbnFrom}','{kbnTo}','{partFrom}','{partTo}','{storeFrom}','{storeTo}'");
                if (rptDT.Rows.Count == 0)
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
                    string _jsondata2 = JsonConvert.SerializeObject(revision);
                    _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsondata + @", 
                                    ""data2"": " + _jsondata2 + @" 
                                    }";

                    return Ok(_result);
                }
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }
        }
        public IActionResult ABNormalReportClick([FromBody] string data)
        {
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
                string prodMonth = _json["prodMonth"];
                string version = _json["version"];
                prodMonth = prodMonth.Replace("-", string.Empty);
                string revision = "0.0";
                string UserName = HttpContext.Session.GetString("USER_NAME");

                DataTable revisionDT = _FillDT.ExecuteSQL($"EXEC [dbo].[SP_KBNRT270_MaxRevision] '{prodMonth}','{supFrom}','{supTo}','{kbnFrom}','{kbnTo}','{partFrom}','{partTo}','{storeFrom}','{storeTo}'");
                if (revisionDT.Rows.Count > 0)
                {
                    revision = revisionDT.Rows[0]["F_revision_no"].ToString();
                }
                DataTable rptDT = _FillDT.ExecuteSQL($"EXEC [dbo].[SP_KBNRT270_ABNormal_RPT] '{prodMonth}','{revision}','{version}','{supFrom}','{supTo}','{kbnFrom}','{kbnTo}','{partFrom}','{partTo}','{storeFrom}','{storeTo}'");
                if (rptDT.Rows.Count == 0)
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
                    string _jsondata2 = JsonConvert.SerializeObject(revision);
                    _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsondata + @", 
                                    ""data2"": " + _jsondata2 + @" 
                                    }";

                    return Ok(_result);
                }
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }
        }
    }
}
