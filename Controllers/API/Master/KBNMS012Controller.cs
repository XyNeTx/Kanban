using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class KBNMS012Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IMasterRepo _masterRepo;
        public KBNMS012Controller
            (
                BearerClass bearerClass,
                IMasterRepo masterRepo
            )
        {
            _BearerClass = bearerClass;
            _masterRepo = masterRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetDropDown(string? F_Supplier_Code, string? F_Kanban_No, string? F_Store_Cd, string? F_Part_No)
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                var data = await _masterRepo.IKBNMS012.GetDropDown(F_Supplier_Code, F_Kanban_No, F_Store_Cd, F_Part_No);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = new
                    {
                        supcode = data.Select(x => new
                        {
                            F_Supplier_Code = x.F_Supplier_Cd.Trim() + "-" + x.F_Supplier_Plant
                        }).DistinctBy(x => x.F_Supplier_Code).OrderBy(x => x.F_Supplier_Code).AsEnumerable(),
                        kanban = data.Select(x => new
                        {
                            F_Kanban_No = x.F_Kanban_No.Trim()
                        }).DistinctBy(x => x.F_Kanban_No).OrderBy(x => x.F_Kanban_No).AsEnumerable(),
                        store = data.Select(x => new
                        {
                            F_Store_Cd = x.F_Store_Code
                        }).DistinctBy(x => x.F_Store_Cd).OrderBy(x => x.F_Store_Cd).AsEnumerable(),
                        partno = data.Select(x => new
                        {
                            F_Part_No = x.F_Part_No.Trim() + "-" + x.F_Ruibetsu
                        }).DistinctBy(x => x.F_Part_No).OrderBy(x => x.F_Part_No).AsEnumerable()
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
        public async Task<IActionResult> GetSupplierDetail(string F_Supplier_Code)
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                var data = await _masterRepo.IKBNMS012.GetSupplierDetail(F_Supplier_Code);

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

        [HttpGet]
        public async Task<IActionResult> Search(string? F_Supplier_Code, string? F_Kanban_No, string? F_Store_Cd, string? F_Part_No)
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                var data = await _masterRepo.IKBNMS012.Search(F_Supplier_Code, F_Kanban_No, F_Store_Cd, F_Part_No);

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

        [HttpGet]
        public async Task<IActionResult> FindDetail(string F_Supplier_Code, string? F_Kanban_No, string? F_Store_Cd, string? F_Part_No)
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                var data = await _masterRepo.IKBNMS012.FindDetail(F_Supplier_Code, F_Kanban_No, F_Store_Cd, F_Part_No);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = JsonConvert.SerializeObject(data)
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
