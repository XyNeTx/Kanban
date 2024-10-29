using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNMS013Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _IEmail;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNMS013Controller
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

        public string now = DateTime.Now.ToString("yyyyMMdd");

        [HttpGet]
        public IActionResult GetList(string supplier,string? store, string? typeOrder)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return Unauthorized();
                }

                string accessPPM = _FillDT.ppmConnect();


                string sql = $@"SELECT  RTRIM(K.F_Plant) AS F_Plant,RTRIM(K.F_Supplier_Cd)+'-'+RTRIM(K.F_Supplier_Plant) AS F_Supplier_Code 
                                ,RTRIM(S.F_name) AS F_name 
                                ,RIGHT('00'+ CONVERT(VARCHAR,S.F_Cycle_A),2) 
                                +'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_B),2) 
                                +'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_C),2) AS F_Cycle 
                                ,RTRIM(K.F_Part_No)+'-'+RTRIM(K.F_Ruibetsu) AS F_Part_No 
                                ,RTRIM(K.F_Kanban_No) AS F_Kanban_No 
                                ,RTRIM(C.F_Part_nm) AS F_Part_nm 
                                ,RTRIM(K.F_Store_Code) AS F_Store_Code 
                                ,SUBSTRING(K.F_Start_Date,7,2)+'/'+SUBSTRING(K.F_Start_Date,5,2)+'/'+SUBSTRING(K.F_Start_Date,1,4) AS F_Start_Date 
                                ,SUBSTRING(K.F_End_Date,7,2)+'/'+SUBSTRING(K.F_End_Date,5,2)+'/'+SUBSTRING(K.F_End_Date,1,4) AS F_End_Date 
                                ,RTRIM(K.F_Type_Order) AS F_Type_Order,RTRIM(tmk.F_Supply_Code) AS F_Dock_Code,RTRIM(K.F_PDS_Group) AS F_PDS_Group 
                                FROM TB_MS_PartOrder K INNER JOIN {accessPPM}.[dbo].[T_Construction] C 
                                ON K.F_Part_No = C.F_Part_no collate Thai_CI_AS 
                                AND K.F_Ruibetsu = C. F_Ruibetsu collate Thai_CI_AS 
                                AND K.F_Store_Code = C.F_Store_cd collate Thai_CI_AS 
                                INNER JOIN {accessPPM}.[dbo].[T_Supplier_ms] S 
                                ON K.F_Supplier_Cd = S.F_supplier_cd COLLATE SQL_Latin1_General_CP1_CI_AI 
                                AND K.F_Supplier_Plant = S.F_Plant_cd COLLATE SQL_Latin1_General_CP1_CI_AI 
                                AND K.F_Store_Code = S.F_Store_cd collate Thai_CI_AS 
                                INNER JOIN TB_MS_Kanban tmk ON K.F_Plant = tmk.F_Plant AND K.F_Supplier_Cd = tmk.F_Supplier_Code 
                                AND K.F_Supplier_Plant = tmk.F_Supplier_Plant AND K.F_Store_Code = tmk.F_Store_Code AND K.F_Store_Code = tmk.F_Store_Code 
                                AND K.F_Part_No = tmk.F_Part_No AND K.F_Ruibetsu = tmk.F_Ruibetsu 
                                WHERE S.F_TC_Str <= convert(char(8),getdate(),112) 
                                AND S.F_TC_End >= convert(char(8),getdate(),112)  
                                AND C.F_Local_Str <= convert(char(8),getdate(),112) 
                                AND C.F_Local_End >= convert(char(8),getdate(),112) ";

                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    sql += $@"AND K.F_Supplier_Cd = '{supplier.Split("-")[0]}' AND K.F_Supplier_Plant = '{supplier.Split("-")[1]}' ";
                }
                if (!string.IsNullOrWhiteSpace(store))
                {
                    sql += $@"AND K.F_Store_Code = '{store}' ";
                }
                if (!string.IsNullOrWhiteSpace(typeOrder))
                {
                    sql += $@"AND K.F_Type_Order = '{typeOrder}' ";
                }

                sql += "GROUP BY RTRIM(K.F_Plant),RTRIM(K.F_Supplier_Cd)+'-'+RTRIM(K.F_Supplier_Plant) ,RTRIM(S.F_name) " +
                    ",RIGHT('00'+ CONVERT(VARCHAR,S.F_Cycle_A),2) +'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_B),2) +'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_C),2) " +
                    ",RTRIM(K.F_Part_No)+'-'+RTRIM(K.F_Ruibetsu) " +
                    ",RTRIM(K.F_Kanban_No) ,RTRIM(C.F_Part_nm) ,RTRIM(K.F_Kanban_No),RTRIM(K.F_Store_Code) " +
                    ",SUBSTRING(K.F_Start_Date,7,2)+'/'+SUBSTRING(K.F_Start_Date,5,2)+'/'+SUBSTRING(K.F_Start_Date,1,4) " +
                    ",SUBSTRING(K.F_End_Date,7,2)+'/'+SUBSTRING(K.F_End_Date,5,2)+'/'+SUBSTRING(K.F_End_Date,1,4) " +
                    ",RTRIM(K.F_Type_Order),RTRIM(tmk.F_Supply_Code),RTRIM(K.F_PDS_Group) " +
                    "ORDER BY F_Supplier_Code, F_name, F_Cycle, F_Part_No, F_Kanban_No " +
                    ",F_Part_nm, F_Store_Code, F_Start_Date, F_End_Date, F_Type_Order ";

                var dt = _FillDT.ExecuteSQL(sql);
                if (dt.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        message = "Data Not Found!"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Data Found!",
                    data = JsonConvert.SerializeObject(dt),
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplier(string action, string? kanban, string? store, string? partNo)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return Unauthorized();
                }

                if (action.ToLower() == "new")
                {
                    var supplier = _PPM3Context.T_Construction
                        .Where(x => x.F_Local_Str.CompareTo(now) <= 0
                        && x.F_Local_End.CompareTo(now) >= 0
                        && x.F_Store_cd.StartsWith(_BearerClass.Plant)).AsQueryable();

                    if (!string.IsNullOrWhiteSpace(kanban))
                    {
                        supplier = supplier.Where(x => x.F_Sebango == kanban.Substring(1, 3));
                    }

                    if (!string.IsNullOrWhiteSpace(store))
                    {
                        supplier = supplier.Where(x => x.F_Store_cd == store);
                    }

                    if (!string.IsNullOrWhiteSpace(partNo))
                    {
                        supplier = supplier.Where(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == partNo);
                    }

                    var data = await supplier.Select(x => new
                    {
                        F_Supplier = x.F_supplier_cd.Trim() + "-" + x.F_plant.ToString().Trim(),
                    }).ToListAsync();


                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        message = "Data Found!",
                        data = data.DistinctBy(x => x.F_Supplier).OrderBy(x => x.F_Supplier)
                    });
                }
                else
                {

                    var supplier = _KB3Context.TB_MS_PartOrder
                        .Where(x => x.F_Start_Date.CompareTo(now) <= 0
                            && x.F_End_Date.CompareTo(now) >= 0
                            && x.F_Plant == _BearerClass.Plant[0].ToString()).AsQueryable();

                    if (!string.IsNullOrWhiteSpace(kanban))
                    {
                        supplier = supplier.Where(x => x.F_Kanban_No == kanban);
                    }
                    if (!string.IsNullOrWhiteSpace(store))
                    {
                        supplier = supplier.Where(x => x.F_Store_Code == store);
                    }
                    if (!string.IsNullOrWhiteSpace(partNo))
                    {
                        supplier = supplier.Where(x => x.F_Part_No.Trim() +"-"+x.F_Ruibetsu == partNo);
                    }

                    var data = await supplier.Select(x => new
                    {
                        F_Supplier = x.F_Supplier_Cd.Trim() + "-" + x.F_Supplier_Plant.ToString().Trim(),
                    }).ToListAsync();
                    
                    
                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        message = "Data Found!",
                        data = data.DistinctBy(x => x.F_Supplier).OrderBy(x => x.F_Supplier)
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetKanban(string action, string? supplier, string? store, string? partNo)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return Unauthorized();
                }

                if (action.ToLower() == "new")
                {
                    var kanban = _PPM3Context.T_Construction
                        .Where(x => x.F_Local_Str.CompareTo(now) <= 0
                        && x.F_Local_End.CompareTo(now) >= 0
                        && x.F_Store_cd.StartsWith(_BearerClass.Plant)).AsQueryable();

                    if (!string.IsNullOrWhiteSpace(supplier))
                    {
                        kanban = kanban.Where(x => x.F_supplier_cd + "-" + x.F_plant == supplier);
                    }

                    if (!string.IsNullOrWhiteSpace(store))
                    {
                        kanban = kanban.Where(x => x.F_Store_cd == store);
                    }

                    if (!string.IsNullOrWhiteSpace(partNo))
                    {
                        kanban = kanban.Where(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == partNo);
                    }

                    var data = await kanban.Select(x => new
                    {
                        F_Kanban = ("0000" + x.F_Sebango).Substring(x.F_Sebango.Length,4)
                    }).ToListAsync();


                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        message = "Data Found!",
                        data = data.DistinctBy(x => x.F_Kanban).OrderBy(x => x.F_Kanban)
                    });
                }
                else
                {

                    var kanban = _KB3Context.TB_MS_PartOrder
                        .Where(x => x.F_Start_Date.CompareTo(now) <= 0
                            && x.F_End_Date.CompareTo(now) >= 0
                            && x.F_Plant == _BearerClass.Plant[0].ToString()).AsQueryable();

                    if (!string.IsNullOrWhiteSpace(supplier))
                    {
                        kanban = kanban.Where(x => x.F_Supplier_Cd.Trim() + "-" + x.F_Supplier_Plant == supplier);
                    }
                    if (!string.IsNullOrWhiteSpace(store))
                    {
                        kanban = kanban.Where(x => x.F_Store_Code == store);
                    }
                    if (!string.IsNullOrWhiteSpace(partNo))
                    {
                        kanban = kanban.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == partNo);
                    }

                    var data = await kanban.Select(x => new
                    {
                        F_Kanban = ("0000" + x.F_Kanban_No).Substring(x.F_Kanban_No.Length, 4)
                    }).ToListAsync();


                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        message = "Data Found!",
                        data = data.DistinctBy(x => x.F_Kanban).OrderBy(x => x.F_Kanban)
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
            }

        }


        [HttpGet]
        public async Task<IActionResult> GetStore(string action, string? supplier, string? kanban, string? partNo)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return Unauthorized();
                }

                if (action.ToLower() == "new")
                {
                    var store = _PPM3Context.T_Construction
                        .Where(x => x.F_Local_Str.CompareTo(now) <= 0
                                && x.F_Local_End.CompareTo(now) >= 0
                                && x.F_Store_cd.StartsWith(_BearerClass.Plant)).AsQueryable();

                    if (!string.IsNullOrWhiteSpace(supplier))
                    {
                        store = store.Where(x => x.F_supplier_cd + "-" + x.F_plant == supplier);
                    }

                    if (!string.IsNullOrWhiteSpace(kanban))
                    {
                        store = store.Where(x => x.F_Sebango == kanban.Substring(1, 3));
                    }

                    if (!string.IsNullOrWhiteSpace(partNo))
                    {
                        store = store.Where(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == partNo);
                    }

                    var data = await store.Select(x => new
                    {
                        F_Store = x.F_Store_cd
                    }).ToListAsync();

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        message = "Data Found!",
                        data = data.DistinctBy(x => x.F_Store).OrderBy(x => x.F_Store)
                    });
                }
                else
                {
                    var store = _KB3Context.TB_MS_PartOrder
                        .Where(x => x.F_Start_Date.CompareTo(now) <= 0
                                && x.F_End_Date.CompareTo(now) >= 0
                                && x.F_Plant == _BearerClass.Plant[0].ToString()).AsQueryable();

                    if (!string.IsNullOrWhiteSpace(supplier))
                    {
                        store = store.Where(x => x.F_Supplier_Cd.Trim() + "-" + x.F_Supplier_Plant == supplier);
                    }
                    if (!string.IsNullOrWhiteSpace(kanban))
                    {
                        store = store.Where(x => x.F_Kanban_No == kanban);
                    }
                    if (!string.IsNullOrWhiteSpace(partNo))
                    {
                        store = store.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == partNo);
                    }

                    var data = await store.Select(x => new
                    {
                        F_Store = x.F_Store_Code
                    }).ToListAsync();

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        message = "Data Found!",
                        data = data.DistinctBy(x => x.F_Store).OrderBy(x => x.F_Store)
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
            }


        }


        [HttpGet]
        public async Task<IActionResult> GetPartNo(string action, string? supplier, string? kanban, string? store)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return Unauthorized();
                }

                if (action.ToLower() == "new")
                {
                    var partNo = _PPM3Context.T_Construction
                        .Where(x => x.F_Local_Str.CompareTo(now) <= 0
                                && x.F_Local_End.CompareTo(now) >= 0
                                && x.F_Store_cd.StartsWith(_BearerClass.Plant)).AsQueryable();

                    if (!string.IsNullOrWhiteSpace(supplier))
                    {
                        partNo = partNo.Where(x => x.F_supplier_cd + "-" + x.F_plant == supplier);
                    }

                    if (!string.IsNullOrWhiteSpace(kanban))
                    {
                        partNo = partNo.Where(x => x.F_Sebango == kanban.Substring(1, 3));
                    }

                    if (!string.IsNullOrWhiteSpace(store))
                    {
                        partNo = partNo.Where(x => x.F_Store_cd == store);
                    }

                    var data = await partNo.Select(x => new
                    {
                        F_PartNo = x.F_Part_no.Trim() + "-" + x.F_Ruibetsu
                    }).ToListAsync();

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        message = "Data Found!",
                        data = data.DistinctBy(x => x.F_PartNo).OrderBy(x => x.F_PartNo)
                    });
                }
                else
                {
                    var partNo = _KB3Context.TB_MS_PartOrder
                        .Where(x => x.F_Start_Date.CompareTo(now) <= 0
                                && x.F_End_Date.CompareTo(now) >= 0
                                && x.F_Plant == _BearerClass.Plant[0].ToString()).AsQueryable();

                    if (!string.IsNullOrWhiteSpace(supplier))
                    {
                        partNo = partNo.Where(x => x.F_Supplier_Cd + "-" + x.F_Supplier_Plant == supplier);
                    }
                    if (!string.IsNullOrWhiteSpace(kanban))
                    {
                        partNo = partNo.Where(x => x.F_Kanban_No == kanban);
                    }
                    if (!string.IsNullOrWhiteSpace(store))
                    {
                        partNo = partNo.Where(x => x.F_Store_Code == store);
                    }

                    var data = await partNo.Select(x => new
                    {
                        F_PartNo = x.F_Part_No.Trim() + "-" + x.F_Ruibetsu
                    }).ToListAsync();

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        message = "Data Found!",
                        data = data.DistinctBy(x => x.F_PartNo).OrderBy(x => x.F_PartNo)
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetPartNoDetail(string partNo, string? supplier, string? kanban, string? store)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return Unauthorized();
                }

                var partDetail = _PPM3Context.T_Construction
                    .Where(x => x.F_Local_Str.CompareTo(now) <= 0
                            && x.F_Local_End.CompareTo(now) >= 0
                            && x.F_Store_cd.StartsWith(_BearerClass.Plant)
                            && x.F_Cycle_A == '1'
                            && x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == partNo).AsQueryable();

                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    partDetail = partDetail.Where(x => x.F_supplier_cd + "-" + x.F_plant == supplier);
                }
                if (!string.IsNullOrWhiteSpace(kanban))
                {
                    partDetail = partDetail.Where(x => x.F_Sebango == kanban.Substring(1, 3));
                }
                if (!string.IsNullOrWhiteSpace(store))
                {
                    partDetail = partDetail.Where(x => x.F_Store_cd == store);
                }

                if (partDetail.Count() == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        message = "Data Not Found!"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Data Found!",
                    data = await partDetail.Select(x => new
                    {
                        x.F_Part_nm,
                    }).FirstOrDefaultAsync()
                });

            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Save([FromQuery] string action,[FromBody] TB_MS_PartOrder obj)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return Unauthorized();
                }

                if (ModelState.IsValid)
                {

                    if(obj.F_Start_Date.CompareTo(obj.F_End_Date) > 0)
                    {
                        return BadRequest(new
                        {
                            status = "400",
                            response = "Bad Request",
                            message = "Invalid Data!",
                            error = "Please Select Start Date Less than End Date Before Process Data!"
                        });
                    }

                    var partOrder = _KB3Context.TB_MS_PartOrder
                        .Where(x => x.F_Plant == obj.F_Plant
                        && x.F_Supplier_Cd == obj.F_Supplier_Cd
                        && x.F_Supplier_Plant == obj.F_Supplier_Plant
                        && x.F_Store_Code == obj.F_Store_Code
                        && x.F_Part_No == obj.F_Part_No
                        && x.F_Ruibetsu == obj.F_Ruibetsu
                        && x.F_Kanban_No == obj.F_Kanban_No
                        && x.F_Start_Date == obj.F_Start_Date).FirstOrDefault();

                    if (partOrder != null)
                    {
                        if (action == "update")
                        {
                            partOrder.F_Update_By = _BearerClass.UserCode;
                            partOrder.F_Update_Date = DateTime.Now;
                            partOrder.F_End_Date = obj.F_End_Date;
                            partOrder.F_PDS_Group = obj.F_PDS_Group;
                            partOrder.F_Type_Order = obj.F_Type_Order;
                            _KB3Context.TB_MS_PartOrder.Update(partOrder);

                            _log.WriteLogMsg($"TB_MS_PartOrder(Save) Update : {JsonConvert.SerializeObject(partOrder)}");
                        }
                        else if (action == "delete")
                        {
                            _KB3Context.TB_MS_PartOrder.Remove(partOrder);
                            _log.WriteLogMsg($"TB_MS_PartOrder(Save) Delete : {JsonConvert.SerializeObject(partOrder)}");
                        }
                    }
                    else if (action == "new")
                    {
                        if (obj.F_Start_Date.CompareTo(DateTime.Now.ToString("yyyyMMdd")) < 0)
                        {
                            return BadRequest(new
                            {
                                status = "400",
                                response = "Bad Request",
                                message = "Invalid Data!",
                                error = "Please Select Start Date From Present OR More Before Process Data!"
                            });
                        }

                        obj.F_Create_By = _BearerClass.UserCode;
                        obj.F_Create_Date = DateTime.Now;
                        obj.F_Update_By = _BearerClass.UserCode;
                        obj.F_Update_Date = DateTime.Now;
                        _KB3Context.TB_MS_PartOrder.Add(obj);
                        _log.WriteLogMsg($"TB_MS_PartOrder(Save) Insert : {JsonConvert.SerializeObject(obj)}");
                    }
                    else
                    {
                        throw new CustomHttpException(400,"Bad Request");
                    }
                    
                    
                    await _KB3Context.SaveChangesAsync();
                    _log.WriteLogMsg($"TB_MS_PartOrder(Save) Commit : {JsonConvert.SerializeObject(obj)}");

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        message = "Data Saved!"
                    });
                }

                return BadRequest(new
                {
                    status = "400",
                    response = "Bad Request",
                    message = "Invalid Data!",
                    error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
                throw;
            }
        }
    }
}
