using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
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
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Confirm(List<VM_Post_Tag_Color> listObj)
        {
            try
            {
                

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
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

    }
}
