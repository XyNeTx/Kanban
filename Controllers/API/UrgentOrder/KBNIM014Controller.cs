using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.EntityFrameworkCore;
using HINOSystem.Libs;
using HINOSystem.Context;
using KANBAN.Models.KB3.UrgentOrder;
using KANBAN.Context;
using Newtonsoft.Json;
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
        private readonly FillDataTable _FillDT;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNIM014Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context,
            PPM3Context pPM3Context,
            SerilogLibs serilogLibs,
            FillDataTable fillDataTable
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
            _FillDT = fillDataTable;
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
            
            try
            {

                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                obj.F_Plant_CD = Plant;
                obj.F_Update_By = UserID; 
                obj.F_Update_Date = DateTime.Now;

                await _KB3Context.TB_Import_EKanban_Pack.AddAsync(obj);
                await _KB3Context.SaveChangesAsync();
                _Log.WriteLogMsg($" | {JsonConvert.SerializeObject(obj)}");

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

                //var intRow = 
                await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC [exec].[spKBNIM014] @p0,@p1",Plant,UserID);

                //if(intRow > 0)
                //{
                //    await _KB3Context.Database.ExecuteSqlRawAsync($"DELETE FROM TB_Import_EKanban_Pack Where F_Plant_CD = @p0 AND F_Update_By = @p1", Plant, UserID);
                //    throw new Exception("Data Imported but Have Some Error Please Try Again");
                //}

                var _delList = await _KB3Context.TB_Import_EKanban_Pack.Where(x => x.F_Plant_CD == Plant && x.F_Update_By == UserID).ToListAsync();

                _KB3Context.RemoveRange(_delList);
                _KB3Context.SaveChanges();

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
                _KB3Transaction.RollbackToSavepoint("Start_AfterImported");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.Message,
                    err = ex.Message.ToString()
                });
            }
        }
    }
}
