using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController][Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/[controller]/[action]")]
    public class KBNOR370Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly ICKDService _CKDRepo;
        public KBNOR370Controller
            (
                BearerClass bearerClass,
                ICKDService CKDRepo
            )
        {
            _BearerClass = bearerClass;
            _CKDRepo = CKDRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Preview(List<VM_Post_KBNOR261> listobj)
        {
            

            var result = await _CKDRepo.IKBNOR370_Repo.Preview(listobj);

            return Ok(new
            {
                status = "200",
                response = "Success",
                message = "Preview Success",
                data = result
                //data = JsonConvert.SerializeObject(result, Formatting.Indented)
            });
        }

        [HttpGet]
        public async Task<IActionResult> PreviewKB()
        {
            
            await _CKDRepo.IKBNOR370_Repo.PreviewKB();
            return Ok(new
            {
                status = "200",
                response = "Success",
                message = "Redirecting to Preview",
            });

        }

        [HttpGet]
        public async Task<IActionResult> GetPDS()
        {
            
            var data = await _CKDRepo.IKBNOR370_Repo.GetPDS();
            
            return Ok(new
            {
                status = "200",
                response = "Success",
                message = "Get PDS Complete",
                data = data.Select(x => new
                {
                    f_OrderNo = x.F_OrderNo,
                    f_OrderType = x.F_OrderType,
                })
            });

        }
        
        [HttpPost]
        public async Task<IActionResult> PDS_GENBARCODE(List<VM_Post_KBNOR261> listObj)
        {
            
            await _CKDRepo.IKBNOR370_Repo.PDS_GENBARCODE(listObj);
            
            return Ok(new
            {
                status = "200",
                response = "Success",
                message = "Generate Barcode Complete",
            });

        }

    }
}
