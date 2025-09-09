using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Import.Interface;

namespace KANBAN.Services.Import.Repository
{
    public class ImportService : IImportService
    {
        public IKBNIM007 KBNIM007 { get; }
        public IKBNIM007T KBNIM007T { get; }
        public IKBNIM007C KBNIM007C { get; }
        public IKBNIM010 KBNIM010 { get; }
        public IKBNIM011 KBNIM011 { get; }
        public IKBNIM0044 KBNIM0044 { get; }
        public IKBNIM012M KBNIM012M { get; }

        public ImportService(KB3Context kbContext,
            BearerClass bearerClass,
            PPM3Context ppm3Context,
            FillDataTable fillDT,
            SerilogLibs log,
            IEmailService emailService,
            ProcDBContext procDBContext,
            IAutoMapService autoMapService,
            ProcDBContext procContext,
            IHttpContextAccessor httpContextAccessor
            )
        {

            KBNIM007 = new KBNIM007(kbContext,
                bearerClass,
                ppm3Context,
                fillDT,
                log,
                emailService,
                autoMapService,
                httpContextAccessor);


            KBNIM010 = new KBNIM010(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService,
                httpContextAccessor);

            KBNIM011 = new KBNIM011(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService);

            KBNIM007T = new KBNIM007T(kbContext,
                bearerClass,
                ppm3Context,
                fillDT,
                log,
                emailService,
                autoMapService);

            KBNIM007C = new KBNIM007C(kbContext,
                bearerClass,
                ppm3Context,
                fillDT,
                log,
                emailService,
                autoMapService);

            KBNIM0044 = new KBNIM0044(kbContext,
                bearerClass,
                ppm3Context,
                fillDT,
                log,
                emailService,
                httpContextAccessor);

            KBNIM012M = new KBNIM012M(kbContext, bearerClass, ppm3Context, fillDT, log, emailService, procContext,
                httpContextAccessor);

        }
    }

}
