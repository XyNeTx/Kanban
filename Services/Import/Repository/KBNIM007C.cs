using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Import.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Services.Import.Repository
{
    public class KBNIM007C : IKBNIM007C
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNIM007C
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

        public string GetPDS(string? DeliDateFrom ,string? DeliDateTo)
        {
            try
            {
                string sql = $@"Select Distinct F_PDS_No From TB_TRANSACTION_TMP 
                    Where (F_Type='Special' or F_TYPE ='Trial' ) ";

                if (!string.IsNullOrEmpty(DeliDateFrom) && !string.IsNullOrEmpty(DeliDateTo))
                {
                    sql += $" and F_Delivery_Date between '{DeliDateFrom}' and '{DeliDateTo}' ";
                }

                sql += " Order by F_PDS_No";

                DataTable dt = _FillDT.ExecuteSQL(sql);

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public string GetUser(string? DeliDateFrom, string? DeliDateTo)
        {
            try
            {
                string sql = $@"Select Distinct rtrim(T.F_Update_By) +':' 
                    + rtrim(U.F_User_Name) as F_Update_By From TB_TRANSACTION_TMP
                    T INNER JOIN TB_USER U ON T.F_Update_By = U.F_User_ID  
                     Where (T.F_Type='Special' or T.F_TYPE ='Trial' ) ";

                if (!string.IsNullOrEmpty(DeliDateFrom) && !string.IsNullOrEmpty(DeliDateTo))
                {
                    sql += $" and T.F_Delivery_Date between '{DeliDateFrom}' and '{DeliDateTo}' ";
                }

                sql += " Order by F_Update_By";

                DataTable dt = _FillDT.ExecuteSQL(sql);

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
