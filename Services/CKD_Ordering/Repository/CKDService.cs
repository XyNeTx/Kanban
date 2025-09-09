using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.CKD_Ordering.IRepository;

namespace KANBAN.Services.CKD_Ordering.Repository
{
    public class CKDService : ICKDService
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly CKDWH_Context _CKDContext;
        private readonly CKDUSA_Context _CKDUSAContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly IKBNOR320 _kBNOR320;

        public CKDService(
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMapService,
            CKDWH_Context cKDContext,
            CKDUSA_Context cKDUSAContext,
            IHttpContextAccessor httpContextAccessor,
            HttpClient httpClient,
            IKBNOR320 kBNOR320
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _automapService = autoMapService;
            _CKDContext = cKDContext;
            _CKDUSAContext = cKDUSAContext;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _kBNOR320 = kBNOR320;
        }

        public IKBNOR300 IKBNOR300_Repo
        {
            get
            {
                return new KBNOR300(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService,
                                    _httpContextAccessor);
            }
        }
        public IKBNOR310 IKBNOR310_Repo
        {
            get
            {
                return new KBNOR310(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService,
                                    _httpContextAccessor);
            }
        }
        public IKBNOR320 IKBNOR320_Repo
        {
            get
            {
                return new KBNOR320(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService,
                                    _httpContextAccessor);
            }
        }
        public IKBNOR321 IKBNOR321_Repo
        {
            get
            {
                return new KBNOR321(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService,
                                    _httpClient,
                                    _httpContextAccessor,
                                    _kBNOR320);
            }
        }
        public IKBNOR330 IKBNOR330_Repo
        {
            get
            {
                return new KBNOR330(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService, _httpContextAccessor);
            }
        }
        public IKBNOR360 IKBNOR360_Repo
        {
            get
            {
                return new KBNOR360(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService
                                    ,_CKDContext,
                                    _CKDUSAContext,
                                    _httpContextAccessor
                                    );
            }
        }
        public IKBNOR361 IKBNOR361_Repo
        {
            get
            {
                return new KBNOR361(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService
                                    ,_CKDContext,
                                    _CKDUSAContext,
                                    _httpContextAccessor
                                    );
            }
        }

        public IKBNOR370 IKBNOR370_Repo
        {
            get
            {
                return new KBNOR370(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService
                                    , _CKDContext,
                                    _CKDUSAContext,
                                    _httpContextAccessor
                                    );
            }
        }

    }
}
