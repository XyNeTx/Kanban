using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class KBNMS029Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IMasterRepo _masterRepo;
        public KBNMS029Controller
            (
                BearerClass bearerClass,
                IMasterRepo masterRepo
            )
        {
            _BearerClass = bearerClass;
            _masterRepo = masterRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetListData(string? Plant, string? DockCode)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _masterRepo.IKBNMS029.GetListData(Plant, DockCode);

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
                throw;
            }
        }

    }
}
