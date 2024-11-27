using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialData.ViewModel;
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

        [HttpGet]
        public async Task<IActionResult> ListDataTable(string? PO, string? PartNo)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = _services.KBNIM007.ListDataTable(PO, PartNo);

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
        public async Task<IActionResult> ListCalendar(string YM, string PO, string StoreCD, string PartNo)
        {
            try
            {
                await _bearer.CheckAuthorize();

                var result = await _services.KBNIM007.ListCalendar(YM, PO, StoreCD, PartNo);

                var data = result.Select(x => new
                {
                    F_PDS_No = x.F_PDS_No,
                    F_Part_No = x.F_Part_Order + "-" + x.F_Ruibetsu_Order,
                    F_Delivery_Date = x.F_Delivery_Date.Substring(6,2) + "/" + x.F_Delivery_Date.Substring(4, 2) + "/" + x.F_Delivery_Date.Substring(0, 4),
                    F_Qty = x.F_Qty_Level1,
                    x.F_Remark,
                    x.F_Store_Order,
                    F_PDS_Issued_Date = x.F_PDS_Issued_Date.Substring(6, 2) + "/" + x.F_PDS_Issued_Date.Substring(4, 2) + "/" + x.F_PDS_Issued_Date.Substring(0, 4),
                }).Distinct().ToList();

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
        public async Task<IActionResult> Save(List<VM_Save_IM007> listObj, [FromQuery] string action)
        {
            try
            {
                await _bearer.CheckAuthorize();
                await _services.KBNIM007.Save(listObj, action);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data was saved successfully",
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

    }
}
