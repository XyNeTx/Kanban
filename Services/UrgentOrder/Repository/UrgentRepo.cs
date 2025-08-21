using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.UrgentOrder.IRepository;

namespace KANBAN.Services.UrgentOrder.Repository
{
    public class UrgentRepo : IUrgentRepo
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _InvenContext;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;

        public UrgentRepo
            (
                KB3Context kbContext,
                BearerClass BearerClass,
                PPM3Context PPM3Context,
                PPMInvenContext InvenContext,
                FillDataTable FillDT,
                SerilogLibs log,
                IEmailService emailService,
                IAutoMapService autoMapService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _InvenContext = InvenContext;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _automapService = autoMapService;
        }

        public IKBNIM017R IKBNIM017R_Repo
        {
            get
            {
                return new KBNIM017R(_kbContext,
                                    _BearerClass,
                                    _PPM3Context,
                                    _InvenContext,
                                    _FillDT, _log,
                                    _emailService,
                                    _automapService);
            }
        }

        public IKBNIM013_INV IKBNIM013_INV_Repo
        {
            get
            {
                return new KBNIM013_INV(_kbContext,
                                        _BearerClass,
                                        _PPM3Context,
                                        _InvenContext,
                                        _FillDT, _log,
                                        _emailService,
                                        _automapService);
            }
        }

    }
}
