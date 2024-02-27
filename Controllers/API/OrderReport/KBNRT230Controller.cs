using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNRT230Controller(
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

        public async Task<IActionResult> F_Customer_TB_MS()
        {
            try
            {
                setConString();
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
                                    ""message"": ""Data Error"",
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
                setConString();
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
