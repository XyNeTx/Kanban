using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNIM012Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNIM012Controller
        (
            BearerClass bearerClass,
            PPM3Context ppm3Context,
            PPMInvenContext ppmInvenContext,
            KB3Context kb3Context,
            FillDataTable fillDataTable,
            SerilogLibs log,
            IEmailService emailService
        )
        {
            _BearerClass = bearerClass;
            _PPM3Context = ppm3Context;
            _PPMInvenContext = ppmInvenContext;
            _KB3Context = kb3Context;
            _FillDT = fillDataTable;
            _log = log;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Onload()
        {
            try
            {

                _BearerClass.Authentication(Request);
                if(_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                string sql = "Select Top 1 F_PO,F_Version From VW_MaxVersionForecast Order by substring(F_PO,1,6) desc,F_Version";

                DataTable dt = _FillDT.ExecuteSQLProcDB(sql);

                sql = "Select Top 1 F_Version,F_Production_Date,F_Revision_NO from TB_IMPORT_FORECAST " +
                    " Order by F_Production_Date desc,F_Version,F_Revision_NO desc";

                DataTable dt2 = _FillDT.ExecuteSQL(sql);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Loaded Successfully!",
                    data = new
                    {
                        maxVersion = dt.Rows[0]["F_Version"].ToString(),
                        maxPO = dt.Rows[0]["F_PO"].ToString(),
                        version = dt2.Rows[0]["F_Version"].ToString(),
                        productionDate = dt2.Rows[0]["F_Production_Date"].ToString(),
                        revisionNo = dt2.Rows[0]["F_Revision_NO"].ToString()
                    }
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
        public IActionResult GetSupplier()
        {
            try
            {

                _BearerClass.Authentication(Request);
                if(_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                string _sql = "Select distinct substring(F_Supplier_code,2,4)+'-'+ rtrim(F_supplier_plant) as F_Supplier " +
                    "From TB_Import_Forecast  Order by 1";

                DataTable dt = _FillDT.ExecuteSQL(_sql);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Get Supplier Successfully!",
                    data = JsonConvert.SerializeObject(dt)
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

        [HttpGet]
        public IActionResult GetKanban(string? supplier)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                string _sql = "Select distinct F_sebango  From TB_Import_Forecast ";

                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    _sql += " Where substring(F_Supplier_code,2,4)+'-'+ rtrim(F_supplier_plant) = '" + supplier + "'";
                }

                _sql += " Order by 1";

                DataTable dt = _FillDT.ExecuteSQL(_sql);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Get Kanban Successfully!",
                    data = JsonConvert.SerializeObject(dt)
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


        [HttpGet]
        public IActionResult GetStore(string? supplier,string? kanban)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                string _sql = "Select distinct F_Store_cd  From TB_Import_Forecast";
                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    _sql += " Where substring(F_Supplier_code,2,4)+'-'+ rtrim(F_supplier_plant) = '" + supplier + "'";
                }
                if(!string.IsNullOrWhiteSpace(kanban))
                {
                    if(_sql.Contains("Where"))
                    {
                        _sql += " And F_sebango = '" + kanban + "'";
                    }
                    else
                    {
                        _sql += " Where F_sebango = '" + kanban + "'";
                    }
                }

                _sql += " Order by 1";

                DataTable dt = _FillDT.ExecuteSQL(_sql);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Get Store Successfully!",
                    data = JsonConvert.SerializeObject(dt)
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

        [HttpGet]
        public IActionResult GetPart(string? supplier,string? kanban,string? store)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                string _sql = "Select distinct rtrim(F_PART_NO) +'-'+rtrim(F_Ruibetsu) as F_PART_NO " +
                    " From TB_Import_Forecast ";

                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    _sql += " Where substring(F_Supplier_code,2,4)+'-'+ rtrim(F_supplier_plant) = '" + supplier + "'";
                }

                if (!string.IsNullOrWhiteSpace(kanban))
                {
                    if(_sql.Contains("Where"))
                    {
                        _sql += " And F_sebango = '" + kanban + "'";
                    }
                    else
                    {
                        _sql += " Where F_sebango = '" + kanban + "'";
                    }
                }

                if (!string.IsNullOrWhiteSpace(store))
                {
                    if (_sql.Contains("Where"))
                    {
                        _sql += " And F_Store_cd = '" + store + "'";
                    }
                    else
                    {
                        _sql += " Where F_Store_cd = '" + store + "'";
                    }
                }

                _sql += " Order by 1";

                DataTable dt = _FillDT.ExecuteSQL(_sql);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Get Part No Successfully!",
                    data = JsonConvert.SerializeObject(dt)
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
        public IActionResult GetStore2nd(string? supplier)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                string _sql = "Select distinct F_Store_cd  From TB_Import_Forecast " +
                    "Where F_Production_Date in (Select max(F_Production_date) from TB_Import_Forecast) ";

                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    _sql += " And substring(F_Supplier_code,2,4)+'-'+ rtrim(F_supplier_plant) = '" + supplier + "'";
                }

                _sql += " Order by 1";

                DataTable dt = _FillDT.ExecuteSQL(_sql);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Get Store2nd Successfully!",
                    data = JsonConvert.SerializeObject(dt)
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
        public async Task<IActionResult> GetSupplierDetail(string supplier)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                var supDetail = await _PPM3Context.T_Supplier_MS.Where(x => x.F_supplier_cd + "-" + x.F_Plant_cd == supplier).FirstOrDefaultAsync();

                if (supDetail == null)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        message = "Supplier Not Found!"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Get Supplier Detail Successfully!",
                    data = supDetail.F_short_name.Trim() + " : " + supDetail.F_name
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
        public async Task<IActionResult> GetPartDetail(string part)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                var partDetail = await _PPM3Context.T_Construction.Where(x=>x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == part)
                    .ToListAsync();
                
                if(partDetail == null)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        message = "Part Detail Not Found!"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Get Part Detail Successfully!",
                    data = partDetail.DistinctBy(x => x.F_Part_nm).FirstOrDefault().F_Part_nm
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
        public IActionResult GetListData(string YM,string Rev,string? supplier,string? kanban,string? store,string? part)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                string plant = Request.Cookies.FirstOrDefault(x => x.Key == "plantCode").Value;
                string dev = Request.Cookies.FirstOrDefault(x => x.Key == "isDev").Value == "1" ? "Dev" : "";
                string plantDev = plant + dev;
                string toPPM = plantDev switch
                {
                    "1" => "[HMMT-PPM].[PPMDB]",
                    "2" => "[HMMT-PPM].[PPMDB]",
                    "3" => "[HMMTA-PPM].[PPMDB]",
                    "1Dev" => "[PPMDB]",
                    "2Dev" => "[PPMDB]",
                    "3Dev" => "[PPMDB]",
                    _ => "[PPMDB]"
                };


                string _sql = "Select F_production_date, F_revision_no, substring(F_Supplier_code,2,4)+'-'+ F_supplier_plant as F_Supplier_Code,S.F_Short_Name, rtrim(F.F_part_no)+'-'+ rtrim(F.F_ruibetsu) as F_Part_No,rtrim(C.F_PART_NM) as F_Part_Name, " +
                    " F_sebango as F_Sebango, F_Delivery_qty,'0'+F_Cycle_Supply as F_cycle_supply, F_Amount_MD1, F_Amount_MD2, F_Amount_MD3, F_Amount_MD4, F_Amount_MD5, " +
                    " F_Amount_MD6, F_Amount_MD7, F_Amount_MD8, F_Amount_MD9, F_Amount_MD10, F_Amount_MD11, F_Amount_MD12, F_Amount_MD13, " +
                    " F_Amount_MD14, F_Amount_MD15, F_Amount_MD16, F_Amount_MD17, F_Amount_MD18, F_Amount_MD19, F_Amount_MD20, F_Amount_MD21, " +
                    " F_Amount_MD22, F_Amount_MD23, F_Amount_MD24, F_Amount_MD25, F_Amount_MD26, F_Amount_MD27, F_Amount_MD28, F_Amount_MD29, " +
                    " F_Amount_MD30, F_Amount_MD31, F.F_Store_cd, F_Amount_M, F_Amount_M1, F_Amount_M2, F_Amount_M3 " +
                    " from TB_IMPORT_FORECAST F INNER JOIN (Select F_PART_NO,F_Ruibetsu,F_Store_Cd,F_PART_NM " +
                    $" From {toPPM}.dbo.T_Construction " +
                    $" Where F_Local_Str <= convert(Char(8),Getdate(),112) and F_Local_End >= convert(Char(8),Getdate(),112) and F_Supplier_Cd <>'9997') C " +
                    $" ON F.F_PART_NO = C.F_PART_NO and F.F_Ruibetsu = C.F_Ruibetsu and F.F_Store_Cd = C.F_Store_Cd" +
                    $" INNER JOIN (Select F_Supplier_Cd,F_Plant_CD,F_Store_CD,F_Short_name " +
                    $" From {toPPM}.dbo.T_Supplier_Ms " +
                    $" Where F_TC_Str <= convert(Char(8),Getdate(),112) and F_TC_End >= convert(Char(8),Getdate(),112)) S " +
                    $" ON substring(F_Supplier_code,2,4) = S.F_Supplier_Cd and F.F_supplier_plant = S.F_Plant_CD and F.F_Store_Cd = S.F_Store_Cd " +
                    $" Where F_production_date ='{YM}'  and F_revision_no='{Rev}'  and F.F_Store_Cd like '{plant}%' ";

                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    _sql += " and substring(F.F_Supplier_code,2,4)+'-'+ F.F_supplier_plant = '" + supplier + "'";
                }
                if (!string.IsNullOrWhiteSpace(kanban))
                {
                    _sql += " and F.F_Sebango = '" + kanban + "'";
                }
                if (!string.IsNullOrWhiteSpace(store))
                {
                    _sql += " and F.F_Store_cd = '" + store + "'";
                }
                if (!string.IsNullOrWhiteSpace(part))
                {
                    _sql += " and rtrim(F.F_part_no)+'-'+ rtrim(F.F_ruibetsu) = '" + part + "'";
                }

                _sql += " Order by F_production_date, F_revision_no, F_Supplier_code, F_supplier_plant,F.F_Store_cd,F.F_part_no,F.F_ruibetsu";

                var dt = _FillDT.ExecuteSQL(_sql);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Get List Data Successfully!",
                    data = JsonConvert.SerializeObject(dt)
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
        public IActionResult CheckBeforeInterface()
        {
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                short F_Value2 = _KB3Context.Database.SqlQueryRaw<short>("Select F_Value2 AS Value From TB_MS_Parameter Where F_Code = 'CI'").FirstOrDefault();
                
                if (F_Value2 >= 2)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        message = "ไม่สามารถกด Interface ได้เนื่องจากระบบ Order กำลังทำงานค่ะ",
                        error = "Check Before Interface is Running!"
                    });
                }

                string F_Value3 = _KB3Context.Database.SqlQueryRaw<string>("Select Substring(F_Value3,9,1) as Value From TB_MS_Parameter Where F_Code = 'LO' ").FirstOrDefault();
                if(F_Value3 == "D")
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        message = "ไม่สามารถกด Interface ได้เนื่องจากกำลังออกออเดอร์เพื่อกะกลางคืน กรุณากดเฉพาะการทำงานเพื่อกะกลางวันเท่านั้นค่ะ",
                        error = "Check Before Interface is Running!"
                    });
                }


                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Check Before Interface Successfully!",
                    data = "OK"
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

        [HttpPost]
        public async Task<IActionResult> Interface(VMKBNIM012_Interface obj)
        {
            using var _kb3Trans = _KB3Context.Database.BeginTransaction();
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                _kb3Trans.CreateSavepoint("Start Interface Forecast");
                _log.WriteLogMsg($"START INTERFACE FORECAST : {obj.YM_L} | REV : {obj.Rev_L}");

                string _sql = "Insert into TB_Import_Forecast_History (F_Version, F_production_date, F_revision_no, F_Supplier_code, F_supplier_plant, F_part_no, F_ruibetsu, F_Store_cd, F_sebango, F_Delivery_qty, " +
                    " F_cycle_supply, F_Amount_M, F_Amount_M1, F_Amount_M2, F_Amount_M3, F_Amount_M4, F_Amount_MD1, F_Amount_MD2, F_Amount_MD3, " +
                    " F_Amount_MD4, F_Amount_MD5, F_Amount_MD6, F_Amount_MD7, F_Amount_MD8, F_Amount_MD9, F_Amount_MD10, F_Amount_MD11, " +
                    "F_Amount_MD12, F_Amount_MD13, F_Amount_MD14, F_Amount_MD15, F_Amount_MD16, F_Amount_MD17, F_Amount_MD18, F_Amount_MD19, " +
                    "F_Amount_MD20, F_Amount_MD21, F_Amount_MD22, F_Amount_MD23, F_Amount_MD24, F_Amount_MD25, F_Amount_MD26, F_Amount_MD27, " +
                    "F_Amount_MD28, F_Amount_MD29, F_Amount_MD30, F_Amount_MD31, F_unit_price, F_amount, F_Fac, F_Import_By, F_Import_Date) " +
                    "Select F_Version, F_production_date, F_revision_no, F_Supplier_code, F_supplier_plant, F_part_no, F_ruibetsu, F_Store_cd, F_sebango, F_Delivery_qty, " +
                    "F_cycle_supply, F_Amount_M, F_Amount_M1, F_Amount_M2, F_Amount_M3, F_Amount_M4, F_Amount_MD1, F_Amount_MD2, F_Amount_MD3, " +
                    "F_Amount_MD4, F_Amount_MD5, F_Amount_MD6, F_Amount_MD7, F_Amount_MD8, F_Amount_MD9, F_Amount_MD10, F_Amount_MD11, " +
                    "F_Amount_MD12, F_Amount_MD13, F_Amount_MD14, F_Amount_MD15, F_Amount_MD16, F_Amount_MD17, F_Amount_MD18, F_Amount_MD19, " +
                    "F_Amount_MD20, F_Amount_MD21, F_Amount_MD22, F_Amount_MD23, F_Amount_MD24, F_Amount_MD25, F_Amount_MD26, F_Amount_MD27, " +
                    "F_Amount_MD28, F_Amount_MD29, F_Amount_MD30, F_Amount_MD31, F_unit_price, F_amount, F_Fac, F_Import_By, F_Import_Date " +
                    $"From TB_Import_Forecast Where F_production_date ='{obj.YM_L}' and F_revision_no='{obj.Rev_L}' ";

                await _KB3Context.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg("Backup Forecast | SQL : " + _sql);
                _kb3Trans.CreateSavepoint("Backup Forecast");

                _sql = $"Exec dbo.SP_UpdateForecast_NEW @p0,@p1,@p2";
                await _KB3Context.Database.ExecuteSqlRawAsync(_sql, obj.YM, obj.Ver, obj.Rev);
                _log.WriteLogMsg($"Finished Interface Forecast | SQL : {_sql} | YM : {obj.YM} | Ver : {obj.Ver} | Rev : {obj.Rev}" );
                _kb3Trans.CreateSavepoint("Finished Interface Forecast");
                //if(Ver.ToUpper() == "CONFIRM")
                //{
                //    _sql = $"Select * From TB_IMport_Forecast Where F_Version <> 'C' and F_production_date ='{YM}' and F_revision_no = '{Rev}' " +
                //        $" and F_Store_Cd like '{_BearerClass.Plant}%' ";
                //    var dt = _FillDT.ExecuteSQL(_sql);

                //    _sql = $"UPDATE TB_IMPORT_FORECAST Set F_Version ='C',F_Amount_M = 0,F_Amount_MD1=0,F_Amount_MD2=0,F_Amount_MD3=0,F_Amount_MD4=0,F_Amount_MD5=0 " +
                //        $",F_Amount_MD6=0,F_Amount_MD7=0,F_Amount_MD8=0,F_Amount_MD9=0,F_Amount_MD10=0 " +
                //        $",F_Amount_MD11=0,F_Amount_MD12=0,F_Amount_MD13=0,F_Amount_MD14=0,F_Amount_MD15=0 " +
                //        $",F_Amount_MD16=0,F_Amount_MD17=0,F_Amount_MD18=0,F_Amount_MD19=0,F_Amount_MD20=0 " +
                //        $",F_Amount_MD21=0,F_Amount_MD22=0,F_Amount_MD23=0,F_Amount_MD24=0,F_Amount_MD25=0 " +
                //        $",F_Amount_MD26=0,F_Amount_MD27=0,F_Amount_MD28=0,F_Amount_MD29=0,F_Amount_MD30=0,F_Amount_MD31=0 " +
                //        $",F_Import_By='{_BearerClass.UserCode}',F_Import_Date=getdate() " +
                //        $"Where F_Version <>'C' and F_production_date ='{YM}' " +
                //        $"and F_revision_no='{Rev}' " +
                //        $"and F_Store_Cd like '{_BearerClass.UserCode}%' ";

                //    await _KB3Context.Database.ExecuteSqlRawAsync(_sql);
                //    _log.WriteLogMsg($"Update Forecast Confirm | SQL : {_sql}");

                //    for(int i =0; i < dt.Rows.Count; i++)
                //    {
                //        _sql = "EXEC SP_UPDateForecast @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8";
                //        await _KB3Context.Database.ExecuteSqlRawAsync(_sql,
                //                YM,
                //                dt.Rows[i]["F_Supplier_Code"].ToString().Trim().Substring(1,4),
                //                dt.Rows[i]["F_Supplier_Plant"].ToString().Trim(),
                //                dt.Rows[i]["F_Store_CD"].ToString().Trim(),
                //                dt.Rows[i]["F_Part_No"].ToString().Trim(),
                //                dt.Rows[i]["F_Ruibetsu"].ToString().Trim(),
                //                "0" + dt.Rows[i]["F_Sebango"].ToString().Trim(),
                //                Ver.Substring(0,1),
                //                Rev);

                //        _log.WriteLogMsg($"Update Forecast Confirm | SQL : {_sql} | YM : {YM} | Supplier : {dt.Rows[i]["F_Supplier_Code"].ToString().Trim().Substring(1, 4)} | Plant : {dt.Rows[i]["F_Supplier_Plant"].ToString().Trim()} | Store : {dt.Rows[i]["F_Store_CD"].ToString().Trim()} | Part : {dt.Rows[i]["F_Part_No"].ToString().Trim()} | Ruibetsu : {dt.Rows[i]["F_Ruibetsu"].ToString().Trim()} | Sebango : {dt.Rows[i]["F_Sebango"].ToString().Trim()} | Ver : {Ver.Substring(0, 1)} | Rev : {Rev}");
                //    }
                //}

                //string ProdDate = DateTime.ParseExact(YM, "yyyyMM", CultureInfo.InvariantCulture).AddMonths(-1).ToString("yyyyMM");

                //_sql = $"INSERT INTO TB_IMPORT_FORECAST " +
                //    $" Select '{Ver.Substring(0, 1)}' as F_Version, convert(Char(6),dateadd(M,-1,substring(F.F_production_date,5,2)+'/01/'+substring(F.F_production_date,1,4)),112) as F_Production_Date," +
                //    $" '0.0' as F_revision_no, F.F_Supplier_code, F.F_supplier_plant, F.F_part_no, F.F_ruibetsu, F.F_Store_cd, F.F_sebango, F.F_Delivery_qty, " +
                //    $" F.F_cycle_supply, 0 as F_Amount_M, 0 as F_Amount_M1, 0 as F_Amount_M2, 0 as F_Amount_M3, 0 as F_Amount_M4, 0 as F_Amount_MD1, 0 as F_Amount_MD2, 0 as F_Amount_MD3, " +
                //    $" 0 as F_Amount_MD4, 0 as F_Amount_MD5, 0 as F_Amount_MD6, 0 as F_Amount_MD7, 0 as F_Amount_MD8, 0 as F_Amount_MD9, 0 as F_Amount_MD10, 0 as F_Amount_MD11, " +
                //    $" 0 as F_Amount_MD12, 0 as F_Amount_MD13, 0 as F_Amount_MD14, 0 as F_Amount_MD15, 0 as F_Amount_MD16, 0 as F_Amount_MD17, 0 as F_Amount_MD18, 0 as F_Amount_MD19, " +
                //    $" 0 as F_Amount_MD20, 0 as F_Amount_MD21, 0 as F_Amount_MD22, 0 as F_Amount_MD23, 0 as F_Amount_MD24, 0 as F_Amount_MD25, 0 as F_Amount_MD26, 0 as F_Amount_MD27, " +
                //    $" 0 as F_Amount_MD28, 0 as F_Amount_MD29, 0 as F_Amount_MD30, 0 as F_Amount_MD31, 0 as F_unit_price, 0 as F_amount, F.F_Fac, F.F_Flag_Update, '{_BearerClass.UserCode}' as F_Import_By, " +
                //    $" getdate() as F_Import_Date,'1' as F_Flag_Ckd " +
                //    $" from TB_IMPORT_FOrecast F INNER JOIN dbo.FN_GetNewPart('{YM}') P ON F.F_Part_No collate Thai_CI_AS = P.F_PART_NO " +
                //    $" and F.F_Ruibetsu collate Thai_CI_AS= P.F_Ruibetsu and Substring(F.F_Supplier_Code collate Thai_CI_AS,2,4) = P.F_Supplier_Code " +
                //    $"and F.F_Supplier_plant collate Thai_CI_AS = P.F_SUpplier_Plant and F.F_Store_Cd collate Thai_CI_AS = P.F_Store_Code " +
                //    $"and F.F_Sebango collate Thai_CI_AS = Substring(P.F_kanban_No,2,3) " +
                //    $"LEFT OUTER JOIN (Select distinct F_Production_Date, F_Supplier_Code,F_Supplier_Plant,F_Part_No,F_Ruibetsu,F_Store_Cd,F_Sebango " +
                //    $"From TB_IMPORT_FOrecast Where F_production_date = '{ProdDate}')F1 ON  " +
                //    $"convert(Char(6),dateadd(M,-1,substring(F.F_production_date,5,2)+'/01/'+substring(F.F_production_date,1,4)),112) = F1.F_production_date " +
                //    $"and F.F_Supplier_code = F1.F_Supplier_code and F.F_supplier_plant = F1.F_supplier_plant and F.F_part_no = F1.F_part_no and F.F_ruibetsu = F1.F_ruibetsu " +
                //    $"and F.F_Store_cd = F1.F_Store_cd and F.F_sebango = F1.F_sebango " +
                //    $"Where F.F_Production_Date='{YM}' and F1.F_part_no is null ";

                //await _KB3Context.Database.ExecuteSqlRawAsync(_sql);

                //string nData = Get_BackDateCalculateForecast(YM);
                //DateTime nMonth = DateTime.ParseExact(YM, "yyyyMM", CultureInfo.InvariantCulture).AddDays(-1);

                string LastOrder = _KB3Context.Database.SqlQueryRaw<string>("Select substring(F_Value3,1,8) AS Value From TB_MS_Parameter Where F_Code = 'LO' ").FirstOrDefault();

                _sql = $"Update TB_MS_Parameter set F_Value2 ='1',F_Value3='{LastOrder}' " +
                    $",F_Update_Date=Getdate(),F_Update_By='{_BearerClass.UserCode}' Where F_Code ='FO' ";

                await _KB3Context.Database.ExecuteSqlRawAsync(_sql);

                _kb3Trans.Commit();

                //await _emailService.SendEmailToPA2(YM, Rev, Ver);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Interface Forecast Successfully!",
                    data = "OK"
                });
            }
            catch (Exception ex)
            {
                _kb3Trans.Rollback();
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
            }
        }

        //public string Get_BackDateCalculateForecast(string nMonth)
        //{
        //    return _KB3Context.Database.SqlQueryRaw<string>($"select dbo.FN_GetDateV2V('{nMonth}01,3) AS Value").FirstOrDefault();
        //}

        [HttpPost]
        public IActionResult CheckInterfaceN1()
        {
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                string prodDate = _KB3Context.Database.SqlQueryRaw<string>
                    ("Select Max(F_Production_Date collate Thai_CI_AS+F_Version+F_REvision_NO) as Value From TB_IMPORT_FORECAST")
                    .FirstOrDefault();

                string Txt_ProdYM = prodDate.Substring(4,2) + "/" + prodDate.Substring(0, 4);
                string Txt_Ver = prodDate.Substring(6, 1);
                string Txt_Rev = prodDate.Substring(7, 3);

                //DateTime dtProdDate = DateTime.ParseExact(prodDate.Substring(0, 6), "yyyyMM", CultureInfo.InvariantCulture);
                string Txt_ProdYM_Next = DateTime.Now.AddMonths(1).ToString("MM/yyyy");
                string Txt_Ver_Next = "1";
                string Txt_Rev_Next = "0.0";

                if(Txt_ProdYM == Txt_ProdYM_Next)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        message = "Status Forecast : Already Interface can't process again.",
                        error = "Status Forecast : Already Interface can't process again."
                    });
                }
                //else if (Txt_ProdYM.CompareTo(Txt_ProdYM_Next) < 0)
                //{
                //    return BadRequest(new
                //    {
                //        status = "400",
                //        response = "Bad Request",
                //        message = "Status Forecast : Waiting interface data.",
                //        error = "Status Forecast : Waiting interface data."
                //    });
                //}

                if((Txt_ProdYM_Next.Substring(3,4) + Txt_ProdYM.Substring(0,2))
                    .CompareTo(DateTime.Now.ToString("MM/yyyy")) <= 0)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        message = "ระบบไม่สามารถทำงานย้อนหลังได้ กรุณาตรวจสอบอีกครั้งค่ะ.",
                        error = "ระบบไม่สามารถทำงานย้อนหลังได้ กรุณาตรวจสอบอีกครั้งค่ะ."
                    });
                }

                int Count_Records = _KB3Context.Database.SqlQueryRaw<int>
                    ($"Select isnull(Count(*),0) AS Value From TB_Import_Forecast" +
                    $" Where F_production_date='{Txt_ProdYM_Next.Substring(3,4) + Txt_ProdYM_Next.Substring(0, 2)}'")
                    .FirstOrDefault();

                if (Count_Records > 0)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        message = $"ระบบไม่สามารถ Interface N1 ของเดือน {Txt_ProdYM} มายังเดือน {Txt_ProdYM_Next}" +
                        $"ได้ เนื่องจากพบข้อมูลในระบบแล้ว. User ต้องกด Interface จาก Forecast เดือน {Txt_ProdYM_Next} เท่านั้น.",
                        error = $"ระบบไม่สามารถ Interface N1 ของเดือน {Txt_ProdYM} มายังเดือน {Txt_ProdYM_Next}" +
                        $"ได้ เนื่องจากพบข้อมูลในระบบแล้ว. User ต้องกด Interface จาก Forecast เดือน {Txt_ProdYM_Next} เท่านั้น."
                    });
                }

                string plant = Request.Cookies.FirstOrDefault(x => x.Key == "plantCode").Value;

                string storeCode = plant switch
                {
                    "1" => "1A",
                    "2" => "2B",
                    "3" => "3C",
                    _ => "1A"
                };

                int Count_Calendar = _KB3Context.Database.SqlQueryRaw<int>
                    ($"Select Count(*) AS Value From TB_Calendar Where F_YM = '{Txt_ProdYM_Next.Substring(3, 4) + Txt_ProdYM_Next.Substring(0, 2)}' " +
                    $"and F_Store_Cd ='{storeCode}' ")
                    .FirstOrDefault();

                if(Count_Calendar == 0)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        message = $"กรุณาเพิ่มปฏิทินการทำงานของเดือน {Txt_ProdYM_Next} ก่อนทำการประมวลผล.",
                    });
                }

                int Count_Forecast = _KB3Context.Database.SqlQueryRaw<int>
                    ($"Select Count(*) AS Value From [HMMT-APP03].[Proc_DB].dbo.T_POM_Detail " +
                    $"Where F_production_date='{Txt_ProdYM_Next.Substring(3, 4) + Txt_ProdYM_Next.Substring(0, 2)}'")
                    .FirstOrDefault();

                if (Count_Forecast > 0)
                {
                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        message = $"ระบบพบ Forecast ของเดือน {Txt_ProdYM_Next} แล้ว ท่านต้องการยกเลิกการ Interface หรือไม่?",
                        confirm = "Yes"
                    });
                }

                return InterfaceN1(Txt_ProdYM,Txt_ProdYM_Next).Result;

            }
            catch (Exception ex )
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

        public async Task<IActionResult> InterfaceN1(string Txt_ProdYM,string Txt_ProdYM_Next)
        {
            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401)
            {
                return StatusCode(401, new
                {
                    status = "401",
                    response = "Unauthorized",
                    message = "Unauthorized Access!"
                });
            }

            using var _kb3Trans = _KB3Context.Database.BeginTransaction();
            try
            {

                _kb3Trans.CreateSavepoint("Start Interface Forecast N1");
                await _KB3Context.Database.ExecuteSqlRawAsync("Exec dbo.SP_INF_FO_NextMonth @p0,@p1",
                    Txt_ProdYM.Substring(3,4) + Txt_ProdYM.Substring(0, 2), Txt_ProdYM_Next.Substring(3, 4) + Txt_ProdYM_Next.Substring(0, 2));

                _kb3Trans.Commit();
                _log.WriteLogMsg($"Finished Interface Forecast N1 | YM : {Txt_ProdYM} | YM_Next : {Txt_ProdYM_Next}");
                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Interface Forecast N1 Successfully!",
                    data = "OK"
                });

            }
            catch (Exception ex)
            {
                _kb3Trans.RollbackToSavepoint("Start Interface Forecast N1");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
            }
        }

    }
}
