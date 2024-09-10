using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNMS006Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _IEmail;

        public KBNMS006Controller
        (
            BearerClass bearerClass,
            PPM3Context ppm3Context,
            PPMInvenContext ppmInvenContext,
            KB3Context kb3Context,
            FillDataTable fillDataTable,
            SerilogLibs log,
            IEmailService IEmail
        )
        {
            _BearerClass = bearerClass;
            _PPM3Context = ppm3Context;
            _PPMInvenContext = ppmInvenContext;
            _KB3Context = kb3Context;
            _FillDT = fillDataTable;
            _log = log;
            _IEmail = IEmail;
        }

        public string now = DateTime.Now.ToString("yyyyMMdd");
        public static string plant = "";

        [HttpGet]
        public async Task<IActionResult> GetSupplier()
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
                }

                plant = Request.Cookies["plantCode"].ToString();

                var supplier = _KB3Context.TB_MS_PartOrder.AsNoTracking()
                    .Where(x => x.F_Start_Date.CompareTo(now) <= 0 &&
                    x.F_End_Date.CompareTo(now) >= 0 &&
                    x.F_Store_Code.StartsWith(plant))
                    .Select(x => new
                    {
                        F_Supplier_Code = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant
                    }).AsEnumerable();

                if (supplier.Count() == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data Not Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = supplier.DistinctBy(x => x.F_Supplier_Code).OrderBy(x => x.F_Supplier_Code).AsEnumerable()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetKanban(string? F_Supplier_Code)
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
                }

                string plant = Request.Cookies["plantCode"].ToString();

                var kanban = _KB3Context.TB_MS_PartOrder.AsNoTracking()
                    .Where(x => x.F_Start_Date.CompareTo(now) <= 0 &&
                    x.F_End_Date.CompareTo(now) >= 0 &&
                    x.F_Store_Code.StartsWith(plant)).AsEnumerable();

                if (!string.IsNullOrEmpty(F_Supplier_Code))
                {
                    kanban = kanban.Where(x => x.F_Supplier_Cd == F_Supplier_Code.Substring(0, 4) &&
                        x.F_Supplier_Plant == F_Supplier_Code.Substring(5, 1)).AsEnumerable();
                }


                if (kanban.Count() == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data Not Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = kanban.Select(x => x.F_Kanban_No).DistinctBy(x => x).OrderBy(x => x).AsEnumerable()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStore(string? F_Part_No)
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
                }

                string plant = Request.Cookies["plantCode"].ToString();

                var store = _KB3Context.TB_MS_PartOrder.AsNoTracking()
                    .Where(x => x.F_Start_Date.CompareTo(now) <= 0 &&
                    x.F_End_Date.CompareTo(now) >= 0 &&
                    x.F_Store_Code.StartsWith(plant)).AsEnumerable();

                if (!string.IsNullOrEmpty(F_Part_No))
                {
                    store = store.Where(x => x.F_Part_No == F_Part_No.Substring(0, 10) &&
                        x.F_Ruibetsu == F_Part_No.Substring(11, 2)).AsEnumerable();
                }

                if (store.Count() == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data Not Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = store.Select(x => x.F_Store_Code).DistinctBy(x => x).OrderBy(x => x).AsEnumerable()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPartNo(string? F_Store_Code)
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
                }

                string plant = Request.Cookies["plantCode"].ToString();

                var partno = _KB3Context.TB_MS_PartOrder.AsNoTracking()
                    .Where(x => x.F_Start_Date.CompareTo(now) <= 0 &&
                    x.F_End_Date.CompareTo(now) >= 0 &&
                    x.F_Store_Code.StartsWith(plant)).AsEnumerable();

                if (!string.IsNullOrEmpty(F_Store_Code))
                {
                    partno = partno.Where(x => x.F_Store_Code == F_Store_Code).AsEnumerable();
                }


                if (partno.Count() == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data Not Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = partno.Select(x => new
                    {
                        F_Part_No = x.F_Part_No + "-" + x.F_Ruibetsu
                    }).DistinctBy(x => x.F_Part_No).OrderBy(x => x.F_Part_No).AsEnumerable()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplierDetail(string F_Supplier_Code, string? F_Store_Code)
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
                }

                var supplierDetail = _PPM3Context.T_Supplier_MS.AsNoTracking()
                    .Where(x => x.F_supplier_cd == F_Supplier_Code.Substring(0, 4) &&
                    x.F_Plant_cd == F_Supplier_Code.Substring(5, 1)[0] &&
                    x.F_TC_Str.CompareTo(now) <= 0 &&
                    x.F_TC_End.CompareTo(now) >= 0 &&
                    x.F_Store_cd.StartsWith(plant))
                    .OrderByDescending(x => x.F_TC_Str).AsEnumerable();

                if (!string.IsNullOrWhiteSpace(F_Store_Code))
                {
                    supplierDetail = supplierDetail.Where(x => x.F_Store_cd == F_Store_Code).AsEnumerable();
                }

                if (supplierDetail.Count() == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Supplier Detail Not Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Supplier Detail Found",
                    data = supplierDetail.Select(x => new
                    {
                        F_Supplier_Name = "(" + x.F_short_name?.Trim() + ") " + x.F_name?.Trim(),
                        F_Cycle = ("00" + x.F_Cycle_A).Substring(("00" + x.F_Cycle_A).Length - 2, 2)
                        + "-" + ("00" + x.F_Cycle_B).Substring(("00" + x.F_Cycle_B).Length - 2, 2)
                        + "-" + ("00" + x.F_Cycle_C).Substring(("00" + x.F_Cycle_C).Length - 2, 2)
                    }).FirstOrDefault()
                });

            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetKanbanDetail(string F_Kanban_No, string? F_Supplier_Code, string? F_Store_Code, string? F_Part_No)
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
                }

                var kanbanDetail = _PPM3Context.T_Construction.AsNoTracking()
                    .Where(x => x.F_Sebango == F_Kanban_No.Substring(1, 3) &&
                    x.F_TC_Str.CompareTo(now) <= 0 &&
                    x.F_TC_End.CompareTo(now) >= 0 &&
                    x.F_Store_cd.StartsWith(plant)).AsEnumerable();

                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    kanbanDetail = kanbanDetail.Where(x => x.F_supplier_cd == F_Supplier_Code.Substring(0, 4) &&
                        x.F_plant == F_Supplier_Code.Substring(5, 1)[0]);
                }
                if (!string.IsNullOrWhiteSpace(F_Store_Code))
                {
                    kanbanDetail = kanbanDetail.Where(x => x.F_Store_cd == F_Store_Code);
                }
                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    kanbanDetail = kanbanDetail.Where(x => x.F_Part_no == F_Part_No.Substring(0, 10) &&
                        x.F_Ruibetsu == F_Part_No.Substring(11, 2));
                }

                if (kanbanDetail.Count() == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Kanban Detail Not Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Kanban Detail Found",
                    data = kanbanDetail.Select(x => new
                    {
                        F_qty_box = x.F_qty_box?.ToString().Trim(),
                    }).DistinctBy(x => x.F_qty_box).AsEnumerable()
                });

            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? F_Kanban_No, string? F_Supplier_Code, string? F_Store_Code, string? F_Part_No)
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
                }

                if (string.IsNullOrWhiteSpace(F_Kanban_No) && string.IsNullOrWhiteSpace(F_Supplier_Code) && string.IsNullOrWhiteSpace(F_Store_Code) && string.IsNullOrWhiteSpace(F_Part_No))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Please Select Data to Search Data"
                    });
                }

                if (!string.IsNullOrWhiteSpace(F_Supplier_Code) && string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Please Select Kanban No Before"
                    });
                }

                if (string.IsNullOrWhiteSpace(F_Supplier_Code) && !string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Please Select Supplier Code Before"
                    });
                }

                if (!string.IsNullOrWhiteSpace(F_Store_Code) && string.IsNullOrWhiteSpace(F_Part_No))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Please Select Part No Before"
                    });
                }

                if (string.IsNullOrWhiteSpace(F_Store_Code) && !string.IsNullOrWhiteSpace(F_Part_No))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Please Select Store Code Before"
                    });
                }

                // ------------------------------ Get All Detail -------------------------
                var _dt = await GetAllDetail(F_Kanban_No, F_Supplier_Code, F_Store_Code, F_Part_No);

                if (_dt.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "All Detail Not Found"
                    });
                }

                else if (_dt.Rows.Count > 1)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Data More Than One Please Select And Try Again"
                    });
                }

                // ------------------------------ Change Box Qty -------------------------

                var chgQty = _KB3Context.TB_Kanban_Chg_Qty.AsNoTracking()
                    .Where(x => x.F_Plant == plant).AsEnumerable();

                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    chgQty = chgQty.Where(x => x.F_Part_No == F_Part_No.Substring(0, 10) &&
                        x.F_Ruibetsu == F_Part_No.Substring(11, 2)).AsEnumerable();
                }
                if (!string.IsNullOrWhiteSpace(F_Store_Code))
                {
                    chgQty = chgQty.Where(x => x.F_Store_Code == F_Store_Code).AsEnumerable();
                }
                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    chgQty = chgQty.Where(x => x.F_Supplier_Code == F_Supplier_Code.Substring(0, 4) &&
                        x.F_Supplier_Plant == F_Supplier_Code.Substring(5, 1)).AsEnumerable();
                }
                if (!string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    chgQty = chgQty.Where(x => x.F_Kanban_No == F_Kanban_No).AsEnumerable();
                }

                // ------------------------------ STOP -------------------------

                var stop = _KB3Context.TB_Kanban_Stop.AsNoTracking()
                    .Where(x => x.F_Plant == plant).AsEnumerable();

                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    stop = stop.Where(x => x.F_Part_No == F_Part_No.Substring(0, 10) &&
                           x.F_Ruibetsu == F_Part_No.Substring(11, 2)).AsEnumerable();
                }
                if (!string.IsNullOrWhiteSpace(F_Store_Code))
                {
                    stop = stop.Where(x => x.F_Store_Code == F_Store_Code).AsEnumerable();
                }
                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    stop = stop.Where(x => x.F_Supplier_Code == F_Supplier_Code.Substring(0, 4) &&
                            x.F_Supplier_Plant == F_Supplier_Code.Substring(5, 1)).AsEnumerable();
                }
                if (!string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    stop = stop.Where(x => x.F_Kanban_No == F_Kanban_No).AsEnumerable();
                }

                // ------------------------------ CUT -------------------------
                var cut = _KB3Context.TB_Kanban_Cut.AsNoTracking()
                    .Where(x => x.F_Plant == plant).AsEnumerable();

                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    cut = cut.Where(x => x.F_Part_No == F_Part_No.Substring(0, 10) &&
                               x.F_Ruibetsu == F_Part_No.Substring(11, 2)).AsEnumerable();
                }
                if (!string.IsNullOrWhiteSpace(F_Store_Code))
                {
                    cut = cut.Where(x => x.F_Store_Code == F_Store_Code).AsEnumerable();
                }
                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    cut = cut.Where(x => x.F_Supplier_Code == F_Supplier_Code.Substring(0, 4) &&
                                x.F_Supplier_Plant == F_Supplier_Code.Substring(5, 1)).AsEnumerable();
                }
                if (!string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    cut = cut.Where(x => x.F_Kanban_No == F_Kanban_No).AsEnumerable();
                }

                // ------------------------------ Master -------------------------

                var master = _KB3Context.TB_MS_Kanban.AsNoTracking()
                    .Where(x => x.F_Plant == plant).AsEnumerable();

                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    master = master.Where(x => x.F_Part_No == F_Part_No.Substring(0, 10) &&
                           x.F_Ruibetsu == F_Part_No.Substring(11, 2)).AsEnumerable();
                }
                if (!string.IsNullOrWhiteSpace(F_Store_Code))
                {
                    master = master.Where(x => x.F_Store_Code == F_Store_Code).AsEnumerable();
                }
                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    master = master.Where(x => x.F_Supplier_Code == F_Supplier_Code.Substring(0, 4) &&
                             x.F_Supplier_Plant == F_Supplier_Code.Substring(5, 1)).AsEnumerable();
                }
                if (!string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    master = master.Where(x => x.F_Kanban_No == F_Kanban_No).AsEnumerable();
                }


                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = new
                    {
                        allData = JsonConvert.SerializeObject(_dt),
                        chgQty = chgQty.Select(x => new
                        {
                            F_Status = x.F_Status == null ? "0" : x.F_Status,
                            F_Delivery_Date = x.F_Delivery_Date.Trim(),
                            F_Delivery_Trip = x.F_Delivery_Trip.Trim(),
                            x.F_New_Qty,
                            F_Start_Date = x.F_Start_Date?.Trim(),
                            F_Start_Shift = x.F_Start_Shift?.Trim(),
                            x.F_Create_Date,
                            F_Create_By = x.F_Create_By?.Trim(),
                            x.F_Update_Date,
                            F_Update_By = x.F_Update_By?.Trim(),
                        }).FirstOrDefault(),
                        stop = stop.Select(x => new
                        {
                            F_Status = x.F_Status == null ? "0" : x.F_Status,
                            F_Delivery_Date = x.F_Delivery_Date.Trim(),
                            F_Delivery_Trip = x.F_Delivery_Trip.Trim(),
                            F_Start_Date = x.F_Start_Date?.Trim(),
                            F_Start_Shift = x.F_Start_Shift?.Trim(),
                            x.F_Create_Date,
                            F_Create_By = x.F_Create_By?.Trim(),
                            x.F_Update_Date,
                            F_Update_By = x.F_Update_By?.Trim(),
                        }).FirstOrDefault(),
                        cut = cut.Select(x => new
                        {
                            F_Status = x.F_Status == null ? "0" : x.F_Status,
                            F_Delivery_Date = x.F_Delivery_Date.Trim(),
                            F_Delivery_Trip = x.F_Delivery_Trip.Trim(),
                            x.F_KB_Cut,
                            x.F_KB_Cut_RN,
                            x.F_KB_Remain,
                            F_Start_Date = x.F_Start_Date.Trim(),
                            F_Start_Shift = x.F_Start_Shift.Trim(),
                            F_Create_By = x.F_Create_By.Trim(),
                            F_Update_By = x.F_Update_By.Trim(),
                            x.F_Create_Date,
                            x.F_Update_Date,
                        }).FirstOrDefault(),
                        master = master.Select(x => new
                        {
                            F_Address = x.F_Address?.Trim(),
                            F_Supply_Code = x.F_Supply_Code.Trim(),
                        }).FirstOrDefault()
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<DataTable> GetAllDetail(string? F_Kanban_No, string? F_Supplier_Code, string? F_Store_Code, string? F_Part_No)
        {
            try
            {
                string accessDB = plant == "3" ? "HMMTA-PPM" : "HMMT-PPM";

                string _sql = $"SELECT TOP 1 RTRIM(P.F_Supplier_Cd)+'-'+RTRIM(P.F_Supplier_Plant) AS F_supplier_cd" +
                        $",RTRIM(S.F_name) AS F_name, RTRIM(P.F_Part_No)+'-'+RTRIM(P.F_Ruibetsu) AS F_Part_no " +
                        $",P.F_Store_Code AS F_Store_cd, RIGHT('0000'+ CONVERT(VARCHAR,P.F_Kanban_No),4) AS F_Kanban " +
                        $",RIGHT('0000'+ CONVERT(VARCHAR,C.F_Sebango),4) AS F_Kanban" +
                        $",RTRIM(C.F_Part_nm) AS F_Part_nm, C.F_qty_box " +
                        $",RIGHT('00'+ CONVERT(VARCHAR,S.F_Cycle_A),2) +'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_B),2) " +
                        $"+'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_C),2) AS F_cycle " +
                        $"FROM TB_MS_PartOrder P LEFT JOIN (Select * From [{accessDB}].[PPMDB].[dbo].T_Supplier_ms " +
                        $"Where F_TC_Str <= convert(char(8),getdate(),112) AND F_TC_End >= convert(char(8),getdate(),112) " +
                        $"AND F_store_cd LIKE '{plant}%' ) S ON S.F_supplier_cd = P.F_Supplier_Cd collate Thai_CI_AS " +
                        $"AND S.F_Plant_cd = P.F_Supplier_Plant collate Thai_CI_AS " +
                        $"AND S.F_Store_cd = P.F_Store_Code collate Thai_CI_AS " +
                        $"LEFT JOIN (Select * From [{accessDB}].[PPMDB].[dbo].T_Construction " +
                        $"Where F_Local_Str <= convert(char(8),getdate(),112) " +
                        $"AND F_Local_End >= convert(char(8),getdate(),112) " +
                        $"AND F_store_cd LIKE '{plant}%') C " +
                        $"ON P.F_Supplier_Cd = C.F_supplier_cd collate Thai_CI_AS  " +
                        $"AND P.F_Supplier_Plant = C.F_plant collate Thai_CI_AS " +
                        $"AND P.F_Store_Code = C.F_Store_cd collate Thai_CI_AS " +
                        $"AND P.F_Part_No = C.F_Part_no collate Thai_CI_AS " +
                        $"AND P.F_Ruibetsu = C.F_Ruibetsu collate Thai_CI_AS " +
                        $"WHERE P.F_Start_Date <= convert(char(8),getdate(),112) " +
                        $"AND P.F_End_Date >= convert(char(8),getdate(),112) " +
                        $"AND P.F_Plant = '{plant}'";

                if (!string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    _sql += $" AND P.F_Kanban_No = '{F_Kanban_No}'";
                }
                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    _sql += $" AND P.F_Supplier_Cd = '{F_Supplier_Code.Substring(0, 4)}' " +
                        $"AND P.F_Supplier_Plant = '{F_Supplier_Code.Substring(5, 1)[0]}'";
                }
                if (!string.IsNullOrWhiteSpace(F_Store_Code))
                {
                    _sql += $" AND P.F_Store_Code = '{F_Store_Code}'";
                }
                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    _sql += $" AND P.F_Part_No = '{F_Part_No.Substring(0, 10)}' " +
                        $"AND P.F_Ruibetsu = '{F_Part_No.Substring(11, 2)}'";
                }

                var _dt = _FillDT.ExecuteSQL(_sql);

                return _dt;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog(ex.Message, _BearerClass.UserCode, _BearerClass.Device);
                return null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveKanban(TB_MS_Kanban obj)
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
                }

                var dbObj = await _KB3Context.TB_MS_Kanban.FirstOrDefaultAsync(x => x.F_Plant == obj.F_Plant
                && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant
                && x.F_Store_Code == obj.F_Store_Code && x.F_Kanban_No == obj.F_Kanban_No
                && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu);

                if(dbObj != null)
                {
                    dbObj.F_Supply_Code = obj.F_Supply_Code;
                    dbObj.F_Address = obj.F_Address;
                    dbObj.F_Update_By = _BearerClass.UserCode;
                    dbObj.F_Update_Date = DateTime.Now;

                    _KB3Context.TB_MS_Kanban.Update(dbObj);
                }
                else
                {
                    obj.F_Update_By = _BearerClass.UserCode;
                    obj.F_Update_Date = DateTime.Now;
                    obj.F_Create_By = _BearerClass.UserCode;
                    obj.F_Create_Date = DateTime.Now;
                    _KB3Context.TB_MS_Kanban.Add(obj);
                }
                _log.WriteLogMsg($"KBNMS006 : SaveKanban | obj : {JsonConvert.SerializeObject(dbObj)}");
                await _KB3Context.SaveChangesAsync();

                return Ok(new
                {
                    status = "200",
                    resposne = "OK",
                    title = "Success",
                    message = "Save Data Success"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500 ,new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveBoxQtyChg (TB_Kanban_Chg_Qty obj)
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
                }

                string supplier = obj.F_Supplier_Code.Substring(0, 4) + "-" + obj.F_Supplier_Plant;

                if (await IsProcessDatePast(obj.F_Delivery_Date, int.Parse(obj.F_Delivery_Trip), supplier, obj.F_Kanban_No))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Delivery Date นี้ Process ไปเรียบร้อยแล้ว"
                    });
                }
                if(await IsProcessDateHoliday(obj.F_Delivery_Date, obj.F_Store_Code))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Delivery Date นี้เป็นวันหยุด"
                    });
                }
                
                if ( int.Parse(obj.F_Delivery_Trip) > await GetCycleTime(obj.F_Delivery_Date, int.Parse(obj.F_Delivery_Trip), supplier))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Delivery Trip มากกว่า Cycle Time"
                    });
                }


                var dbObj = await _KB3Context.TB_Kanban_Chg_Qty.FirstOrDefaultAsync(x => x.F_Plant == obj.F_Plant
                && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant
                && x.F_Store_Code == obj.F_Store_Code && x.F_Kanban_No == obj.F_Kanban_No
                && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu);

                if(dbObj != null)
                {
                    _KB3Context.TB_Kanban_Chg_Qty.Remove(dbObj);
                    await _KB3Context.SaveChangesAsync();
                    dbObj.F_Status = "0";
                    dbObj.F_Delivery_Date = obj.F_Delivery_Date;
                    dbObj.F_Delivery_Trip = obj.F_Delivery_Trip;
                    dbObj.F_New_Qty = obj.F_New_Qty;
                    dbObj.F_Update_Date = DateTime.Now;
                    dbObj.F_Update_By = _BearerClass.UserCode;
                    _KB3Context.TB_Kanban_Chg_Qty.Add(dbObj);
                }
                else
                {
                    obj.F_Status = "0";
                    obj.F_Create_By = _BearerClass.UserCode;
                    obj.F_Create_Date = DateTime.Now;
                    obj.F_Update_By = _BearerClass.UserCode;
                    obj.F_Update_Date = DateTime.Now;
                    _KB3Context.TB_Kanban_Chg_Qty.Add(obj);
                }

                _log.WriteLogMsg($"KBNMS006 : SaveBoxQtyChg | obj : {JsonConvert.SerializeObject(dbObj)}");

                await _KB3Context.SaveChangesAsync();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Save Data Success"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> StartBoxQtyChg (TB_Kanban_Chg_Qty obj)
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
                }

                var dbObj = await _KB3Context.TB_Kanban_Chg_Qty.FirstOrDefaultAsync(x => x.F_Plant == obj.F_Plant
                && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant
                && x.F_Store_Code == obj.F_Store_Code && x.F_Kanban_No == obj.F_Kanban_No
                && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu);

                if (dbObj == null)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data Not Found"
                    });
                }

                dbObj.F_Status = "1";
                dbObj.F_Update_Date = DateTime.Now;
                dbObj.F_Update_By = _BearerClass.UserCode;

                _KB3Context.TB_Kanban_Chg_Qty.Update(dbObj);
                await _KB3Context.SaveChangesAsync();
                _log.WriteLogMsg($"KBNMS006 : StartBoxQtyChg | obj : {JsonConvert.SerializeObject(dbObj)}");

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Start Success"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> StopBoxQtyChg(TB_Kanban_Chg_Qty obj)
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
                }

                var dbObj = await _KB3Context.TB_Kanban_Chg_Qty.FirstOrDefaultAsync(x => x.F_Plant == obj.F_Plant
                && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant
                && x.F_Store_Code == obj.F_Store_Code && x.F_Kanban_No == obj.F_Kanban_No
                && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu);

                if (dbObj == null)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data Not Found"
                    });
                }

                dbObj.F_Status = "0";
                dbObj.F_Update_Date = DateTime.Now;
                dbObj.F_Update_By = _BearerClass.UserCode;

                _KB3Context.TB_Kanban_Chg_Qty.Update(dbObj);
                await _KB3Context.SaveChangesAsync();
                _log.WriteLogMsg($"KBNMS006 : StopBoxQtyChg | obj : {JsonConvert.SerializeObject(dbObj)}");

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Stop Success"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveKBStop(TB_Kanban_Stop obj)
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
                }

                string supplier = obj.F_Supplier_Code.Substring(0, 4) + "-" + obj.F_Supplier_Plant;

                if (await IsProcessDatePast(obj.F_Delivery_Date, int.Parse(obj.F_Delivery_Trip), supplier, obj.F_Kanban_No))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Delivery Date นี้ Process ไปเรียบร้อยแล้ว"
                    });
                }
                
                if (await IsProcessDateHoliday(obj.F_Delivery_Date, obj.F_Store_Code))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Delivery Date นี้เป็นวันหยุด"
                    });
                }

                if (int.Parse(obj.F_Delivery_Trip) > await GetCycleTime(obj.F_Delivery_Date, int.Parse(obj.F_Delivery_Trip), supplier))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Delivery Trip มากกว่า Cycle Time"
                    });
                }

                var dbObj = await _KB3Context.TB_Kanban_Stop.FirstOrDefaultAsync(x => x.F_Plant == obj.F_Plant
                               && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant
                                              && x.F_Store_Code == obj.F_Store_Code && x.F_Kanban_No == obj.F_Kanban_No
                                                             && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu);

                if (dbObj != null)
                {
                    _KB3Context.TB_Kanban_Stop.Remove(dbObj);
                    await _KB3Context.SaveChangesAsync();
                    dbObj.F_Status = "0";
                    dbObj.F_Delivery_Date = obj.F_Delivery_Date;
                    dbObj.F_Delivery_Trip = obj.F_Delivery_Trip;
                    dbObj.F_Update_Date = DateTime.Now;
                    dbObj.F_Update_By = _BearerClass.UserCode;
                    _KB3Context.TB_Kanban_Stop.Add(dbObj);
                }
                else
                {
                    obj.F_Status = "0";
                    obj.F_Create_By = _BearerClass.UserCode;
                    obj.F_Create_Date = DateTime.Now;
                    obj.F_Update_By = _BearerClass.UserCode;
                    obj.F_Update_Date = DateTime.Now;
                    _KB3Context.TB_Kanban_Stop.Add(obj);
                }

                _log.WriteLogMsg($"KBNMS006 : SaveKBStop | obj : {JsonConvert.SerializeObject(dbObj)}");

                await _KB3Context.SaveChangesAsync();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Save Data Success"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> StartKBStop(TB_Kanban_Stop obj)
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

                var dbObj = await _KB3Context.TB_Kanban_Stop.FirstOrDefaultAsync(x => x.F_Plant == obj.F_Plant
                && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant
                && x.F_Store_Code == obj.F_Store_Code && x.F_Kanban_No == obj.F_Kanban_No
                && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu);

                if (dbObj == null)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data Not Found"
                    });
                }

                dbObj.F_Status = "1";
                dbObj.F_Update_Date = DateTime.Now;
                dbObj.F_Update_By = _BearerClass.UserCode;

                _KB3Context.TB_Kanban_Stop.Update(dbObj);
                await _KB3Context.SaveChangesAsync();

                _log.WriteLogMsg($"KBNMS006 : StartKBStop | obj : {JsonConvert.SerializeObject(dbObj)}");

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Start KB Stop Success"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> StopKBStop(TB_Kanban_Stop obj)
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
                }

                var dbObj = await _KB3Context.TB_Kanban_Stop.FirstOrDefaultAsync(x => x.F_Plant == obj.F_Plant
                               && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant
                                              && x.F_Store_Code == obj.F_Store_Code && x.F_Kanban_No == obj.F_Kanban_No
                                                             && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu);

                if (dbObj == null)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data Not Found"
                    });
                }

                dbObj.F_Status = "0";
                dbObj.F_Update_Date = DateTime.Now;
                dbObj.F_Update_By = _BearerClass.UserCode;

                _KB3Context.TB_Kanban_Stop.Update(dbObj);
                await _KB3Context.SaveChangesAsync();

                _log.WriteLogMsg($"KBNMS006 : StopKBStop | obj : {JsonConvert.SerializeObject(dbObj)}");

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Stop KB Stop Success"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveKBCut(TB_Kanban_Cut obj)
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
                }

                string supplier = obj.F_Supplier_Code.Substring(0, 4) + "-" + obj.F_Supplier_Plant;
                
                if(await IsProcessDatePast(obj.F_Delivery_Date, int.Parse(obj.F_Delivery_Trip), supplier, obj.F_Kanban_No))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Delivery Date นี้ Process ไปเรียบร้อยแล้ว"
                    });
                }
                if (await IsProcessDateHoliday(obj.F_Delivery_Date, obj.F_Store_Code))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Delivery Date นี้เป็นวันหยุด"
                    });
                }
                if(int.Parse(obj.F_Delivery_Trip) > await GetCycleTime(obj.F_Delivery_Date, int.Parse(obj.F_Delivery_Trip), supplier))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Delivery Trip มากกว่า Cycle Time"
                    });
                }

                var dbObj = await _KB3Context.TB_Kanban_Cut.FirstOrDefaultAsync(x => x.F_Plant == obj.F_Plant
                               && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant
                                              && x.F_Store_Code == obj.F_Store_Code && x.F_Kanban_No == obj.F_Kanban_No
                                                             && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu);

                if (dbObj != null)
                {
                    _KB3Context.TB_Kanban_Cut.Remove(dbObj);
                    await _KB3Context.SaveChangesAsync();
                    dbObj.F_Status = "0";
                    dbObj.F_Delivery_Date = obj.F_Delivery_Date;
                    dbObj.F_Delivery_Trip = obj.F_Delivery_Trip;
                    dbObj.F_KB_Cut = obj.F_KB_Cut;
                    dbObj.F_KB_Cut_RN = obj.F_KB_Cut_RN;
                    dbObj.F_Update_Date = DateTime.Now;
                    dbObj.F_Update_By = _BearerClass.UserCode;
                    _KB3Context.TB_Kanban_Cut.Add(dbObj);
                }
                else
                {
                    obj.F_Status = "0";
                    obj.F_Create_By = _BearerClass.UserCode;
                    obj.F_Create_Date = DateTime.Now;
                    obj.F_Update_By = _BearerClass.UserCode;
                    obj.F_Update_Date = DateTime.Now;
                    _KB3Context.TB_Kanban_Cut.Add(obj);
                }

                _log.WriteLogMsg($"KBNMS006 : SaveKBCut | obj : {JsonConvert.SerializeObject(obj)}");

                await _KB3Context.SaveChangesAsync();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Save Data Success"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> StartKBCut(TB_Kanban_Cut obj)
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
                }

                var dbObj = await _KB3Context.TB_Kanban_Cut.FirstOrDefaultAsync(x => x.F_Plant == obj.F_Plant
                && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant
                && x.F_Store_Code == obj.F_Store_Code && x.F_Kanban_No == obj.F_Kanban_No
                && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu);

                if (dbObj == null)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data Not Found"
                    });
                }

                dbObj.F_Status = "1";
                dbObj.F_Update_Date = DateTime.Now;
                dbObj.F_Update_By = _BearerClass.UserCode;

                _KB3Context.TB_Kanban_Cut.Update(dbObj);
                await _KB3Context.SaveChangesAsync();

                _log.WriteLogMsg($"KBNMS006 : StartKBCut | obj : {JsonConvert.SerializeObject(dbObj)}");

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Start KB Stop Success"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> StopKBCut(TB_Kanban_Cut obj)
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
                }

                var dbObj = await _KB3Context.TB_Kanban_Cut.FirstOrDefaultAsync(x => x.F_Plant == obj.F_Plant
                               && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant
                                              && x.F_Store_Code == obj.F_Store_Code && x.F_Kanban_No == obj.F_Kanban_No
                                                             && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu);

                if (dbObj == null)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data Not Found"
                    });
                }

                dbObj.F_Status = "0";
                dbObj.F_Update_Date = DateTime.Now;
                dbObj.F_Update_By = _BearerClass.UserCode;

                _KB3Context.TB_Kanban_Cut.Update(dbObj);
                await _KB3Context.SaveChangesAsync();

                _log.WriteLogMsg($"KBNMS006 : StopKBCut | obj : {JsonConvert.SerializeObject(dbObj)}");

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Stop KB Stop Success"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        private async Task<bool> IsProcessDatePast (string date,int trip,string supplier,string kanban)
        {
            try
            {
                bool result = false;
                string _sql = $"SELECT DISTINCT V.F_Delivery_Date, V.F_Delivery_Trip, F_Lock " +
                    $"FROM TB_Calculate_D D LEFT JOIN (Select F_Process_Date, F_Process_Round, F_Supplier_Code, F_Supplier_Plant " +
                    $", F_Store_Code, F_Part_No, F_Ruibetsu, F_Kanban_No " +
                    $", CASE WHEN F_Flag_Adj ='1' THEN F_Adj_DeliDate ELSE F_Delivery_Date END F_Delivery_Date " +
                    $", CASE WHEN F_Flag_Adj ='1' THEN F_Adj_Deli_Trip ELSE F_Delivery_Trip END F_Delivery_Trip " +
                    $", CASE WHEN F_Flag_Adj ='1' THEN F_Adj_Qty ELSE F_Qty END F_Qty,F_Lock " +
                    $"From TB_Calculate_Volume) V ON D.F_Supplier_Code = V.F_Supplier_Code " +
                    $"AND D.F_Supplier_Plant = V.F_Supplier_Plant AND D.F_Part_No = V.F_Part_No " +
                    $"AND D.F_Ruibetsu = V.F_Ruibetsu AND D.F_Store_Code = V.F_Store_Code " +
                    $"AND D.F_Kanban_No = V.F_Kanban_No AND D.F_Process_Date = V.F_Process_Date " +
                    $"AND D.F_Process_Round = V.F_Process_Round ";

                if(supplier != "9999")
                {
                    _sql += "WHERE D.F_Process_Date = (Select LEFT(F_Value3,8) From TB_MS_Parameter Where F_Code = 'LO') " +
                        "AND D.F_Process_Shift = (Select RIGHT(F_Value3,1) From TB_MS_Parameter Where F_Code = 'LO') ";
                }
                else
                {
                    _sql += "WHERE D.F_Process_Date = (Select LEFT(F_Value3,8) From TB_MS_Parameter Where F_Code = 'LO_CKD') " +
                        "AND D.F_Process_Shift = (Select RIGHT(F_Value3,1) From TB_MS_Parameter Where F_Code = 'LO_CKD') ";
                }

                _sql += $"AND V.F_Supplier_Code = '{supplier.Substring(0, 4)}' AND V.F_Supplier_Plant = '{supplier[5]}' " +
                    $"AND V.F_Kanban_No = '{kanban}' AND D.F_KB_STOP  = '0'";

                var _dt = _FillDT.ExecuteSQL(_sql);

                if (_dt.Rows.Count == 0)
                {
                    return false;
                }

                else
                {
                    for(int i = 0; i < _dt.Rows.Count; i++)
                    {

                        string _base = (date + ("00" + trip.ToString()).Substring(("00" + trip.ToString()).Length - 2, 2));

                        string _diff = _dt.Rows[i]["F_Delivery_Date"].ToString().Trim() +
                                        ("00" + _dt.Rows[i]["F_Delivery_Trip"].ToString().Trim())
                                        .Substring(("00" + _dt.Rows[i]["F_Delivery_Trip"].ToString().Trim()).Length - 2, 2);

                        if (_base.CompareTo(_diff) <= 0)
                        {
                            result = true;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> IsProcessDateHoliday (string date, string store)
        {
            try
            {
                bool result = false;

                string _sql = $"SELECT SUM(CAST(F_Work AS INT)) " +
                    $"FROM TB_Calendar_UNPIVOT WHERE F_Date = '{date}' " +
                    $"AND F_Store_cd = '{store}' HAVING SUM(CAST(F_Work AS INT)) = 0";

                var _dt = _FillDT.ExecuteSQL(_sql);

                if (_dt.Rows.Count > 0)
                {
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<int> GetCycleTime (string date , int trip,string supplier)
        {
            string _sql = $"exec [dbo].[sp_getCycleTime] @p0,@p1,@p2,@p3";
            var _dt = _FillDT.ExecuteSQL(_sql, supplier.Substring(0, 4), supplier[5], date, date);

            if (_dt.Rows.Count > 0)
            {
                return int.TryParse(_dt.Rows[0]["F_Cycle"].ToString().Substring(4, 2), out int result) ? result : 0;
            }
            return 0;
        }
    }
}
