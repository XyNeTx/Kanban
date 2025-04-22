using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class KBNOR321Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly ICKDService _CKDRepo;
        public KBNOR321Controller
            (
                BearerClass bearerClass,
                ICKDService CKDRepo
            )
        {
            _BearerClass = bearerClass;
            _CKDRepo = CKDRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetDropDownData(string? F_Supplier_Code, string? F_Store_Code)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _CKDRepo.IKBNOR321.GetDropDownData(F_Supplier_Code, F_Store_Code);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Success",
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
        public async Task<IActionResult> Get_All_Data(string action, string F_Supplier_Code, string? F_KanbanFrom, string? F_KanbanTo, string? F_StoreFrom, string? F_StoreTo, string? F_PartFrom, string? F_PartTo)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _CKDRepo.IKBNOR321.Get_All_Data(action, F_Supplier_Code, F_KanbanFrom, F_KanbanTo, F_StoreFrom, F_StoreTo, F_PartFrom, F_PartTo);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Success",
                    data = new
                    {
                        DT_DeliveryDate = data[0],
                        DT_Date = data[1],
                        DT_Period = data[2],
                        DT_PartControl = data[3],
                        DT_Header = data[4],
                        DT_Detail = data[5],
                        DT_Volume = data[6],
                        DT_AdjustOrder_Trip = data[7],
                        DT_Actual_Receive = data[8],
                    }
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Onload(string _loginDate)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                await _CKDRepo.IKBNOR321.Onload(_loginDate);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Success"
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetBL(string strDate, string Row_Num, int intRow)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _CKDRepo.IKBNOR321.GetBL(strDate, Row_Num, intRow);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Success",
                    data = data
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
