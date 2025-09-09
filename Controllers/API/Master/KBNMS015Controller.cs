using HINOSystem.Libs;
using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    public class KBNMS015Controller : ControllerBase
    {

        private readonly BearerClass _BearerClass;
        private readonly IMasterRepo _masterRepo;

        public KBNMS015Controller(
            BearerClass bearerClass,
            IMasterRepo masterRepo
            )
        {
            _BearerClass = bearerClass;
            _masterRepo = masterRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetDropDown(string? Supplier, string? Kanban, string? StoreCode, string? PartNo, bool isNew)
        {
            try
            {
                

                if (isNew)
                {
                    var data = await _masterRepo.IKBNMS015.GetDropDownNew(Supplier, Kanban, StoreCode, PartNo);

                    List<string> list = new List<string>
                    {
                        JsonConvert.SerializeObject(data.Select(x => new
                        {
                            F_supplier_cd = x.F_supplier_cd + "-" + x.F_plant
                        }).Distinct().OrderBy(x=>x.F_supplier_cd).ToList()),

                        JsonConvert.SerializeObject(data.Select(x => new
                        {
                            F_Kanban_No = x.F_Sebango == null ? "" : "0" + x.F_Sebango.Trim()
                        }).Distinct().OrderBy(x=>x.F_Kanban_No).ToList()),

                        JsonConvert.SerializeObject(data.Select(x => new
                        {
                            F_Part_No = x.F_Part_no.Trim() + "-" + x.F_Ruibetsu
                        }).Distinct().OrderBy(x=>x.F_Part_No).ToList()),

                        JsonConvert.SerializeObject(data.Select(x => new
                        {
                            x.F_Store_cd
                        }).Distinct().OrderBy(x=>x.F_Store_cd).ToList()),
                    };


                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = list
                    });
                }
                else
                {
                    var data = await _masterRepo.IKBNMS015.GetDropDownInq(Supplier, Kanban, StoreCode, PartNo);

                    List<string> list = new List<string>
                    {
                        JsonConvert.SerializeObject(data.Select(x => new
                        {
                            F_supplier_cd = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant
                        }).Distinct().OrderBy(x=>x.F_supplier_cd).ToList()),

                        JsonConvert.SerializeObject(data.Select(x => new
                        {
                            F_Kanban_No = x.F_Kanban_No.Trim()
                        }).Distinct().OrderBy(x=>x.F_Kanban_No).ToList()),

                        JsonConvert.SerializeObject(data.Select(x => new
                        {
                            F_Part_No = x.F_Part_No.Trim() + "-" + x.F_Ruibetsu
                        }).Distinct().OrderBy(x=>x.F_Part_No).ToList()),

                        JsonConvert.SerializeObject(data.Select(x => new
                        {
                            F_Store_cd = x.F_Store_Code
                        }).Distinct().OrderBy(x=>x.F_Store_cd).ToList()),
                    };

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = list
                    });
                }
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SupplierChanged(string SupplierCode, string? StoreCode)
        {
            try
            {
                

                var data = await _masterRepo.IKBNMS015.SupplierChanged(SupplierCode, StoreCode);

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
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> PartNoSelected(string PartNo, string? SupplierCode, string? Kanban, string? StoreCode, bool IsNew)
        {
            try
            {
                

                if (IsNew)
                {
                    var data = await _masterRepo.IKBNMS015.PartNoSelectedNew(PartNo, SupplierCode, Kanban, StoreCode);
                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = data
                    });
                }
                else
                {
                    var data = await _masterRepo.IKBNMS015.PartNoSelectedInq(PartNo, SupplierCode, Kanban, StoreCode);
                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data Found",
                        data = data
                    });
                }
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] List<VM_KBNMS015> listObj, [FromQuery] string action)
        {
            try
            {
                
                await _masterRepo.IKBNMS015.Save(listObj, action);
                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Saved"
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListData(string? PartNo, string? SupplierCode, string? Kanban, string? StoreCode)
        {
            try
            {
                
                var data = await _masterRepo.IKBNMS015.GetListData(PartNo, SupplierCode, Kanban, StoreCode);
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
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

    }
}
