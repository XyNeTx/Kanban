using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Import.Interface;
using KANBAN.Services.SpecialOrdering.Interface;

namespace KANBAN.Services.Import.Repository
{
    public class ImportService : IImportService
    {
        public IKBNIM007 KBNIM007 { get; }
        public IKBNIM007T KBNIM007T { get; }
        public IKBNIM010 KBNIM010 { get; }
        public IKBNIM011 KBNIM011 { get; }

        public ImportService(KB3Context kbContext,
            BearerClass bearerClass,
            PPM3Context ppm3Context,
            FillDataTable fillDT,
            SerilogLibs log,
            IEmailService emailService,
            ProcDBContext procDBContext,
            IAutoMapService autoMapService)
        {

            KBNIM007 = new KBNIM007(kbContext,
                bearerClass,
                ppm3Context,
                fillDT,
                log,
                emailService,
                autoMapService);


            KBNIM010 = new KBNIM010(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService);

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
        }
    }

}
