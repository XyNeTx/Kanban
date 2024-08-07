using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Controllers
{
    [Route("xapi/[action]")]
    [ApiController]
    public class xAPIController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly BearerClass _BearerClass;
        private readonly KB3Context _KB3Context;
        private readonly PPM3Context _PPM3Context;
        private readonly ProcDBContext _ProcDB;
        private readonly KanbanConnection _KBCN;
        private readonly FillDataTable _FillDT;

        public xAPIController(
                        IHttpContextAccessor httpContextAccessor,
                        IConfiguration configuration,
                        KB3Context kB3Context,
                        BearerClass bearerClass,
                        PPM3Context pPM3Context,
                        ProcDBContext procDB,
                        KanbanConnection kBCN,
                        FillDataTable fillDT
                       )
        {
            _httpContextAccessor = httpContextAccessor;
            _config = configuration;
            _KB3Context = kB3Context;
            _PPM3Context = pPM3Context;
            _ProcDB = procDB;
            _KBCN = kBCN;
            _FillDT = fillDT;
            _BearerClass = bearerClass;
        }

        [HttpGet]
        public IActionResult GetProcessDate(string dateShift)
        {
            string sql = $"Select dbo.FN_GetProcess('{dateShift}',2)";
            var dt = _FillDT.ExecuteSQL(sql);
            return Ok(JsonConvert.SerializeObject(dt));
        }

        [HttpGet]
        public IActionResult GetLoginDate()
        {
            var masterControl = _KB3Context.TB_MS_CTL.Where(x => x.F_Shift == "A")
                .OrderBy(x => x.F_Shift).FirstOrDefault();

            DateTime now = DateTime.Now;
            string shift = "1";

            if (now.ToString("HH:mm").CompareTo(masterControl.F_Start_Time) < 0)
            {
                now = now.AddDays(-1);
                shift = "2";
            }
            else if (now.ToString("HH:mm").CompareTo(masterControl.F_End_Time) > 0)
            {
                shift = "2";
            }

            return Ok(new
            {
                status = "200",
                response = "OK",
                message = "Success",
                data = new
                {
                    date = now.ToString("yyyy-MM-dd"),
                    shift = shift
                }
            });

        }
    }
}
