using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT200Controller : Controller
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

        public KBNRT200Controller(
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
        public async Task<IActionResult> OnReportBtnClick([FromBody] string data)
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
                char Plant = _KBCN.Plant.ToString()[0];

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Late_Deli_Rpt_TMP WHERE F_Update_By = {0} AND F_Host_name = {1}",
                    UserName, HostName);

                await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SP_KBNRT200_RPT_INSERT_TB_Late_Deli_Rpt_TMP] @UserName , " +
                    "@HostName, @Plant, @SupFrom, @SupTo, @DateFrom, @DateTo",
                    new SqlParameter("@UserName", UserName),
                    new SqlParameter("@HostName", HostName),
                    new SqlParameter("@Plant", Plant),
                    new SqlParameter("@SupFrom", supFrom),
                    new SqlParameter("@SupTo", supTo),
                    new SqlParameter("@DateFrom", dateFrom),
                    new SqlParameter("@DateTo", dateTo)
                    );

                var tempList = await _KB3Context.TB_Late_Deli_Rpt_TMP.ToListAsync();

                foreach (var each in tempList)
                {
                    DateTime date = DateTime.Parse(each.F_Date);

                    int currentUse = await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [dbo].[sp_getCurrentUse] {0} {1} {2} {3} {4} {5} {6} {7}",
                        each.F_Plant,
                        each.F_Supplier_cd.Trim().Substring(0, 4),
                        each.F_Supplier_cd.Trim().Substring(5, 1),
                        each.F_Part_no,
                        each.F_Ruibetsu,
                        each.F_Code,
                        each.F_Store_Code,
                        date);

                    double safety_STK = currentUse * double.Parse(each.F_Safety_stk_day);
                }

                string _jsonData = JsonConvert.SerializeObject(UserName);
                string _jsonData2 = JsonConvert.SerializeObject(HostName);

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
                return Content(ex.ToString());
            }
        }
    }
}
