using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Import;

namespace KANBAN.Services.SpecialOrdering
{

    public interface ISpecialOrderingServices
    {
        IKBNOR210 IKBNOR210 { get; }
        IKBNOR210_1 IKBNOR210_1 { get; }
        IKBNOR210_2 IKBNOR210_2 { get; }
        IKBNOR210_3 IKBNOR210_3 { get; }
        IKBNOR293 IKBNOR293 { get; }
        IKBNOR294 IKBNOR294 { get; }
        IKBNOR295 IKBNOR295 { get; }
        IKBNMS004 IKBNMS004 { get; }
    }

    public class SpecialOrderingServices : ISpecialOrderingServices
    {
        public IKBNOR210 IKBNOR210 { get; }
        public IKBNOR210_1 IKBNOR210_1 { get; }
        public IKBNOR210_2 IKBNOR210_2 { get; }
        public IKBNOR210_3 IKBNOR210_3 { get; }
        public IKBNOR293 IKBNOR293 { get; }
        public IKBNOR294 IKBNOR294 { get; }
        public IKBNOR295 IKBNOR295 { get; }
        public IKBNMS004 IKBNMS004 { get; }

        public SpecialOrderingServices(KB3Context kbContext,
            BearerClass bearerClass,
            PPM3Context ppm3Context,
            FillDataTable fillDT,
            SerilogLibs log,
            IEmailService emailService)
        {

            IKBNOR210 = new KBNOR210(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService);

            IKBNOR210_1 = new KBNOR210_1(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService);

            IKBNOR210_2 = new KBNOR210_2(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService);
            
            IKBNOR210_3 = new KBNOR210_3(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService);

            IKBNOR293 = new KBNOR293(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService);
            
            IKBNOR294 = new KBNOR294(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService);
            
            IKBNOR295 = new KBNOR295(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService);

            IKBNMS004 = new KBNMS004(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService);

        }
    }
}
