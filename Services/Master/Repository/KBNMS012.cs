using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS012 : IKBNMS012
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public KBNMS012
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMapService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _automapService = autoMapService;
            _httpContextAccessor = httpContextAccessor;
        }

        public static string strDateNow = DateTime.Now.ToString("yyyyMMdd");


        public async Task<List<TB_MS_PartOrder>> GetDropDown(string? F_Supplier_Code, string? F_Kanban_No, string? F_Store_Cd, string? F_Part_No)
        {
            try
            {
                var data = await _kbContext.TB_MS_PartOrder.AsNoTracking()
                    .Where(x => x.F_Start_Date.CompareTo(strDateNow) <= 0
                    && x.F_End_Date.CompareTo(strDateNow) >= 0
                    && x.F_Store_Code.StartsWith(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value)).ToListAsync();

                if ((!string.IsNullOrWhiteSpace(F_Supplier_Code) && !string.IsNullOrWhiteSpace(F_Kanban_No))
                    || (!string.IsNullOrWhiteSpace(F_Part_No) && !string.IsNullOrWhiteSpace(F_Store_Cd)))
                {
                    return null;
                }

                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    data = data.Where(x => x.F_Supplier_Cd == F_Supplier_Code.Substring(0, 4)
                    && x.F_Supplier_Plant == F_Supplier_Code.Substring(5, 1)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    data = data.Where(x => x.F_Kanban_No == F_Kanban_No).ToList();
                }

                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    data = data.Where(x => x.F_Part_No == F_Part_No.Substring(0, 10)
                    && x.F_Ruibetsu == F_Part_No.Substring(11, 2)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(F_Store_Cd))
                {
                    data = data.Where(x => x.F_Store_Code == F_Store_Cd).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<string[]> GetSupplierDetail(string F_Supplier_Code)
        {
            try
            {
                var cycle = await _kbContext.TB_MS_DeliveryTime.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.F_Supplier_Code == F_Supplier_Code.Substring(0, 4)
                    && x.F_Supplier_Plant == F_Supplier_Code.Substring(5, 1)
                    && x.F_Start_Order_Date.CompareTo(strDateNow) <= 0
                    && x.F_End_Order_Date.CompareTo(strDateNow) >= 0);

                var supname = await _PPM3Context.T_Supplier_MS.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.F_supplier_cd == F_Supplier_Code.Substring(0, 4)
                    && x.F_Plant_cd == F_Supplier_Code.Substring(5, 1)[0]
                    && x.F_TC_Str.CompareTo(strDateNow) <= 0
                    && x.F_TC_End.CompareTo(strDateNow) >= 0
                    && x.F_Store_cd.StartsWith(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value));

                if (cycle != null && supname != null)
                {
                    return new string[] {
                        cycle.F_Cycle.Trim(),
                        "(" + supname.F_short_name.Trim() + ")" + supname.F_name.Trim()
                    };
                }
                else
                {
                    return new string[] { };
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<List<TB_Kanban_SetOrder>> Search(string? F_Supplier_Code, string? F_Kanban_No, string? F_Store_Cd, string? F_Part_No)
        {
            try
            {
                var data = await _kbContext.TB_Kanban_SetOrder.AsNoTracking()
                    .Where(x => x.F_Plant == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value).ToListAsync();

                //if ((!string.IsNullOrWhiteSpace(F_Supplier_Code) && !string.IsNullOrWhiteSpace(F_Kanban_No))
                //    || (!string.IsNullOrWhiteSpace(F_Part_No) && !string.IsNullOrWhiteSpace(F_Store_Cd)))
                //{
                //    return null;
                //}

                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    data = data.Where(x => x.F_Supplier_Code.Trim() + "-" + x.F_Supplier_Plant == F_Supplier_Code).ToList();
                }
                if (!string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    data = data.Where(x => x.F_Kanban_No == F_Kanban_No).ToList();
                }

                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    data = data.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == F_Part_No).ToList();
                }
                if (!string.IsNullOrWhiteSpace(F_Store_Cd))
                {
                    data = data.Where(x => x.F_Store_Cd == F_Store_Cd).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<object> FindDetail(string F_Supplier_Code, string? F_Kanban_No, string? F_Store_Cd, string? F_Part_No)
        {
            try
            {
                var cycle = _FillDT.ExecuteSQL("exec sp_getCycleTime @p0,@p1,@p2,@p3",
                    F_Supplier_Code.Split("-")[0], F_Supplier_Code.Split("-")[1], strDateNow, strDateNow);

                string SqlQuery = $@"SELECT TOP 1 RTRIM(S.F_supplier_cd)+'-'+RTRIM(S.F_Plant_cd) AS F_supplier_cd 
                    ,'('+RTRIM(F_short_name)+')'+RTRIM(F_name) AS F_name 
                    ,RTRIM(C.F_Part_no)+'-'+RTRIM(C.F_Ruibetsu) AS F_Part_no 
                    ,RTRIM(C.F_Store_cd) AS F_Store_cd 
                    ,RIGHT('0000'+ CONVERT(VARCHAR,C.F_Sebango),4) AS F_Kanban 
                    ,RTRIM(C.F_Part_nm) AS F_Part_nm, C.F_qty_box 
                    FROM T_Supplier_ms S INNER JOIN  T_Construction C 
                    ON S.F_supplier_cd = C.F_supplier_cd collate Thai_CI_AS 
                    AND S.F_Plant_cd = C.F_plant collate Thai_CI_AS 
                    AND S.F_Store_cd = C.F_Store_cd collate Thai_CI_AS 
                    WHERE S.F_TC_Str <= convert(char(8),getdate(),112) 
                    AND S.F_TC_End >= convert(char(8),getdate(),112) 
                    AND C.F_Local_Str <= convert(char(8),getdate(),112) 
                    AND C.F_Local_End >= convert(char(8),getdate(),112) 
                    AND S.F_store_cd LIKE '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}%' 
                    AND C.F_store_cd LIKE '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}%' 
                    ";

                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    SqlQuery += $@"AND C.F_supplier_cd + '-' + C.F_plant = '{F_Supplier_Code}' ";
                }
                if (!string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    SqlQuery += Environment.NewLine + $@"AND C.F_Sebango = '{F_Kanban_No.Substring(1, 3)}' ";
                }
                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    SqlQuery += Environment.NewLine + $@"AND C.F_Part_no + '-' + AND C.F_Ruibetsu = '{F_Part_No}' ";
                }
                if (!string.IsNullOrWhiteSpace(F_Store_Cd))
                {
                    SqlQuery += Environment.NewLine + $@"AND C.F_Store_cd = '{F_Store_Cd}' ";
                }

                var dt = _FillDT.ExecuteSQLPPMDB(SqlQuery);

                return new { dt, cycle };

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task Save(List<TB_Kanban_SetOrder> listObj)
        {
            try
            {
                string logMsg = "";

                foreach (var obj in listObj)
                {

                    var existObj = await _kbContext.TB_Kanban_SetOrder.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.F_Plant == obj.F_Plant
                        && x.F_Supplier_Code == obj.F_Supplier_Code
                        && x.F_Supplier_Plant == obj.F_Supplier_Plant
                        && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu
                        && x.F_Kanban_No == obj.F_Kanban_No && x.F_Store_Cd == obj.F_Store_Cd);

                    if (existObj != null)
                    {
                        obj.F_Update_Date = DateTime.Now;
                        obj.F_Update_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value;
                        logMsg += "UPDATE TB_Kanban_SetOrder BEFORE => " + JsonConvert.SerializeObject(existObj);
                        _kbContext.Attach(obj);
                        _kbContext.Entry(obj).State = EntityState.Modified;
                        logMsg += Environment.NewLine + "UPDATE TB_Kanban_SetOrder AFTER => " + JsonConvert.SerializeObject(obj);
                    }
                    else
                    {
                        obj.F_Create_Date = DateTime.Now;
                        obj.F_Update_Date = DateTime.Now;
                        obj.F_Update_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value;
                        obj.F_Create_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value;
                        logMsg += "INSERT TB_Kanban_SetOrder => " + JsonConvert.SerializeObject(obj);
                        await _kbContext.TB_Kanban_SetOrder.AddAsync(obj);
                    }

                    await _kbContext.SaveChangesAsync();
                    _log.WriteLogMsg(logMsg);
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
