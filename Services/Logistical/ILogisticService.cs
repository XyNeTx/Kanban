using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;

namespace KANBAN.Services.Logistical
{

    public interface ILogisticService
    {
        IKBNLC150 IKBNLC150 { get; }
        IKBNLC190 IKBNLC190 { get; }
    }

    public class LogisticService : ILogisticService
    {
        public IKBNLC150 IKBNLC150 { get; }
        public IKBNLC190 IKBNLC190 { get; }

        public LogisticService(
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService
            )
        {
            IKBNLC150 = new KBNLC150(kbContext,BearerClass,PPM3Context, FillDT, log, emailService);
            IKBNLC190 = new KBNLC190(kbContext,BearerClass,PPM3Context, FillDT, log, emailService);
        }
    }
}
