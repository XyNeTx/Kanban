using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using System.Data;

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

        public async Task<IActionResult> OnReportBtnClick([FromBody] string data)
        {
            try
            {
                
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject<dynamic>(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];

                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                char Plant = HttpContext.Request.Cookies["plantCode"].ToString()[0];

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

                var tempList = await _KB3Context.TB_Late_Deli_Rpt_TMP.Where(x => x.F_Update_By == UserName && x.F_Host_Name == HostName).ToListAsync();

                foreach (var each in tempList)
                {
                    DateTime date = DateTime.ParseExact(each.F_Date.Trim(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                    var plant = each.F_Plant;
                    var sup = each.F_Supplier_cd.Trim().Substring(0, 4);
                    var supplant = each.F_Supplier_cd.Trim().Substring(5, 1);
                    var partno = each.F_Part_no.Trim();
                    var ruibet = each.F_Ruibetsu.Trim();
                    var kanban = each.F_Code.Trim().Substring(1, 3);
                    var store = each.F_Store_Code.Trim();
                    int currentUse = 0;

                    using (var cmd = _KB3Context.Database.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = "[dbo].[sp_getCurrentUse]";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        if (cmd.Connection.State != System.Data.ConnectionState.Open) cmd.Connection.Open();
                        cmd.Parameters.Add(new SqlParameter("Plant", plant));
                        cmd.Parameters.Add(new SqlParameter("Supplier_Code", sup));
                        cmd.Parameters.Add(new SqlParameter("Supplier_Plant", supplant));
                        cmd.Parameters.Add(new SqlParameter("Part_No", partno));
                        cmd.Parameters.Add(new SqlParameter("Ruibetsu", ruibet));
                        cmd.Parameters.Add(new SqlParameter("Kanban_No", kanban));
                        cmd.Parameters.Add(new SqlParameter("Store_Code", store));
                        cmd.Parameters.Add(new SqlParameter("Date", date));
                        currentUse = (int)cmd.ExecuteScalar();
                    }

                    double safety_STK = currentUse * double.Parse(each.F_Safety_stk_day);

                    await _KB3Context.Database.ExecuteSqlRawAsync("UPDATE TB_Late_Deli_Rpt_TMP SET F_Usage_day = {0} " +
                        ", F_Safety_stk_pcs = {1} WHERE F_Update_By = {2} AND F_Host_name = {3} AND F_Date = {4}" +
                        " AND F_Trip = {5} AND F_Supplier_cd = {6} AND F_Code = {7} AND F_Store_Code = {8} " +
                        " AND F_Part_no = {9} AND F_Ruibetsu = {10}",
                        currentUse, safety_STK, UserName, HostName, each.F_Date, each.F_Trip, each.F_Supplier_cd, each.F_Code, each.F_Store_Code, each.F_Part_no, each.F_Ruibetsu);
                }

                await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SP_KBNRT200_RPT_UPDATE_SHIFT_1] {0}, {1}, {2}", UserName, HostName, Plant);
                await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SP_KBNRT200_RPT_UPDATE_SHIFT_2] {0}, {1}, {2}", UserName, HostName, Plant);
                await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SP_KBNRT200_RPT_UPDATE_SHIFT_3] {0}, {1}, {2}", UserName, HostName, Plant);
                await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SP_KBNRT200_RPT_UPDATE_Delay_Safety_EVA] {0}, {1}", UserName, HostName);

                var reportList = await _KB3Context.TB_Late_Deli_Rpt_TMP.Where(x => x.F_Update_By == UserName && x.F_Host_Name == HostName).ToListAsync();

                if (reportList.Count > 0)
                {

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
                else
                {
                    _result = @"{
                                    ""status"":""404"",
                                    ""response"":""OK"",
                                    ""title"":""Report Data not Found"",
                                    ""message"": ""Please Select Other Option and Try Again""
                                    }";

                    return Ok(_result);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
    }
}
