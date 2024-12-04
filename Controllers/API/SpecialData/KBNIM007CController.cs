using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.Import.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNIM007CController : ControllerBase
    {
        private readonly IImportService _services;
        private readonly BearerClass _bearer;

        public KBNIM007CController(IImportService services, BearerClass bearer)
        {
            _services = services;
            _bearer = bearer;
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
    }
}
