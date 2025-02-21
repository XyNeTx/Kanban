using HINOSystem.Libs;
using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class KBNMS019Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IMasterRepo _masterRepo;
        public KBNMS019Controller
            (
                BearerClass bearerClass,
                IMasterRepo masterRepo
            )
        {
            _BearerClass = bearerClass;
            _masterRepo = masterRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplier(bool isNew)
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                if (isNew)
                {
                    var data = await _masterRepo.IKBNMS019.GetSupplierNew();

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = data.Select(x => new
                        {
                            F_Supplier_Code = x.F_supplier_cd.Trim() + "-" + x.F_Plant_cd
                        }).DistinctBy(x => x.F_Supplier_Code)
                        .OrderBy(x => x.F_Supplier_Code).AsEnumerable()
                    });
                }
                else
                {
                    var data = await _masterRepo.IKBNMS019.GetSupplierInq();

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = data.Select(x => new
                        {
                            F_Supplier_Code = x.F_Supplier_Code.Trim() + "-" + x.F_Supplier_Plant
                        }).DistinctBy(x => x.F_Supplier_Code)
                        .OrderBy(x => x.F_Supplier_Code).AsEnumerable()
                    });
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPartNo(string? F_Supplier, bool isNew)
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                if (isNew)
                {
                    var data = await _masterRepo.IKBNMS019.GetPartNew();

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = data
                            .Where(x => x.F_Store_cd.StartsWith(_BearerClass.Plant)
                            && x.F_supplier_cd + "-" + x.F_plant == F_Supplier)
                            .Select(x => new
                            {
                                F_Part_No = x.F_Part_no.Trim() + "-" + x.F_Ruibetsu
                            }).DistinctBy(x => x.F_Part_No)
                            .OrderBy(x => x.F_Part_No).AsEnumerable()
                    });
                }
                else
                {
                    var data = await _masterRepo.IKBNMS019.GetPartInq(F_Supplier);

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = data
                            .Where(x => x.F_Supplier_Code + "-" + x.F_Supplier_Plant == F_Supplier
                            && x.F_Plant == _BearerClass.Plant)
                            .Select(x => new
                            {
                                F_Part_No = x.F_Part_No + "-" + x.F_Ruibetsu
                            }).DistinctBy(x => x.F_Part_No)
                            .OrderBy(x => x.F_Part_No).AsEnumerable()
                    });
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreCD(string? F_Supplier, string? F_PartNo, bool isNew)
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                if (isNew)
                {
                    var data = await _masterRepo.IKBNMS019.GetPartNew();

                    if (!string.IsNullOrWhiteSpace(F_PartNo))
                    {
                        data = data.Where(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == F_PartNo).ToList();
                    }

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = data
                            .Where(x => x.F_supplier_cd + "-" + x.F_plant == F_Supplier
                            && x.F_Store_cd.StartsWith(_BearerClass.Plant))
                            .Select(x => new
                            {
                                F_Store_Code = x.F_Store_cd
                            }).DistinctBy(x => x.F_Store_Code)
                            .OrderBy(x => x.F_Store_Code).AsEnumerable()
                    });
                }
                else
                {
                    var data = await _masterRepo.IKBNMS019.GetPartInq(F_Supplier);

                    if (!string.IsNullOrWhiteSpace(F_PartNo))
                    {
                        data = data.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == F_PartNo).ToList();
                    }

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = data
                            .Where(x => x.F_Supplier_Code + "-" + x.F_Supplier_Plant == F_Supplier
                            && x.F_Plant == _BearerClass.Plant)
                            .Select(x => new
                            {
                                F_Store_Code = x.F_Store_CD
                            }).DistinctBy(x => x.F_Store_Code)
                            .OrderBy(x => x.F_Store_Code).AsEnumerable()
                    });
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetKanbanNo(string? F_Supplier, string? F_PartNo, string? F_StoreCD, bool isNew)
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                if (isNew)
                {
                    var data = await _masterRepo.IKBNMS019.GetPartNew();

                    if (!string.IsNullOrWhiteSpace(F_PartNo))
                    {
                        data = data.Where(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == F_PartNo).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(F_StoreCD))
                    {
                        data = data.Where(x => x.F_Store_cd == F_StoreCD).ToList();
                    }

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = data
                            .Select(x => new
                            {
                                F_Kanban_No = x.F_Sebango
                            }).DistinctBy(x => x.F_Kanban_No)
                            .OrderBy(x => x.F_Kanban_No).AsEnumerable()
                    });
                }
                else
                {
                    var data = await _masterRepo.IKBNMS019.GetPartInq(F_Supplier);

                    if (!string.IsNullOrWhiteSpace(F_PartNo))
                    {
                        data = data.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == F_PartNo).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(F_StoreCD))
                    {
                        data = data.Where(x => x.F_Store_CD == F_StoreCD).ToList();
                    }

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = data
                            .Where(x => x.F_Supplier_Code + "-" + x.F_Supplier_Plant == F_Supplier)
                            .Select(x => new
                            {
                                F_Kanban_No = x.F_Kanban_No
                            }).DistinctBy(x => x.F_Kanban_No)
                            .OrderBy(x => x.F_Kanban_No).AsEnumerable()
                    });
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPartName(string F_PartNo)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _masterRepo.IKBNMS019.GetPartName(F_PartNo);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.F_Part_nm
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMaxTrip(string F_Supplier, string F_PartNo, string F_StoreCD, string F_KanbanNo)
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                var data = await _masterRepo.IKBNMS019.GetMaxTrip(F_Supplier, F_PartNo, F_StoreCD, F_KanbanNo);

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
                else throw new CustomHttpException(500, null, ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListMaxArea(string? F_Supplier, string? F_PartNo, string? F_StoreCD, string? F_KanbanNo)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _masterRepo.IKBNMS019.GetListMaxArea(F_Supplier, F_PartNo, F_StoreCD, F_KanbanNo);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data,
                    maxTotal = data.Sum(x => x.F_Max_Trip)
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(List<VM_KBNMS019> listObj, string action)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                await _masterRepo.IKBNMS019.Save(listObj, action);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Saved",
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

    }
}
