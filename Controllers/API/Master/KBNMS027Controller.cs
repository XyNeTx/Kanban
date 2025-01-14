using HINOSystem.Libs;
using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNMS027Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IMasterRepo _masterRepo;

        public KBNMS027Controller(
            BearerClass bearerClass,
            IMasterRepo masterRepo
            )
        {
            _BearerClass = bearerClass;
            _masterRepo = masterRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetShortLogistic()
        {
            try
            {
                var data = await _masterRepo.IKBNMS027.GetShortLogistic();

                var selData = data.Select(x => new
                {
                    x.F_short_Logistic
                }).ToList();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = selData
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShortName()
        {
            try
            {
                var data = await _masterRepo.IKBNMS027.GetShortName();

                var selData = data.Select(x => new
                {
                    x.F_short_name
                }).ToList();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = selData
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListData(string? F_Short_Logistic)
        {
            try
            {
                var data = await _masterRepo.IKBNMS027.GetListData(F_Short_Logistic);
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

        [HttpGet]
        public async Task<IActionResult> SupOrderSelected(string F_Short_Name)
        {
            try
            {
                var data = await _masterRepo.IKBNMS027.SupOrderSelected(F_Short_Name);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        F_supplier_cd = x.F_supplier_cd + "-" + x.F_Plant_cd,
                        F_name = x.F_name
                    }).ToList()
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(List<VM_KBNMS027> listObj, string action)
        {
            try
            {
                await _masterRepo.IKBNMS027.Save(listObj, action);
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
