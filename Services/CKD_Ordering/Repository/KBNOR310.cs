using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.CKD_Ordering.IRepository;
using System.Data;

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


        public KBNOR310
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMapService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _automapService = autoMapService;
        }

        public static string strDateNow = DateTime.Now.ToString("yyyyMMdd");

        public static DateTime dateProcessDate_CKD;
        public static string chrProcessShift_CKD = "";
        public static string strProcessCycle = "";
        public static DateTime dateProcessLastDate_CKD;
        public static string chrProcessLastShift_CKD = "";

        public async Task Interface()
        {
            try
            {
                string sqlQuery = $@"SELECT A.F_Value3 AS Last_Order, B.F_Value2 AS Step_Order 
                    FROM TB_MS_Parameter A, TB_MS_Parameter B 
                    WHERE A.F_Code = 'LO_CKD' AND B.F_Code = 'ST_CKD' ";

                var _dt = _FillDT.ExecuteSQL(sqlQuery);

                if (_dt.Rows.Count > 0)
                {
                    //if (_dt.Rows[0]["Last_Order"].ToString())
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
                string sqlQuery = $@"EXEC [CKD_Inhouse].[sp_getProcessDateTime] '{_BearerClass.Plant}' 
                    ,'{DateTime.Now.ToString("dd/MM/yyyy")}','{(_BearerClass.Shift == "Day" ? "07:30:00" : "19:30:00")}' ";

                var _dt = _FillDT.ExecuteSQL(sqlQuery);

                dateProcessDate_CKD = DateTime.ParseExact(_dt.Rows[0]["ProcessDate"].ToString())

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
