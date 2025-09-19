using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Models.KB3.VLT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    public class KBNIM004Controller : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;

        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNIM004Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;

        }

        [HttpPost]
        public async Task<IActionResult> ImportData(List<TB_Import_VLT> listObj)
        {
            //using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                _BearerClass.Authentication();

                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });
                // _KB3Transaction.CreateSavepoint("BeforeImport");
                string Plant = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Locality).Value.ToString();
                string UserID = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData).Value.ToString();

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Import_VLT WHERE F_Update_By = @p0", UserID!);

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Import_Error WHERE F_Type = 'KBNIM004' AND F_Update_By = @p0", UserID!);

                foreach (var obj in listObj)
                {
                    obj.F_PDS_No = obj.F_VHD_Order_No;
                    obj.F_Update_By = UserID;
                    obj.F_Update_Date = DateTime.Now;

                    obj.F_Bridge_F = _KB3Context.TB_MS_PartCode.FirstOrDefault(x => x.F_Code == obj.F_Frame_Code)?.F_Bridge ?? "N";
                    obj.F_Bridge_S = _KB3Context.TB_MS_PartCode.FirstOrDefault(x => x.F_Code == obj.F_Side_Panel)?.F_Bridge ?? "N";
                    obj.F_Bridge_T = _KB3Context.TB_MS_PartCode.FirstOrDefault(x => x.F_Code == obj.F_Tail_Gate)?.F_Bridge ?? "N";
                    obj.F_Bridge_R = _KB3Context.TB_MS_PartCode.FirstOrDefault(x => x.F_Code == obj.F_RR_Axle)?.F_Bridge ?? "N";
                }

                await _KB3Context.TB_Import_VLT.AddRangeAsync(listObj);
                await _KB3Context.SaveChangesAsync();

                await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [exec].[SPKBNIM004_Import_MRP] @p0,@p1", Plant, UserID);

                string ql = "SELECT * FROM TB_Import_Error WHERE (F_Type = 'KBNIM004') AND (F_Update_By = '" + UserID + "')";

                int count = await _KB3Context.Database.ExecuteSqlRawAsync(ql);

                if (count > 0)
                {
                    //_KB3Transaction.RollbackToSavepoint("BeforeImport");
                    return Ok(new
                    {
                        status = "400",
                        response = "Bad Request",
                        message = "Have Some Error Please Check Report",
                        type = "KBNIM004",
                        userid = UserID
                    });
                }

                //await _KB3Transaction.CommitAsync();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Import Data Success",
                });
            }
            catch (Exception ex)
            {
                //_KB3Transaction.RollbackToSavepoint("BeforeImport");
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
