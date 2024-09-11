using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KANBAN.Controllers.API.OrderingProcess
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR140Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly KB3Context _KB3Context;
        private readonly SerilogLibs _Log;
        private readonly FillDataTable _FillDT;

        public KBNOR140Controller(
            BearerClass bearerClass,
            KB3Context kB3Context,
            PPM3Context pPM3Context,
            SerilogLibs serilogLibs,
            FillDataTable fillDataTable
            )
        {
            _BearerClass = bearerClass;
            _KB3Context = kB3Context;
            _PPM3Context = pPM3Context;
            _Log = serilogLibs;
            _FillDT = fillDataTable;
        }

        [HttpGet]
        public IActionResult GetPDSNo(string type)
        {
            try
            {

                if(!_BearerClass.CheckAuthen())
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized"
                    });
                }

                var data = _FillDT.ExecuteSQL("SELECT   distinct   F_OrderNo,substring(F_OrderNo,3,9) " +
                    "as F_OrderNO1  FROM  TB_PDS_HEADER " +
                    $"WHERE  F_OrderType = '{type}' " +
                    $"AND F_Plant='{_BearerClass.Plant}' " +
                    $"Order by  F_OrderNo ");

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Success",
                    data = JsonConvert.SerializeObject(data)
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = ex.Message
                });
                
            }
        }

    }
}
