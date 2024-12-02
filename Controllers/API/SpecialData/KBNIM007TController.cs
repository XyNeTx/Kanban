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
    public class KBNIM007TController : ControllerBase
    {
        private readonly IImportService _services;
        private readonly BearerClass _bearer;

        public KBNIM007TController(IImportService services, BearerClass bearer)
        {
            _services = services;
            _bearer = bearer;
        }

        public async Task<IActionResult> SetCalendar(string YM, string? ParentStoreCD, string TypeSpc)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = _services.KBNIM007T.SetCalendar(YM, ParentStoreCD, TypeSpc);

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

        public async Task<IActionResult> GetPO(string YM, string TypeSpc)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = await _services.KBNIM007T.GetPO(YM, TypeSpc);

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

        public async Task<IActionResult> GetParentStore(string YM, string? PO, bool isNew)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = await _services.KBNIM007T.GetParentStore(YM, PO, isNew);

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

        public async Task<IActionResult> GetParentPart(string YM, string PO, string ParentStoreCD, bool isNew)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = await _services.KBNIM007T.GetParentPart(YM, PO, ParentStoreCD, isNew);

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

        public async Task<IActionResult> GetParentPartDetail(string YM, string ParentStoreCD, string ParentPartNo, bool isNew)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = await _services.KBNIM007T.GetParentPartDetail(YM, ParentStoreCD, ParentPartNo, isNew);

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

        public async Task<IActionResult> GetComponentStore(string YM, string? PO, string? ParentPartNo, bool isNew)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = await _services.KBNIM007T.GetComponentStore(YM, PO, ParentPartNo, isNew);

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

        public async Task<IActionResult> GetComponentPartNo(string YM, string PO, string? CompStoreCD, string? ParentPartNo, bool isNew)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = await _services.KBNIM007T.GetComponentPartNo(YM, PO, CompStoreCD, ParentPartNo, isNew);

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

        public async Task<IActionResult> ComponentStoreSelected(string YM, string? PO, string? CompStoreCD, string? CompPartNo, string IssuedDate, bool isNew)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = await _services.KBNIM007T.ComponentStoreSelected(YM, PO, CompStoreCD, CompPartNo, IssuedDate, isNew);

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

        public async Task<IActionResult> ComponentPartSelected(string YM, string? CompStoreCD, string? CompPartNo, string? ParentPartNo, bool isNew)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = await _services.KBNIM007T.ComponentPartSelected(YM, CompStoreCD, CompPartNo, ParentPartNo, isNew);

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
