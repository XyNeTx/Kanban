using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.SpecialOrdering
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR290Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearer;

        public KBNOR290Controller(ISpecialOrderingServices services, BearerClass bearer)
        {
            _services = services;
            _bearer = bearer;
        }

        [HttpGet]
        public async Task<IActionResult> ProdYMChanged(string YM)
        {
            try
            {
                var data = await _services.IKBNOR290.ProdYMChanged(YM);
                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        x.F_PO_Customer
                    })
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSuppCD(string? PO)
        {
            try
            {
                var data = await _services.IKBNOR290.GetSuppCD(PO);
                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        x.F_Supplier_CD
                    })
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
    }
}
