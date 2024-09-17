using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.SpecialOrdering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.SpecialOrder
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR210_3Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _BearerClass;
        public KBNOR210_3Controller(
            ISpecialOrderingServices specialOrderingServices,
            BearerClass bearerClass)
        {
            _services = specialOrderingServices;
            _BearerClass = bearerClass;
        }

        [HttpGet]
        public async Task<IActionResult> LoadOrderNo()
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

                var data = await _services.IKBNOR210_3.LoadOrderNo();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x=>x.F_PDS_No).Distinct().OrderBy(x => x)
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
        public async Task<IActionResult> LoadCustomerPO(string NewCusPO)
        {
            try
            {

                if(_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = await _services.IKBNOR210_3.LoadCustomPo(NewCusPO);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.OrderBy(x => x.F_Delivery_Date).ThenBy(x => x.F_PDS_No).Select(x => new
                    {
                        F_ProdYM = x.F_Delivery_Date.Substring(0, 6),
                        F_PDS_No = x.F_PDS_No_New,
                        F_Delivery_Date = x.F_Delivery_Date.Substring(6, 2) + "/" + x.F_Delivery_Date.Substring(4, 2) + "/" + x.F_Delivery_Date.Substring(0, 4),
                    }).Distinct()
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


        [HttpPost]
        public async Task<IActionResult> Unmerge(List<VM_Merge_KBNOR210_2> listObj) // ใช้เพราะไม่อยากสร้าง model ใหม่้เฉยๆ
        {

            try
            {
                if(_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });

                }

               await _services.IKBNOR210_3.Unmerge(listObj);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Unmerge Data Complete."
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
