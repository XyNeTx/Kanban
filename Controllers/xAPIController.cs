using HINOSystem.Context;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Controllers
{
    [Route("xapi/[action]")]
    [ApiController]
    public class xAPIController : ControllerBase
    {
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly BearerClass _BearerClass;

        public xAPIController(
                        KB3Context kB3Context,
                        FillDataTable fillDT,
                        BearerClass bearerClass
                       )
        {
            _KB3Context = kB3Context;
            _FillDT = fillDT;
            _BearerClass = bearerClass;
        }

        [HttpGet]
        public async Task<IActionResult> GetProcessDate(string dateShift)
        {
            string sql = $"Select dbo.FN_GetProcess('{dateShift}',2)";
            var dt = _FillDT.ExecuteSQL(sql);
            return Ok(JsonConvert.SerializeObject(dt));
        }

        [HttpGet]
        public IActionResult GetCheckNormalProcess()
        {
            try
            {
                string sql = $"Select * From TB_MS_Parameter WHERE F_Code = 'ST' ";
                var data = _KB3Context.TB_MS_Parameter.FromSqlRaw(sql).FirstOrDefault();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Success",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = ex.Message
                });
            }
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
