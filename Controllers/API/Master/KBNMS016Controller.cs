using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController][Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/[controller]/[action]")]
    [Authorize]
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


        [HttpGet]
        public async Task<IActionResult> GetDropDown(string? F_Supplier_Cd, string? F_Kanban_No, string? F_Part_No, string? F_Store_Cd, string? F_Supplier_Plant, string? F_Ruibetsu, string? F_Group, string? action)
        {
            try
            {
                

                if (action == "new")
                {
                    var data = await _masterRepo.IKBNMS016.GetDropDownNew(F_Supplier_Cd, F_Kanban_No, F_Part_No, F_Store_Cd, F_Supplier_Plant, F_Ruibetsu);

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = new
                        {
                            supplier = data.Select(x => new
                            {
                                F_Supplier_Cd = x.F_supplier_cd?.Trim() + "-" + x.F_plant
                            }).DistinctBy(x => x.F_Supplier_Cd).OrderBy(x => x.F_Supplier_Cd).ToList(),

                            partno = data.Select(x => new
                            {
                                F_Part_No = x.F_Part_no.Trim() + "-" + x.F_Ruibetsu
                            }).DistinctBy(x => x.F_Part_No).OrderBy(x => x.F_Part_No).ToList(),

                            store = data.Select(x => new
                            {
                                F_Store_Cd = x.F_Store_cd
                            }).DistinctBy(x => x.F_Store_Cd).OrderBy(x => x.F_Store_Cd).ToList(),

                            kanban = data.Select(x => new
                            {
                                F_Kanban_No = "0" + x.F_Sebango?.Trim()
                            }).DistinctBy(x => x.F_Kanban_No).OrderBy(x => x.F_Kanban_No).ToList()

                        }
                    });

                }
                else
                {
                    var data = await _masterRepo.IKBNMS016.GetDropDownInq(F_Supplier_Cd, F_Kanban_No, F_Part_No, F_Store_Cd, F_Supplier_Plant, F_Ruibetsu, F_Group);

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = new
                        {

                            supplier = data.Select(x => new
                            {
                                F_Supplier_Cd = x.F_Supplier_Cd.Trim() + "-" + x.F_Supplier_Plant
                            }).DistinctBy(x => x.F_Supplier_Cd).OrderBy(x => x.F_Supplier_Cd).ToList(),

                            partno = data.Select(x => new
                            {
                                F_Part_No = x.F_Part_No.Trim() + "-" + x.F_Ruibetsu
                            }).DistinctBy(x => x.F_Part_No).OrderBy(x => x.F_Part_No).ToList(),

                            store = data.Select(x => new
                            {
                                F_Store_Cd = x.F_Store_Cd
                            }).DistinctBy(x => x.F_Store_Cd).OrderBy(x => x.F_Store_Cd).ToList(),

                            kanban = data.Select(x => new
                            {
                                F_Kanban_No = x.F_Kanban_No.Trim()
                            }).DistinctBy(x => x.F_Kanban_No).OrderBy(x => x.F_Kanban_No).ToList(),

                            group = data.Select(x => new
                            {
                                F_Group = x.F_Group.Trim()
                            }).DistinctBy(x => x.F_Group).OrderBy(x => x.F_Group).ToList(),
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplierDetail(string F_Supplier_Cd, string F_Supplier_Plant)
        {
            try
            {
                

                var data = await _masterRepo.IKBNMS016.GetSupplierDetail(F_Supplier_Cd, F_Supplier_Plant);

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
        public async Task<IActionResult> GetPartNoDetail(string? F_Part_No, string? F_Ruibetsu)
        {
            try
            {
                

                var data = await _masterRepo.IKBNMS016.GetPartNoDetail(F_Part_No, F_Ruibetsu);

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

        [HttpPost]
        public async Task<IActionResult> Save(List<TB_MS_PairOrder> listObj, string action)
        {
            try
            {
                

                await _masterRepo.IKBNMS016.Save(listObj, action);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Saved"
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
