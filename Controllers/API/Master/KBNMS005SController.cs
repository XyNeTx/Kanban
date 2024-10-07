using Azure;
using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master;
using KANBAN.Models.PPM3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System.Data;

namespace KANBAN.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNMS005SController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly KB3Context _KB3Context;
        private readonly SerilogLibs _Log;
        private readonly FillDataTable _FillDT;

        public KBNMS005SController(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context,
            PPM3Context pPM3Context,
            SerilogLibs serilogLibs,
            FillDataTable fillDataTable
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _PPM3Context = pPM3Context;
            _Log = serilogLibs;
            _FillDT = fillDataTable;
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplierCode(bool IsCmdNew)
        {
            try
            {
                
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });

                string now = DateTime.Now.ToString("yyyyMMdd");

                if (IsCmdNew)
                {
                    var data = _KB3Context.TB_MS_PartOrder
                        .Where(x => x.F_Start_Date.CompareTo(now) <= 0 && x.F_End_Date.CompareTo(now) >= 0 && x.F_Store_Code.StartsWith(_BearerClass.Plant))
                        .Select(x => new
                        {
                            F_Supplier_Code = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant
                        })
                        .AsNoTracking().AsEnumerable()
                        .DistinctBy(x => x.F_Supplier_Code)
                        .OrderBy(x => x.F_Supplier_Code)
                        .ToList();

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        title = "Success",
                        message = "Data Found",
                        data = data
                    });
                }
                else
                {
                    var data = _KB3Context.TB_BL_SET
                        .Where(x => x.F_Store_Cd.StartsWith(_BearerClass.Plant))
                        .Select(x => new
                        {
                            F_Supplier_Code = x.F_Sup_Cd + "-" + x.F_Sup_Plant
                        })
                        .AsNoTracking().AsEnumerable()
                        .DistinctBy(x => x.F_Supplier_Code)
                        .OrderBy(x => x.F_Supplier_Code)
                        .ToList();

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        title = "Success",
                        message = "Data Found",
                        data = data
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.Message.ToString(),
                    error = ex.Message.ToString()
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreCode(bool IsCmdNew, string? F_Supplier_Code)
        {
            try
            {
                
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });

                string now = DateTime.Now.ToString("yyyyMMdd");
                string F_Supplier_Name = "";

                if (IsCmdNew)
                {
                    var data = _KB3Context.TB_MS_PartOrder
                        .Where(x => x.F_Start_Date.CompareTo(now) <= 0 && x.F_End_Date.CompareTo(now) >= 0 && x.F_Store_Code.StartsWith(_BearerClass.Plant))
                        .Select(x => new
                        {
                            F_Store_Code = x.F_Store_Code.Trim(),
                            F_Supplier_Code = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant
                        })
                        .AsNoTracking().AsEnumerable()
                        .ToList();


                    if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                    {
                        data = data.Where(x => x.F_Supplier_Code == F_Supplier_Code).ToList();
                        char Sup_Plant = F_Supplier_Code.Split("-")[1][0];
                        string Supplier_Code = F_Supplier_Code.Split("-")[0];

                        var Supplier_Name = _PPM3Context.T_Supplier_MS
                            .Where(x => x.F_supplier_cd == Supplier_Code && x.F_Plant_cd == Sup_Plant
                                && x.F_TC_Str.CompareTo(now) <= 0 && x.F_TC_End.CompareTo(now) >= 0
                                && x.F_Store_cd.StartsWith(_BearerClass.Plant))
                            .Select(x => new
                            {
                                F_name = "(" + x.F_short_name.Trim() + ") " + x.F_name.Trim()
                            })
                            .FirstOrDefault();

                        F_Supplier_Name = Supplier_Name.F_name;
                    }

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        title = "Success",
                        message = "Data Found",
                        data = data.DistinctBy(x => x.F_Store_Code)
                        .OrderBy(x => x.F_Store_Code),
                        supplier_name = F_Supplier_Name
                    });
                }
                else
                {
                    var data = _KB3Context.TB_BL_SET
                        .Where(x => x.F_Store_Cd.StartsWith(_BearerClass.Plant))
                        .Select(x => new
                        {
                            F_Store_Code = x.F_Store_Cd.Trim(),
                            F_Supplier_Code = x.F_Sup_Cd + "-" + x.F_Sup_Plant
                        })
                        .AsNoTracking().AsEnumerable()
                        .ToList();

                    if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                    {
                        data = data.Where(x => x.F_Supplier_Code == F_Supplier_Code).ToList();
                        char Sup_Plant = F_Supplier_Code.Split("-")[1][0];
                        string Supplier_Code = F_Supplier_Code.Split("-")[0];

                        var Supplier_Name = _PPM3Context.T_Supplier_MS
                            .Where(x => x.F_supplier_cd == Supplier_Code && x.F_Plant_cd == Sup_Plant
                                && x.F_TC_Str.CompareTo(now) <= 0 && x.F_TC_End.CompareTo(now) >= 0
                                && x.F_Store_cd.StartsWith(_BearerClass.Plant))
                            .Select(x => new
                            {
                                F_name = "(" + x.F_short_name.Trim() + ") " + x.F_name.Trim()
                            })
                            .FirstOrDefault();

                        F_Supplier_Name = Supplier_Name.F_name;
                    }

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        title = "Success",
                        message = "Data Found",
                        data = data.DistinctBy(x => x.F_Store_Code)
                        .OrderBy(x => x.F_Store_Code),
                        supplier_name = F_Supplier_Name
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.Message.ToString(),
                    error = ex.Message.ToString()
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDateAndShift()
        {
            try
            {
                //
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });
                string Shift = "";

                string now = DateTime.Now.ToString("yyyyMMdd");
                DateTime time = DateTime.Now;
                if (time.Hour > 19)
                {
                    if (time.Minute > 30)
                    {
                        Shift = "N";
                    }
                }
                else Shift = "D";

                DataTable dt = _FillDT.ExecuteSQL(@$"SELECT TOP 1 F_YM+F_Day COLLATE SQL_Latin1_General_CP1_CI_AS AS F_Date, F_Shift
                    FROM  VW_Calendar_Shift WHERE F_YM+F_Day COLLATE SQL_Latin1_General_CP1_CI_AS+F_Shift < '{now}{Shift}' AND F_WorkCD = '1'
                    ORDER BY F_YM DESC,F_Day DESC,F_Shift DESC");

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = JsonConvert.SerializeObject(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.Message.ToString(),
                    error = ex.Message.ToString()
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchClick(bool IsCmdNew, string F_Date, string F_Shift, string F_Supplier_Code, string F_Store_Code)
        {
            try
            {
                
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });

                using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
                string _SQL = "";
                DataTable dt = new DataTable();

                _KB3Transaction.CreateSavepoint("BeginSearchClick");
                if (IsCmdNew)
                {
                    if (!F_Supplier_Code.Contains("9999"))
                    {
                        _SQL = $@"EXEC [dbo].[sp_ShowList_SetStockBalance] 'NEW',@p0,@p1,@p2,@p3,@p4";
                    }
                    else
                    {
                        _SQL = $@"EXEC [CKD_Inhouse].[sp_ShowList_SetStockBalance] 'NEW',@p0,@p1,@p2,@p3,@p4";
                    }
                }
                else
                {
                    if (!F_Supplier_Code.Contains("9999"))
                    {
                        _SQL = $@"EXEC [dbo].[sp_ShowList_SetStockBalance] 'UPDATE',@p0,@p1,@p2,@p3,@p4";
                    }
                    else
                    {
                        _SQL = $@"EXEC [CKD_Inhouse].[sp_ShowList_SetStockBalance] 'UPDATE',@p0,@p1,@p2,@p3,@p4";
                    }
                }

                dt = _FillDT.ExecuteSQL(_SQL, F_Date, F_Shift, F_Supplier_Code.Split("-")[0], F_Supplier_Code.Split("-")[1], F_Store_Code);

                if (dt.Rows.Count == 0) return NotFound(new
                {
                    status = "404",
                    response = "Not Found",
                    title = "Not Found",
                    message = "Data Not Found"
                });

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = JsonConvert.SerializeObject(dt)

                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.Message.ToString(),
                    error = ex.Message.ToString()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(List<TB_BL_SET> ListObj)
        {
            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });
                _KB3Transaction.CreateSavepoint("BeginSave");

                foreach (var each in ListObj)
                {
                    var existed = _KB3Context.TB_BL_SET.Where(x => x.F_Date == each.F_Date && x.F_Sup_Cd == each.F_Sup_Cd && x.F_Sup_Plant == each.F_Sup_Plant
                    && x.F_Store_Cd == each.F_Store_Cd && x.F_Sebango == each.F_Sebango.Substring(1, 3) && x.F_Shift == each.F_Shift && x.F_Part_No == each.F_Part_No && x.F_Ruibetsu == each.F_Ruibetsu)
                        .FirstOrDefault();

                    if (existed != null)
                    {
                        existed = each;
                        string KanbanNo = each.F_Sebango;
                        existed.F_Sebango = each.F_Sebango.Substring(1, 3);
                        existed.F_Update_By = _BearerClass.UserCode;
                        existed.F_Update_Date = DateTime.Now;
                        _KB3Context.TB_BL_SET.Update(existed);
                        _KB3Context.SaveChanges();

                        if (each.F_Sup_Cd == "9999")
                        {
                            await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [CKD_Inhouse].sp_setStockBalance @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10"
                                , _BearerClass.Plant, each.F_Sup_Cd, each.F_Sup_Plant, each.F_Part_No, each.F_Ruibetsu, KanbanNo, each.F_Store_Cd
                                , each.F_Date, each.F_Shift, each.F_BL, _BearerClass.UserCode);
                        }
                        else
                        {
                            await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [dbo].sp_setStockBalance @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10"
                                , _BearerClass.Plant, each.F_Sup_Cd, each.F_Sup_Plant, each.F_Part_No, each.F_Ruibetsu, KanbanNo, each.F_Store_Cd
                                , each.F_Date, each.F_Shift, each.F_BL, _BearerClass.UserCode);
                        }

                        string _logData = JsonConvert.SerializeObject(each);
                        _Log.WriteLog(_logData, _BearerClass.UserCode, _BearerClass.Device);
                    }
                    else
                    {
                        string KanbanNo = each.F_Sebango;
                        each.F_Sebango = each.F_Sebango.Substring(1, 3);
                        each.F_Update_By = _BearerClass.UserCode;
                        each.F_Update_Date = DateTime.Now;
                        _KB3Context.TB_BL_SET.Add(each);
                        _KB3Context.SaveChanges();

                        if (each.F_Sup_Cd == "9999")
                        {
                            await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [CKD_Inhouse].sp_setStockBalance @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10"
                            , _BearerClass.Plant, each.F_Sup_Cd, each.F_Sup_Plant, each.F_Part_No, each.F_Ruibetsu, KanbanNo, each.F_Store_Cd
                            , each.F_Date, each.F_Shift, each.F_BL, _BearerClass.UserCode);
                        }
                        else
                        {
                            await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [dbo].sp_setStockBalance @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10"
                            , _BearerClass.Plant, each.F_Sup_Cd, each.F_Sup_Plant, each.F_Part_No, each.F_Ruibetsu, KanbanNo, each.F_Store_Cd
                            , each.F_Date, each.F_Shift, each.F_BL, _BearerClass.UserCode);
                        }

                        string _logData = JsonConvert.SerializeObject(each);
                        _Log.WriteLog(_logData, _BearerClass.UserCode, _BearerClass.Device);
                    }
                }

                await _KB3Context.Database.ExecuteSqlRawAsync($"Exec [exec].spKBNMS005S_UPD_D '{_BearerClass.Plant}','{ListObj[0].F_Date}','{ListObj[0].F_Shift}','{ListObj[0].F_Sup_Cd + "-" + ListObj[0].F_Sup_Plant}','{ListObj[0].F_Store_Cd}'");


                _KB3Transaction.Commit();

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
                _KB3Transaction.RollbackToSavepoint("BeginSave");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.Message.ToString(),
                    error = ex.Message.ToString()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(TB_BL_SET Obj)
        {
            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });

                _KB3Transaction.CreateSavepoint("BeginDelete");

                var existed = _KB3Context.TB_BL_SET.Where(x => x.F_Date == Obj.F_Date && x.F_Sup_Cd == Obj.F_Sup_Cd && x.F_Sup_Plant == Obj.F_Sup_Plant
                    && x.F_Store_Cd == Obj.F_Store_Cd && x.F_Sebango == Obj.F_Sebango.Substring(1, 3) && x.F_Shift == Obj.F_Shift && x.F_Part_No == Obj.F_Part_No && x.F_Ruibetsu == Obj.F_Ruibetsu)
                        .FirstOrDefault();

                if (existed == null) throw new Exception("Data Not Found");

                TB_BL_SET_HISTORY_DELETE _DelObj = JsonConvert.DeserializeObject<TB_BL_SET_HISTORY_DELETE>(JsonConvert.SerializeObject(existed));

                if (_DelObj == null) throw new Exception("Can't Deserialize to Delete Object");

                var delExisted = _KB3Context.TB_BL_SET_HISTORY_DELETE
                        .Any(x => x.F_Date == _DelObj.F_Date && x.F_Sup_Cd == _DelObj.F_Sup_Cd && x.F_Sup_Plant == _DelObj.F_Sup_Plant
                        && x.F_Store_Cd == _DelObj.F_Store_Cd && x.F_Sebango == _DelObj.F_Sebango && x.F_Shift == _DelObj.F_Shift && x.F_Part_No == _DelObj.F_Part_No && x.F_Ruibetsu == _DelObj.F_Ruibetsu);

                if (delExisted) throw new Exception("Data Already Deleted");

                _KB3Context.TB_BL_SET_HISTORY_DELETE.Add(_DelObj);
                string _logData = JsonConvert.SerializeObject(_DelObj);
                _Log.WriteLog("INSERT TB_BL_SET_HISTORY_DELETE | " + _logData, _BearerClass.UserCode, _BearerClass.Device);

                _KB3Context.TB_BL_SET.Remove(existed);
                _logData = JsonConvert.SerializeObject(existed);
                _Log.WriteLog("DELETE TB_BL_SET | " + _logData, _BearerClass.UserCode, _BearerClass.Device);

                _KB3Context.SaveChanges();

                if (existed.F_Sup_Cd == "9999" || existed.F_Sup_Cd == "9995")
                {
                    await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [CKD_Inhouse].sp_set_StockBalance_Delete @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9",
                        _BearerClass.Plant, Obj.F_Sup_Cd, Obj.F_Sup_Plant, Obj.F_Part_No, Obj.F_Ruibetsu,Obj.F_Sebango,Obj.F_Store_Cd,Obj.F_Date,Obj.F_Shift,_BearerClass.UserCode);
                }
                else await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [dbo].sp_set_StockBalance_Delete @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9",
                        _BearerClass.Plant, Obj.F_Sup_Cd, Obj.F_Sup_Plant, Obj.F_Part_No, Obj.F_Ruibetsu,Obj.F_Sebango,Obj.F_Store_Cd,Obj.F_Date,Obj.F_Shift,_BearerClass.UserCode);

                _Log.WriteLog("Update TB_Calculate_D : Flag_Chg_BL_Stock ", _BearerClass.UserCode, _BearerClass.Device);

                _KB3Transaction.Commit();


                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Deleted",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.Message.ToString(),
                    error = ex.Message.ToString()
                });
            }
        }
    }
}
