using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace KANBAN.Controllers.API.SpecialOrdering
{
    [Route("api/[controller]/[action]")]
    [ApiController][Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class KBNOR260Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearer;

        public KBNOR260Controller(ISpecialOrderingServices services, BearerClass bearer)
        {
            _services = services;
            _bearer = bearer;
        }

        [HttpGet]
        public async Task<IActionResult> GetPDSDataNoApprove(string fac)
        {
            try
            {
                
                var data = await _services.IKBNOR260.GetPDSDataNoApprove(fac);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        x.F_OrderNo,
                        x.F_PO_Customer,
                        F_Delivery_Date = DateTime.ParseExact(x.F_Delivery_Date,"yyyyMMdd", CultureInfo.CurrentCulture).ToString("dd/MM/yyyy"),
                        F_Supp_CD = x.F_Supplier_Code.Trim() + "-" + x.F_Supplier_Plant,
                        F_Status = x.F_Status == 'W' ? "Send Approve" : ""
                    })
                });
               
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetApproverList()
        {
            try
            {
                
                var data = _services.IKBNOR260.GetApproverList();

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
        public async Task<IActionResult> SendApprove(List<VM_TB_Rec_Header> listObj)
        {
            try
            {
                
                await _services.IKBNOR260.SendApprove(listObj);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Send Approve PDS No. Special Complete.",
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPDSWaitApprove(string? fac)
        {
            try
            {
                
                var data = await _services.IKBNOR260.GetPDSWaitApprove(fac);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        x.F_OrderNo,
                        x.F_PO_Customer,
                        F_Delivery_Date = DateTime.ParseExact(x.F_Delivery_Date, "yyyyMMdd", CultureInfo.CurrentCulture).ToString("dd/MM/yyyy"),
                        F_Supp_CD = x.F_Supplier_Code.Trim() + "-" + x.F_Supplier_Plant,
                        F_Status = "Wait Approve",
                        x.F_Approver
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
