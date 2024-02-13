using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT150Controller : Controller
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

        public KBNRT150Controller(
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
                string now = DateTime.Now.ToString("yyyyMMdd");
                var typeDB = await _KB3Context.TB_Transaction
                    .Where(x => x.F_PDS_Issued_Date == now)
                    .Select(x => new
                    {
                        Type = x.F_Type.Trim()
                    }).OrderBy(x => x.Type).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(typeDB);

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
                return Content(ex.Message);
            }
        }
        public async Task<IActionResult> OnOrderChange([FromBody] string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return BadRequest();
            }
            try
            {
                string _result = "";
                setConString();
                dynamic _json = JsonConvert.DeserializeObject(data);
                string orderFrom = _json["orderFrom"];
                string orderTo = _json["orderTo"];
                var typeDB = await _KB3Context.TB_Transaction
                    .Where(x => x.F_PDS_Issued_Date.CompareTo(orderFrom) >= 0 && x.F_PDS_Issued_Date.CompareTo(orderTo) <= 0)
                    .Select(x => new
                    {
                        Type = x.F_Type.Trim()
                    }).OrderBy(x => x.Type).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(typeDB);

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
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest();
            }
            try
            {
                setConString();
                string _result = "";
                string userName = HttpContext.Session.GetString("USER_NAME");
                string hostName = HttpContext.Session.GetString("USER_DEVICENAME");
                await _KB3Context.Database.ExecuteSqlRawAsync
                        ("DELETE FROM TB_Imp_Ord_rpt_tmp WHERE F_Update_By = @UserLogon AND F_Host_name = @Host_name",
                        new SqlParameter("@UserLogon", userName),
                        new SqlParameter("@Host_name", hostName)
                        );

                dynamic _json = JsonConvert.DeserializeObject(data);
                string orderFrom = _json["orderFrom"];
                string orderTo = _json["orderTo"];
                string typeFrom = _json["typeFrom"];
                string typeTo = _json["typeTo"];

                await _KB3Context.Database.ExecuteSqlRawAsync
                    ("INSERT INTO TB_Imp_Ord_rpt_tmp(Type, Date, Part_no, store_cd, Part_nm, Kanban_no, Qty, Order_no, Remark, chk_date, F_Update_By, F_Host_Name,F_Parent_Part) " +
                    "SELECT F_Type,substring(F_PDS_Issued_Date,7,2)+'/'+substring(F_PDS_Issued_Date,5,2)+'/'+substring(F_PDS_Issued_Date,1,4) as F_date ,F_part_No+'-'+F_Ruibetsu as F_PART_No,F_Store_Cd,F_part_name,F_kanban_No, " +
                    " sum(F_QTY) as f_QTY,F_PDS_NO,F_REMARK,F_PDS_Issued_Date,@UserName as F_Update_By,@HostName as F_Host_name,F_Part_Order+'-'+F_Ruibetsu_Order as F_Parent_Part " +
                    " FROM TB_Transaction WHERE F_Plant = @Plant AND ( F_PDS_Issued_Date >= @OrderFrom AND F_PDS_Issued_Date <= @OrderTo ) AND (F_Type >= @TypeFrom AND F_Type <= @TypeTo) " +
                    " Group by F_Type,F_part_No+'-'+F_Ruibetsu,F_Store_Cd,F_part_name,F_kanban_No,F_PDS_NO,F_REMARK,F_PDS_Issued_Date,F_Part_Order+'-'+F_Ruibetsu_Order ",
                    new SqlParameter("@UserName", userName),
                    new SqlParameter("@HostName", hostName),
                    new SqlParameter("@Plant", 1),//for test only
                    new SqlParameter("@OrderFrom", orderFrom),
                    new SqlParameter("@OrderTo", orderTo),
                    new SqlParameter("@TypeFrom", typeFrom),
                    new SqlParameter("@TypeTo", typeTo)
                    );

                string _jsonData = JsonConvert.SerializeObject(userName);
                string _jsonData2 = JsonConvert.SerializeObject(hostName);

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
                return Content(ex.Message);
            }
        }
    }
}
