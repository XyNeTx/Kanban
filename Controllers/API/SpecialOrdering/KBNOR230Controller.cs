using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.SpecialOrdering
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR230Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearer;
        private readonly ISpecialLibs _specialLibs;

        public KBNOR230Controller(ISpecialOrderingServices services,
            BearerClass bearer, ISpecialLibs specialLibs)
        {
            _services = services;
            _bearer = bearer;
            _specialLibs = specialLibs;
        }

        [HttpGet]
        public async Task<IActionResult> LoadSurvey()
        {
            try
            {
                var data = await _services.IKBNOR230.LoadSurvey();

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

        [HttpGet]
        public IActionResult CheckPriceFlag(string SurveyDoc)
        {
            try
            {
                var data = _specialLibs.CheckPriceFlag(SurveyDoc);

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
        public async Task<IActionResult> Upload(List<VM_Upload_KBNOR230> listObj)
        {
            try
            {
                await _services.IKBNOR230.Upload(listObj);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Uploaded"
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
    }
}
