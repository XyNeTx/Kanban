using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.KB3.UrgentOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.ServiceData
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNIM001CController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly KB3Context _KB3Context;
        private readonly SerilogLibs _Log;
        private readonly FillDataTable _FillDT;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNIM001CController(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context,
            PPM3Context pPM3Context,
            SerilogLibs serilogLibs,
            FillDataTable fillDT
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _PPM3Context = pPM3Context;
            _Log = serilogLibs;
            _FillDT = fillDT;
        }


        [HttpPost]
        public async Task<IActionResult> ImportData (List<TB_Import_Service> listObj)
        {
            _BearerClass.Authentication(Request);

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            string UserID = HttpContext.Session.GetString("USER_CODE");
            try
            {
                _KB3Transaction.CreateSavepoint("BeforeImport");
                await _KB3Context.Database.ExecuteSqlRawAsync($"DELETE FROM TB_Import_Service WHERE F_Update_By = '{UserID}'");

                foreach (var each in listObj)
                {
                    if (each.F_Part_No.Count() == 10)
                    {
                        each.F_Ruibetsu = "00";
                    }
                    else
                    {
                        each.F_Ruibetsu = each.F_Part_No.Substring(10, 2);
                        each.F_Part_No = each.F_Part_No.Substring(0, 10);
                    }
                    each.F_Update_By = UserID;
                }


                await _KB3Context.TB_Import_Service.AddRangeAsync(listObj);
                await _KB3Context.SaveChangesAsync();
                _KB3Transaction.Commit();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Data has been imported successfully",
                    data = listObj,
                });
            }
            catch (Exception ex)
            {
                _KB3Transaction.RollbackToSavepoint("BeforeImport");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AfterImported(string advDate)
        {
            _BearerClass.Authentication(Request);

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            string UserID = HttpContext.Session.GetString("USER_CODE");
            string Plant = HttpContext.Session.GetString("USER_PLANT");

            try
            {
                await _KB3Context.Database.ExecuteSqlRawAsync($"Exec dbo.SP_IM001_IMPORT_SRV '{Plant}','{UserID}' ,'{advDate}' ");
                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Data has been imported successfully",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }
    }
}
