using HINOSystem.Libs;
using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNMS025Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IMasterRepo _MasterRepo;

        public KBNMS025Controller(
            BearerClass BearerClass,
            IMasterRepo MasterRepo
            )
        {
            _BearerClass = BearerClass;
            _MasterRepo = MasterRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogisticSupplier(string? TruckType)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                //Console.WriteLine(_BearerClass.Data["Table"]["Name"]);

                var data = await _MasterRepo.IKBNMS025.GetLogisticSupplier(TruckType);

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
        public async Task<IActionResult> GetTruckType(bool isNew, string? Logistic)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _MasterRepo.IKBNMS025.GetTruckType(isNew, Logistic);

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
        public async Task<IActionResult> TruckTypeSelected(bool isNew, string? Logistic, string? TruckType)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _MasterRepo.IKBNMS025.TruckTypeSelected(isNew, Logistic, TruckType);

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
        public async Task<IActionResult> GetListData(string? Logistic, string? TruckType)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _MasterRepo.IKBNMS025.GetListData(Logistic, TruckType);

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
        public async Task<IActionResult> Save([FromBody] List<VM_Save_KBNMS025> listObj, [FromQuery] string action)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                await _MasterRepo.IKBNMS025.Save(listObj, action);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Saved",
                    //data = listObj
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

    }
}
