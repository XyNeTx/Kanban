using HINOSystem.Libs;
using KANBAN.Models.KB3.OtherCondition.Model;
using KANBAN.Models.KB3.OtherCondition.ViewModel;
using KANBAN.Services;
using KANBAN.Services.OtherCondition.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace KANBAN.Controllers.API.OtherCondition
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOC120Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IOtherConditionRepo _otherConditionRepo;

        public KBNOC120Controller(BearerClass BearerClass, IOtherConditionRepo otherConditionRepo)
        {
            _BearerClass = BearerClass;
            _otherConditionRepo = otherConditionRepo;
        }


        [HttpGet]
        public async Task<IActionResult> GetSupplier(string? StoreCD)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var result = await _otherConditionRepo.IKBNOC120.GetSupplier(StoreCD);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been retrieved",
                    data = result.Select(x => new
                    {
                        F_Supplier_Code = x.F_Supplier_Cd.Trim() + "-" + x.F_Supplier_Plant.Trim(),
                    }).Distinct()
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStore(string? SupplierCD)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var result = await _otherConditionRepo.IKBNOC120.GetStore(SupplierCD);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been retrieved",
                    data = result.Select(x => new
                    {
                        F_Store_cd = x.F_Store_cd
                    }).Distinct()
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] List<VM_SAVE_KBNOC120> ListVMObj, [FromQuery] string action)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                await _otherConditionRepo.IKBNOC120.Save(ListVMObj, action);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been saved"
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListData(string? SupplierCD, string? StoreCD)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var result = await _otherConditionRepo.IKBNOC120.GetListData(SupplierCD, StoreCD);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been retrieved",
                    data = result.Select(x => new
                    {
                        F_Supplier_Code = x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim(),
                        F_Store_cd = x.F_Store_CD,
                        x.F_Plant,
                        F_Delivery_Date = DateTime.ParseExact(x.F_Delivery_Date.Trim(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                        x.F_Delivery_Trip,
                        F_Keep_Order = x.F_Keep_Order == "0" ? "Cancel" : "Slide",
                        F_Slide_Date = DateTime.ParseExact(x.F_Slide_Date.Trim(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                        x.F_Slide_Trip
                    })
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
