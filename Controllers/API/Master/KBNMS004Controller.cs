using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNMS004Controller : ControllerBase
    {
        private readonly IMasterRepo _masterRepo;
        private readonly BearerClass _bearerClass;

        public KBNMS004Controller(IMasterRepo masterRepo, BearerClass bearerClass)
        {
            _masterRepo = masterRepo;
            _bearerClass = bearerClass;
        }

        [HttpGet]
        public async Task<IActionResult> GetSelectList(string? kanban, string? storecd, string? partno, string? supplier, bool isNew)
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                var result = await _masterRepo.IKBNMS004.GetSelectList(kanban, storecd, partno, supplier, isNew);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been retrieved successfully",
                    data = result
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListData(string? kanban, string? storecd, string? partno, string? supplier, string? type)
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                var result = _masterRepo.IKBNMS004.GetListData(kanban, storecd, partno, supplier, type);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been retrieved successfully",
                    data = result
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SelectedSupplier(string supplier, string? storecd)
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                var result = await _masterRepo.IKBNMS004.SelectedSupplier(supplier, storecd);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been retrieved successfully",
                    data = result
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SelectedPartNo(string partno, string? supplier, string? kanban, string? storecd)
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                var result = await _masterRepo.IKBNMS004.SelectedPartNo(partno, supplier, kanban, storecd);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been retrieved successfully",
                    data = result
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(List<VM_SAVE_KBNMS004> listObj)
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                await _masterRepo.IKBNMS004.Save(listObj);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been saved successfully"
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }
    }
}
