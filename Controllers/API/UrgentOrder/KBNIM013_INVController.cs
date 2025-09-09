using HINOSystem.Libs;
using KANBAN.Models.PPM;
using KANBAN.Services.UrgentOrder.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            

            var result = await _UrgentRepo.IKBNIM013_INV_Repo.GetList_Header();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetList_Detail(string inDeclareNo)
        {
            

            var result = await _UrgentRepo.IKBNIM013_INV_Repo.GetList_Detail(inDeclareNo);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> InterfaceDataToTransactionTemp(List<VM_KBNIM013_INV> listObj,string PDS)
        {
            

            await _UrgentRepo.IKBNIM013_INV_Repo.InterfaceDataToTransactionTemp(listObj,PDS);

            return Ok(new
            {
                status = "200",
                message = "Interface Data Complete !!"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(List<VM_KBNIM013_INV> listObj)
        {
            
            await _UrgentRepo.IKBNIM013_INV_Repo.Delete(listObj);
            return Ok(new
            {
                status = "200",
                message = "Delete Data Complete !!"
            });
        }

    }
}
