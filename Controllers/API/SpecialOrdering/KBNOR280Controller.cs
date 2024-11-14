using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KANBAN.Controllers.API.SpecialOrdering
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR280Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearer;

        public KBNOR280Controller(ISpecialOrderingServices services, BearerClass bearer)
        {
            _services = services;
            _bearer = bearer;
        }

        [HttpGet]
        public async Task<IActionResult> GetPDSData(string DeliYM)
        {
            try
            {
                //await _bearer.CheckAuthorize();

                string FacCD = _bearer.Plant switch
                {
                    "1" => "9Z",
                    "2" => "8Y",
                    "3" => "7Z",
                    _ => "9Z"
                };

                var data = _services.IKBNOR280.GetPDSData(FacCD, DeliYM);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(List<VM_Register_KBNOR280> listObj)
        {
            try
            {
                //await _bearer.CheckAuthorize();
                await _services.IKBNOR280.Register(listObj);

                return Ok(new { status = "200", response = "Success", message = "Registration PDS No. Complete." });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
    }
}
