using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KANBAN.Controllers.API.SpecialOrdering
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR270Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearer;

        public KBNOR270Controller(ISpecialOrderingServices services, BearerClass bearer)
        {
            _services = services;
            _bearer = bearer;
        }

        [HttpPost]
        public async Task<IActionResult> Preview([FromBody] List<VM_Post_KBNOR261> listObj)
        {
            try
            {
                await _bearer.CheckAuthorize();

                await _services.IKBNOR270.Preview(listObj);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Redirecting to Preview",
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> PreviewKB()
        {
            try
            {
                await _bearer.CheckAuthorize();

                await _services.IKBNOR270.PreviewKB();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Redirecting to Preview",
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

    }
}
