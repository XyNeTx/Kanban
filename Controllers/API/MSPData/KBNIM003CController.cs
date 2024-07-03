using HINOSystem.Context;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNIM003CController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;        
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly SerilogLibs _log;
        private readonly FillDataTable _FillDT;
        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNIM003CController(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context,
            SerilogLibs log,
            FillDataTable fillDataTable
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _log = log;
            _FillDT = fillDataTable;
        }

        [HttpGet]
        public async Task<IActionResult> Search()
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
                string now = DateTime.Now.ToString("yyyyMMdd");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                DataTable dt = _FillDT.ExecuteSQL($"EXEC [exec].[spKBNIM003C_SEARCH] '{Plant}' , '{UserID}' ");

                if (dt.Rows.Count == 0)
                {
                    return StatusCode(500, new
                    {
                        status = "500",
                        response = "internal server error",
                        title = "Error",
                        message = "Initial Data Not Found"
                    });
                }


                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Search Data Complete",
                    data = JsonConvert.SerializeObject(dt),
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "internal server error",
                    title = "Error",
                    message = e.Message.ToString()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(List<object> listObj)
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
                _KB3Transaction.CreateSavepoint("BeforeConfirm");

                foreach (var obj in listObj)
                {
                    JObject jObject = JObject.Parse(obj.ToString());
                    string F_PDS_No = jObject["F_PDS_No"].ToString();
                    string F_PDS_Issued_Date = jObject["F_PDS_ISSUED_DATE"].ToString();
                    string F_Delivery_Date = jObject["F_Delivery_Date"].ToString();

                    F_PDS_Issued_Date = F_PDS_Issued_Date.Replace("/", string.Empty);
                    F_Delivery_Date =  F_Delivery_Date.Replace("/", string.Empty);

                    F_PDS_Issued_Date = F_PDS_Issued_Date.Substring(4, 4) + F_PDS_Issued_Date.Substring(2, 2) + F_PDS_Issued_Date.Substring(0, 2);
                    F_Delivery_Date = F_Delivery_Date.Substring(4, 4) + F_Delivery_Date.Substring(2, 2) + F_Delivery_Date.Substring(0, 2);

                    string Plant = HttpContext.Session.GetString("USER_PLANT");
                    string UserID = HttpContext.Session.GetString("USER_CODE");

                    await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC [exec].[spKBNIM003C_CONFIRM] '@Plant', '@Type', '@PDSNo', '@PDSIssueDate', '@DeliveryDate', '@UserID' ",
                        new SqlParameter("@Plant",Plant),
                        new SqlParameter("@Type", "EKanban"),
                        new SqlParameter("@PDSNo", F_PDS_No),
                        new SqlParameter("@PDSIssueDate", F_PDS_Issued_Date),
                        new SqlParameter("@DeliveryDate", F_Delivery_Date),
                        new SqlParameter("@UserID", UserID)
                        );
                }

                _KB3Transaction.Commit();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Confirm Data Complete",
                });
            }
            catch (Exception ex)
            {
                _KB3Transaction.RollbackToSavepoint("BeforeConfirm");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "internal server error",
                    title = "Error",
                    message = ex.Message.ToString()
                });
            }
        }

    }
}
