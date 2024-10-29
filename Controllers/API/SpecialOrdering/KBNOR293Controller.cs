using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.SpecialOrder
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR293Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearerClass;

        public KBNOR293Controller(ISpecialOrderingServices services, BearerClass bearerClass)
        {
            _services = services;
            _bearerClass = bearerClass;
        }


        [HttpGet]
        public async Task<IActionResult> LoadColorTag()
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                var result = _services.IKBNOR293.LoadColorTag();

                return Ok(new
                {
                    status = 200,
                    response = "Success",
                    message = "Data Found",
                    data = result
                });
            }
            catch (CustomHttpException ex)
            {
                return StatusCode(ex.StatusCode, new
                {
                    status = ex.StatusCode,
                    response = "Error",
                    message = ex.Message
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Confirm(List<VM_Post_Tag_Color> listObj)
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                await _services.IKBNOR293.Confirm(listObj);

                return Ok(new
                {
                    status = 200,
                    response = "Success",
                    message = "Data Saved",
                });
            }
            catch (CustomHttpException ex)
            {
                return StatusCode(ex.StatusCode, new
                {
                    status = ex.StatusCode,
                    response = "Error",
                    message = ex.Message
                });
            }
        }

    }
}
