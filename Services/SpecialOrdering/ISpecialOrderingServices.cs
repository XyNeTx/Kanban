using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
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
        IKBNOR220 IKBNOR220 { get; }
        IKBNOR220_1 IKBNOR220_1 { get; }
        IKBNOR220_2 IKBNOR220_2 { get; }
        IKBNOR230 IKBNOR230 { get; }
        IKBNOR240 IKBNOR240 { get; }
        IKBNOR250 IKBNOR250 { get; }
        IKBNOR280 IKBNOR280 { get; }
        IKBNOR290 IKBNOR290 { get; }
        IKBNOR297 IKBNOR297 { get; }

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
        public IKBNOR220 IKBNOR220 { get; }
        public IKBNOR220_1 IKBNOR220_1 { get; }
        public IKBNOR220_2 IKBNOR220_2 { get; }
        public IKBNOR230 IKBNOR230 { get; }
        public IKBNOR240 IKBNOR240 { get; }
        public IKBNOR250 IKBNOR250 { get; }
        public IKBNOR280 IKBNOR280 { get; }
        public IKBNOR290 IKBNOR290 { get; }
        public IKBNOR297 IKBNOR297 { get; }

        public SpecialOrderingServices(KB3Context kbContext,
            BearerClass bearerClass,
            PPM3Context ppm3Context,
            FillDataTable fillDT,
            SerilogLibs log,
            IEmailService emailService,
            ISpecialLibs specialLibs,
            ProcDBContext procDBContext,
            IAutoMapService autoMapService)
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

            IKBNOR220 = new KBNOR220(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            specialLibs,
            emailService
            );

            IKBNOR220_1 = new KBNOR220_1(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService,
            specialLibs,
            this);

            IKBNOR220_2 = new KBNOR220_2(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService,
            specialLibs
            );
            
            IKBNOR230 = new KBNOR230(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService,
            specialLibs,
            procDBContext
            );
            
            IKBNOR240 = new KBNOR240(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService,
            specialLibs
            );
            
            IKBNOR250 = new KBNOR250(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService,
            specialLibs
            );
            
            IKBNOR280 = new KBNOR280(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService,
            specialLibs,
            autoMapService
            );
            
            IKBNOR290 = new KBNOR290(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService,
            specialLibs
            );
            
            IKBNOR297 = new KBNOR297(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService,
            specialLibs
            );

        }
    }
}
