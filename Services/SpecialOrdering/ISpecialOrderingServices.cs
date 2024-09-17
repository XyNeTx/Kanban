using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Import;

namespace KANBAN.Services.SpecialOrdering
{

    public interface ISpecialOrderingServices
    {
        IKBNOR210_2 IKBNOR210_2 { get; }
    }

    public class SpecialOrderingServices : ISpecialOrderingServices
    {
        public IKBNOR210_2 IKBNOR210_2 { get; }

        public SpecialOrderingServices(KB3Context kbContext,
            BearerClass bearerClass,
            PPM3Context ppm3Context,
            FillDataTable fillDT,
            SerilogLibs log,
            IEmailService emailService)
        {

            IKBNOR210_2 = new KBNOR210_2(kbContext,
            bearerClass,
            ppm3Context,
            fillDT,
            log,
            emailService);
        }
    }
}
