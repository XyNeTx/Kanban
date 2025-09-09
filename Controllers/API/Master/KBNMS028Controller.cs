using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController][Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    public class KBNMS028Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IMasterRepo _masterRepo;

        public KBNMS028Controller(
            BearerClass bearerClass,
            IMasterRepo masterRepo
            )
        {
            _BearerClass = bearerClass;
            _masterRepo = masterRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetDockCode()
        {
            try
            {
                

                var data = await _masterRepo.IKBNMS028.GetDockCode();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        F_Dock_Cd = x.F_Dock_Cd
                    }).AsEnumerable()
                });
            }
            catch (CustomHttpException ex)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetShortLogistic()
        {
            try
            {
                

                var data = await _masterRepo.IKBNMS028.GetShortLogistic();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        F_short_Logistic = x.F_short_Logistic
                    }).AsEnumerable()
                });
            }
            catch (CustomHttpException ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListData(string? F_Dock_Cd)
        {
            try
            {
                

                var data = await _masterRepo.IKBNMS028.GetListData(F_Dock_Cd);

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
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(List<TB_MS_Remark_DocSheet> listObj, string action)
        {
            try
            {
                

                foreach (var obj in listObj)
                {
                    await _masterRepo.IKBNMS028.Save(obj, action);
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been Saved",
                });
            }
            catch (CustomHttpException ex)
            {
                throw;
            }
        }

    }
}
