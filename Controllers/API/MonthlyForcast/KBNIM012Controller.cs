using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNIM012Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;


        public KBNIM012Controller
        (
            BearerClass bearerClass,
            PPM3Context ppm3Context,
            PPMInvenContext ppmInvenContext,
            KB3Context kb3Context,
            FillDataTable fillDataTable,
            SerilogLibs log
        )
        {
            _BearerClass = bearerClass;
            _PPM3Context = ppm3Context;
            _PPMInvenContext = ppmInvenContext;
            _KB3Context = kb3Context;
            _FillDT = fillDataTable;
            _log = log;
        }

        [HttpGet]
        public IActionResult Onload()
        {
            try
            {

                _BearerClass.Authentication(Request);
                if(_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                string sql = "Select Top 1 F_PO,F_Version From VW_MaxVersionForecast Order by substring(F_PO,1,6) desc,F_Version";

                DataTable dt = _FillDT.ExecuteSQLProcDB(sql);

                sql = "Select Top 1 F_Version,F_Production_Date,F_Revision_NO from TB_IMPORT_FORECAST " +
                    " Order by F_Production_Date desc,F_Version,F_Revision_NO desc";

                DataTable dt2 = _FillDT.ExecuteSQL(sql);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Loaded Successfully!",
                    data = new
                    {
                        maxVersion = dt.Rows[0]["F_Version"].ToString(),
                        maxPO = dt.Rows[0]["F_PO"].ToString(),
                        version = dt2.Rows[0]["F_Version"].ToString(),
                        productionDate = dt2.Rows[0]["F_Production_Date"].ToString(),
                        revisionNo = dt2.Rows[0]["F_Revision_NO"].ToString()
                    }
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplier()
        {
            try
            {

                _BearerClass.Authentication(Request);
                if(_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
                throw;
            }
        }
    }
}
