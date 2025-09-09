using HINOSystem.Libs;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/[controller]/[action]")]
    public class KBNMS005Controller : ControllerBase
    {
        private readonly IMasterRepo _MasterRepo;
        private readonly BearerClass _BearerClass;
        public KBNMS005Controller(
            BearerClass bearerClass,
            IMasterRepo MasterRepo

            )
        {
            _BearerClass = bearerClass;
            _MasterRepo = MasterRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetListOption(string? Sup, string? Part, string? PartT, string? Store,string? StoreT)
        {
            

            var result = await _MasterRepo.IKBNMS005_Repo.GetListOption(Sup, Part,PartT, Store,StoreT);

            return Ok(new
            {
                status = "200",
                response = "Success",
                message = "Data Found",
                data = JsonConvert.SerializeObject(result, Formatting.Indented)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllData(string inpSup, string? inpKB, string? inpKBT, string? inpStore, string? inpStoreT, string? inpPart, string? inpPartT, string? inpDate, string? inpDateT,bool chkOK, bool chkNo, bool chkPS)
        {
            

            var data = await _MasterRepo.IKBNMS005_Repo.GetAllData(inpSup, inpKB, inpKBT, inpStore, inpStoreT, inpPart, inpPartT, inpDate, inpDateT,chkOK,chkNo,chkPS);

            return Ok(new
            {
                status = "200",
                response = "Success",
                message = "Data Found",
                data = data
            });
        }

    }
}
