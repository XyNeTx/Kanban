using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Security.Claims;

namespace KANBAN.Services.CKD_Ordering.Repository
{
    public class KBNOR310 : IKBNOR310
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public KBNOR310
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

        public static DateTime DateLogin;
        public static string strDateNow = DateTime.Now.ToString("yyyyMMdd");

        public static DateTime dateProcessDate_CKD { get; set; }
        public static string chrProcessShift_CKD { get; set; } = "";
        public static string strProcessCycle { get; set; } = "";
        public static DateTime dateProcessLastDate_CKD { get; set; }
        public static string chrProcessLastShift_CKD { get; set; } = "";

        public async Task Interface()
        {
            try
            {
                await getCKD_ProcessDateTime();

                string sqlQuery = $@"SELECT A.F_Value3 AS Last_Order, B.F_Value2 AS Step_Order
                    FROM TB_MS_Parameter A, TB_MS_Parameter B
                    WHERE A.F_Code = 'LO_CKD' AND B.F_Code = 'ST_CKD' ";

                var _dt = _FillDT.ExecuteSQL(sqlQuery);

                if (_dt.Rows.Count > 0)
                {
                    if (_dt.Rows[0]["Last_Order"].ToString().CompareTo(dateProcessDate_CKD.ToString("yyyyMMdd") + chrProcessShift_CKD) < 0
                        && _dt.Rows[0]["Step_Order"].ToString() == "0")
                    {
                        await _kbContext.Database.ExecuteSqlRawAsync("Exec CKD_Inhouse.SP_INF_CKDORDER {0},{1}",
                            dateProcessDate_CKD.ToString("yyyyMMdd"), chrProcessShift_CKD);

                        _log.WriteLogMsg($"Exec CKD_Inhouse.SP_INF_CKDORDER '{dateProcessDate_CKD.ToString("yyyyMMdd")}' , '{chrProcessShift_CKD}' ");
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<DataTable> getCKD_ProcessDateTime()
        {
            try
            {
                string strDateLogin = _httpContextAccessor.HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "loginDate").Value.ToString();
                DateLogin = DateTime.ParseExact(strDateLogin.Replace("N", string.Empty).Replace("D", string.Empty), "yyyy-MM-dd",null);
                var test = _httpContextAccessor.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "Shift").Value.ToString();
                string sqlQuery = $@"EXEC [CKD_Inhouse].[sp_getProcessDateTime] '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}'
                    ,'{DateLogin.ToString("yyyy-MM-dd")} {(_httpContextAccessor.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "Shift").Value.ToString() == "1" ? "07:30:00" : "19:30:00")}' ";


                var _dt = await _FillDT.ExecuteSQLAsync(sqlQuery);

                //_log.WriteLogMsg($"_httpContextAccessor.HttpContext.Request.Path.Value!.ToLower(): {_httpContextAccessor.HttpContext.Request.Path.Value!.ToLower()}");
                if(_httpContextAccessor.HttpContext.Request.Host.Value!.ToLower().Contains("localhost"))
                {
                    dateProcessDate_CKD = DateTime.ParseExact(_dt.Rows[0]["ProcessDate"].ToString(), "M/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    chrProcessShift_CKD = _dt.Rows[0]["ProcessShift"].ToString();
                    strProcessCycle = _dt.Rows[0]["ProcessCycleTime"].ToString();
                    dateProcessLastDate_CKD = DateTime.ParseExact(_dt.Rows[0]["LastProcessDate"].ToString(), "M/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    chrProcessLastShift_CKD = _dt.Rows[0]["LastProcessShift"].ToString();
                }
                else
                {
                    dateProcessDate_CKD = DateTime.ParseExact(_dt.Rows[0]["ProcessDate"].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    chrProcessShift_CKD = _dt.Rows[0]["ProcessShift"].ToString();
                    strProcessCycle = _dt.Rows[0]["ProcessCycleTime"].ToString();
                    dateProcessLastDate_CKD = DateTime.ParseExact(_dt.Rows[0]["LastProcessDate"].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    chrProcessLastShift_CKD = _dt.Rows[0]["LastProcessShift"].ToString();
                }

                return _dt;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
