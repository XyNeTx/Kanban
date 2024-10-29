using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KANBAN.Controllers.API.SpecialOrdering
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR220Controller : ControllerBase
    {

        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearerClass;

        public KBNOR220Controller(ISpecialOrderingServices services, BearerClass bearerClass)
        {
            _services = services;
            _bearerClass = bearerClass;
        }

        [HttpGet]
        public async Task<IActionResult> LoadColorofTag()
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                var data = _services.IKBNOR220.LoadColorofTag();

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadListView()
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                var data = _services.IKBNOR220.LoadListView();

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> InitialScreenCmb()
        {
            try
            {

                await _bearerClass.CheckAuthorize();

                var DeptMS = _services.IKBNOR220.GetDeptMS(DateTime.Now.ToString("yyyyMMdd"), "");
                var ACCMS = _services.IKBNOR220.GetACCOUNTMS(DateTime.Now.ToString("yyyyMMdd"));

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = new
                    {
                        deptMS = JsonConvert.SerializeObject(DeptMS),
                        accMS = JsonConvert.SerializeObject(ACCMS)
                    }
                });

            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

    }
}
