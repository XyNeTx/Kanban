using HINOSystem.Libs;
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
                            F_Supplier_Code = x.F_supplier_cd + x.F_Plant_cd
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
                            F_Supplier_Code = x.F_Supplier_Code + x.F_Supplier_Plant
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
        public async Task<IActionResult> GetPartNo(string? SupplierCode, bool isNew)
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
                            .Where(x => x.F_Store_cd.StartsWith(_BearerClass.Plant))
                            .Select(x => new
                            {
                                F_Part_No = x.F_Part_no + "-" + x.F_Ruibetsu
                            }).DistinctBy(x => x.F_Part_No)
                            .OrderBy(x => x.F_Part_No).AsEnumerable()
                    });
                }
                else
                {
                    var data = await _masterRepo.IKBNMS019.GetPartInq(SupplierCode);

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = data
                            .Where(x => x.F_Supplier_Code + "-" + x.F_Supplier_Plant == SupplierCode
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
        public async Task<IActionResult> GetStoreCode(string? SupplierCode, string? PartNo, bool isNew)
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
                            .Where(x => x.F_Part_no + "-" + x.F_Ruibetsu == PartNo
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
                    var data = await _masterRepo.IKBNMS019.GetPartInq(SupplierCode);

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = data
                            .Where(x => x.F_Supplier_Code + "-" + x.F_Supplier_Plant == SupplierCode
                            && x.F_Plant == _BearerClass.Plant
                            && x.F_Part_No + "-" + x.F_Ruibetsu == PartNo)
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
        public async Task<IActionResult> GetKanbanNo(string? SupplierCode, string? PartNo, string? StoreCode, bool isNew)
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
                            .Where(x => x.F_Part_no + "-" + x.F_Ruibetsu == PartNo
                            && x.F_Store_cd == StoreCode)
                            .Select(x => new
                            {
                                F_Kanban_No = "0" + x.F_Sebango
                            }).DistinctBy(x => x.F_Kanban_No)
                            .OrderBy(x => x.F_Kanban_No).AsEnumerable()
                    });
                }
                else
                {
                    var data = await _masterRepo.IKBNMS019.GetPartInq(SupplierCode);

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = data
                            .Where(x => x.F_Supplier_Code + "-" + x.F_Supplier_Plant == SupplierCode
                            && x.F_Part_No + "-" + x.F_Ruibetsu == PartNo
                            && x.F_Store_CD == StoreCode)
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
        public async Task<IActionResult> GetPartName(string PartNo)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = _masterRepo.IKBNMS019.GetPartName(PartNo);

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
        public async Task<IActionResult> GetMaxTrip(string SupplierCode, string PartNo, string StoreCode, string KanbanNo)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = _masterRepo.IKBNMS019.GetMaxTrip(SupplierCode, PartNo, StoreCode, KanbanNo);

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
    }
}
