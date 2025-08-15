using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class KBNOR320Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly ICKDService _CKDRepo;
        public KBNOR320Controller
            (
                BearerClass bearerClass,
                ICKDService CKDRepo
            )
        {
            _BearerClass = bearerClass;
            _CKDRepo = CKDRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Calculate(string[]? arryVariable)
        {
            try
            {
                if(arryVariable == null || arryVariable?.Length == 0)
                {
                    await _BearerClass.CheckAuthorize();
                }
                await _CKDRepo.IKBNOR320_Repo.completeRecalculateCKD(arryVariable);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Calculate CKD Order Success"
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
