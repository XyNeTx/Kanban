using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.SpecialOrder
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
                    throw new CustomHttpException(400, "Error found in the process.");
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
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Page_Load()
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

                var data = _services.IKBNOR210.Page_Load();

                if (string.IsNullOrWhiteSpace(data) || data == null)
                {
                    throw new CustomHttpException(404, "Data not found.");
                }

                return Ok(new
                {
                    status = 200,
                    response = "OK",
                    message = "Data Found",
                    data = data
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }
    }
}
