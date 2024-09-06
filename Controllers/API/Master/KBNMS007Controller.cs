using HINOSystem.Context;
using HINOSystem.Libs;
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
    public class KBNMS007Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _IEmail;

        public KBNMS007Controller
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
                
                var dbObj = _KB3Context.TB_Kanban_Add.Where(x=>x.F_Plant == _BearerClass.Plant).AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    dbObj = dbObj.Where(x => x.F_Kanban_No == F_Kanban_No);
                }
                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    dbObj = dbObj.Where(x => x.F_Supplier_Code == F_Supplier_Code.Substring(0,4)
                    && x.F_Supplier_Plant == F_Supplier_Code.Substring(5, 1));
                }
                if (!string.IsNullOrWhiteSpace(F_Store_Code))
                {
                    dbObj = dbObj.Where(x => x.F_Store_Code == F_Store_Code);
                }
                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    dbObj = dbObj.Where(x => x.F_Part_No == F_Part_No.Substring(0,10)
                    && x.F_Ruibetsu == F_Part_No.Substring(11,2));
                }


                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = await dbObj.FirstOrDefaultAsync()

                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error !",
                    error = ex.Message
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Save(TB_Kanban_Add obj)
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

                string supplier = obj.F_Supplier_Code + "-" + obj.F_Supplier_Plant;

                if (await IsProcessDatePast(obj.F_Delivery_Date, int.TryParse(obj.F_Delivery_Trip,out int trip) ? trip : 24, supplier, obj.F_Kanban_No))
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


                var dbObj = _KB3Context.TB_Kanban_Add.FirstOrDefault(x => x.F_Kanban_No == obj.F_Kanban_No && x.F_Plant == _BearerClass.Plant
                && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant && x.F_Store_Code == obj.F_Store_Code
                && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu);

                if(dbObj == null)
                {
                    obj.F_Status = "0";
                    obj.F_Create_By = _BearerClass.UserCode;
                    obj.F_Create_Date = DateTime.Now;
                    obj.F_Update_By = _BearerClass.UserCode;
                    obj.F_Update_Date = DateTime.Now;
                    obj.F_Start_Date = "";
                    obj.F_Start_Shift = "";
                    obj.F_Finish_Date = null;
                    obj.F_Finish_Trip = null;

                    _KB3Context.TB_Kanban_Add.Add(obj);
                }
                else
                {
                    _KB3Context.TB_Kanban_Add.Remove(dbObj);

                    obj.F_Create_By = dbObj.F_Create_By;
                    obj.F_Create_Date = dbObj.F_Create_Date;
                    obj.F_KB_Remain = dbObj.F_KB_Remain;
                    dbObj = obj;
                    dbObj.F_Status = "0";
                    dbObj.F_Update_By = _BearerClass.UserCode;
                    dbObj.F_Update_Date = DateTime.Now;
                    dbObj.F_Start_Date = "";
                    dbObj.F_Start_Shift = "";
                    dbObj.F_Finish_Date = null;
                    dbObj.F_Finish_Trip = null;

                    _KB3Context.TB_Kanban_Add.Add(dbObj);
                }

                await _KB3Context.SaveChangesAsync();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Saved",
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error !",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Start (TB_Kanban_Add obj)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if(_BearerClass.Status == 401)
                {
                    return Unauthorized(new
                    {
                        status = "401",
                        response = "Unauthorize",
                        title = "Unauthorize",
                        message = "Please Login then Try Again",
                    });
                }

                var dbObj = _KB3Context.TB_Kanban_Add.FirstOrDefault(x => x.F_Kanban_No == obj.F_Kanban_No 
                && x.F_Plant == _BearerClass.Plant && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu
                && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant && x.F_Store_Code == obj.F_Store_Code);

                if (dbObj == null)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Data Not Found"
                    });
                }

                dbObj.F_Status = "1";
                dbObj.F_KB_Remain = dbObj.F_KB_Add;
                dbObj.F_Update_Date = DateTime.Now;
                dbObj.F_Update_By = _BearerClass.UserCode;
                _KB3Context.TB_Kanban_Add.Update(dbObj);
                await _KB3Context.SaveChangesAsync();
                _log.WriteLogMsg($"KBNMS007 : Start | Obj : {JsonConvert.SerializeObject(dbObj)}");

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Process Started",
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

        public async Task<IActionResult> Stop (TB_Kanban_Add obj)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return Unauthorized(new
                    {
                        status = "401",
                        response = "Unauthorize",
                        title = "Unauthorize",
                        message = "Please Login then Try Again",
                    });
                }

                var dbObj = _KB3Context.TB_Kanban_Add.FirstOrDefault(x => x.F_Kanban_No == obj.F_Kanban_No
                && x.F_Plant == _BearerClass.Plant && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu
                && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant && x.F_Store_Code == obj.F_Store_Code);

                if (dbObj == null)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Data Not Found"
                    });
                }

                dbObj.F_Status = "0";
                dbObj.F_Update_Date = DateTime.Now;
                dbObj.F_Update_By = _BearerClass.UserCode;
                _KB3Context.TB_Kanban_Add.Update(dbObj);
                await _KB3Context.SaveChangesAsync();
                _log.WriteLogMsg($"KBNMS007 : Stop | Obj : {JsonConvert.SerializeObject(dbObj)}");

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Process Stopped",
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

        public async Task<bool> IsProcessDatePast(string date, int trip, string supplier, string kanban)
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

                if (supplier != "9999")
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
                    for (int i = 0; i < _dt.Rows.Count; i++)
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

        public async Task<bool> IsProcessDateHoliday(string date, string store)
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

    }
}
