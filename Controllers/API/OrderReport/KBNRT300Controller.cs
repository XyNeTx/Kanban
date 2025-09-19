using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT300Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _Serilog;
        private readonly ProcDBContext _ProcDB;

        public KBNRT300Controller(
                    IConfiguration configuration,
                    BearerClass bearerClass,
                    KanbanConnection kanbanConnection,
                    PPMInvenContext pPMInvenContext,
                    PPM3Context pPM3Context,
                    KB3Context kB3Context,
                    FillDataTable fillDataTable,
                    SerilogLibs serilog,
                    ProcDBContext procDB
                    )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPM3Context = pPM3Context;
            _PPMInvenContext = pPMInvenContext;
            _Serilog = serilog;
            _FillDT = fillDataTable;
            _ProcDB = procDB;
        }


        public async Task<IActionResult> ClickReport([FromBody] string data)
        {
            try
            {
                
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string kbnFrom = _json["kbnFrom"];
                string kbnTo = _json["kbnTo"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                string shiftFrom = _json["shiftFrom"];
                string shiftTo = _json["shiftTo"];
                string tripFrom = _json["tripFrom"];
                string tripTo = _json["tripTo"];
                string kbnType = _json["kbnType"];
                string UserName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value.ToString();
                string HostName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.WindowsDeviceClaim).Value.ToString();

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}");
                }
                DataTable DT = new DataTable();
                if (kbnType.ToUpper() == "STOP")
                {
                    await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC [dbo].[SP_RT300_STOP] '{dateFrom}',{dateTo},{UserName}");
                }

                string _jsondata = JsonConvert.SerializeObject(UserName);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsondata + @"
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
