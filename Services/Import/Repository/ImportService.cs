using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Import.Interface;

namespace KANBAN.Services.Import.Repository
{
    public class ImportService : IImportService
    {
        public IKBNIM010 KBNIM010 { get; }
        public IKBNIM011 KBNIM011 { get; }

        public ImportService(KB3Context kbContext,
            BearerClass bearerClass,
            PPM3Context ppm3Context,
            FillDataTable fillDT,
            SerilogLibs log,
            IEmailService emailService)
        {

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
        }
    }

}
