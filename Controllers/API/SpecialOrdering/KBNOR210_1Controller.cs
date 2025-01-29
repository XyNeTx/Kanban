using HINOSystem.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KANBAN.Controllers.API.SpecialOrder
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR210_1Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _BearerClass;
        public KBNOR210_1Controller(
                       ISpecialOrderingServices specialOrderingServices,
                                  BearerClass bearerClass)
        {
            _services = specialOrderingServices;
            _BearerClass = bearerClass;
        }

        [HttpGet]
        public async Task<IActionResult> LoadDataChangeDelivery(string? PDSNo, string? SuppCd, string? PartNo, bool? chkDeli, string? DeliFrom, string? DeliTo)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = await _services.IKBNOR210_1.LoadDataChangeDelivery(PDSNo, SuppCd, PartNo, chkDeli, DeliFrom, DeliTo);

                if (data.Count == 0)
                {
                    throw new CustomHttpException(404, "Data not found");
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = new
                    {
                        customer = data.Select(x => x.F_PDS_No).Distinct().OrderBy(x => x),
                        supplier = data.Select(x => x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim()).Distinct().OrderBy(x => x),
                        partNo = data.Select(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu).Distinct().OrderBy(x => x),

                    }
                });
            }

            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetSupplierName(string SuppCd)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message

                    });
                }

                var data = await _services.IKBNOR210_1.GetSupplierName(SuppCd);

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
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPartName(string PartNo)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = await _services.IKBNOR210_1.GetPartName(PartNo);
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
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPOMergeData(string? PDSNo, string? SuppCd, string? PartNo, bool? chkDeli, string? DeliFrom, string? DeliTo)
        {
            try
            {

                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = await _services.IKBNOR210_1.GetPOMergeData(PDSNo, SuppCd, PartNo, chkDeli, DeliFrom, DeliTo);

                if (data.Rows.Count == 0)
                {
                    throw new CustomHttpException(404, "Data not found");
                }

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
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(List<VM_Save_KBNOR210_1> obj)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }


                foreach (var item in obj)
                {
                    await _services.IKBNOR210_1.Save(item);
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data saved"
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult LoadGridData(string OrderNo)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = _services.IKBNOR210_1.LoadGridData(OrderNo);

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
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveKBNOR210_1_STC_3(List<VM_Save_KBNOR210_1_STC_3> listObj)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                await _services.IKBNOR210_1.SaveKBNOR210_1_STC_3(listObj);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data saved"
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetDataKBNOR210_1_STC_3_1(string F_OrderNo, string F_Part_No, string F_Store_Cd, string F_Supplier, string? Delivery, int F_Use_StockQty)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = await _services.IKBNOR210_1.GetDataKBNOR210_1_STC_3_1(F_OrderNo, F_Part_No, F_Store_Cd, F_Supplier, Delivery, F_Use_StockQty);

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
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveKBNOR210_1_STC_3_1(List<VM_Save_KBNOR210_1_STC_3_1> listObj)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                await _services.IKBNOR210_1.SaveKBNOR210_1_STC_3_1(listObj);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data saved"
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }


        [HttpGet]
        public IActionResult STC_1_ListDatatogrid(string? SupplierCD = "", string? PartNo = "", string? StoreCode = "", string? StockDate = "", string? UpdateBy = "", string? UpdateDate = "")
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = _services.IKBNOR210_1.ListDatatogrid(SupplierCD, PartNo, StoreCode, StockDate, UpdateBy, UpdateDate);

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
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult STC_1_GetSupplierCode(bool isNew, string StockDate)
        {

            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = _services.IKBNOR210_1.STC_1_GetSupplierCode(isNew, StockDate);

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
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }

        }

        [HttpGet]
        public IActionResult STC_1_GetPartNo(bool isNew, string StockDate, string? Supplier_Code)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = _services.IKBNOR210_1.STC_1_GetPartNo(isNew, StockDate, Supplier_Code);

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
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult STC_1_GetStore(bool isNew, string StockDate, string? Supplier_Code, string? Part_No)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = _services.IKBNOR210_1.STC_1_GetStore(isNew, StockDate, Supplier_Code, Part_No);

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
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult STC_1_GetSupplierName(string Supplier_Code)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message

                    });
                }

                var data = _services.IKBNOR210_1.STC_1_GetSupplierName(Supplier_Code);

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
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult STC_1_GetPartName(string Supplier_Code, string Part_No)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new { status = _BearerClass.Status, response = _BearerClass.Response, message = _BearerClass.Message });
                }

                var data = _services.IKBNOR210_1.STC_1_GetPartName(Supplier_Code, Part_No);

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
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult STC_1_GetKB_Qty(string Supplier_Code, string Part_No, string Stock_Date, string Store_Code)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = _services.IKBNOR210_1.STC_1_GetKB_Qty(Supplier_Code, Part_No, Stock_Date, Store_Code);
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
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> STC_1_Save(VM_Post_KBNOR210_STC_1 obj)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                await _services.IKBNOR210_1.STC_1_Save(obj);

                return Ok(new { status = "200", response = "Success", message = "Data saved" });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> STC_1_Import(List<VM_Import_KBNOR210_STC_1> listObj)
        {

            try
            {

                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                await _services.IKBNOR210_1.STC_1_Import(listObj);

                return Ok(new { status = "200", response = "Success", message = "Data imported" });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult STC_2_GetSupplier(string Type, bool chkFlg, bool chkFlgDT, string DateFrom, string DateTo)
        {

            try
            {

                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                var data = _services.IKBNOR210_1.STC_2_LoadSupplier(Type, chkFlg, chkFlgDT, DateFrom, DateTo);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }

        }

        [HttpGet]
        public IActionResult STC_2_GetPartNo(string Type, bool chkFlg, bool chkFlgDT, string DateFrom, string DateTo, string SuppF, string SuppT)
        {
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new { status = _BearerClass.Status, response = _BearerClass.Response, message = _BearerClass.Message });
                }

                var data = _services.IKBNOR210_1.STC_2_LoadPartNo(Type, chkFlg, chkFlgDT, DateFrom, DateTo, SuppF, SuppT);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Del_LoadSupplier(string DeliYM)
        {
            try
            {

                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new { status = _BearerClass.Status, response = _BearerClass.Response, message = _BearerClass.Message });
                }

                var data = _services.IKBNOR210_1.Del_LoadSupplier(DeliYM);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Del_LoadPartNo(string DeliYM, string Supplier)
        {
            try
            {

                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new { status = _BearerClass.Status, response = _BearerClass.Response, message = _BearerClass.Message });
                }

                var data = _services.IKBNOR210_1.Del_LoadPartNo(DeliYM, Supplier);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    var customEx = ex as CustomHttpException;
                    throw new CustomHttpException(customEx.StatusCode, customEx.Message);
                }
                throw new CustomHttpException(500, ex.Message);
            }
        }
    }
}