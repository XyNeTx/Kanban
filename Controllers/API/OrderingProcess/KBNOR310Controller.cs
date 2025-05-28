using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class KBNOR310Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly ICKDService _CKDRepo;
        public KBNOR310Controller
            (
                BearerClass bearerClass,
                ICKDService CKDRepo
            )
        {
            _BearerClass = bearerClass;
            _CKDRepo = CKDRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Onload()
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                var data = await _CKDRepo.IKBNOR310_Repo.getCKD_ProcessDateTime();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Onload Completed",
                    data = JsonConvert.SerializeObject(data, Formatting.Indented)
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Interface()
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                await _CKDRepo.IKBNOR310_Repo.Interface();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Interface Data Completed"
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
