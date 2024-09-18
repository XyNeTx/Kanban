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
    }

    public class SpecialOrderingServices : ISpecialOrderingServices
    {
        public IKBNOR210 IKBNOR210 { get; }
        public IKBNOR210_1 IKBNOR210_1 { get; }
        public IKBNOR210_2 IKBNOR210_2 { get; }
        public IKBNOR210_3 IKBNOR210_3 { get; }

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
        }
    }
}
