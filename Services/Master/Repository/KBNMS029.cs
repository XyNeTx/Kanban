using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
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
                    , (dock, fac) => new { dock, fac })
                    .Select(x => new
                    {
                        F_Create_By = x.dock.F_Create_By,
                        F_Create_Date = x.dock.F_Create_Date,
                        F_Dock_Code = x.dock.F_Dock_Code,
                        F_End_Date = x.dock.F_End_Date,
                        F_PlantCode = x.fac.F_Plant + " : " + x.fac.F_Plant_Name.Trim(),
                        F_Plant = x.dock.F_Plant,
                        F_Start_Date = x.dock.F_Start_Date,
                        F_Update_By = x.dock.F_Update_By,
                        F_Update_Date = x.dock.F_Update_Date,
                    }).ToListAsync();

                if (!string.IsNullOrEmpty(Plant))
                {
                    data = data.Where(x => x.F_Plant == Plant).ToList();
                }
                if (!string.IsNullOrEmpty(DockCode))
                {
                    data = data.Where(x => x.F_Dock_Code.Trim() == DockCode).ToList();
                }

                return JsonConvert.SerializeObject(data);


            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<List<TB_MS_Dock_Code>> GetDockCode()
        {
            try
            {
                var data = await _kbContext.TB_MS_Dock_Code
                    .AsNoTracking()
                    .Where(x => x.F_Start_Date.CompareTo(strDateNow) <= 0
                    && x.F_End_Date.CompareTo(strDateNow) >= 0)
                    .ToListAsync();

                return data.DistinctBy(x => x.F_Dock_Code)
                    .OrderBy(x => x.F_Dock_Code)
                    .ToList();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task Save(List<TB_MS_Dock_Code> listObj, string action)
        {
            try
            {
                var firstObj = listObj.FirstOrDefault();

                var existObj = await _kbContext.TB_MS_Dock_Code.AsNoTracking()
                    .Where(x => x.F_Start_Date.CompareTo(strDateNow) <= 0
                    && x.F_End_Date.CompareTo(strDateNow) >= 0
                    && x.F_Dock_Code == firstObj.F_Dock_Code).FirstOrDefaultAsync();

                string logMessage = "";

                if (action.ToLower() == "new")
                {
                    if (existObj != null)
                    {
                        throw new CustomHttpException(400, "Can't Insert Data. Data was exist in System");
                    }

                    firstObj.F_Plant = _BearerClass.Plant;
                    firstObj.F_Create_Date = DateTime.Now;
                    firstObj.F_Create_By = _BearerClass.UserCode;
                    firstObj.F_Update_Date = DateTime.Now;
                    firstObj.F_Update_By = _BearerClass.UserCode;
                    await _kbContext.TB_MS_Dock_Code.AddAsync(firstObj);
                    logMessage = "INSERT INTO TB_MS_Dock_Code => " + JsonConvert.SerializeObject(firstObj);
                }
                else if (action.ToLower() == "upd")
                {
                    if (existObj == null)
                    {
                        throw new CustomHttpException(400, "Can't Update Data Because Data not found");
                    }

                    logMessage = "UPDATE TO TB_MS_Dock_Code BEFORE => " + JsonConvert.SerializeObject(existObj);
                    _log.WriteLogMsg(logMessage);

                    _kbContext.TB_MS_Dock_Code.Attach(existObj);
                    existObj.F_End_Date = firstObj.F_End_Date;
                    existObj.F_Update_Date = DateTime.Now;
                    existObj.F_Update_By = _BearerClass.UserCode;

                    _kbContext.TB_MS_Dock_Code.Update(existObj);
                    logMessage = "UPDATE TO TB_MS_Dock_Code AFTER => " + JsonConvert.SerializeObject(existObj);
                }
                else if (action.ToLower() == "del")
                {

                    foreach (var each in listObj)
                    {
                        var delObj = await _kbContext.TB_MS_Dock_Code.AsNoTracking()
                            .Where(x => x.F_Start_Date.CompareTo(strDateNow) <= 0
                            && x.F_End_Date.CompareTo(strDateNow) >= 0
                            && x.F_Dock_Code == each.F_Dock_Code).FirstOrDefaultAsync();

                        if (existObj == null)
                        {
                            throw new CustomHttpException(400, "Can't Delete Data Because Data not found");
                        }

                        _kbContext.TB_MS_Dock_Code.Remove(delObj);
                        logMessage = "DELETE TO TB_MS_Dock_Code => " + JsonConvert.SerializeObject(delObj);
                    }
                }
                else
                {
                    throw new CustomHttpException(400, "Please Select action to proceed");
                }

                await _kbContext.SaveChangesAsync();
                _log.WriteLogMsg(logMessage);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

    }
}
