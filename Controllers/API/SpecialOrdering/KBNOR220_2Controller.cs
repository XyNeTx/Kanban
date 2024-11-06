using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.SpecialOrdering
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR220_2Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearer;

        public KBNOR220_2Controller(ISpecialOrderingServices services, BearerClass bearer)
        {
            _services = services;
            _bearer = bearer;
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendar(string YM)
        {
            try
            {
                var data = await _services.IKBNOR220_2.GetCalendar(YM);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPOList()
        {
            try
            {
                var data = await _services.IKBNOR220_2.GetPOList();

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetSurvey(string YM)
        {
            try
            {
                var data = _services.IKBNOR220_2.GetSurvey(YM);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
        
        [HttpGet]
        public IActionResult GetSuppCD(string Survey, string? YM)
        {
            try
            {
                var data = _services.IKBNOR220_2.GetSuppCD(Survey, YM);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPartNo(string Survey,string SuppCD)
        {
            try
            {
                var data = await _services.IKBNOR220_2.GetPartNo(Survey, SuppCD);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPOQty(string Survey, string SuppCD, string PartNo)
        {
            try
            {
                var data = await _services.IKBNOR220_2.PartNoSelected(Survey, SuppCD, PartNo);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarQty(string YM, string Survey, string SuppCD, string PartNo)
        {
            try
            {
                var data = await _services.IKBNOR220_2.GetCalendarQty(Survey, SuppCD, YM, PartNo);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(List<VM_Post_KBNOR220_2> listObj)
        {
            try
            {
                await _services.IKBNOR220_2.Save(listObj);

                return Ok(new { status = "200", response = "Success", message = "Data Saved" });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplierName(string SuppCD, string SuppPlant)
        {
            try
            {
                var data = await _services.IKBNOR220_2.GetSupplierName(SuppCD, SuppPlant);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

    }
}
