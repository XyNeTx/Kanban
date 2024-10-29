using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.SpecialOrdering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.SpecialOrder
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR294Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearerClass;

        public KBNOR294Controller(ISpecialOrderingServices services, BearerClass bearerClass)
        {
            _services = services;
            _bearerClass = bearerClass;
        }


        [HttpGet]
        public IActionResult LoadContactList()
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

                var result = _services.IKBNOR294.LoadContactList();

                return Ok(new
                {
                    status = 200,
                    response = "Success",
                    message = "Data Found",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    response = "Error!",
                    message = ex.Message
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Confirm(List<TB_MS_Operator> listObj)
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                await _services.IKBNOR294.Confirm(listObj);

                return Ok(new
                {
                    status = 200,
                    response = "Success",
                    message = "Data Saved",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    response = "Error!",
                    message = ex.Message
                });
            }
        }

    }
}
