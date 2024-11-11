using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KANBAN.Controllers.API.SpecialOrdering
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR297Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearer;

        public KBNOR297Controller(ISpecialOrderingServices services, BearerClass bearer)
        {
            _services = services;
            _bearer = bearer;
        }

        [HttpGet]
        public IActionResult GenReportExcel(string CustOrderNo)
        {
            try
            {
                var result = _services.IKBNOR297.GenReportExcel(CustOrderNo);
                return Ok(new { status = "200", response = "Success", message = "Data Found", data = result });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetCustomerPO(string? DeliYM)
        {
            try
            {
                var data = _services.IKBNOR297.GetCustomerPO(DeliYM);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode,ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAllCustomerPODetail(string CustOrderNo)
        {
            try
            {
                var result = _services.IKBNOR297.GetListAllPDSDetail(CustOrderNo);
                var data = JsonConvert.SerializeObject(result);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
    }
}
