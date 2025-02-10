using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.ReportOrder;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS028 : IKBNMS028
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _autoMap;


        public KBNMS028
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

        public async Task<List<TB_Import_Delivery>> GetDockCode()
        {
            try
            {
                var data = await _kbContext.TB_Import_Delivery
                    .AsNoTracking()
                    .Where(x => x.F_Plant == _BearerClass.Plant)
                    .ToListAsync();

                return data.DistinctBy(x => x.F_Dock_Cd).OrderBy(x => x.F_Dock_Cd).ToList();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<List<TB_MS_Matching_Supplier>> GetShortLogistic()
        {
            try
            {
                var data = await _kbContext.TB_MS_Matching_Supplier
                    .AsNoTracking()
                    .ToListAsync();

                return data.DistinctBy(x => x.F_short_Logistic).OrderBy(x => x.F_short_Logistic).ToList();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<IEnumerable<TB_MS_Remark_DocSheet>> GetListData(string? F_Dock_Cd)
        {
            try
            {
                var data = _kbContext.TB_MS_Remark_DocSheet
                    .AsNoTracking()
                    .Where(x => x.F_Plant == _BearerClass.Plant)
                    .OrderBy(x => x.F_Plant).ThenBy(x => x.F_Dock_Cd)
                    .ThenBy(x => x.F_short_Logistic1).ThenBy(x => x.F_Remark1)
                    .AsEnumerable();

                if (!string.IsNullOrWhiteSpace(F_Dock_Cd))
                {
                    data = data.Where(x => x.F_Dock_Cd == F_Dock_Cd).AsEnumerable();
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task Save(TB_MS_Remark_DocSheet obj, string action)
        {
            try
            {
                var existObj = await _kbContext.TB_MS_Remark_DocSheet
                    //.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.F_Plant == obj.F_Plant
                    && x.F_Dock_Cd == obj.F_Dock_Cd);

                if (action.ToLower() == "new")
                {

                    if (existObj != null) throw new CustomHttpException(400, "Can not Insert this Dock Code, Because exist in this Database!!");

                    else
                    {
                        obj.F_Plant = _BearerClass.Plant;
                        obj.F_Update_By = _BearerClass.UserCode;
                        obj.F_Update_Date = DateTime.Now;
                        await _kbContext.TB_MS_Remark_DocSheet.AddAsync(obj);
                        _log.WriteLogMsg("INSERT INTO TB_MS_Remark_DocSheet => " + JsonConvert.SerializeObject(obj));
                    }

                }
                else if (action.ToLower() == "upd")
                {

                    if (existObj != null)
                    {
                        _log.WriteLogMsg("Updating TB_MS_Remark_DocSheet Before => " + JsonConvert.SerializeObject(existObj));

                        existObj = obj;
                        existObj.F_Update_By = _BearerClass.UserCode;
                        existObj.F_Update_Date = DateTime.Now;
                        _kbContext.TB_MS_Remark_DocSheet.Update(existObj);

                        _log.WriteLogMsg("Updated TB_MS_Remark_DocSheet After => " + JsonConvert.SerializeObject(existObj));
                    }
                    else throw new CustomHttpException(404, "Data is not existed to Update");
                }
                else if (action.ToLower() == "del")
                {
                    if (existObj != null)
                    {
                        _kbContext.TB_MS_Remark_DocSheet.Remove(existObj);
                        _log.WriteLogMsg("DELETE DATA TB_MS_Remark_DocSheet => " + JsonConvert.SerializeObject(obj));
                    }
                }
                else
                {
                    throw new CustomHttpException(400, "Please Select Action to Proceed");
                }

                await _kbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

    }
}
