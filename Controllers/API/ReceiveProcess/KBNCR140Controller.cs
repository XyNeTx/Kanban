using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Controllers.API.ReceiveProcess
{
    public class KBNCR140Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly SerilogLibs _Log;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNCR140Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            PPMInvenContext pPMInvenContext,
            PPM3Context pPM3Context,
            KB3Context kB3Context,
            SerilogLibs log
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
            _Log = log;
        }

        public void setConString()
        {
            try
            {
                if (_KBCN.Plant.ToString() == "3")
                {
                    var connectString = _configuration.GetConnectionString("KB3Connection");
                    _KB3Context.Database.SetConnectionString(connectString);
                }
                else if (_KBCN.Plant.ToString() == "2")
                {
                    var connectString = _configuration.GetConnectionString("KB2Connection");
                    _KB3Context.Database.SetConnectionString(connectString);
                }
                else if (_KBCN.Plant.ToString() == "1")
                {
                    var connectString = _configuration.GetConnectionString("KB1Connection");
                    _KB3Context.Database.SetConnectionString(connectString);
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
                
                var supList = await _KB3Context.TB_MS_PartOrder.Select(x => new
                {
                    F_Supplier_Code = x.F_Supplier_Cd + '-' + x.F_Supplier_Plant
                }).OrderBy(x => x.F_Supplier_Code).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(supList);
                _result = @"{
                                ""status"":""200"",
                                ""response"":""OK"",
                                ""message"": ""Data Found"",
                                ""data"": " + _jsonData + @"}";
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

    }
}
