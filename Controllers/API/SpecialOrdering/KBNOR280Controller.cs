using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KANBAN.Controllers.API.SpecialOrdering
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class KBNOR280Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearer;
        private readonly IHttpContextAccessor _http;

        public KBNOR280Controller(ISpecialOrderingServices services, BearerClass bearer,IHttpContextAccessor http)
        {
            _services = services;
            _bearer = bearer;
            _http = http;
        }

        [HttpGet]
        public async Task<IActionResult> GetPDSData(string DeliYM)
        {
            try
            {
                

                string FacCD = _http.HttpContext.User.FindFirst(ClaimTypes.Locality).Value switch
                {
                    "1" => "9Z",
                    "2" => "8Y",
                    "3" => "7Z",
                    _ => "9Z"
                };

                var data = _services.IKBNOR280.GetPDSData(FacCD, DeliYM);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(List<VM_Register_KBNOR280> listObj)
        {
            try
            {
                
                await _services.IKBNOR280.Register(listObj);

                return Ok(new { status = "200", response = "Success", message = "Registration PDS No. Complete." });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }


        //------------------------------------- Modal --------------------------------

        [HttpGet]
        public async Task<IActionResult> GetSupplier()
        {
            try
            {
                

                var data = await _services.IKBNOR280.GetSupplier();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        F_Supplier_Code = x.F_Supplier_Code + "-" + x.F_Supplier_Plant,
                    }).OrderBy(x => x.F_Supplier_Code)
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPO(string IssuedYM)
        {
            try
            {
                

                var data = await _services.IKBNOR280.GetPO(IssuedYM);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        F_PO_Customer = x.F_PO_Customer,
                    }).OrderBy(x => x.F_PO_Customer)
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPDS(string? POFrom, string? POTo)
        {
            try
            {
                

                var data = await _services.IKBNOR280.GetPDS(POFrom, POTo);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        F_PDS_No = x.F_OrderNo,
                    }).OrderBy(x => x.F_PDS_No)
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportData(string? PONoFrom, string? PONoTo, string? PDSNoFrom, string? PDSNoTo, string? SupplierFrom, string? SupplierTo,
                                                    string? DeliveryFrom, string? DeliveryTo)
        {
            try
            {
                
                var data = _services.IKBNOR280.ExportData(PONoFrom, PONoTo, PDSNoFrom, PDSNoTo, SupplierFrom, SupplierTo, DeliveryFrom, DeliveryTo);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
    }
}
