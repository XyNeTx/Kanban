using HINOSystem.Libs;
using KANBAN.Services.SpecialOrdering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.SpecialOrder
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR210_1Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _BearerClass;
        public KBNOR210_1Controller(
                       ISpecialOrderingServices specialOrderingServices,
                                  BearerClass bearerClass)
        {
            _services = specialOrderingServices;
            _BearerClass = bearerClass;
        }

        [HttpGet]
        public async Task<IActionResult> LoadDataChangeDelivery(string? PDSNo, string? SuppCd, string? PartNo, bool? chkDeli, string? DeliFrom, string? DeliTo)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = await _services.IKBNOR210_1.LoadDataChangeDelivery(PDSNo, SuppCd, PartNo, chkDeli, DeliFrom, DeliTo);

                if(data.Count == 0)
                {
                    return StatusCode(404, new
                    {
                        status = "404",
                        response = "Not Found",
                        message = "Data not found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = new
                    {
                        customer = data.Select(x => x.F_PDS_No).Distinct().OrderBy(x => x),
                        supplier = data.Select(x => x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim()).Distinct().OrderBy(x => x),
                        partNo = data.Select(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu).Distinct().OrderBy(x => x),

                    }
                });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Error",
                    message = ex.Message
                });
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetSupplierName (string SuppCd)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message

                    });
                }

                var data = await _services.IKBNOR210_1.GetSupplierName(SuppCd);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data
                });

            }

            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Error",
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPartName(string PartNo)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = await _services.IKBNOR210_1.GetPartName(PartNo);
                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Error",
                    message = ex.Message
                });
            }
        }
    }
}
