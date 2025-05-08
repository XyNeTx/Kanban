using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;

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

        private string yyyyMMdd = DateTime.Now.ToString("yyyyMMdd");
        private static DataTable DT_PartControl = new DataTable();

        [HttpGet]
        public IActionResult GetSupplier()
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

                var dbObj = _KB3Context.TB_MS_PartOrder.Where(x => x.F_Start_Date.CompareTo(yyyyMMdd) <= 0
                    && x.F_End_Date.CompareTo(yyyyMMdd) >= 0 && x.F_Type_Order.Trim() == "Pattern"
                    && x.F_Store_Code.StartsWith(_BearerClass.Plant))
                    .ToList();

                if (dbObj.Count == 0)
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
                    data = dbObj.Select(x => new
                    {
                        F_Supplier_Code = x.F_Supplier_Cd.Trim() + "-" + x.F_Supplier_Plant,
                    }).DistinctBy(x => x.F_Supplier_Code)
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
        public IActionResult GetSupplierDetail(string supplier, string? store, string? storeTo)
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
                    name.Where(x => x.F_Store_cd.CompareTo(store) >= 0 && x.F_Store_cd.CompareTo(storeTo) <= 0);
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
        public IActionResult GetKanban(string? supplier, string? store, string? partNo, string? storeTo, string? partNoTo)
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
                        && x.F_Supplier_Plant == supplier[5].ToString());
                }
                if (!string.IsNullOrWhiteSpace(store) && (!string.IsNullOrWhiteSpace(storeTo)))
                {
                    kanban = kanban.Where(x => x.F_Store_Code.CompareTo(store) >= 0 && x.F_Store_Code.CompareTo(storeTo) <= 0);
                }
                if (!string.IsNullOrWhiteSpace(partNo) && (!string.IsNullOrWhiteSpace(partNoTo)))
                {
                    kanban = kanban.Where(x => x.F_Part_No.CompareTo(partNo) >= 0 && x.F_Part_No.CompareTo(partNoTo) <= 0
                    && x.F_Ruibetsu.CompareTo(partNo.Substring(11, 2)) >= 0 && x.F_Ruibetsu.CompareTo(partNoTo.Substring(11, 2)) <= 0);
                }

                if (kanban == null)
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

        [HttpGet]
        public IActionResult GetStore(string? supplier, string? kanban, string? partNo, string? kanbanTo, string? partNoTo)
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

                var store = _KB3Context.TB_MS_PartOrder.Where(x => x.F_Start_Date.CompareTo(yyyyMMdd) <= 0
                        && x.F_End_Date!.CompareTo(yyyyMMdd) >= 0 && x.F_Store_Code.StartsWith(_BearerClass.Plant)
                        && x.F_Type_Order!.Trim() == "Pattern").AsQueryable();

                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    store = store.Where(x => x.F_Supplier_Cd.Trim() == supplier.Substring(0, 4)
                                           && x.F_Supplier_Plant == supplier[5].ToString());
                }
                if (!string.IsNullOrWhiteSpace(kanban) && (!string.IsNullOrWhiteSpace(kanbanTo)))
                {
                    store = store.Where(x => x.F_Kanban_No.CompareTo(kanban) >= 0 && x.F_Kanban_No.CompareTo(kanbanTo) <= 0);
                }
                if (!string.IsNullOrWhiteSpace(partNo) && (!string.IsNullOrWhiteSpace(partNoTo)))
                {
                    store = store.Where(x => x.F_Part_No.CompareTo(partNo.Substring(0, 10)) >= 0
                        && x.F_Part_No.CompareTo(partNoTo.Substring(0, 10)) <= 0
                                             && x.F_Ruibetsu.CompareTo(partNo.Substring(11, 2)) >= 0
                                             && x.F_Ruibetsu.CompareTo(partNoTo.Substring(11, 2)) <= 0);
                }

                if (store == null)
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
                    data = store.Select(x => x.F_Store_Code).Distinct().OrderBy(x => x)
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

        [HttpGet]
        public IActionResult GetPartNo(string? supplier, string? kanban, string? store, string? kanbanTo, string? storeTo)
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

                var part = _KB3Context.TB_MS_PartOrder.Where(x => x.F_Start_Date.CompareTo(yyyyMMdd) <= 0
                                       && x.F_End_Date.CompareTo(yyyyMMdd) >= 0 && x.F_Store_Code.StartsWith(_BearerClass.Plant)
                                                              && x.F_Type_Order.Trim() == "Pattern").AsQueryable();

                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    part = part.Where(x => x.F_Supplier_Cd.Trim() == supplier.Substring(0, 4)
                                                              && x.F_Supplier_Plant == supplier[5].ToString());
                }
                if (!string.IsNullOrWhiteSpace(kanban) && (!string.IsNullOrWhiteSpace(kanbanTo)))
                {
                    part = part.Where(x => x.F_Kanban_No.CompareTo(kanban) >= 0 && x.F_Kanban_No.CompareTo(kanbanTo) <= 0);
                }
                if (!string.IsNullOrWhiteSpace(store) && (!string.IsNullOrWhiteSpace(storeTo)))
                {
                    part = part.Where(x => x.F_Store_Code.CompareTo(store) >= 0 && x.F_Store_Code.CompareTo(storeTo) <= 0);
                }

                if (part == null)
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
                    data = part.Select(x => new
                    {
                        F_Part_No = x.F_Part_No.Trim() + "-" + x.F_Ruibetsu,
                    }).Distinct().OrderBy(x => x.F_Part_No)
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
        public IActionResult Search(string? supplier, string? kanban, string? kanbanTo, string? store, string? storeTo, string? partNo, string? partNoTo)
        {

            if (string.IsNullOrWhiteSpace(supplier) && string.IsNullOrWhiteSpace(kanban)
                && string.IsNullOrWhiteSpace(kanbanTo) && string.IsNullOrWhiteSpace(store)
                && string.IsNullOrWhiteSpace(storeTo) && string.IsNullOrWhiteSpace(partNo)
                && string.IsNullOrWhiteSpace(partNoTo))
            {
                return BadRequest(new
                {
                    status = "400",
                    response = "Bad Request",
                    title = "Error",
                    message = "Please Select Data Process Data",
                });
            }

            if (!string.IsNullOrWhiteSpace(supplier) && string.IsNullOrWhiteSpace(kanban)
                && string.IsNullOrWhiteSpace(kanbanTo))
            {
                return BadRequest(new
                {
                    status = "400",
                    response = "Bad Request",
                    title = "Error",
                    message = "Please Select Kanban No",
                });
            }

            if (!string.IsNullOrWhiteSpace(kanban) && !string.IsNullOrWhiteSpace(kanbanTo)
                && string.IsNullOrWhiteSpace(supplier))
            {
                return BadRequest(new
                {
                    status = "400",
                    response = "Bad Request",
                    title = "Error",
                    message = "Please Select Supplier",
                });
            }

            if (!string.IsNullOrWhiteSpace(store) && !string.IsNullOrWhiteSpace(storeTo)
                && string.IsNullOrWhiteSpace(partNo) && string.IsNullOrWhiteSpace(partNoTo))
            {
                return BadRequest(new
                {
                    status = "400",
                    response = "Bad Request",
                    title = "Error",
                    message = "Please Select Part No",
                });
            }

            if (!string.IsNullOrWhiteSpace(partNo) && !string.IsNullOrWhiteSpace(partNoTo)
                && string.IsNullOrWhiteSpace(store) && string.IsNullOrWhiteSpace(storeTo))
            {
                return BadRequest(new
                {
                    status = "400",
                    response = "Bad Request",
                    title = "Error",
                    message = "Please Select Store",
                });
            }

            var dbObj = _KB3Context.TB_MS_PartOrder.Where(x => x.F_Start_Date.CompareTo(yyyyMMdd) <= 0
                         && x.F_End_Date.CompareTo(yyyyMMdd) >= 0 && x.F_Store_Code.StartsWith(_BearerClass.Plant)
                         && x.F_Type_Order.Trim() == "Pattern").AsQueryable();

            if (!string.IsNullOrWhiteSpace(supplier))
            {
                dbObj = dbObj.Where(x => x.F_Supplier_Cd.Trim() == supplier.Substring(0, 4)
                        && x.F_Supplier_Plant == supplier[5].ToString());
            }
            if (!string.IsNullOrWhiteSpace(kanban) && (!string.IsNullOrWhiteSpace(kanbanTo)))
            {
                dbObj = dbObj.Where(x => x.F_Kanban_No.CompareTo(kanban) >= 0 && x.F_Kanban_No.CompareTo(kanbanTo) <= 0);
            }
            if (!string.IsNullOrWhiteSpace(store) && (!string.IsNullOrWhiteSpace(storeTo)))
            {
                dbObj = dbObj.Where(x => x.F_Store_Code.CompareTo(store) >= 0 && x.F_Store_Code.CompareTo(storeTo) <= 0);
            }
            if (!string.IsNullOrWhiteSpace(partNo) && (!string.IsNullOrWhiteSpace(partNoTo)))
            {
                dbObj = dbObj.Where(x => x.F_Part_No.CompareTo(partNo.Substring(0, 10)) >= 0
                        && x.F_Part_No.CompareTo(partNoTo.Substring(0, 10)) <= 0
                        && x.F_Ruibetsu.CompareTo(partNo.Substring(11, 2)) >= 0
                        && x.F_Ruibetsu.CompareTo(partNoTo.Substring(11, 2)) <= 0);
            }

            DT_PartControl = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(dbObj));

            if (dbObj == null)
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
                data = dbObj.Select(x => new
                {
                    F_Supplier_Cd = x.F_Supplier_Cd.Trim(),
                    F_Supplier_Plant = x.F_Supplier_Plant,
                    F_Kanban_No = x.F_Kanban_No.Trim(),
                    F_Store_Code = x.F_Store_Code.Trim(),
                    F_Part_No = x.F_Part_No.Trim(),
                    F_Ruibetsu = x.F_Ruibetsu,
                }).Distinct().AsEnumerable()
            });
        }

        [HttpPost]
        public async Task<IActionResult> SelectDateChange(TMP_Planning_Order obj, [FromQuery] bool isImport = false)
        {
            try
            {
                var loginDate = Request.Cookies.FirstOrDefault(x => x.Key == "loginDate").Value;
                string periodDay = "";
                string periodNight = "";

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

                var cycle = _KB3Context.TB_MS_DeliveryTime.Where(x => x.F_Supplier_Code == obj.F_Supplier_Code
                            && x.F_Supplier_Plant == obj.F_Supplier_Plant.ToString()
                            && x.F_Start_Date.CompareTo(obj.F_Delivery_Date) <= 0
                            && x.F_End_Date.CompareTo(obj.F_Delivery_Date) >= 0)
                    .AsQueryable();

                if (cycle == null)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data not found",
                    });
                }

                var storecode = _BearerClass.Plant switch
                {
                    "1" => "1A",
                    "2" => "2B",
                    "3" => "3C",
                    _ => _BearerClass.Plant
                };

                int dd = int.Parse(obj.F_Delivery_Date.Substring(6, 2));

                string sql = $"SELECT CONVERT(Integer, F_workCd_D{dd} " +
                    $") + CONVERT(Integer, F_workCd_N{dd}) AS F_Work " +
                    $",F_workCd_D{dd} AS Day, F_workCd_N{dd} AS Night " +
                    $"FROM TB_Calendar WHERE F_Store_cd = '{storecode}' AND F_YM = '{obj.F_Delivery_Date.Substring(0, 6)}' ";

                var workDate = _FillDT.ExecuteSQL(sql);

                if (workDate.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Not Working",
                    });
                };

                if (obj.F_Part_No == "486540K160")
                {
                    Console.WriteLine("Test");
                }

                var isInsert = await Insert_TMP(obj, isImport);
                var isDetail = await Detail_Data(obj);

                if (!isInsert || !isDetail)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Process Error",
                    });
                }

                if (workDate.Rows[0]["F_Work"].ToString() == "1")
                {
                    if (workDate.Rows[0]["Day"].ToString() == "1")
                    {
                        string _cycleB = cycle.Select(x => x.F_Cycle).FirstOrDefault().Substring(2, 2);
                        sql = "exec [dbo].[sp_findPeriod] @p0,@p1,@p2,@p3,@p4,@p5";
                        var periodData = _FillDT.ExecuteSQL(sql,
                            _BearerClass.Plant, obj.F_Supplier_Code,
                            obj.F_Supplier_Plant, obj.F_Delivery_Date,
                            _BearerClass.UserCode, _cycleB);

                        periodDay = JsonConvert.SerializeObject(periodData.Rows[0]["F_Period"]);
                    }
                    else
                    {
                        string _cycleB = cycle.Select(x => x.F_Cycle).FirstOrDefault().Substring(2, 2);
                        sql = "exec [dbo].[sp_findPeriod] @p0,@p1,@p2,@p3,@p4,@p5";
                        var periodData = _FillDT.ExecuteSQL(sql,
                            _BearerClass.Plant, obj.F_Supplier_Code,
                            obj.F_Supplier_Plant, obj.F_Delivery_Date,
                            _BearerClass.UserCode, _cycleB);

                        periodNight = JsonConvert.SerializeObject(periodData.Rows[1]["F_Period"]);
                    }
                }



                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Process Completed",
                    data = new
                    {
                        workDate = JsonConvert.SerializeObject(workDate),
                        periodDay = periodDay,
                        periodNight = periodNight
                    }
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

        private async Task<bool> Insert_TMP(TMP_Planning_Order obj, bool isImport)
        {
            try
            {
                string loginDate = Request.Cookies.FirstOrDefault(x => x.Key == "loginDate").Value;
                string isDev = Request.Cookies.FirstOrDefault(x => x.Key == "isDev").Value == "1" ? "Dev" : "";
                string plant = Request.Cookies.FirstOrDefault(x => x.Key == "plantCode").Value;
                string cPlantDev = plant + isDev;

                string ppmConnect = cPlantDev switch
                {
                    "1" => "[HMMT-PPM].[PPMDB]",
                    "2" => "[HMMT-PPM].[PPMDB]",
                    "3" => "[HMMTA-PPM].[PPMDB]",
                    "1Dev" => "[HMMT-PPM].[PPMDB]",
                    "2Dev" => "[HMMT-PPM].[PPMDB]",
                    "3Dev" => "[PPMDB]",
                    _ => "[HMMTA-PPM].[PPMDB]"
                };

                string sql = "";
                var TMP_Planning = new List<TMP_Planning_Order>();

                if (isImport == false)
                {
                    for (int i = 0; i < DT_PartControl.Rows.Count; i++)
                    {

                        TMP_Planning = _KB3Context.TMP_Planning_Order.Where(x => x.F_Supplier_Code == DT_PartControl.Rows[i]["F_Supplier_Cd"].ToString().Trim() &&
                                x.F_Supplier_Plant == DT_PartControl.Rows[i]["F_Supplier_Plant"].ToString().Trim() &&
                                x.F_Kanban_No == DT_PartControl.Rows[i]["F_Kanban_No"].ToString().Trim() &&
                                x.F_Store_Code == DT_PartControl.Rows[i]["F_Store_Code"].ToString().Trim() &&
                                x.F_Part_No == DT_PartControl.Rows[i]["F_Part_No"].ToString().Trim() &&
                                x.F_Ruibetsu == DT_PartControl.Rows[i]["F_Ruibetsu"].ToString().Trim() &&
                                x.F_Delivery_Date == obj.F_Delivery_Date && x.F_Plant == _BearerClass.Plant).ToList();

                        if (TMP_Planning.Count == 0)
                        {
                            string cycle = obj.F_Cycle == null ? "P.F_Cycle" : $"'{obj.F_Cycle}'";
                            sql = $"INSERT INTO TMP_Planning_Order(F_Plant, F_Delivery_Date, F_Supplier_Code, F_Supplier_Plant " +
                                $", F_Store_Code, F_Kanban_No, F_Part_No, F_Ruibetsu, F_Cycle, F_Qty, F_Safety_Stk " +
                                $", F_Trip1, F_Trip2, F_Trip3, F_Trip4, F_Trip5, F_Trip6, F_Trip7, F_Trip8, F_Trip9, F_Trip10 " +
                                $", F_Trip11, F_Trip12, F_Trip13, F_Trip14, F_Trip15, F_Trip16, F_Trip17, F_Trip18, F_Trip19, F_Trip20 " +
                                $", F_Trip21, F_Trip22, F_Trip23, F_Trip24, F_Trip25, F_Trip26, F_Trip27, F_Trip28, F_Trip29, F_Trip30 " +
                                $", F_Trip31, F_Trip32, F_Create_Date, F_Create_By) SELECT '{_BearerClass.Plant}' AS F_Plant " +
                                $", '{obj.F_Delivery_Date}' AS F_Delivery_Date  , RTRIM(C.F_supplier_cd) AS F_Supplier_Code " +
                                $", RTRIM(C.F_plant) AS F_Supplier_Plant " +
                                $", RTRIM(C.F_Store_cd) AS F_Store_Code " +
                                $", RIGHT('0000'+ CONVERT(VARCHAR,C.F_Sebango),4) AS F_Kanban_No " +
                                $", RTRIM(C.F_Part_no) AS F_Part_No " +
                                $", RTRIM(C.F_Ruibetsu) AS F_Ruibetsu " +
                                $", {cycle} AS F_Cycle " +
                                $", CASE WHEN Chg.F_New_Qty = '' OR Chg.F_New_Qty IS NULL THEN C.F_qty_box " +
                                $" ELSE Chg.F_New_Qty END AS F_Qty " +
                                $", RTRIM(C.F_Safety_Stk) AS F_Safety_Stock " +
                                $", '{obj.F_Trip1}' AS F_Trip1 , '{obj.F_Trip2}' AS F_Trip2" +
                                $", '{obj.F_Trip3}' AS F_Trip3 , '{obj.F_Trip4}' AS F_Trip4" +
                                $", '{obj.F_Trip5}' AS F_Trip5 , '{obj.F_Trip6}' AS F_Trip6" +
                                $", '{obj.F_Trip7}' AS F_Trip7 , '{obj.F_Trip8}' AS F_Trip8" +
                                $", '{obj.F_Trip9}' AS F_Trip9 , '{obj.F_Trip10}' AS F_Trip10" +
                                $", '{obj.F_Trip11}' AS F_Trip11 , '{obj.F_Trip12}' AS F_Trip12" +
                                $", '{obj.F_Trip13}' AS F_Trip13 , '{obj.F_Trip14}' AS F_Trip14" +
                                $", '{obj.F_Trip15}' AS F_Trip15 , '{obj.F_Trip16}' AS F_Trip16" +
                                $", '{obj.F_Trip17}' AS F_Trip17 , '{obj.F_Trip18}' AS F_Trip18" +
                                $", '{obj.F_Trip19}' AS F_Trip19 , '{obj.F_Trip20}' AS F_Trip20" +
                                $", '{obj.F_Trip21}' AS F_Trip21 , '{obj.F_Trip22}' AS F_Trip22" +
                                $", '{obj.F_Trip23}' AS F_Trip23 , '{obj.F_Trip24}' AS F_Trip24" +
                                $", '{obj.F_Trip25}' AS F_Trip25 , '{obj.F_Trip26}' AS F_Trip26" +
                                $", '{obj.F_Trip27}' AS F_Trip27 , '{obj.F_Trip28}' AS F_Trip28" +
                                $", '{obj.F_Trip29}' AS F_Trip29 , '{obj.F_Trip30}' AS F_Trip30" +
                                $", '{obj.F_Trip31}' AS F_Trip31 , '{obj.F_Trip32}' AS F_Trip32" +
                                $", '{DateTime.Now}' AS F_Create_Date " +
                                $", '{_BearerClass.UserCode}' AS F_Create_By " +
                                $"FROM {ppmConnect}.[dbo].[T_Construction] C  " +
                                $"INNER JOIN TB_MS_PartOrder P " +
                                $"ON C.F_supplier_cd = P.F_Supplier_Cd collate Thai_CI_AS " +
                                $"AND C.F_plant = P.F_Supplier_Plant collate Thai_CI_AS " +
                                $"AND C.F_Store_cd = P.F_Store_Code collate Thai_CI_AS " +
                                $"AND RIGHT('0000'+C.F_Sebango,4) = P.F_Kanban_No collate Thai_CI_AS " +
                                $"AND C.F_Part_no = P.F_Part_No collate Thai_CI_AS " +
                                $"AND C.F_Ruibetsu = P.F_Ruibetsu collate Thai_CI_AS " +
                                $"LEFT JOIN TB_Kanban_Chg_Qty Chg " +
                                $"ON  C.F_supplier_cd = Chg.F_Supplier_Code collate Thai_CI_AS " +
                                $"AND C.F_plant = Chg.F_Supplier_Plant collate Thai_CI_AS " +
                                $"AND C.F_Store_cd = Chg.F_Store_Code collate Thai_CI_AS " +
                                $"AND RIGHT('0000'+C.F_Sebango,4) = Chg.F_Kanban_No collate Thai_CI_AS " +
                                $"AND C.F_Part_no = Chg.F_Part_No collate Thai_CI_AS " +
                                $"AND C.F_Ruibetsu = Chg.F_Ruibetsu collate Thai_CI_AS " +
                                $"WHERE C.F_Part_no = '{DT_PartControl.Rows[i]["F_Part_No"].ToString().Trim()}' " +
                                $"AND C.F_Ruibetsu = '{DT_PartControl.Rows[i]["F_Ruibetsu"].ToString().Trim()}' " +
                                $"AND C.F_Store_cd = '{DT_PartControl.Rows[i]["F_Store_Code"].ToString().Trim()}' " +
                                $"AND C.F_supplier_cd = '{DT_PartControl.Rows[i]["F_Supplier_Cd"].ToString().Trim()}' " +
                                $"AND C.F_plant = '{DT_PartControl.Rows[i]["F_Supplier_Plant"].ToString().Trim()}' " +
                                $"AND C.F_Sebango = '{DT_PartControl.Rows[i]["F_Kanban_No"].ToString().Trim().Substring(1, 3)}' " +
                                $"AND C.F_Local_Str <= convert(char(8),getdate(),112) " +
                                $"AND C.F_Local_End >= convert(char(8),getdate(),112) ";

                            await _KB3Context.Database.ExecuteSqlRawAsync(sql);
                            _log.WriteLogMsg($"Insert TMP_Planning_Order : {sql}");

                        }
                    }
                }
                else
                {
                    string getStoreCd = _PPM3Context.T_Construction.Where(x => x.F_supplier_cd == obj.F_Supplier_Code &&
                            x.F_plant == obj.F_Supplier_Plant[0] &&
                            x.F_Sebango == obj.F_Kanban_No.Substring(1, 3) &&
                            x.F_Part_no == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu)
                        .Select(x => x.F_Store_cd).FirstOrDefault();

                    TMP_Planning = _KB3Context.TMP_Planning_Order.Where(x => x.F_Supplier_Code == obj.F_Supplier_Code &&
                            x.F_Supplier_Plant == obj.F_Supplier_Plant &&
                            x.F_Kanban_No == obj.F_Kanban_No &&
                            x.F_Store_Code == getStoreCd &&
                            x.F_Part_No == obj.F_Part_No &&
                            x.F_Ruibetsu == obj.F_Ruibetsu &&
                            x.F_Delivery_Date == obj.F_Delivery_Date &&
                            x.F_Plant == _BearerClass.Plant).ToList();


                    if (TMP_Planning.Count == 0)
                    {
                        string cycle = obj.F_Cycle == null ? "P.F_Cycle" : $"'{obj.F_Cycle}'";
                        sql = $"INSERT INTO TMP_Planning_Order(F_Plant, F_Delivery_Date, F_Supplier_Code, F_Supplier_Plant " +
                            $", F_Store_Code, F_Kanban_No, F_Part_No, F_Ruibetsu, F_Cycle, F_Qty, F_Safety_Stk " +
                            $", F_Trip1, F_Trip2, F_Trip3, F_Trip4, F_Trip5, F_Trip6, F_Trip7, F_Trip8, F_Trip9, F_Trip10 " +
                            $", F_Trip11, F_Trip12, F_Trip13, F_Trip14, F_Trip15, F_Trip16, F_Trip17, F_Trip18, F_Trip19, F_Trip20 " +
                            $", F_Trip21, F_Trip22, F_Trip23, F_Trip24, F_Trip25, F_Trip26, F_Trip27, F_Trip28, F_Trip29, F_Trip30 " +
                            $", F_Trip31, F_Trip32, F_Create_Date, F_Create_By) SELECT '{_BearerClass.Plant}' AS F_Plant " +
                            $", '{obj.F_Delivery_Date}' AS F_Delivery_Date  , RTRIM(C.F_supplier_cd) AS F_Supplier_Code " +
                            $", RTRIM(C.F_plant) AS F_Supplier_Plant " +
                            $", RTRIM(C.F_Store_cd) AS F_Store_Code " +
                            $", RIGHT('0000'+ CONVERT(VARCHAR,C.F_Sebango),4) AS F_Kanban_No " +
                            $", RTRIM(C.F_Part_no) AS F_Part_No " +
                            $", RTRIM(C.F_Ruibetsu) AS F_Ruibetsu " +
                            $", {cycle} AS F_Cycle " +
                            $", CASE WHEN Chg.F_New_Qty = '' OR Chg.F_New_Qty IS NULL THEN C.F_qty_box " +
                            $" ELSE Chg.F_New_Qty END AS F_Qty " +
                            $", RTRIM(C.F_Safety_Stk) AS F_Safety_Stock " +
                            $", '{obj.F_Trip1}' AS F_Trip1 , '{obj.F_Trip2}' AS F_Trip2" +
                            $", '{obj.F_Trip3}' AS F_Trip3 , '{obj.F_Trip4}' AS F_Trip4" +
                            $", '{obj.F_Trip5}' AS F_Trip5 , '{obj.F_Trip6}' AS F_Trip6" +
                            $", '{obj.F_Trip7}' AS F_Trip7 , '{obj.F_Trip8}' AS F_Trip8" +
                            $", '{obj.F_Trip9}' AS F_Trip9 , '{obj.F_Trip10}' AS F_Trip10" +
                            $", '{obj.F_Trip11}' AS F_Trip11 , '{obj.F_Trip12}' AS F_Trip12" +
                            $", '{obj.F_Trip13}' AS F_Trip13 , '{obj.F_Trip14}' AS F_Trip14" +
                            $", '{obj.F_Trip15}' AS F_Trip15 , '{obj.F_Trip16}' AS F_Trip16" +
                            $", '{obj.F_Trip17}' AS F_Trip17 , '{obj.F_Trip18}' AS F_Trip18" +
                            $", '{obj.F_Trip19}' AS F_Trip19 , '{obj.F_Trip20}' AS F_Trip20" +
                            $", '{obj.F_Trip21}' AS F_Trip21 , '{obj.F_Trip22}' AS F_Trip22" +
                            $", '{obj.F_Trip23}' AS F_Trip23 , '{obj.F_Trip24}' AS F_Trip24" +
                            $", '{obj.F_Trip25}' AS F_Trip25 , '{obj.F_Trip26}' AS F_Trip26" +
                            $", '{obj.F_Trip27}' AS F_Trip27 , '{obj.F_Trip28}' AS F_Trip28" +
                            $", '{obj.F_Trip29}' AS F_Trip29 , '{obj.F_Trip30}' AS F_Trip30" +
                            $", '{obj.F_Trip31}' AS F_Trip31 , '{obj.F_Trip32}' AS F_Trip32" +
                            $", '{DateTime.Now}' AS F_Create_Date " +
                            $", '{_BearerClass.UserCode}' AS F_Create_By " +
                            $"FROM {ppmConnect}.[dbo].[T_Construction] C  " +
                            $"INNER JOIN TB_MS_PartOrder P " +
                            $"ON C.F_supplier_cd = P.F_Supplier_Cd collate Thai_CI_AS " +
                            $"AND C.F_plant = P.F_Supplier_Plant collate Thai_CI_AS " +
                            $"AND C.F_Store_cd = P.F_Store_Code collate Thai_CI_AS " +
                            $"AND RIGHT('0000'+C.F_Sebango,4) = P.F_Kanban_No collate Thai_CI_AS " +
                            $"AND C.F_Part_no = P.F_Part_No collate Thai_CI_AS " +
                            $"AND C.F_Ruibetsu = P.F_Ruibetsu collate Thai_CI_AS " +
                            $"LEFT JOIN TB_Kanban_Chg_Qty Chg " +
                            $"ON  C.F_supplier_cd = Chg.F_Supplier_Code collate Thai_CI_AS " +
                            $"AND C.F_plant = Chg.F_Supplier_Plant collate Thai_CI_AS " +
                            $"AND C.F_Store_cd = Chg.F_Store_Code collate Thai_CI_AS " +
                            $"AND RIGHT('0000'+C.F_Sebango,4) = Chg.F_Kanban_No collate Thai_CI_AS " +
                            $"AND C.F_Part_no = Chg.F_Part_No collate Thai_CI_AS " +
                            $"AND C.F_Ruibetsu = Chg.F_Ruibetsu collate Thai_CI_AS " +
                            $"WHERE C.F_Part_no = '{obj.F_Part_No}' " +
                            $"AND C.F_Ruibetsu = '{obj.F_Ruibetsu}' " +
                            $"AND C.F_Store_cd = '{getStoreCd}' " +
                            $"AND C.F_supplier_cd = '{obj.F_Supplier_Code}' " +
                            $"AND C.F_plant = '{obj.F_Supplier_Plant}' " +
                            $"AND C.F_Sebango = '{obj.F_Kanban_No.Substring(1, 3)}' " +
                            $"AND C.F_Local_Str <= convert(char(8),getdate(),112) " +
                            $"AND C.F_Local_End >= convert(char(8),getdate(),112) ";

                        await _KB3Context.Database.ExecuteSqlRawAsync(sql);
                        _log.WriteLogMsg($"Insert TMP_Planning_Order : {sql}");

                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                _log.WriteLogMsg($"Insert TMP_Planning_Order ERROR!!! : {ex.Message}");
                return false;
            }
        }

        private async Task<bool> Detail_Data(TMP_Planning_Order obj)
        {
            try
            {
                var storecode = _BearerClass.Plant switch
                {
                    "1" => "1A",
                    "2" => "2B",
                    "3" => "3C",
                    _ => _BearerClass.Plant
                };
                string dateDelivery = obj.F_Delivery_Date;
                DateTime last_Date = DateTime.ParseExact(dateDelivery, "yyyyMMdd", CultureInfo.InvariantCulture);
                DataTable dt = new DataTable();
                int Last_Order, Forcasst3Shift, TotalPcs, DiffLast = 0;

                for (int i = 0; i < DT_PartControl.Rows.Count; i++)
                {
                    do
                    {
                        last_Date = last_Date.AddDays(-1);

                        string _sql = $"SELECT CONVERT(Integer,F_workCd_D{last_Date.Day.ToString("d")}) + CONVERT(Integer,F_workCd_N{last_Date.Day.ToString("d")}) AS F_Work " +
                            $"FROM TB_Calendar WHERE F_Store_cd = '{storecode}' AND F_YM = '{last_Date.ToString("yyyyMM")}' ";

                        dt = _FillDT.ExecuteSQL(_sql);
                    }
                    while (dt.Rows.Count > 0 && dt.Rows[0]["F_Work"].ToString() == "0");

                    var TMP_Planning = _KB3Context.TMP_Planning_Order
                        .Where(x => x.F_Supplier_Code == DT_PartControl.Rows[i]["F_Supplier_Cd"].ToString().Trim() &&
                        x.F_Supplier_Plant == DT_PartControl.Rows[i]["F_Supplier_Plant"].ToString().Trim() &&
                        x.F_Kanban_No == DT_PartControl.Rows[i]["F_Kanban_No"].ToString().Trim() &&
                        x.F_Store_Code == DT_PartControl.Rows[i]["F_Store_Code"].ToString().Trim() &&
                        x.F_Part_No == DT_PartControl.Rows[i]["F_Part_No"].ToString().Trim() &&
                        x.F_Ruibetsu == DT_PartControl.Rows[i]["F_Ruibetsu"].ToString().Trim() &&
                        x.F_Delivery_Date == last_Date.ToString("yyyyMMdd") &&
                        x.F_Total_KB != 0).Select(x => new
                        {
                            F_Last_Order = (x.F_Order_Diff == 0) ? 0 : x.F_Order_Diff,
                        }).ToList();

                    if (TMP_Planning.Count > 0)
                    {
                        Last_Order = TMP_Planning[0].F_Last_Order.Value;
                        if (Last_Order <= 10000000)
                        {
                            Last_Order = 0;
                        }
                    }
                    else { Last_Order = 0; }

                    string sql = $"SELECT ISNULL(F_HMMT_Prod,0) AS F_HMMT_Prod " +
                        $"FROM TB_Calculate_H " +
                        $"WHERE F_Supplier_Code = '{DT_PartControl.Rows[i]["F_Supplier_Cd"].ToString().Trim()}' " +
                        $"AND F_Supplier_Plant = '{DT_PartControl.Rows[i]["F_Supplier_Plant"].ToString().Trim()}' " +
                        $"AND F_Kanban_No = '{DT_PartControl.Rows[i]["F_Kanban_No"].ToString().Trim()}' " +
                        $"AND F_Part_No = '{DT_PartControl.Rows[i]["F_Part_No"].ToString().Trim()}' " +
                        $"AND F_Ruibetsu = '{DT_PartControl.Rows[i]["F_Ruibetsu"].ToString().Trim()}' " +
                        $"AND F_Store_Code = '{DT_PartControl.Rows[i]["F_Store_Code"].ToString().Trim()}' " +
                        $"AND F_Process_Date = '{dateDelivery}' ";

                    dt = _FillDT.ExecuteSQL(sql);

                    if (dt.Rows.Count > 0)
                    {
                        Forcasst3Shift = Convert.ToInt32(dt.Rows[0]["F_HMMT_Prod"]);
                    }
                    else
                    {
                        Forcasst3Shift = 0;
                    }

                    sql = "SELECT ISNULL(F_Total_Pcs,0) AS F_Total_Pcs " +
                        "FROM TMP_Planning_Order " +
                        $"WHERE F_Supplier_Code = '{DT_PartControl.Rows[i]["F_Supplier_Cd"].ToString().Trim()}' " +
                        $"AND F_Supplier_Plant = '{DT_PartControl.Rows[i]["F_Supplier_Plant"].ToString().Trim()}' " +
                        $"AND F_Kanban_No = '{DT_PartControl.Rows[i]["F_Kanban_No"].ToString().Trim()}' " +
                        $"AND F_Part_No = '{DT_PartControl.Rows[i]["F_Part_No"].ToString().Trim()}' " +
                        $"AND F_Ruibetsu = '{DT_PartControl.Rows[i]["F_Ruibetsu"].ToString().Trim()}' " +
                        $"AND F_Store_Code = '{DT_PartControl.Rows[i]["F_Store_Code"].ToString().Trim()}' " +
                        $"AND F_Delivery_Date = '{dateDelivery}' ";

                    dt = _FillDT.ExecuteSQL(sql);

                    if (dt.Rows.Count > 0)
                    {
                        TotalPcs = Convert.ToInt32(dt.Rows[0]["F_Total_Pcs"]);
                        if (TotalPcs <= 10000000)
                        {
                            TotalPcs = 0;
                        }
                    }
                    else
                    {
                        TotalPcs = 0;
                    }

                    DiffLast = Last_Order - (Forcasst3Shift - TotalPcs);

                    sql = $"UPDATE TMP_Planning_Order " +
                        $"SET F_Last_Order = '{Last_Order}' " +
                        $",F_FC_Max = '{Forcasst3Shift}' " +
                        $",F_Order_Diff = '{DiffLast}' " +
                        $"WHERE F_Plant = '{_BearerClass.Plant}' " +
                        $"AND F_Delivery_Date = '{dateDelivery}' " +
                        $"AND F_Supplier_Code = '{DT_PartControl.Rows[i]["F_Supplier_Cd"].ToString().Trim()}' " +
                        $"AND F_Supplier_Plant = '{DT_PartControl.Rows[i]["F_Supplier_Plant"].ToString().Trim()}' " +
                        $"AND F_Kanban_No = '{DT_PartControl.Rows[i]["F_Kanban_No"].ToString().Trim()}' " +
                        $"AND F_Part_No = '{DT_PartControl.Rows[i]["F_Part_No"].ToString().Trim()}' " +
                        $"AND F_Ruibetsu = '{DT_PartControl.Rows[i]["F_Ruibetsu"].ToString().Trim()}' " +
                        $"AND F_Store_Code = '{DT_PartControl.Rows[i]["F_Store_Code"].ToString().Trim()}' ";

                    await _KB3Context.Database.ExecuteSqlRawAsync(sql);
                    _log.WriteLogMsg($"Update TMP_Planning_Order : {sql}");


                }
                return true;
            }
            catch (Exception ex)
            {
                _log.WriteLogMsg($"Detail_Data ERROR!!! : {ex.Message}");

                return false;
            }
        }

        [HttpGet]
        public IActionResult Show_Data(string? supplier, string? kanban, string? kanbanTo, string? store, string? storeTo, string? partNo, string? partNoTo, string selDate)
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

                DateTime dateNow = DateTime.ParseExact(selDate, "yyyyMMdd", CultureInfo.InvariantCulture);

                var dbObj = _KB3Context.TMP_Planning_Order.Where(x => x.F_Plant == _BearerClass.Plant
                    && x.F_Delivery_Date == dateNow.ToString("yyyyMMdd")
                    && x.F_Store_Code.StartsWith(_BearerClass.Plant)).AsQueryable();

                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    dbObj = dbObj.Where(x => x.F_Supplier_Code.Trim() == supplier.Substring(0, 4)
                                && x.F_Supplier_Plant == supplier[5].ToString());
                }
                if (!string.IsNullOrWhiteSpace(kanban) && (!string.IsNullOrWhiteSpace(kanbanTo)))
                {
                    dbObj = dbObj.Where(x => x.F_Kanban_No.CompareTo(kanban) >= 0 && x.F_Kanban_No.CompareTo(kanbanTo) <= 0);
                }
                if (!string.IsNullOrWhiteSpace(store) && (!string.IsNullOrWhiteSpace(storeTo)))
                {
                    dbObj = dbObj.Where(x => x.F_Store_Code.CompareTo(store) >= 0 && x.F_Store_Code.CompareTo(storeTo) <= 0);
                }
                if (!string.IsNullOrWhiteSpace(partNo) && (!string.IsNullOrWhiteSpace(partNoTo)))
                {
                    dbObj = dbObj.Where(x => x.F_Part_No.CompareTo(partNo.Substring(0, 10)) >= 0
                                && x.F_Part_No.CompareTo(partNoTo.Substring(0, 10)) <= 0
                                && x.F_Ruibetsu.CompareTo(partNo.Substring(11, 2)) >= 0
                                && x.F_Ruibetsu.CompareTo(partNoTo.Substring(11, 2)) <= 0);
                }

                if (dbObj == null)
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
                    data = dbObj.OrderBy(x => x.F_Part_No).ThenBy(x => x.F_Kanban_No)
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

        [HttpPost]
        public async Task<IActionResult> UpToList(List<TMP_Planning_Order> listObj)
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

                foreach (var obj in listObj)
                {
                    var dbObj = _KB3Context.TMP_Planning_Order.Where(x => x.F_Plant == _BearerClass.Plant
                            && x.F_Delivery_Date == obj.F_Delivery_Date
                            //&& x.F_Store_Code == obj.F_Store_Code
                            && x.F_Supplier_Code == obj.F_Supplier_Code
                            && x.F_Supplier_Plant == obj.F_Supplier_Plant
                            && x.F_Kanban_No == obj.F_Kanban_No
                            && x.F_Part_No == obj.F_Part_No
                            && x.F_Ruibetsu == obj.F_Ruibetsu).AsNoTracking().SingleOrDefault();

                    if (dbObj == null)
                    {
                        return NotFound(new
                        {
                            status = "404",
                            response = "Not Found",
                            title = "Error",
                            message = "Data not found",
                        });
                    }

                    obj.F_Create_By = dbObj.F_Create_By;
                    obj.F_Create_Date = dbObj.F_Create_Date;
                    obj.F_Update_By = _BearerClass.UserCode;
                    obj.F_Update_Date = DateTime.Now;

                    _KB3Context.TMP_Planning_Order.Update(obj);

                    var propList = obj.GetType().GetProperties()
                        .Where(x => x.Name.StartsWith("F_Trip")).ToList();
                    int cycle = int.Parse(obj.F_Cycle.Substring(2, 2));

                    foreach (var prop in propList)
                    {

                        if (prop.Name.StartsWith("F_Trip"))
                        {
                            string _deliveryTrip = prop.Name.Substring(6);
                            if (int.Parse(_deliveryTrip) > cycle)
                            {
                                break;
                            }

                            int _orderKB = int.Parse(prop.GetValue(obj).ToString());

                            TB_Kanban_Planning planObj = new TB_Kanban_Planning
                            {
                                F_Create_By = obj.F_Update_By,
                                F_Create_Date = obj.F_Update_Date.Value,
                                F_Update_By = obj.F_Update_By,
                                F_Update_Date = obj.F_Update_Date.Value,
                                F_Plant = obj.F_Plant,
                                F_Supplier_Code = obj.F_Supplier_Code,
                                F_Supplier_Plant = obj.F_Supplier_Plant,
                                F_Store_Code = obj.F_Store_Code,
                                F_Kanban_No = obj.F_Kanban_No,
                                F_Part_No = obj.F_Part_No,
                                F_Ruibetsu = obj.F_Ruibetsu,
                                F_Status = "0",
                                F_Delivery_Date = obj.F_Delivery_Date,
                                F_Delivery_Trip = _deliveryTrip.ToString(),
                                F_Order_PCS = (_orderKB * obj.F_Qty.Value),
                                F_Order_KB = _orderKB,
                                F_Qty_Pack = obj.F_Qty.Value,
                                F_Cycle = obj.F_Cycle,
                            };

                            var exist = await _KB3Context.TB_Kanban_Planning.AsNoTracking()
                                .Where(x => x.F_Plant == planObj.F_Plant
                                && x.F_Supplier_Code == planObj.F_Supplier_Code
                                && x.F_Supplier_Plant == planObj.F_Supplier_Plant
                                && x.F_Store_Code == planObj.F_Store_Code
                                && x.F_Kanban_No == planObj.F_Kanban_No
                                && x.F_Part_No == planObj.F_Part_No
                                && x.F_Ruibetsu == planObj.F_Ruibetsu
                                && x.F_Delivery_Date == planObj.F_Delivery_Date
                                && x.F_Delivery_Trip == planObj.F_Delivery_Trip).FirstOrDefaultAsync();

                            if (exist == null)
                            {
                                _KB3Context.TB_Kanban_Planning.Add(planObj);
                            }
                            else
                            {
                                _KB3Context.TB_Kanban_Planning.Update(planObj);
                            }
                        }
                    }

                    _log.WriteLogMsg($"Update TMP_Planning_Order : {JsonConvert.SerializeObject(dbObj)}");
                }

                await _KB3Context.SaveChangesAsync();
                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Updated",
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

        [HttpPost]
        public async Task<IActionResult> GetImportList(List<TMP_Planning_Order> listObj)
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

                var existList = _KB3Context.TMP_Planning_Order.Where(x => x.F_Plant == _BearerClass.Plant
                                   //&& x.F_Create_By == _BearerClass.UserCode
                                   && x.F_Supplier_Code == listObj[0].F_Supplier_Code
                                   && x.F_Supplier_Plant == listObj[0].F_Supplier_Plant
                                   ).ToList();

                _KB3Context.TMP_Planning_Order.RemoveRange(existList);
                await _KB3Context.SaveChangesAsync();

                var dbObj = _KB3Context.TB_MS_PartOrder.Where(x => x.F_Start_Date.CompareTo(yyyyMMdd) <= 0
                         && x.F_End_Date.CompareTo(yyyyMMdd) >= 0 && x.F_Store_Code.StartsWith(_BearerClass.Plant)
                         && x.F_Type_Order.Trim() == "Pattern").AsNoTracking().AsQueryable();

                int lastList = listObj.Count - 1;

                foreach (var each in listObj)
                {
                    each.F_Create_By = _BearerClass.UserCode;
                    each.F_Create_Date = DateTime.Now;
                }

                if (!string.IsNullOrWhiteSpace(listObj[0].F_Supplier_Code))
                {
                    dbObj = dbObj.Where(x => x.F_Supplier_Cd.Trim() == listObj[0].F_Supplier_Code.Substring(0, 4)
                               && x.F_Supplier_Plant == listObj[0].F_Supplier_Plant[0].ToString());
                }
                if (!string.IsNullOrWhiteSpace(listObj[0].F_Kanban_No) && (!string.IsNullOrWhiteSpace(listObj[lastList].F_Kanban_No)))
                {
                    dbObj = dbObj.Where(x => x.F_Kanban_No.CompareTo(listObj[0].F_Kanban_No) >= 0
                            && x.F_Kanban_No.CompareTo(listObj[lastList].F_Kanban_No) <= 0);
                }
                //if (!string.IsNullOrWhiteSpace(listObj[0].F_Store_Code) && (!string.IsNullOrWhiteSpace(listObj[lastList].F_Store_Code)))
                //{
                //    dbObj = dbObj.Where(x => x.F_Store_Code.CompareTo(listObj[0].F_Store_Code) >= 0
                //            && x.F_Store_Code.CompareTo(listObj[lastList].F_Store_Code) <= 0);
                //}
                if (!string.IsNullOrWhiteSpace(listObj[0].F_Part_No) && (!string.IsNullOrWhiteSpace(listObj[lastList].F_Part_No)))
                {
                    dbObj = dbObj.Where(x => x.F_Part_No.CompareTo(listObj[0].F_Part_No) >= 0
                        && x.F_Part_No.CompareTo(listObj[lastList].F_Part_No) <= 0
                        && x.F_Ruibetsu.CompareTo(listObj[0].F_Ruibetsu) >= 0
                        && x.F_Ruibetsu.CompareTo(listObj[lastList].F_Ruibetsu) <= 0);
                }

                DT_PartControl = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(dbObj));

                if (DT_PartControl.Rows.Count <= 0)
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
                    data = dbObj.Select(x => new
                    {
                        F_Supplier_Cd = x.F_Supplier_Cd.Trim(),
                        F_Supplier_Plant = x.F_Supplier_Plant,
                        F_Kanban_No = x.F_Kanban_No.Trim(),
                        F_Store_Code = x.F_Store_Code.Trim(),
                        F_Part_No = x.F_Part_No.Trim(),
                        F_Ruibetsu = x.F_Ruibetsu,
                    }).Distinct().AsEnumerable()
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

        [HttpPost]
        public async Task<IActionResult> UploadImportToKanbanPlan()
        {
            using var _kb3Trans = _KB3Context.Database.BeginTransaction();

            try
            {
                _kb3Trans.CreateSavepoint("Star UploadToKanbanPlan");
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return Unauthorized(new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message,
                    });
                }

                var listObj = _KB3Context.TMP_Planning_Order.Where(x => x.F_Plant == _BearerClass.Plant
                                   && x.F_Create_By == _BearerClass.UserCode).ToList();

                foreach (var obj in listObj)
                {
                    var propList = obj.GetType().GetProperties()
                        .Where(x => x.Name.StartsWith("F_Trip")).ToList();

                    int cycle = int.Parse(obj.F_Cycle.Substring(2, 2));

                    foreach (var prop in propList)
                    {
                        if (prop.Name.StartsWith("F_Trip"))
                        {
                            string _deliveryTrip = prop.Name.Substring(6);

                            if (int.Parse(_deliveryTrip) > cycle)
                            {
                                break;
                            }

                            int _orderKB = int.Parse(prop.GetValue(obj).ToString());

                            string sql = $"SELECT F_Lock AS VALUE FROM TB_calculate_volume " +
                                $"WHERE F_Supplier_Code = '{obj.F_Supplier_Code}' AND F_Supplier_Plant = '{obj.F_Supplier_Plant}' " +
                                $"AND F_Store_Code = '{obj.F_Store_Code}' AND F_Kanban_No = '{obj.F_Kanban_No}' " +
                                $"AND F_Part_No = '{obj.F_Part_No}' AND F_Ruibetsu= '{obj.F_Ruibetsu}' " +
                                $"AND F_Delivery_Date = '{obj.F_Delivery_Date}' AND F_Process_Round = '{_deliveryTrip.ToString()}' ";

                            string _lock = _KB3Context.Database.SqlQueryRaw<string?>(sql).FirstOrDefault();


                            TB_Kanban_Planning planObj = new TB_Kanban_Planning
                            {
                                F_Create_By = obj.F_Create_By,
                                F_Create_Date = obj.F_Create_Date.Value,
                                F_Update_By = obj.F_Create_By,
                                F_Update_Date = obj.F_Create_Date.Value,
                                F_Plant = obj.F_Plant,
                                F_Supplier_Code = obj.F_Supplier_Code,
                                F_Supplier_Plant = obj.F_Supplier_Plant,
                                F_Store_Code = obj.F_Store_Code,
                                F_Kanban_No = obj.F_Kanban_No,
                                F_Part_No = obj.F_Part_No,
                                F_Ruibetsu = obj.F_Ruibetsu,
                                F_Status = "0",
                                F_Delivery_Date = obj.F_Delivery_Date,
                                F_Delivery_Trip = _deliveryTrip.ToString(),
                                F_Order_PCS = (_orderKB * obj.F_Qty.Value),
                                F_Order_KB = _orderKB,
                                F_Qty_Pack = obj.F_Qty.Value,
                                F_Cycle = obj.F_Cycle,
                            };

                            var exist = _KB3Context.TB_Kanban_Planning.Where(x => x.F_Plant == planObj.F_Plant
                                    && x.F_Supplier_Code == planObj.F_Supplier_Code
                                    && x.F_Supplier_Plant == planObj.F_Supplier_Plant
                                    && x.F_Store_Code == planObj.F_Store_Code
                                    && x.F_Kanban_No == planObj.F_Kanban_No
                                    && x.F_Part_No == planObj.F_Part_No
                                    && x.F_Ruibetsu == planObj.F_Ruibetsu
                                    && x.F_Delivery_Date == planObj.F_Delivery_Date
                                    && x.F_Delivery_Trip == planObj.F_Delivery_Trip).AsNoTracking().SingleOrDefault();

                            if (exist == null && (_lock == null || _lock == "0"))
                            {
                                _KB3Context.TB_Kanban_Planning.Add(planObj);
                                _log.WriteLogMsg($"TB_Kanban_Planning Insert To => : {JsonConvert.SerializeObject(planObj)}");
                                await _KB3Context.SaveChangesAsync();
                            }
                            else
                            {
                                if (exist != null && (_lock == "0" || _lock == null))
                                {
                                    _KB3Context.TB_Kanban_Planning.Update(planObj);
                                    _log.WriteLogMsg($"TB_Kanban_Planning Update To => : {JsonConvert.SerializeObject(planObj)}");
                                    await _KB3Context.SaveChangesAsync();
                                }
                                else
                                {
                                    _log.WriteLogMsg($"Lock Data : {JsonConvert.SerializeObject(planObj)}");
                                }
                            }
                        }
                    }
                }
                _kb3Trans.Commit();
                _log.WriteLogMsg($" Upload Import To Kanban Planning : {JsonConvert.SerializeObject(listObj, Formatting.Indented)}");

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Imported",
                });

            }
            catch (Exception ex)
            {
                _log.WriteLogMsg($"UploadImportToKanbanPlan ERROR!!! : {ex.Message}");
                _kb3Trans.RollbackToSavepoint("Star UploadToKanbanPlan");
                return StatusCode(500, new
                {
                    status = "500",
                    message = "Unexpected error occurred",
                    error = ex.Message
                });
            }
        }
    }
}
