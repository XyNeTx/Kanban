using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.OtherCondition.IRepository;

namespace KANBAN.Services.OtherCondition.Repository
{
    public class OtherConditionRepo : IOtherConditionRepo
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OtherConditionRepo(
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

        public IKBNOC120 IKBNOC120
        {
            get
            {
                return new KBNOC120(_kbContext, _BearerClass, _PPM3Context, _FillDT, _log, _emailService, _automapService,
                                    _httpContextAccessor);
            }
        }

        public IKBNOC121 IKBNOC121
        {
            get
            {
                return new KBNOC121(_kbContext, _BearerClass, _PPM3Context, _FillDT, _log, _emailService, _automapService,
                                    _httpContextAccessor);
            }
        }

        public IKBNOC150 IKBNOC150
        {
            get
            {
                return new KBNOC150(_kbContext, _BearerClass, _PPM3Context, _FillDT, _log, _emailService, _automapService,
                                    _httpContextAccessor);
            }
        }

        public IKBNOC160 IKBNOC160
        {
            get
            {
                return new KBNOC160(_kbContext, _BearerClass, _PPM3Context, _FillDT, _log, _emailService, _automapService, _httpContextAccessor);
            }
        }

    }
}
