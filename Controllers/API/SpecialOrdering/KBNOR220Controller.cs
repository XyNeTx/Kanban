using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KANBAN.Controllers.API.SpecialOrdering
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpPost]
        public async Task<IActionResult> Save(VM_Post_KBNOR220_Gen model)
        {
            try
            {
                

                await _services.IKBNOR220.Save(model);

                return Ok(new { status = "200", response = "OK", message = "Data Saved" });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Generate(List<VM_Post_KBNOR220_Gen> listModel)
        {
            try
            {
                

                await _services.IKBNOR220.Generate(listModel);

                return Ok(new { status = "200", response = "OK", message = "Generate Survey Completed !" });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
    }
}
