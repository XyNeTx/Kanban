using HINOSystem.Libs;
using KANBAN.Models.KB3.OtherCondition.ViewModel;
using KANBAN.Services;
using KANBAN.Services.OtherCondition.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.OtherCondition
{
    [Route("api/[controller]/[action]")]
    [ApiController][Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class KBNOC150Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IOtherConditionRepo _otherConditionRepo;

        public KBNOC150Controller(BearerClass BearerClass, IOtherConditionRepo otherConditionRepo)
        {
            _BearerClass = BearerClass;
            _otherConditionRepo = otherConditionRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplier()
        {
            var result = await _otherConditionRepo.IKBNOC150.Sup_DropDown();
            return Ok(new
            {
                status = "200",
                response = "Success",
                message = "Data has been retrieved",
                data = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> Print(VM_REPORT_KBNOC150 model)
        {
            try
            {
                
                await _otherConditionRepo.IKBNOC150.Print(model);
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
