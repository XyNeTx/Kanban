using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.SpecialOrder
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class KBNOR210_2Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _BearerClass;
        public KBNOR210_2Controller(ISpecialOrderingServices specialOrderingServices,
            BearerClass bearerClass)
        {
            _services = specialOrderingServices;
            _BearerClass = bearerClass;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerPO(string? DeliDT, string? OrderNo)
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

            try
            {

                var result = await _services.IKBNOR210_2.GetCustomerPO(DeliDT, OrderNo);
                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = result.DistinctBy(x => new
                    {
                        x.F_Delivery_Date,
                        x.F_PDS_No_New,
                        x.F_CusOrderType_CD,
                    }).OrderBy(x => x.F_Delivery_Date).Select(x=> new
                    {
                        F_ProdYM = x.F_Delivery_Date.Substring(0,6),
                        F_PDS_No = x.F_PDS_No_New,
                        F_Delivery_Date = x.F_Delivery_Date.Substring(6,2) + "/" + x.F_Delivery_Date.Substring(4,2) + "/" + x.F_Delivery_Date.Substring(0,4),
                        F_Delivery_Full = x.F_Delivery_Date,
                        x.F_CusOrderType_CD
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new{
                    status = "500",
                    response = "Internal Server Error",
                    message = ex.Message
                });
            }

        }

        [HttpPost]
        public async Task<IActionResult> Merge (List<VM_Merge_KBNOR210_2> listObj)
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
            try
            {

                if (await _services.IKBNOR210_2.Merge(listObj) == false)
                {
                    return StatusCode(500, new
                    {
                        status = "500",
                        response = "Internal Server Error",
                        message = "Failed to Merge Customer Order No"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Success to Merge Customer Order No"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = ex.Message
                });
            }

        }
    }
}
