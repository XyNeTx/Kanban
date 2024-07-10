using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.EntityFrameworkCore;

using HINOSystem.Libs;
using HINOSystem.Context;
using KANBAN.Models.KB3.UrgentOrder;
using KANBAN.Context;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNIM014Controller : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly KB3Context _KB3Context;
        private readonly SerilogLibs _Log;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNIM014Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context,
            PPM3Context pPM3Context,
            SerilogLibs serilogLibs
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


        [HttpPost]
        public async Task<IActionResult> ImportSave(TB_Import_EKanban_Pack obj)
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
            try
            {
                _KB3Transaction.CreateSavepoint("Start_ImportSave");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                obj.F_Plant_CD = Plant;
                obj.F_Update_By = UserID;
                obj.F_Update_Date = DateTime.Now;

                if (ModelState.IsValid)
                {
                    _KB3Context.TB_Import_EKanban_Pack.Add(obj);
                }
                else
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Bad Request",
                        message = "Data Not Valid!"
                    });
                }

                await _KB3Context.SaveChangesAsync();
                _KB3Transaction.Commit();
                _Log.WriteLog("KBNIM014 ImportSave ", UserID, _BearerClass.Device);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
                    message = "Import Data Success!"
                });
            }
            catch (Exception ex)
            {
                _KB3Transaction.Rollback();
                return StatusCode(500,new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = "Can't Import Data!",
                    err = ex.Message.ToString()
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AfterImported()
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
            try
            {
                _KB3Transaction.CreateSavepoint("Start_AfterImported");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                await _KB3Context.Database.ExecuteSqlRawAsync($"DELETE From TB_Import_error Where F_Update_By = @p0 AND F_Type = 'KBNIM014' ",UserID);

                await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC [exec].[spKBNIM014] @p0,@p1",Plant,UserID);


                var _delList = await _KB3Context.TB_Import_EKanban_Pack.Where(x => x.F_Plant_CD == Plant && x.F_Update_By == UserID).ToListAsync();

                _KB3Context.RemoveRange(_delList);

                _KB3Transaction.Commit();


                int _haveError = await _KB3Context.Database.ExecuteSqlRawAsync($"SELECT * FROM TB_Import_Error Where F_Update_By = @p0 and F_Type = 'KBNIM014'; ",UserID);

                _Log.WriteLog("KBNIM014 AfterImported ", UserID, _BearerClass.Device);

                if (_haveError > 0)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Bad Request",
                        message = "Data Imported but Have Some Error",
                        userid = UserID,
                        type = "KBNIM014"

                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
                    message = "Import Direct Supply Complete!!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = "Data Didn't Import!",
                    err = ex.Message.ToString()
                });
            }
        }
    }
}
