using HINOSystem.Libs;
using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNMS014Controller : ControllerBase
    {
        private readonly IMasterRepo _masterRepo;
        private readonly BearerClass _bearerClass;

        public KBNMS014Controller(IMasterRepo masterRepo, BearerClass bearerClass)
        {
            _masterRepo = masterRepo;
            _bearerClass = bearerClass;
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplierCode(bool isNew)
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                var data = await _masterRepo.IKBNMS014.GetSupplierCode(isNew);


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

        [HttpGet]
        public async Task<IActionResult> GetSupplierPlant(bool isNew, string? SupplierCode)
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                var data = await _masterRepo.IKBNMS014.GetSupplierPlant(isNew, SupplierCode);


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

        [HttpGet]
        public async Task<IActionResult> GetShortName(string SupplierCode, string SupplierPlant, bool isNew)
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                var data = await _masterRepo.IKBNMS014.GetShortName(SupplierCode, SupplierPlant, isNew);

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
        public async Task<IActionResult> Save(List<VM_Save_KBNMS014> listObj, string action)
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                await _masterRepo.IKBNMS014.Save(listObj, action);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Saved"
                });

            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListData(string? SupplierCode, string? SupplierPlant)
        {
            try
            {
                await _bearerClass.CheckAuthorize();

                var data = await _masterRepo.IKBNMS014.GetListData(SupplierCode, SupplierPlant);

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

    }
}
