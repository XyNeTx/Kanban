using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SkiaSharp;
using System.Globalization;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR100Controller : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;
        private readonly FillDataTable _FillDT;
        private readonly KB3Context _KB3Context;

        public KBNOR100Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            KB3Context kB3Context,
            FillDataTable fillDataTable
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
            _KB3Context = kB3Context;
            _FillDT = fillDataTable;

        }


        [HttpGet]
        public IActionResult Onload(string dateShift)
        {
            try
            {
                var waitCCR = _KB3Context.TB_MS_Parameter
                    .Where(x => x.F_Code == "CI").FirstOrDefault();

                List<string> list = new(new string[]{
                    "KBNOR110",
                    "KBNOR120",
                    "KBNOR130",
                    "KBNOR140",
                    "KBNOR150"
                });

                string sql = $"Select dbo.FN_GetProcess('{dateShift}',2) AS VALUE";
                string date = _KB3Context.Database.SqlQueryRaw<string>(sql).FirstOrDefault();

                if (waitCCR == null)
                {
                    return StatusCode(500, new
                    {
                        status = "500",
                        response = "Internal Server Error",
                        title = "Data Not Found",
                        message = "ไม่พบข้อมูลในระบบ กรุณาติดต่อผู้ดูแลระบบ",
                        error = "Data Not Found"
                    });
                }

                if (waitCCR.F_Value2 == 0)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Data Not Confirm",
                        message = "กรุณารอการยืนยันข้อมูลจาก CCR ก่อนประมวลผลยอดการสั่งซื้อชิ้นส่วน",
                        data = waitCCR,
                        cmd = list.ToArray()
                    });
                }

                if (waitCCR.F_Value3 != dateShift)
                {

                    DateTime nDate = DateTime.ParseExact(waitCCR.F_Value3.Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture);

                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Data Not Confirm",
                        message = $"กรุณายืนยันวันที่จะดำเนินการออก Order อีกครั้งหนึง CCR แจ้งยืนยันการนำเข้าข้อมูลเพื่อออก Order วันที่ " +
                                    $"{nDate.ToString("dd/MM/yyyy")} Shift {waitCCR.F_Value3.Substring(8, 1)}",
                        data = waitCCR,
                        cmd = list.ToArray()
                    });
                }

                if (waitCCR.F_Value2 == 3)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "DATA IS CALCULATING",
                        message = $"กรุณารอสักครู่..ระบบกำลังคำนวณการออก Order ชิ้นส่วน",
                        data = waitCCR,
                        cmd = list.ToArray()
                    });
                }

                int compareForecast = _KB3Context.Database.SqlQueryRaw<int>("Select dbo.FN_CompareForecast() AS VALUE").FirstOrDefault();

                if (compareForecast > 0)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "DATA IS CALCULATING",
                        message = "มีข้อมูล Forecast ใหม่ กรุณากด Interface ข้อมูลจาก PPM",
                        data = waitCCR,
                        compareForecast = compareForecast
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Normal",
                    data = waitCCR,
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500 ,new
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
