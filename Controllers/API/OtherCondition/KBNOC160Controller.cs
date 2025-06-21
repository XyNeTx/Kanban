using HINOSystem.Libs;
using KANBAN.Models.KB3.OtherCondition.ViewModel;
using KANBAN.Services;
using KANBAN.Services.OtherCondition.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.OtherCondition
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOC160Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IOtherConditionRepo _otherConditionRepo;

        public KBNOC160Controller(BearerClass BearerClass, IOtherConditionRepo otherConditionRepo)
        {
            _BearerClass = BearerClass;
            _otherConditionRepo = otherConditionRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Print(VM_REPORT_KBNOC160 model)
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                await _otherConditionRepo.IKBNOC160.Print(model);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Process Success"
                });

            }
            catch (Exception ex)
            {

                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }


}
