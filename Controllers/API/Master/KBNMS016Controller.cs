using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class KBNMS016Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IMasterRepo _masterRepo;
        public KBNMS016Controller
            (
                BearerClass bearerClass,
                IMasterRepo masterRepo
            )
        {
            _BearerClass = bearerClass;
            _masterRepo = masterRepo;
        }

        [HttpGet]
        public async Task<IActionResult> List_Data(string? F_Supplier_Cd, string? F_Kanban_No, string? F_Part_No, string? F_Store_Cd, string? F_Supplier_Plant, string? F_Ruibetsu, string? F_Group)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _masterRepo.IKBNMS016.List_Data(F_Supplier_Cd, F_Kanban_No, F_Part_No, F_Store_Cd, F_Supplier_Plant, F_Ruibetsu, F_Group);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
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
