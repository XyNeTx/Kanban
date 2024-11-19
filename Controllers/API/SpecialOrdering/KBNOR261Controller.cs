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
    public class KBNOR261Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearer;

        public KBNOR261Controller(ISpecialOrderingServices services, BearerClass bearer)
        {
            _services = services;
            _bearer = bearer;
        }

        [HttpGet]
        public async Task<IActionResult> GetPDSWaitApprove()
        {
            try
            {
                await _bearer.CheckAuthorize();

                var data = _services.IKBNOR261.GetPDSWaitApprove();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Preview(VM_Post_KBNOR261 obj)
        {
            try
            {
                await _bearer.CheckAuthorize();

                await _services.IKBNOR261.Preview(obj);

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

        [HttpPost]
        public async Task<IActionResult> Approve([FromBody] List<VM_Post_KBNOR261> listObj)
        {
            try
            {
                await _bearer.CheckAuthorize();

                await _services.IKBNOR261.Approve(listObj);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Approved",
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
    }
}
