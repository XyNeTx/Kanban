using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Controllers.API.SpecialOrdering
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class KBNOR240Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearer;
        private readonly ISpecialLibs _specialLibs;

        public KBNOR240Controller(ISpecialOrderingServices services,
            ISpecialLibs specialLibs, BearerClass bearer)
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
                
                DataTable data = _specialLibs.GetSurveyHeaderDownload();
                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = JsonConvert.SerializeObject(data)
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DownloadClicked(List<VM_Upload_KBNOR230> listObj)
        {
            try
            {
                
                foreach (var item in listObj)
                {
                    await _services.IKBNOR240.DownloadClicked(item.F_Survey_Doc);
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Get Status Survey Complete."
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
    }
}
