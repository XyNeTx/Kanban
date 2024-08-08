using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT140Controller : Controller
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

        public KBNRT140Controller(
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

       

        public async Task<IActionResult> Initial()
        {
            try
            {
                
                string _result = "";

                var MonthYear = await _KB3Context.V_KBNRT_140_rpt.Select(x => new
                {
                    chk_VM = x.chk_YM,
                    Month_Year = x.Month_Year
                }).Distinct().ToListAsync();
                var monthYearSorted = MonthYear.OrderByDescending(x => x.chk_VM).ToList();
                var storeDB = await _KB3Context.V_KBNRT_140_rpt.OrderBy(x => x.F_Store_cd).Select(x => x.F_Store_cd).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(monthYearSorted);
                string _jsonData2 = JsonConvert.SerializeObject(storeDB);

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

        public async Task<IActionResult> OnMonthChange([FromBody] string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            { return BadRequest(); }
            try
            {
                
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string monthFrom = _json["monthFrom"];
                string monthTo = _json["monthTo"];

                var storeDB = await _KB3Context.V_KBNRT_140_rpt.OrderBy(x => x.F_Store_cd)
                    .Where(x => x.chk_YM.CompareTo(monthFrom) >= 0 && x.chk_YM.CompareTo(monthTo) <= 0)
                    .Select(x => x.F_Store_cd).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(storeDB);

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
    }
}
