using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KANBAN.Controllers.API.SpecialOrdering
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR261Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearer;

        public KBNOR261Controller(ISpecialOrderingServices services, BearerClass bearer)
        {
            _services = services;
            _bearer = bearer;
        }



    }
}
