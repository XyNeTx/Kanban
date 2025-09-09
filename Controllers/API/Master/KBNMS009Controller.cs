using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class KBNMS009Controller : ControllerBase
    {
        private readonly IMasterRepo _masterRepo;
        private readonly BearerClass _bearerClass;

        public KBNMS009Controller(IMasterRepo masterRepo, BearerClass bearerClass)
        {
            _masterRepo = masterRepo;
            _bearerClass = bearerClass;
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplier()
        {
            try
            {
                

                var data = await _masterRepo.IKBNMS009.GetSupplier();

                return Ok(data.Select(x => new
                {
                    F_supplier_cd = x.F_supplier_cd + "-" +x.F_plant
                }).DistinctBy(x => x.F_supplier_cd).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SupplierClicked(string Supplier)
        {
            try
            {
                

                var data = await _masterRepo.IKBNMS009.SupplierClicked(Supplier);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(List<TB_MS_Print_Replace_KB> listObj)
        {
            try
            {
                

                await _masterRepo.IKBNMS009.Save(listObj);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been saved"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
