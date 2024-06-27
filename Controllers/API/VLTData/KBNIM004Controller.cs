using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Models.KB3.VLT;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNIM004Controller : Controller
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
        public async Task<IActionResult> ImportData (List<TB_Import_VLT> listObj)
        {
            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                _KB3Transaction.CreateSavepoint("BeforeImport");
                string Plant = HttpContext.Session.GetString("USER_PLANT");
                string UserID = HttpContext.Session.GetString("USER_CODE");

                foreach(var obj in listObj)
                {
                    obj.F_PDS_No = obj.F_VHD_Order_No;
                    obj.F_Update_By = UserID;
                    obj.F_Update_Date = DateTime.Now;

                    obj.F_Bridge_F = _KB3Context.TB_MS_PartCode.FirstOrDefault(x => x.F_Code == obj.F_Frame_Code).F_Bridge ?? "N";
                    obj.F_Bridge_S = _KB3Context.TB_MS_PartCode.FirstOrDefault(x => x.F_Code == obj.F_Side_Panel).F_Bridge ?? "N";
                    obj.F_Bridge_T = _KB3Context.TB_MS_PartCode.FirstOrDefault(x => x.F_Code == obj.F_Tail_Gate).F_Bridge ?? "N";
                    obj.F_Bridge_R = _KB3Context.TB_MS_PartCode.FirstOrDefault(x => x.F_Code == obj.F_RR_Axle).F_Bridge ?? "N";
                }

                await _KB3Context.TB_Import_VLT.AddRangeAsync(listObj);
                await _KB3Context.SaveChangesAsync();
                await _KB3Transaction.CommitAsync();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Import Data Success",
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
    }
}
