using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HINOSystem.Controllers.API.Master
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNMS008Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _IEmail;

        public KBNMS008Controller
        (
            BearerClass bearerClass,
            PPM3Context ppm3Context,
            PPMInvenContext ppmInvenContext,
            KB3Context kb3Context,
            FillDataTable fillDataTable,
            SerilogLibs log
        )
        {
            _BearerClass = bearerClass;
            _PPM3Context = ppm3Context;
            _PPMInvenContext = ppmInvenContext;
            _KB3Context = kb3Context;
            _FillDT = fillDataTable;
            _log = log;
        }

        public string yyyyMMdd = DateTime.Now.ToString("yyyyMMdd");

        [HttpGet]
        public IActionResult GetSupplier()
        {
            try
            {
                _BearerClass.Authentication(Request);
                if(_BearerClass.Status == 401)
                {
                    return Unauthorized(new
                    {
                        status = "401",
                        response = "Unauthorized",
                        title = "Error",
                        message = "Please Login then try again",
                    });
                }

                var dbObj = _KB3Context.TB_MS_PartOrder.Where(x=> x.F_Start_Date.CompareTo(yyyyMMdd) <= 0 
                    && x.F_End_Date.CompareTo(yyyyMMdd) >= 0 && x.F_Type_Order.Trim() == "Pattern"
                    && x.F_Store_Code.StartsWith(_BearerClass.Plant))
                    .ToList();

                if(dbObj.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data not found",
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    title = "Success",
                    message = "Data found",
                    data = dbObj.Select(x=> new
                    {
                        F_Supplier_Code = x.F_Supplier_Cd.Trim() + "-" + x.F_Supplier_Plant,
                    }).DistinctBy(x=>x.F_Supplier_Code)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    message = "Unexpected error occurred",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult GetSupplierDetail(string supplier,string? store,string? storeTo)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return Unauthorized(new
                    {
                        status = "401",
                        response = "Unauthorized",
                        title = "Error",
                        message = "Please Login then try again",
                    });
                };

                var name = _PPM3Context.T_Supplier_MS.Where(x => x.F_supplier_cd == supplier.Substring(0, 4) &&
                    x.F_Plant_cd == supplier[5] && x.F_TC_Str.CompareTo(yyyyMMdd) <= 0 && x.F_TC_End.CompareTo(yyyyMMdd) >= 0
                    && x.F_Store_cd.StartsWith(_BearerClass.Plant)).AsNoTracking().AsQueryable();

                var cycle = _KB3Context.TB_MS_DeliveryTime.AsNoTracking().Where(x => x.F_Supplier_Code == supplier.Substring(0, 4) &&
                        x.F_Supplier_Plant == supplier[5].ToString() && x.F_Start_Date.CompareTo(yyyyMMdd) <= 0 && x.F_End_Date.CompareTo(yyyyMMdd) >= 0
                        && x.F_Plant.StartsWith(_BearerClass.Plant)).AsQueryable();

                if (!string.IsNullOrWhiteSpace(store) && (!string.IsNullOrWhiteSpace(storeTo)))
                {
                    name.Where(x=> x.F_Store_cd.CompareTo(store) >= 0 && x.F_Store_cd.CompareTo(storeTo) <= 0);
                }

                if (cycle == null || name == null)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data not found",
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    title = "Success",
                    message = "Data found",
                    data = name.Select(x => new
                    {
                        F_Supplier_Name = "(" + x.F_short_name.Trim() + ")" + x.F_name.Trim(),
                        x.F_Safety_Stk
                    }).FirstOrDefault(),
                    cycle = cycle.Select(x => x.F_Cycle).Distinct().FirstOrDefault()
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    message = "Unexpected error occurred",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult GetKanban(string? supplier,string? store, string? partNo)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return Unauthorized(new
                    {
                        status = "401",
                        response = "Unauthorized",
                        title = "Error",
                        message = "Please Login then try again",
                    });
                };

                var kanban = _KB3Context.TB_MS_PartOrder.Where(x => x.F_Start_Date.CompareTo(yyyyMMdd) <= 0
                && x.F_End_Date.CompareTo(yyyyMMdd) >= 0 && x.F_Store_Code.StartsWith(_BearerClass.Plant)
                && x.F_Type_Order.Trim() == "Pattern").OrderBy(x => x.F_Kanban_No).AsQueryable();

                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    kanban = kanban.Where(x => x.F_Supplier_Cd.Trim() == supplier.Substring(0, 4) 
                        && x.F_Supplier_Plant == supplier[5]);
                }
                if (!string.IsNullOrWhiteSpace(store))
                {
                    kanban = kanban.Where(x => x.F_Store_Code == store);
                }
                if (!string.IsNullOrWhiteSpace(partNo))
                {
                    kanban = kanban.Where(x => x.F_Part_No.Trim() == partNo.Substring(0,10) 
                        && x.F_Ruibetsu == partNo.Substring(11,2));
                }

                if(kanban == null)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data not found",
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data found",
                    data = kanban.Select(x => x.F_Kanban_No).Distinct()
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Unexpected error occurred",
                    Error = ex.Message
                });
            }
        }
    }
}
