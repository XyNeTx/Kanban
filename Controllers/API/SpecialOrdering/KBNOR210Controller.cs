using HINOSystem.Libs;
using KANBAN.Services.SpecialOrdering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.SpecialOrder
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR210Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _BearerClass;

        public KBNOR210Controller(ISpecialOrderingServices services, BearerClass BearerClass)
        {
            _services = services;
            _BearerClass = BearerClass;
        }

        [HttpPost]
        public async Task<IActionResult> Interface()
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                await _services.IKBNOR210.Interface();
                bool check = await _services.IKBNOR210.Check_Error();
                if (check)
                {
                    return BadRequest(new
                    {
                        status = 400,
                        response = "Bad Request",
                        message = "Error found in the process."
                    });
                }

                return Ok(new
                {
                    status = 200,
                    response = "OK",
                    message = "Process completed."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    response = "Internal Server Error",
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult Page_Load()
        {
            try
            {
                if(_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                return Ok(new
                {
                    status = 200,
                    response = "OK",
                    message = "Data Found",
                    data = _services.IKBNOR210.Page_Load()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    response = "Internal Server Error",
                    message = ex.Message

                });
            }
        }
    }
}
