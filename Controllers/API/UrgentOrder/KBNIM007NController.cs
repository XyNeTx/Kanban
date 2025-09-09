using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.KB3.UrgentOrder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]

    public class KBNIM007NController : ControllerBase
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

        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNIM007NController(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context,
            PPM3Context pPM3Context,
            SerilogLibs serilogLibs,
            FillDataTable fillDT
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
            _FillDT = fillDT;
        }

        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {

            dynamic _json = null;
            string _SQL = "";
            try
            {
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                if (pData != null) _json = JsonConvert.DeserializeObject(pData);

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_MS_Factory"" : " + _jsTB_MS_Factory + @"
                            }
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Inquiry()
        {
            _BearerClass.Authentication();

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            try
            {
                string now = DateTime.Now.ToString("yyyyMMdd");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                DataTable dt = _FillDT.ExecuteSQL($"Select DISTINCT F_PDS_No FROM TB_Transaction_TMP " +
                    $"Where F_Update_By = '{UserID}' AND F_Plant = '{Plant}' ");

                if (dt.Rows.Count == 0)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Bad Request",
                        message = "Data not Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
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
                    message = ex.InnerException.Message == null ? ex.Message : ex.InnerException.Message,
                    err = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Update(VM_KBNIM007N_OK obj)
        {
            _BearerClass.Authentication();

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            try
            {
                string now = DateTime.Now.ToString("yyyyMMdd");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                string Part_No = obj.F_Part_No.Split("-")[0];
                string Ruibetsu = obj.F_Part_No.Split("-")[1];
                string Supplier_Code = obj.F_Supplier.Split("-")[0];
                char Supplier_Plant = obj.F_Supplier.Split("-")[1][0];

                var changeObj = await _KB3Context.TB_Transaction_TMP
                    .Where(x => x.F_Part_No == Part_No
                    && x.F_Ruibetsu == Ruibetsu
                    && x.F_PDS_No == obj.F_PDS_No
                    && x.F_Supplier_CD == Supplier_Code
                    && x.F_Supplier_Plant == Supplier_Plant
                    && x.F_Update_By == UserID
                    && x.F_Plant == Plant[0]
                    ).SingleOrDefaultAsync();

                if (changeObj == null)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Bad Request",
                        message = "Data not Found"
                    });
                }

                changeObj.F_Qty = obj.F_Qty;
                changeObj.F_Qty_Level1 = obj.F_Qty;
                changeObj.F_Update_By = UserID;
                changeObj.F_Update_Date = DateTime.Now;

                _KB3Context.TB_Transaction_TMP.Update(changeObj);
                await _KB3Context.SaveChangesAsync();


                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
                    message = "Data Found",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.InnerException.Message == null ? ex.Message : ex.InnerException.Message,
                    err = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(TB_Transaction_TMP obj)
        {
            _BearerClass.Authentication();

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            try
            {
                string now = DateTime.Now.ToString("yyyyMMdd");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                var deleteObj = await _KB3Context.TB_Transaction_TMP
                    .Where(x => x.F_PDS_No == obj.F_PDS_No
                    && x.F_Update_By == UserID
                    && x.F_Plant == Plant[0]).ToListAsync();

                _KB3Context.TB_Transaction_TMP.RemoveRange(deleteObj);
                await _KB3Context.SaveChangesAsync();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
                    message = "Data Was Deleted",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.InnerException.Message == null ? ex.Message : ex.InnerException.Message,
                    err = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderNoSelected(string F_PDS_No)
        {
            _BearerClass.Authentication();

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });


            try
            {
                string now = DateTime.Now.ToString("yyyyMMdd");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                string _SQL = "SELECT F_Type, F_Type_Spc, F_Plant, F_PDS_No, F_PDS_Issued_Date, F_Store_CD, " +
                    " F_Ruibetsu, F_Kanban_No, F_Part_Name, F_Qty_Pack, F_Part_Code, F_Part_Order, F_Ruibetsu_Order, " +
                    " F_Store_Order, F_Name_Order, F_Qty, F_Qty_Level1, F_Seq_No, F_Seq_Type, F_Cut_Flag, " +
                    " F_Delivery_Date, F_Adv_Deli_Date, F_OrderType, F_Country, F_Reg_Flg, F_Inventory_Flg, " +
                    " F_Supplier_CD, F_Supplier_Plant, F_Cycle_Time, F_Safty_Stock, F_Part_Refer, " +
                    " F_Ruibetsu_Refer, F_Update_By, F_Update_Date, F_Remark, F_Parent_Level2, " +
                    " F_Qty_Level2, F_Parent_Level3, F_Qty_Level3, F_Parent_Level4, F_Qty_Level4, " +
                    " F_Round, F_Org_Store_CD, F_Ratio, F_Customer_orderType, " +
                    " F_Survey_DOC, F_Part_No+'-'+F_Ruibetsu AS F_Part_No, F_Round AS F_Delivery_Trip, FLOOR(F_Qty_Pack/F_Qty) AS F_Pack " +
                    $" FROM  TB_Transaction_TMP Where F_Update_By = '{UserID}' AND F_Plant = '{Plant}' AND F_PDS_No = '{F_PDS_No}' ";

                DataTable dt = _FillDT.ExecuteSQL(_SQL);

                if (dt.Rows.Count == 0)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Bad Request",
                        message = "Data not Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
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
                    message = ex.InnerException.Message == null ? ex.Message : ex.InnerException.Message,
                    err = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenPDSNo()
        {
            _BearerClass.Authentication();

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });
            string now = DateTime.Now.ToString("yyyyMMdd");
            string UserID = HttpContext.Session.GetString("USER_CODE");
            string Plant = HttpContext.Session.GetString("USER_PLANT");

            await _KB3Context.Database.ExecuteSqlRawAsync($"DELETE FROM TB_IMPORT_URGENT WHERE F_UPDATE_BY='{UserID}'");
            return Ok(new
            {
                status = "200",
                response = "OK",
                title = "OK",
                message = "Data Found",
                data = "IMKB" + DateTime.Now.ToString("yyyyMMddHHmm")
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplier()
        {
            _BearerClass.Authentication();

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            try
            {
                string now = DateTime.Now.ToString("yyyyMMdd");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                var data = await _KB3Context.TB_MS_PartOrder
                        .Where(x => x.F_Start_Date.CompareTo(now) <= 0 && x.F_End_Date.CompareTo(now) >= 0
                        && x.F_Store_Code.StartsWith(Plant))
                        .Select(x => new
                        {
                            F_Supplier_Cd = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant
                        }).OrderBy(x => x.F_Supplier_Cd).ToListAsync();

                data = data.DistinctBy(x => x.F_Supplier_Cd).OrderBy(x => x.F_Supplier_Cd).ToList();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
                    message = "Data Found",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.InnerException.Message == null ? ex.Message : ex.InnerException.Message,
                    err = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplierDetail(string F_Supplier_Cd, string F_ProcessDate)
        {
            _BearerClass.Authentication();

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            try
            {
                string now = DateTime.Now.ToString("yyyyMMdd");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");
                string SupCode = F_Supplier_Cd.Split("-")[0];
                string SupPlant = F_Supplier_Cd.Split("-")[1];

                var data = await _PPM3Context.T_Supplier_MS.Where(x => x.F_TC_Str.CompareTo(now) <= 0 && x.F_TC_End.CompareTo(now) >= 0
                    && x.F_Store_cd.StartsWith(Plant) && x.F_supplier_cd == SupCode && x.F_Plant_cd == SupPlant[0])
                    .Select(x => new
                    {
                        x.F_supplier_cd,
                        x.F_Plant_cd,
                        x.F_name,
                        x.F_short_name
                    }).FirstOrDefaultAsync();

                var KB_No = await _KB3Context.TB_MS_PartOrder
                        .Where(x => x.F_Start_Date.CompareTo(now) <= 0 && x.F_End_Date.CompareTo(now) >= 0
                        && x.F_Store_Code.StartsWith(Plant) && x.F_Supplier_Cd == SupCode && x.F_Supplier_Plant == SupPlant[0].ToString())
                        .Select(x => new
                        {
                            F_Kanban_No = x.F_Kanban_No
                        }).ToListAsync();

                KB_No = KB_No.DistinctBy(x => x.F_Kanban_No).OrderBy(x => x.F_Kanban_No).ToList();

                if (data == null)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Bad Request",
                        message = "Can't Get Supplier Detail",
                        err = "Supplier Not Found"
                    });
                }

                var data2 = await _KB3Context.TB_MS_DeliveryTime
                        .Where
                        (x =>
                            x.F_Start_Date.CompareTo(F_ProcessDate) <= 0
                            && x.F_End_Date.CompareTo(F_ProcessDate) >= 0
                            && x.F_Plant == Plant
                            && x.F_Supplier_Code == SupCode
                            && x.F_Supplier_Plant == SupPlant
                        ).Select(x => new
                        {
                            F_Cycle = x.F_Cycle.Substring(1, 1) + "-" + x.F_Cycle.Substring(2, 2) + "-" + x.F_Cycle.Substring(4, 2),
                        }).FirstOrDefaultAsync();

                if (data2 == null)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Bad Request",
                        message = "Can't Get Supplier Detail",
                        err = "Supplier Delivery Time Not Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
                    message = "Data Found",
                    data = data,
                    data2 = data2,
                    data3 = KB_No
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.InnerException.Message == null ? ex.Message : ex.InnerException.Message,
                    err = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPartNo(string? F_Supplier_Cd = null)
        {
            _BearerClass.Authentication();

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            try
            {
                string now = DateTime.Now.ToString("yyyyMMdd");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                string _SQL = "SELECT DISTINCT rtrim(F_Part_No) +'-'+rtrim(F_Ruibetsu) AS F_Part_No " +
                    $" FROM TB_MS_PartOrder WHERE (F_Start_Date <= '{now}' ) AND (F_End_Date >= '{now}' ) " +
                    $" and F_Store_Code like '{Plant}%' ";

                if (F_Supplier_Cd != null)
                {
                    _SQL += " AND (F_supplier_cd + '-' + F_Supplier_Plant = '" + F_Supplier_Cd + "') ";
                }

                _SQL += " ORDER BY rtrim(F_Part_No) +'-'+rtrim(F_Ruibetsu) ";

                DataTable dt = _FillDT.ExecuteSQL(_SQL);

                if (dt.Rows.Count == 0)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Bad Request",
                        message = "ไม่พบ Part ในระบบ",
                        err = "Part No Not Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
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
                    message = ex.InnerException.Message == null ? ex.Message : ex.InnerException.Message,
                    err = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PartNoChanged(string? F_Supplier_Cd = null, string? F_Part_No = null, string? F_Kanban_No = null)
        {
            _BearerClass.Authentication();

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            try
            {
                string now = DateTime.Now.ToString("yyyyMMdd");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                string SupCode = F_Supplier_Cd.Split("-")[0];
                string SupPlant = F_Supplier_Cd.Split("-")[1];


                string _SQL = "SELECT RTRIM(P.F_Kanban_No) AS F_Kanban_No " +
                  ", RTRIM(P.F_PART_NO) +'-'+RTRIM(P.F_Ruibetsu) as F_Part_No " +
                  ", ISNULL(Q.F_NEW_QTY,C.F_qty_box) AS F_qty_box " +
                  ", P.F_Store_Code as F_Store_CD " +
                  "FROM TB_MS_PartOrder P LEFT JOIN TB_Kanban_Chg_Qty Q " +
                  "ON  P.F_Supplier_Cd = Q.F_SUpplier_Code " +
                  "AND P.F_SUpplier_plant = Q.F_SUpplier_plant " +
                  "AND P.F_PART_NO = Q.F_Part_No " +
                  "AND P.F_Ruibetsu = Q.F_Ruibetsu " +
                  "AND P.F_Kanban_No = Q.F_Kanban_No " +
                  "AND P.F_Store_Code = Q.F_Store_Code AND Q.F_Delivery_Date <= '" + now + "' " +
                  "RIGHT JOIN [PPMDB].[dbo].T_Construction C " +
                  "ON P.F_Supplier_Cd = C.F_supplier_cd collate Thai_CI_AS " +
                  "AND P.F_Supplier_plant = C.F_plant collate Thai_CI_AS " +
                  "AND P.F_PART_NO = C.F_Part_no collate Thai_CI_AS " +
                  "AND P.F_Ruibetsu = C.F_Ruibetsu collate Thai_CI_AS " +
                  "AND P.F_Kanban_No = RIGHT('0000'+C.F_Sebango,4) collate Thai_CI_AS " +
                  "AND P.F_Store_Code = C.F_Store_cd collate Thai_CI_AS " +
                  "Where P.F_Start_Date <= convert(Char(8),getdate(),112) " +
                  "and P.F_ENd_Date >= convert(Char(8),getdate(),112) " +
                  "AND C.F_Local_Str  <= convert(Char(8),getdate(),112) " +
                  "AND C.F_Local_End >= convert(Char(8),getdate(),112) " +
                  "AND P.F_Store_Code like '" + Plant + "%' ";

                if (F_Supplier_Cd != null)
                {
                    _SQL += " AND (rtrim(P.F_Supplier_Cd) + '-' + P.F_SUpplier_plant = '" + F_Supplier_Cd + "') ";
                }
                if (F_Part_No != null)
                {
                    _SQL += "  AND rtrim(P.F_PART_NO) +'-'+rtrim(P.F_Ruibetsu) = '" + F_Part_No + "' ";
                }
                if (F_Kanban_No != null)
                {
                    _SQL += "  AND rtrim(P.F_Kanban_No) = '" + F_Kanban_No + "' ";
                }


                _SQL += "  ORDER BY 1,3 ";

                DataTable dt = _FillDT.ExecuteSQL(_SQL);



                if (dt.Rows.Count == 0)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Bad Request",
                        message = "ไม่พบ Part นี้ในระบบ กรุณาติดต่อ PA1",
                        err = "Part No Not Found"
                    });
                }

                string data = JsonConvert.SerializeObject(dt);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
                    message = "Data Found",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.InnerException.Message == null ? ex.Message : ex.InnerException.Message,
                    err = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> OKClicked(VM_KBNIM007N_OK obj)
        {
            _BearerClass.Authentication();

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                await _KB3Transaction.CreateSavepointAsync("Start_ImportSave");

                string now = DateTime.Now.ToString("yyyyMMdd");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                TB_Import_Urgent _Import_Urgent = JsonConvert.DeserializeObject<TB_Import_Urgent>(JsonConvert.SerializeObject(obj));
                _Import_Urgent.F_Part_No = _Import_Urgent.F_Part_No.Replace("-", string.Empty);
                _Import_Urgent.F_Update_By = UserID;
                _Import_Urgent.F_Update_Date = DateTime.Now;

                _KB3Context.TB_Import_Urgent.Add(_Import_Urgent);
                _KB3Context.SaveChanges();
                _KB3Transaction.Commit();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
                    message = "Data saved",
                });
            }
            catch (Exception ex)
            {
                await _KB3Transaction.RollbackToSavepointAsync("Start_ImportSave");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.InnerException.Message == null ? ex.Message : ex.InnerException.Message,
                    err = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AllDataWasSaved(string? F_PDS_No = null)
        {
            _BearerClass.Authentication();

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                _KB3Transaction.CreateSavepoint("Start_ImportSave");

                string now = DateTime.Now.ToString("yyyyMMdd");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                await _KB3Context.Database.ExecuteSqlRawAsync($"DELETE FROM TB_Import_Error Where F_Type = 'KBNIM007N' AND F_Update_By = '{UserID}' ");

                await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC [exec].[spKBNIM007N] '{Plant}','{UserID}', '{F_PDS_No}' ");

                _KB3Transaction.Commit();

                string _SQL = "SELECT F_PDS_CD, F_Row, F_Field, F_Remark, F_Update_By, F_Update_Date, F_Type FROM " +
                        $"TB_Import_Error Where F_Type ='KBNIM007N' and F_Update_By = '{UserID}' ";

                DataTable dtErr = _FillDT.ExecuteSQL(_SQL);

                if (dtErr.Rows.Count > 0)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Bad Request",
                        message = "Data Saved but Have Some Error Please Check",
                        userid = UserID,
                        type = "KBNIM007N",
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
                    message = "Data Saved",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = ex.InnerException.Message == null ? ex.Message : ex.InnerException.Message,
                    err = ex.Message
                });
            }
        }
    }
}
