using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.CKD_Ordering.IRepository;
using KANBAN.Services.CKD_Ordering.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Spire.Barcode;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
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
        public async Task<IActionResult> Preview(VM_Post_KBNOR261 obj)
        {
            await _BearerClass.CheckAuthorize();

            var result = await _CKDRepo.IKBNOR370_Repo.Preview(obj);

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
            await _BearerClass.CheckAuthorize();
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
            await _BearerClass.CheckAuthorize();
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
            await _BearerClass.CheckAuthorize();
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
