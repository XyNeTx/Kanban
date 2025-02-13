using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS030 : IKBNMS030
    {

        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNMS030
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMapService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _automapService = autoMapService;
        }

        public static string strDateNow = DateTime.Now.ToString("yyyyMMdd");


        public async Task<List<TB_MS_LineControl>> GetListData(string? F_Line_ID, string? F_Description, string? F_Customer)
        {
            try
            {
                var data = await _kbContext.TB_MS_LineControl.AsNoTracking()
                    .ToListAsync();

                if (!string.IsNullOrEmpty(F_Line_ID))
                {
                    data = data.Where(x => x.F_Line_ID == F_Line_ID).ToList();
                }
                if (!string.IsNullOrEmpty(F_Description))
                {
                    data = data.Where(x => x.F_Description == F_Description).ToList();
                }
                if (!string.IsNullOrEmpty(F_Customer))
                {
                    data = data.Where(x => x.F_Customer == F_Customer).ToList();
                }

                return data.OrderBy(x => x.F_Line_ID).ToList();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Insert(TB_MS_LineControl data)
        {
            try
            {
                var check = await _kbContext.TB_MS_LineControl.AsNoTracking()
                    .Where(x => x.F_Line_Customer == data.F_Line_Customer)
                    .FirstOrDefaultAsync();

                if (check != null)
                {
                    throw new CustomHttpException(400, "Data already exist");
                }

                data.F_Update_Date = DateTime.Now;
                data.F_Update_By = _BearerClass.UserCode;

                _kbContext.TB_MS_LineControl.Add(data);
                _log.WriteLogMsg("INSERT TB_MS_LineControl : " + JsonConvert.SerializeObject(data));
                await _kbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Update(TB_MS_LineControl data)
        {
            try
            {
                var check = await _kbContext.TB_MS_LineControl
                    .Where(x => x.F_Line_Customer == data.F_Line_Customer)
                    .FirstOrDefaultAsync();

                if (check == null)
                {
                    throw new CustomHttpException(400, "Data not found");
                }

                _log.WriteLogMsg("UPDATE TB_MS_LineControl BEFORE : " + JsonConvert.SerializeObject(data));

                _kbContext.TB_MS_LineControl.Remove(check);
                data.F_Update_By = _BearerClass.UserCode;
                data.F_Update_Date = DateTime.Now;

                _kbContext.TB_MS_LineControl.Add(data);
                _log.WriteLogMsg("UPDATE TB_MS_LineControl AFTER : " + JsonConvert.SerializeObject(data));
                await _kbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Delete(List<TB_MS_LineControl> listData)
        {
            try
            {
                foreach (var data in listData)
                {
                    var check = await _kbContext.TB_MS_LineControl
                        .Where(x => x.F_Line_ID == data.F_Line_ID)
                        .FirstOrDefaultAsync();

                    if (check == null)
                    {
                        throw new CustomHttpException(400, "Data not found");
                    }

                    _kbContext.TB_MS_LineControl.Remove(check);
                    _log.WriteLogMsg("DELETE TB_MS_LineControl : " + JsonConvert.SerializeObject(check));
                }
                await _kbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

    }
}
