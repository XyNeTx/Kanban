using HINOSystem.Libs;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class KBNOR360Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly ICKDService _CKDRepo;
        public KBNOR360Controller
            (
                BearerClass bearerClass,
                ICKDService CKDRepo
            )
        {
            _BearerClass = bearerClass;
            _CKDRepo = CKDRepo;
        }



    }
}
