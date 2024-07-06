using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.KB3.OrderingProcess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Controllers.API.OrderingProcess
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR160Controller : ControllerBase
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

        public KBNOR160Controller(
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

        [HttpGet]
        public async Task<IActionResult> List_Data(string? conditionDate = null, string? MRPRadio = null)
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
                string UserID = _BearerClass.UserCode;
                string Plant = _BearerClass.Plant;

                if(conditionDate == null)
                {
                    conditionDate = DateTime.Now.ToString("yyyyMMdd");
                }

                string _SQL = " SELECT rtrim(F_Supplier_Code)+'-'+ rtrim(F_Supplier_Plant) as F_Supplier_Code, rtrim(F_Part_No)+'-'+rtrim(F_Ruibetsu) as F_Part_No, F_Store_Code, F_Kanban_No, F_Process_Date, " +
                    $" F_TMT_FO, F_HMMT_Prod, F_HMMT_Order,F_Cycle_Order, F_MRP From TB_Calculate_H WHERE  F_Process_Date = {conditionDate} ";

                if(MRPRadio == "-20")
                {
                    _SQL += " and F_MRP < F_HMMT_Prod * 0.8 ";
                }
                else if (MRPRadio == "+20")
                {
                    _SQL += " and F_MRP > F_HMMT_Prod * 1.2 ";
                }

                _SQL += " ORDER BY F_Supplier_Code, F_Store_Code, F_Part_no, F_Kanban_No ";

                DataTable dt = _FillDT.ExecuteSQL(_SQL);

                if(dt.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        message = "Data not found",
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data found",
                    data = JsonConvert.SerializeObject(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected error",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportData(List<TB_Import_UpdMRP_FG> listObj)
        {
            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            _BearerClass.Authentication(Request);
            if(_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });
            string UserID = _BearerClass.UserCode;
            string Plant = _BearerClass.Plant;

            try
            {
                
                _KB3Transaction.CreateSavepoint("BeforeImport");

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Import_UpdMRP_FG WHERE F_Update_By = {0}", UserID);

                foreach (var obj in listObj)
                {
                    obj.F_Update_By = UserID;
                    obj.F_Update_Date = DateTime.Now;
                }

                await _KB3Context.TB_Import_UpdMRP_FG.AddRangeAsync(listObj);
                await _KB3Context.SaveChangesAsync();

                _KB3Transaction.CreateSavepoint("AfterImport");

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_IMPORT_ERROR WHERE F_Update_By = {0} and F_Type = 'KBNOR160'", UserID);

                await _KB3Context.Database.ExecuteSqlRawAsync("Exec [exec].[SPKBNOR160_IMPORT_UPDATEMRP_FG] {0},{1} ",Plant,UserID);

                await _KB3Transaction.CommitAsync();

                DataTable dt = _FillDT.ExecuteSQL($"SELECT * FROM TB_IMPORT_ERROR WHERE F_Update_By = {UserID} and F_Type = 'KBNOR160'");

                if (dt.Rows.Count > 0)
                {
                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data has been imported but Have Some data Error",
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been imported"
                });
            }
                       catch (Exception ex)
            {
                _KB3Transaction.RollbackToSavepoint("BeforeImport");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected error",
                    error = ex.Message
                });
            }
        }

    }
}
