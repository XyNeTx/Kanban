using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.KB3.ReportOrder;
using MathNet.Numerics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT210Controller : Controller
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

        public KBNRT210Controller(
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
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Inquriy_KB_rpt_TMP WHERE F_Update_By = {0} AND F_Host_name = {1}", UserName, HostName);

                string _jsonData = "Success";
                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @"
                                    }";


                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
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
                string kbnFrom = _json["kbnFrom"];
                string kbnTo = _json["kbnTo"];
                string storeFrom = _json["storeFrom"];
                string storeTo = _json["storeTo"];
                string partFrom = _json["partFrom"];
                string partTo = _json["partTo"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                char Plant = _KBCN.Plant.ToString()[0];

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Inquriy_KB_rpt_TMP WHERE F_Update_By = {0} AND F_Host_name = {1}", UserName, HostName);

                await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [dbo].[Insert_TB_Inquriy_KB_rpt_TMP] {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}",
                    supFrom, supTo, kbnFrom, kbnTo, storeFrom, storeTo, partFrom, partTo, dateFrom, dateTo, Plant, UserName, HostName);

                List<TB_Inquriy_KB_rpt_TMP> InquriyList = new();
                InquriyList = await _KB3Context.TB_Inquriy_KB_rpt_TMP.Where(x => x.F_Update_by == UserName && x.F_Host_Name == HostName).ToListAsync();
                foreach (var inquriy in InquriyList)
                {
                    var processDate = inquriy.F_Deli_date.Trim();
                    var deliDate = inquriy.chk_deli_date.Trim().Substring(0, 6);
                    var chkDeliDate = inquriy.chk_deli_date.Trim();
                    var partNo = inquriy.F_Part_No.Trim().Replace("-", string.Empty).Substring(0, 10);
                    var partNoDash = inquriy.F_Part_No.Trim();
                    var KanbanNo = inquriy.F_kb_no.Trim();
                    var SupCode = inquriy.F_Sup_cd.Trim().Split('-')[0];
                    char SupPlant = inquriy.F_Sup_cd.Trim().Split('-')[1][0];
                    var SupCodePlant = inquriy.F_Sup_cd.Trim();
                    var revision = inquriy.F_revision_no.Trim();
                    var StoreCode = inquriy.F_str_cd.Trim();
                    var Ruibetsu = inquriy.F_Part_No.Trim().Substring(inquriy.F_Part_No.Trim().Length - 2);

                    var forecast = await _KB3Context.TB_Import_Forecast
                        .Where(x =>
                            x.F_Production_date.TrimEnd() == deliDate &&
                            x.F_part_no.TrimEnd() == partNo &&
                            '0' + x.F_sebango.TrimEnd() == KanbanNo &&
                            x.F_Supplier_code.TrimEnd().Substring(1, 4) == SupCode &&
                            x.F_supplier_plant == SupPlant &&
                            x.F_revision_no.TrimEnd() == revision &&
                            x.F_Store_cd.TrimEnd() == StoreCode &&
                            x.F_ruibetsu.TrimEnd() == Ruibetsu
                        )
                        .Select(x => new
                        {
                            x.F_Amount_MD1,
                            x.F_Amount_MD2,
                            x.F_Amount_MD3,
                            x.F_Amount_MD4,
                            x.F_Amount_MD5,
                            x.F_Amount_MD6,
                            x.F_Amount_MD7,
                            x.F_Amount_MD8,
                            x.F_Amount_MD9,
                            x.F_Amount_MD10,
                            x.F_Amount_MD11,
                            x.F_Amount_MD12,
                            x.F_Amount_MD13,
                            x.F_Amount_MD14,
                            x.F_Amount_MD15,
                            x.F_Amount_MD16,
                            x.F_Amount_MD17,
                            x.F_Amount_MD18,
                            x.F_Amount_MD19,
                            x.F_Amount_MD20,
                            x.F_Amount_MD21,
                            x.F_Amount_MD22,
                            x.F_Amount_MD23,
                            x.F_Amount_MD24,
                            x.F_Amount_MD25,
                            x.F_Amount_MD26,
                            x.F_Amount_MD27,
                            x.F_Amount_MD28,
                            x.F_Amount_MD29,
                            x.F_Amount_MD30,
                            x.F_Amount_MD31,
                        })
                        .SingleOrDefaultAsync();

                    if (forecast != null)
                    {
                        int j = int.Parse(processDate.Substring(0, 2));
                        string property = "F_Amount_MD" + processDate.Substring(0, 2);
                        int Max_Std = 0;
                        string MaxForecastDate = "";
                        if (Max_Std == 0)
                        {
                            Max_Std = (int)forecast.GetType().GetProperty(property).GetValue(forecast);
                            int Use_Day = Max_Std;
                            await _KB3Context.Database.ExecuteSqlRawAsync("UPDATE TB_Inquriy_KB_rpt_TMP SET F_max_forcast = @Max_Std " +
                                                "WHERE chk_deli_date = @Deli_Date AND F_Part_no = @PartNo AND F_kb_no = @KanbanNo AND F_Sup_cd = @SupCodePlant " +
                                                "AND F_revision_no = @RevisionNo AND F_str_cd = @StoreCode AND F_Part_no = @PartNo ",
                                                new SqlParameter("@Max_Std", Max_Std),
                                                new SqlParameter("@Deli_Date", chkDeliDate),
                                                new SqlParameter("@PartNo", partNoDash),
                                                new SqlParameter("@KanbanNo", KanbanNo),
                                                new SqlParameter("@SupCodePlant", SupCodePlant),
                                                new SqlParameter("@RevisionNo", revision),
                                                new SqlParameter("@StoreCode", StoreCode)
                                                );
                        }
                        if (Max_Std != 0)
                        {
                            for (int k = j - 1; k != j - 6; k--)
                            {
                                if (k >= 1)
                                {
                                    string propertyLoop = "F_Amount_MD" + k.ToString();
                                    int Max_Std_Loop = (int)forecast.GetType().GetProperty(propertyLoop).GetValue(forecast);
                                    if (Max_Std_Loop > Max_Std)
                                    {
                                        Max_Std = Max_Std_Loop;
                                        MaxForecastDate = processDate.Substring(processDate.Length - 4, 4) + processDate.Substring(3, 2) + k;
                                        await _KB3Context.Database.ExecuteSqlRawAsync("UPDATE TB_Inquriy_KB_rpt_TMP SET F_max_forcast = @Max_Std " +
                                                "WHERE chk_deli_date = @Deli_Date AND F_Part_no = @PartNo AND F_kb_no = @KanbanNo AND F_Sup_cd = @SupCodePlant " +
                                                "AND F_revision_no = @RevisionNo AND F_str_cd = @StoreCode AND F_Part_no = @PartNo ",
                                                new SqlParameter("@Max_Std", Max_Std),
                                                new SqlParameter("@Deli_Date", chkDeliDate),
                                                new SqlParameter("@PartNo", partNoDash),
                                                new SqlParameter("@KanbanNo", KanbanNo),
                                                new SqlParameter("@SupCodePlant", SupCodePlant),
                                                new SqlParameter("@RevisionNo", revision),
                                                new SqlParameter("@StoreCode", StoreCode)
                                                );
                                    }
                                }
                                else if (k < 1)
                                {
                                    break;
                                }
                            }
                            for (int k = j + 1; k != j + 6; k++)
                            {
                                if (k <= 31)
                                {
                                    string propertyLoop = "F_Amount_MD" + k.ToString();
                                    int Max_Std_Loop = (int)forecast.GetType().GetProperty(propertyLoop).GetValue(forecast);
                                    if (Max_Std_Loop > Max_Std)
                                    {
                                        Max_Std = Max_Std_Loop;
                                        MaxForecastDate = processDate.Substring(processDate.Length - 4, 4) + processDate.Substring(3, 2) + k;
                                        await _KB3Context.Database.ExecuteSqlRawAsync("UPDATE TB_Inquriy_KB_rpt_TMP SET F_max_forcast = @Max_Std " +
                                                "WHERE chk_deli_date = @Deli_Date AND F_Part_no = @PartNo AND F_kb_no = @KanbanNo AND F_Sup_cd = @SupCodePlant " +
                                                "AND F_revision_no = @RevisionNo AND F_str_cd = @StoreCode AND F_Part_no = @PartNo ",
                                                new SqlParameter("@Max_Std", Max_Std),
                                                new SqlParameter("@Deli_Date", chkDeliDate),
                                                new SqlParameter("@PartNo", partNoDash),
                                                new SqlParameter("@KanbanNo", KanbanNo),
                                                new SqlParameter("@SupCodePlant", SupCodePlant),
                                                new SqlParameter("@RevisionNo", revision),
                                                new SqlParameter("@StoreCode", StoreCode)
                                                );
                                    }
                                }
                                else if (k > 31)
                                {
                                    break;
                                }
                            }
                        }
                    }

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
