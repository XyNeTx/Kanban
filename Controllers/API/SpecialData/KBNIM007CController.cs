using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialData.ViewModel;
using KANBAN.Services;
using KANBAN.Services.Import.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNIM007CController : ControllerBase
    {
        private readonly IImportService _services;
        private readonly BearerClass _bearer;
        private readonly IHttpContextAccessor _http;

        public KBNIM007CController(IImportService services, BearerClass bearer, IHttpContextAccessor http)
        {
            _services = services;
            _bearer = bearer;
            _http = http;
        }

        [HttpGet]
        public async Task<IActionResult> GetPDS(string? DeliDateFrom, string? DeliDateTo)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = _services.KBNIM007C.GetPDS(DeliDateFrom, DeliDateTo);
                return Ok(new
                {
                    status = "200",
                    message = "Success",
                    response = "Data Found",
                    data = result
                });
            }
            catch (CustomHttpException ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(string? DeliDateFrom, string? DeliDateTo)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = _services.KBNIM007C.GetUser(DeliDateFrom, DeliDateTo);
                return Ok(new
                {
                    status = "200",
                    message = "Success",
                    response = "Data Found",
                    data = result
                });
            }
            catch (CustomHttpException ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListData(string? DeliDateFrom, string? DeliDateTo, string? PDSNo, string? User)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = _services.KBNIM007C.GetListData(DeliDateFrom, DeliDateTo, PDSNo, User);
                return Ok(new
                {
                    status = "200",
                    message = "Success",
                    response = "Data Found",
                    data = result
                });
            }
            catch (CustomHttpException ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update_Cycle()
        {
            try
            {
                await _bearer.CheckAuthorize();

                string ProcessDate = DateTime.ParseExact(_http.HttpContext.Request.Cookies["processDate"].ToString().Split("D")[0], "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("yyyyMMdd");

                await _services.KBNIM007C.Update_Cycle(ProcessDate);
                return Ok(new
                {
                    status = "200",
                    message = "Success",
                    response = "Data Updated"
                });
            }
            catch (CustomHttpException ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Confirm([FromBody] List<VM_CONF_KBNIM007C> listObj)
        {
            try
            {
                await _bearer.CheckAuthorize();
                string ProcessDate = "";

                if (_http.HttpContext.Request.Cookies["processDate"].ToString().Contains("D"))
                {
                    ProcessDate = _http.HttpContext.Request.Cookies["processDate"].ToString().Split("D")[0];
                }
                else
                {
                    ProcessDate = _http.HttpContext.Request.Cookies["processDate"].ToString().Split("N")[0];
                }

                string ProcessShift = _http.HttpContext.Request.Cookies["processDate"].ToString().Substring(10, 1);

                await _services.KBNIM007C.Confirm(listObj, ProcessDate, ProcessShift);
                return Ok(new
                {
                    status = "200",
                    message = "Success",
                    response = "Data Confirmed"
                });
            }
            catch (CustomHttpException ex)
            {
                throw;
            }
        }

    }
}
