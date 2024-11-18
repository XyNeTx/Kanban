using HINOSystem.Libs;
using KANBAN.Models.KB3;
using KANBAN.Services.Import.Interface;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.Import
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNIM010Controller : ControllerBase
    {
        private readonly IImportService _service;
        private readonly BearerClass _bearerClass;

        public KBNIM010Controller(IImportService service,
            BearerClass bearerClass)
        {
            _service = service;
            _bearerClass = bearerClass;
        }

        [HttpGet]
        public IActionResult Onload(string date,string shift)
        {
            try
            {
                if (_bearerClass.CheckAuthen() == 401 || _bearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_bearerClass.Status, new
                    {
                        status = _bearerClass.Status,
                        response = _bearerClass.Response,
                        message = _bearerClass.Message
                    });
                }

                if (_service.KBNIM010.Check_Holiday(date,shift))
                {
                    return StatusCode(404, new
                    {
                        status = "404",
                        response = "Internal Server Error",
                        message = "กรุณายืนยันปริมาณการใช้ชิ้นส่วนในวันทำงาน วันนี้คือวันหยุดตามปฏิทินของระบบ"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Success"
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
        public IActionResult ListData(string date,string shift)
        {
            try
            {

                if (_bearerClass.CheckAuthen() == 401 || _bearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_bearerClass.Status, new
                    {
                        status = _bearerClass.Status,
                        response = _bearerClass.Response,
                        message = _bearerClass.Message
                    });
                }
                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Success",
                    data = _service.KBNIM010.ListData(date, shift)
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

        [HttpPost]
        public async Task<IActionResult> Confirm(VM_DateShift obj)
        {
            try
            {

                if (_bearerClass.CheckAuthen() == 401 || _bearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_bearerClass.Status, new
                    {
                        status = _bearerClass.Status,
                        response = _bearerClass.Response,
                        message = _bearerClass.Message
                    });
                }

                if (await _service.KBNIM010.Confirm(obj.date, obj.shift))
                {
                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        message = "Success"
                    });
                }

                return StatusCode(404, new
                {
                    status = "404",
                    response = "Internal Server Error",
                    message = "No Data"
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

    }
}
