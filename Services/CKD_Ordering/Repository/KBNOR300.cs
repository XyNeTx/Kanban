using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace KANBAN.Services.CKD_Ordering.Repository
{
    public class KBNOR300 : IKBNOR300
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNOR300
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

        public async Task<List<string>> GetUserAuthorizeAsync()
        {
            try
            {
                var UserAuth = await _kbContext.UserAuthorize.AsNoTracking()
                    //.Include(x => x.UserErp)
                    //.Include(x => x.MenuErp)
                    .Where(x => x.UserErp.Code == _BearerClass.UserCode
                    && x.MenuErp.i18n.StartsWith("KBNOR3")
                    && x.MenuErp.i18n != "KBNOR300")
                    .Select(x => x.MenuErp.i18n ?? null)
                    .OrderBy(x => x)
                    .ToListAsync();

                if (UserAuth.Count == 0)
                {
                    return new List<string>();
                }
                else
                {
                    return UserAuth!;
                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<List<TB_MS_Parameter>> GetParameterAsync()
        {
            try
            {
                var _Parameter = await _kbContext.TB_MS_Parameter.AsNoTracking()
                    .Where(x => x.F_Code == "LO_CKD" || x.F_Code == "ST_CKD")
                    .ToListAsync();

                if (_Parameter.Count == 0)
                {
                    return null;
                }
                else
                {
                    return _Parameter;
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
