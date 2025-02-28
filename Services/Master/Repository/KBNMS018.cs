using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS018 : IKBNMS018
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _autoMap;


        public KBNMS018
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


        public async Task<List<TB_MS_Heijunka>> GetListData(string? CycleB)
        {
            try
            {
                var data = await _kbContext.TB_MS_Heijunka.AsNoTracking()
                    .Where(x => x.F_Plant == _BearerClass.Plant).ToListAsync();

                if (!string.IsNullOrWhiteSpace(CycleB))
                {
                    data = data.Where(x => x.F_CycleB == CycleB).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }

        }

        public async Task Save(TB_MS_Heijunka obj, string action)
        {
            try
            {
                var existObj = await _kbContext.TB_MS_Heijunka.AsNoTracking()
                    .Where(x => x.F_CycleB == obj.F_CycleB && x.F_Plant == _BearerClass.Plant)
                    .FirstOrDefaultAsync();

                if (action == "del" && existObj != null)
                {
                    _kbContext.TB_MS_Heijunka.Remove(existObj);
                }
                else if (existObj != null)
                {
                    obj.F_Plant = existObj.F_Plant;
                    obj.F_CycleB = existObj.F_CycleB;
                    obj.F_Create_Date = existObj.F_Create_Date;
                    obj.F_Create_By = existObj.F_Create_By;
                    obj.F_Update_By = _BearerClass.UserCode;
                    obj.F_Update_Date = DateTime.Now;
                    _kbContext.TB_MS_Heijunka.Attach(obj);
                    _kbContext.Entry(obj).State = EntityState.Modified;
                }
                else
                {
                    obj.F_Plant = _BearerClass.Plant;
                    obj.F_Create_By = _BearerClass.UserCode;
                    obj.F_Create_Date = DateTime.Now;
                    obj.F_Update_Date = DateTime.Now;
                    obj.F_Update_By = _BearerClass.UserCode;

                    _kbContext.TB_MS_Heijunka.Add(obj);
                }

                await _kbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
