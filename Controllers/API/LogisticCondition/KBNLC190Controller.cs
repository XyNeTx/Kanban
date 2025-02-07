using HINOSystem.Libs;
using KANBAN.Models.KB3.LogisticCondition;
using KANBAN.Services.Logistical.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNLC190Controller : ControllerBase
    {
        private readonly ILogisticService _services;
        private readonly BearerClass _BearerClass;

        public KBNLC190Controller(
            ILogisticService services,
            BearerClass Bearer
            )
        {
            _BearerClass = Bearer;
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> GetRev(string YM)
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
            try
            {
                var data = await _services.IKBNLC190.GetRev(YM);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been Found",
                    data = data

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

        [HttpGet]
        public async Task<IActionResult> Search(string YM, int Rev)
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

            try
            {
                var data = await _services.IKBNLC190.Search(YM, Rev);
                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been Found",
                    data = data.Select(x => new
                    {
                        x.F_short_Logistic,
                        x.F_Supplier_Code,
                        x.F_Cycle_Time,
                        F_Route = x.F_Truck_Card.Substring(0, 4),
                    })

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

        [HttpPost]
        public async Task<IActionResult> Interface(VM_KBNLC190_Interface obj)
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

            try
            {
                if (await _services.IKBNLC190.Interface(obj.YM, obj.Rev, obj.StartDate))
                {
                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data has been Interface"
                    });
                }

                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Data has not been Interface",
                    userid = _BearerClass.UserCode,
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
