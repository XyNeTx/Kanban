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

        public CKDService(
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

        public IKBNOR310 IKBNOR310
        {
            get
            {
                return new KBNOR310(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService);
            }
        }
        public IKBNOR320 IKBNOR320
        {
            get
            {
                return new KBNOR320(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService);
            }
        }
        public IKBNOR321 IKBNOR321
        {
            get
            {
                return new KBNOR321(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService);
            }
        }

    }
}
