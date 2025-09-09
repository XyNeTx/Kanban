using HINOSystem.Libs;
using KANBAN.Models.KB3.CKD_Ordering;
using KANBAN.Services;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpGet]
        public async Task<IActionResult> Check_CKDStatus()
        {
            try
            {
                
                await _CKDRepo.IKBNOR310_Repo.getCKD_ProcessDateTime();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Onload Completed",
                    data = await _CKDRepo.IKBNOR360_Repo.Check_CKDStatus()
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> List_Data()
        {
            try
            {
                

                var data = await _CKDRepo.IKBNOR360_Repo.List_Data();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Get List Data Complete",
                    data = JsonConvert.SerializeObject(data,Formatting.Indented)
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] List<VM_KBNOR360_Register> listObj)
        {
            try
            {
                

                await _CKDRepo.IKBNOR360_Repo.Register(listObj);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Register Data Success"
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePicking_Click()
        {
            try
            {
                

                var data = await _CKDRepo.IKBNOR360_Repo.GeneratePicking_Click();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Generate Picking Click Success",
                    data = data
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
