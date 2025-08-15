using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.CKD_Ordering;
using KANBAN.Services;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class KBNOR361Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly ICKDService _CKDRepo;
        public KBNOR361Controller
            (
                BearerClass bearerClass,
                ICKDService CKDRepo
            )
        {
            _BearerClass = bearerClass;
            _CKDRepo = CKDRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetDataList(string? Supplier_Code, string? Kanban_No, string? Store_Code, string? Part_No, bool IsNew)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _CKDRepo.IKBNOR361_Repo.GetDataList(Supplier_Code, Kanban_No, Store_Code, Part_No, IsNew);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Get List Data Complete",
                    data = data
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplier(string Supplier_Code, string? Store_Code)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _CKDRepo.IKBNOR361_Repo.GetSupplier(Supplier_Code, Store_Code);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Get Supplier Data Complete",
                    data = data
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPartNo(string Part_No, string? Supplier_Code, string? Kanban_No, string? Store_Code)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _CKDRepo.IKBNOR361_Repo.GetPartNo(Part_No, Supplier_Code, Kanban_No, Store_Code);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Get Part Data Complete",
                    data = data
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetList(string? Supplier_Code, string? Kanban_No, string? Store_Code, string? Part_No)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _CKDRepo.IKBNOR361_Repo.GetList(Supplier_Code, Kanban_No, Store_Code, Part_No);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Get List Data Complete",
                    data = data
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFlgClearModule([FromBody] List<VM_KBNOR361_Save> listObj)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                await _CKDRepo.IKBNOR361_Repo.UpdateFlgClearModule(listObj);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Update Flag Clear Module Success"
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
