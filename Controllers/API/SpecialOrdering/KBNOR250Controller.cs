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
    public class KBNOR250Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearer;

        public KBNOR250Controller(ISpecialOrderingServices services, BearerClass bearer)
        {
            _services = services;
            _bearer = bearer;
        }

        [HttpGet]
        public async Task<IActionResult> GetSurveyNoPDS(string fac, string? SurveyDoc = "", string? DeliveryDT = "", string? DeliYM = "")
        {
            try
            {
                

                var data = _services.IKBNOR250.GetSurveyNoPDS(fac, SurveyDoc, DeliveryDT, DeliYM);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = JsonConvert.SerializeObject(data) });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckPriceAndPackageFlag(string SurveyDoc)
        {
            try
            {
                

                var data = _services.IKBNOR250.CheckPriceAndPackageFlag(SurveyDoc);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = JsonConvert.SerializeObject(data) });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                

                await _services.IKBNOR250.Refresh();

                return Ok(new { status = "200", response = "Success", message = "Complete Update Price and Package for PDS." });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Generate(List<VM_KBNOR250> listObj, [FromQuery] string DeliYM)
        {
            try
            {
                

                await _services.IKBNOR250.Generate(listObj, DeliYM);

                return Ok(new { status = "200", response = "Success", message = "Generate PDS Complete!!" });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Unlock(List<VM_KBNOR250> listObj)
        {
            try
            {
                

                await _services.IKBNOR250.Unlock(listObj);

                return Ok(new { status = "200", response = "Success", message = "Unlock Complete!!" });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
    }
}
