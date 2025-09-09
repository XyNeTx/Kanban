using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController][Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/[controller]/[action]")]
    public class KBNOR330Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly ICKDService _CKDRepo;
        public KBNOR330Controller
            (
                BearerClass bearerClass,
                ICKDService CKDRepo
            )
        {
            _BearerClass = bearerClass;
            _CKDRepo = CKDRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Generate()
        {
            try
            {
                

                var tupleDT_Str = await _CKDRepo.IKBNOR330_Repo.Generate();

                if (tupleDT_Str.Item2.Contains("ไม่สามารถ Generate PDS สำหรับ CKD Order ได้"))
                {
                    return Ok(new
                    {
                        status = "200",
                        response = "Error",
                        message = tupleDT_Str.Item2,
                        data = JsonConvert.SerializeObject(tupleDT_Str.Item1, Formatting.Indented)
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Generate Success",
                    data = JsonConvert.SerializeObject(tupleDT_Str.Item1, Formatting.Indented)
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
