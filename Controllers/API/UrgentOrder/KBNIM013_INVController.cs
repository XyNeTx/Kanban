using HINOSystem.Libs;
using KANBAN.Models.KB3.UrgentOrder;
using KANBAN.Models.PPM;
using KANBAN.Services.UrgentOrder.IRepository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNIM013_INVController : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IUrgentRepo _UrgentRepo;
        public KBNIM013_INVController
            (
                BearerClass bearerClass,
                IUrgentRepo UrgentRepo
            )
        {
            _BearerClass = bearerClass;
            _UrgentRepo = UrgentRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetList_Header()
        {
            await _BearerClass.CheckAuthorize();

            var result = await _UrgentRepo.IKBNIM013_INV_Repo.GetList_Header();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetList_Detail(string inDeclareNo)
        {
            await _BearerClass.CheckAuthorize();

            var result = await _UrgentRepo.IKBNIM013_INV_Repo.GetList_Detail(inDeclareNo);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> InterfaceDataToTransactionTemp(List<VM_KBNIM013_INV> listObj,string PDS)
        {
            await _BearerClass.CheckAuthorize();

            await _UrgentRepo.IKBNIM013_INV_Repo.InterfaceDataToTransactionTemp(listObj,PDS);

            return Ok(new
            {
                status = "200",
                message = "Interface Data Complete !!"
            });
        }

    }
}
