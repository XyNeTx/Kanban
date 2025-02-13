using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS029 : IKBNMS029
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _autoMap;


        public KBNMS029
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMap
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _autoMap = autoMap;
        }

        private readonly string strDateNow = DateTime.Now.ToString("yyyyMMdd");

        public async Task<string> GetListData(string? Plant, string? DockCode)
        {
            try
            {
                var data = await _kbContext.TB_MS_Dock_Code
                    .AsNoTracking()
                    .Where(x => x.F_Start_Date.CompareTo(strDateNow) <= 0
                        && x.F_End_Date.CompareTo(strDateNow) >= 0)
                    .Join(_kbContext.TB_MS_Factory.AsNoTracking()
                    , dock => new { dock.F_Plant }
                    , fac => new { fac.F_Plant }
                    , (dock, fac) => new { dock, fac }).ToListAsync();

                if (!string.IsNullOrEmpty(Plant))
                {
                    data = data.Where(x => x.dock.F_Plant == Plant).ToList();
                }
                if (!string.IsNullOrEmpty(DockCode))
                {
                    data = data.Where(x => x.dock.F_Dock_Code == DockCode).ToList();
                }

                return JsonConvert.SerializeObject(data);


            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

    }
}
