using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
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
    }
}
