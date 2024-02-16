using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.KB3.ReportOrder;
using MathNet.Numerics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

                    await _KB3Context.Database.ExecuteSqlRawAsync("SELECT P.F_Amount_MD1, P.F_Amount_MD2, P.F_Amount_MD3, P.F_Amount_MD4, P.F_Amount_MD5, P.F_Amount_MD6," +
                        " P.F_Amount_MD7, P.F_Amount_MD8, P.F_Amount_MD9, P.F_Amount_MD10, P.F_Amount_MD11, P.F_Amount_MD12," +
                        " P.F_Amount_MD13, P.F_Amount_MD14, P.F_Amount_MD15, P.F_Amount_MD16, P.F_Amount_MD17, P.F_Amount_MD18," +
                        " P.F_Amount_MD19, P.F_Amount_MD20, P.F_Amount_MD21, P.F_Amount_MD22, P.F_Amount_MD23, P.F_Amount_MD24," +
                        " P.F_Amount_MD25, P.F_Amount_MD26, P.F_Amount_MD27, P.F_Amount_MD28, P.F_Amount_MD29, P.F_Amount_MD30, P.F_Amount_MD31" +
                        " FROM TB_IMPORT_FORECAST P" +
                        " WHERE RTRIM(P.F_production_date) collate Thai_CS_AI = @Deli_Date AND RTRIM(P.F_part_no) collate Thai_CS_AI = @PartNo" +
                        " AND '0'+RTRIM(P.F_sebango) collate Thai_CS_AI = @KanbanNo AND RIGHT(RTRIM(P.F_Supplier_code),4)+'-'+RTRIM(P.F_supplier_plant) collate Thai_CS_AI = @SupCode" +
                        " AND RTRIM(P.F_revision_no) collate Thai_CS_AI = @Revision AND RTRIM(P.F_Store_cd) collate Thai_CS_AI = @StoreCode AND RTRIM(P.F_ruibetsu) collate Thai_CS_AI = @Ruibetsu",
                        new SqlParameter("@Deli_Date", inquriy.F_Part_No.Trim().Substring(0, 10)),
                        new SqlParameter("@PartNo", inquriy.chk_deli_date.Trim().Substring(0, 6)),
                        new SqlParameter("@KanbanNo", inquriy.F_kb_no.Trim()),
                        new SqlParameter("@SupCode", inquriy.F_Sup_cd.Trim()),
                        new SqlParameter("@Revision", inquriy.F_revision_no.Trim()),
                        new SqlParameter("@StoreCode", inquriy.F_str_cd.Trim()),
                        new SqlParameter("@Ruibetsu", inquriy.F_Part_No.Trim().Substring(inquriy.F_Part_No.Trim().Length - 2))
                        );
                }

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
    }
}
