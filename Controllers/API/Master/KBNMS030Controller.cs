using HINOSystem.Libs;
using KANBAN.Models.KB3.Master;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController][Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    public class KBNMS030Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IMasterRepo _MasterRepo;

        public KBNMS030Controller(
            BearerClass BearerClass,
            IMasterRepo MasterRepo
            )
        {
            _BearerClass = BearerClass;
            _MasterRepo = MasterRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetListData(string? F_Line_ID, string? F_Description, string? F_Customer)
        {
            try
            {
                

                var data = await _MasterRepo.IKBNMS030.GetListData(F_Line_ID, F_Description, F_Customer);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(List<TB_MS_LineControl> listObj, [FromQuery] string action)
        {
            try
            {
                

                var data = listObj.FirstOrDefault();

                if (data == null)
                {
                    throw new CustomHttpException(400, "Save Data is Empty");
                }

                if (action.ToLower() == "new")
                {
                    await _MasterRepo.IKBNMS030.Insert(data);
                }
                else if (action.ToLower() == "upd")
                {
                    await _MasterRepo.IKBNMS030.Update(data);
                }
                else if (action.ToLower() == "del")
                {
                    await _MasterRepo.IKBNMS030.Delete(listObj);
                }
                else
                {
                    throw new CustomHttpException(400, "Please select action before save data");
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Saved"
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

    }
}
