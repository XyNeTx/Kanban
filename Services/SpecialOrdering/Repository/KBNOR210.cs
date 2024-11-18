using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Services.SpecialOrdering.Repository
{
    public class KBNOR210 : IKBNOR210
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNOR210
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
        }

        //public int progressBar = 0;

        public string Page_Load()
        {
            try
            {
                var dt = _FillDT.ExecuteSQL($"EXEC [exec].spKBNOR210_SEARCH '{_BearerClass.Plant}','{_BearerClass.UserCode}'");

                if (dt.Rows.Count == 0)
                {
                    throw new Exception("No data found.");
                }

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Interface()
        {
            try
            {
                await _kbContext.Database.ExecuteSqlRawAsync($"EXEC [exec].spKBNOR210_INF '{_BearerClass.Plant}','{_BearerClass.UserCode}'");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Check_Error()
        {
            try
            {

                int count = await _kbContext.Database.ExecuteSqlRawAsync($@"SELECT * From TB_Import_Error
                        WHERE F_Type = 'KBNOR210' and F_Update_By = '{_BearerClass.UserCode}' ");

                if (count > 0)
                {
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
