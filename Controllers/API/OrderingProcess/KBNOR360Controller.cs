using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class KBNOR360Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly ICKDService _CKDRepo;
        public KBNOR360Controller
            (
                BearerClass bearerClass,
                ICKDService CKDRepo
            )
        {
            _BearerClass = bearerClass;
            _CKDRepo = CKDRepo;
        }

        public async Task<IActionResult> Check_CKDStatus()
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                await _CKDRepo.IKBNOR310.getCKD_ProcessDateTime();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Onload Completed",
                    data = await _CKDRepo.IKBNOR360.Check_CKDStatus()
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
