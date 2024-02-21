using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT220Controller : Controller
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

        public KBNRT220Controller(
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

        public async Task<IActionResult> DeleteTemp()
        {
            try
            {
                setConString();
                string _result = "";
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Stock_planing_rpt_TMP WHERE F_Update_By = {0} AND F_Host_name = {1}", UserName, HostName);

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

        public async Task<IActionResult> ReportClick([FromBody] string data)
        {
            try
            {
                setConString();
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject<dynamic>(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");


                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Stock_planing_rpt_TMP WHERE F_Update_By = {0} AND F_Host_name = {1}", UserName, HostName);

                var viewList = _KB3Context.V_KBNRT_220_rpt.Where(x => x.F_Delivery_Date.CompareTo(dateFrom) >= 0 && x.F_Delivery_Date.CompareTo(dateTo) <= 0)
                    .Where(x => x.F_Supplier_code.CompareTo(supFrom) >= 0 && x.F_Supplier_code.CompareTo(supTo) <= 0).ToListAsync();


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
    }
}
