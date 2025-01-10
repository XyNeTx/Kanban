using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;

namespace KANBAN.Services.Master.Repository
{
    public class MasterRepo : IMasterRepo
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;

        public MasterRepo(
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

        public IKBNMS004 IKBNMS004
        {
            get
            {
                return new KBNMS004(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService);
            }
        }

        public IKBNMS009 IKBNMS009
        {
            get
            {
                return new KBNMS009(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService);
            }
        }

        public IKBNMS014 IKBNMS014
        {
            get
            {
                return new KBNMS014(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService);
            }
        }

        public IKBNMS021 IKBNMS021
        {
            get
            {
                return new KBNMS021(_kbContext,
                    _BearerClass,
                    _PPM3Context,
                    _FillDT, _log,
                    _emailService,
                    _automapService);
            }
        }

        public IKBNMS025 IKBNMS025
        {
            get
            {
                return new KBNMS025(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService);
            }
        }

    }
}
