using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.Import.Interface;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNIM007Controller : ControllerBase
    {
        private readonly IImportService _services;
        private readonly BearerClass _bearer;

        public KBNIM007Controller(IImportService services, BearerClass bearer)
        {
            _services = services;
            _bearer = bearer;
        }

        [HttpGet]
        public async Task<IActionResult> GetPO(string YM)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = await _services.KBNIM007.GetPO(YM);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = result.Select(x => x.F_PDS_No).Distinct().ToList()
                });

            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SetCalendar(string YM, string StoreCD)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = _services.KBNIM007.SetCalendar(YM, StoreCD);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = result
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreCD(string YM, string? PO, bool isNew)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = _services.KBNIM007.GetStoreCD(YM, PO, isNew);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = result
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPartNo(string YM, string? PO, string? StoreCD, bool isNew)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = _services.KBNIM007.GetPartNo(YM, PO, StoreCD, isNew);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = result
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> PartNoSelected(string YM, string PO, string StoreCD, string PartNo, bool isNew)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = _services.KBNIM007.PartNoSelected(YM, PO, StoreCD, PartNo, isNew);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = result
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

    }
}
