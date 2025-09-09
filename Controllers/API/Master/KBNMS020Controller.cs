using HINOSystem.Libs;
using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/[controller]/[action]")]
    public class KBNMS020Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IMasterRepo _masterRepo;
        public KBNMS020Controller
            (
                BearerClass bearerClass,
                IMasterRepo masterRepo
            )
        {
            _BearerClass = bearerClass;
            _masterRepo = masterRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetDropDownList(string? F_Supplier, string? F_KanbanNo, string? F_StoreCD, string? F_PartNo)
        {
            try
            {
                

                var data = await _masterRepo.IKBNMS020.GetDropDownList(F_Supplier, F_KanbanNo, F_StoreCD, F_PartNo);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = new
                    {
                        F_Supplier = data.Select(x => new
                        {
                            F_Supplier = x.F_supplier_cd + "-" + x.F_plant
                        }).DistinctBy(x => x.F_Supplier).OrderBy(x => x.F_Supplier),

                        F_KanbanNo = data.Select(x => new
                        {
                            F_KanbanNo = "0" + x.F_Sebango
                        }).DistinctBy(x => x.F_KanbanNo).OrderBy(x => x.F_KanbanNo),

                        F_StoreCD = data.Select(x => new
                        {
                            F_StoreCD = x.F_Store_cd
                        }).DistinctBy(x => x.F_StoreCD).OrderBy(x => x.F_StoreCD),

                        F_PartNo = data.Select(x => new
                        {
                            F_PartNo = x.F_Part_no.Trim() + "-" + x.F_Ruibetsu
                        }).DistinctBy(x => x.F_PartNo).OrderBy(x => x.F_PartNo),
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
        public async Task<IActionResult> GetSupplierName(string F_Supplier, string? F_StoreCD)
        {
            try
            {
                

                var data = await _masterRepo.IKBNMS020.GetSupplierName(F_Supplier, F_StoreCD);

                string cycle = data.F_Cycle_A.Trim().Length == 1 ? "0" + data.F_Cycle_A.Trim() + "-" : data.F_Cycle_A.Trim() + "-";
                cycle += data.F_Cycle_B.Trim().Length == 1 ? "0" + data.F_Cycle_B.Trim() + "-" : data.F_Cycle_B.Trim() + "-";
                cycle += data.F_Cycle_C.Trim().Length == 1 ? "0" + data.F_Cycle_C.Trim() : data.F_Cycle_C.Trim();

                string name = data.F_name.Trim();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = new
                    {
                        cycle = cycle,
                        name = name,
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
        public async Task<IActionResult> GetQtyBox(string? F_Supplier, string? F_KanbanNo, string? F_StoreCD, string? F_PartNo)
        {
            try
            {
                

                var data = await _masterRepo.IKBNMS020.GetDropDownList(F_Supplier, F_KanbanNo, F_StoreCD, F_PartNo);

                var obj = data.FirstOrDefault();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = obj?.F_qty_box ?? 0
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPartName(string? F_Supplier, string? F_KanbanNo, string? F_StoreCD, string? F_PartNo)
        {
            try
            {
                

                var data = await _masterRepo.IKBNMS020.GetDropDownList(F_Supplier, F_KanbanNo, F_StoreCD, F_PartNo);

                var obj = data.FirstOrDefault();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = obj?.F_Part_nm?.Trim() ?? ""
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(List<VM_KBNMS020> listObj, string action)
        {
            try
            {
                

                await _masterRepo.IKBNMS020.Save(listObj, action);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Saved Successfully",
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListData(string? F_Supplier, string? F_KanbanNo, string? F_StoreCD, string? F_PartNo)
        {
            try
            {
                

                var data = await _masterRepo.IKBNMS020.GetListData(F_Supplier, F_KanbanNo, F_StoreCD, F_PartNo);

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
