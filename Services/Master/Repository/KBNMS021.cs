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
    public class KBNMS021 : IKBNMS021
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNMS021
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

        public async Task<List<TB_MS_PartCode>> GetListDataTables(string? Line, string? PartCode, string? PartNo)
        {
            try
            {
                var data = await _kbContext.TB_MS_PartCode
                    .Where(x => (string.IsNullOrEmpty(Line) || x.F_Line == Line) &&
                    (string.IsNullOrEmpty(PartCode) || x.F_Code == PartCode) &&
                    (string.IsNullOrEmpty(PartNo) || x.F_Part_No + "-" + x.F_Ruibetsu == PartNo) &&
                    !string.IsNullOrEmpty(x.F_Line))
                    .ToListAsync();

                if (data.Count == 0)
                {
                    throw new CustomHttpException(404, "Data Not Found");
                }

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_MS_PartCode>> GetLine(string? PartCode, string? PartNo)
        {
            try
            {
                var data = await _kbContext.TB_MS_PartCode
                    .Where(x => (string.IsNullOrEmpty(PartCode) || x.F_Code == PartCode) &&
                    (string.IsNullOrEmpty(PartNo) || x.F_Part_No == PartNo) &&
                    !string.IsNullOrEmpty(x.F_Line))
                    .ToListAsync();

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_MS_PartCode>> GetPartCode(string? Line, string? PartNo)
        {
            try
            {
                var data = await _kbContext.TB_MS_PartCode
                    .Where(x => (string.IsNullOrEmpty(Line) || x.F_Line == Line) &&
                    (string.IsNullOrEmpty(PartNo) || x.F_Part_No == PartNo) &&
                    !string.IsNullOrEmpty(x.F_Code))
                    .ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_MS_PartCode>> GetPartNo(string? Line, string? PartCode)
        {
            try
            {
                var data = await _kbContext.TB_MS_PartCode
                    .Where(x => (string.IsNullOrEmpty(Line) || x.F_Line == Line) &&
                    (string.IsNullOrEmpty(PartCode) || x.F_Code == PartCode) &&
                    (!string.IsNullOrEmpty(x.F_Part_No) && !string.IsNullOrEmpty(x.F_Ruibetsu)))
                    .ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Save(List<TB_MS_PartCode> listObj, string action)
        {
            try
            {
                if (action.ToLower() == "new")
                {

                    var isExisted = _kbContext.TB_MS_PartCode.Any(x => x.F_Line == listObj[0].F_Line &&
                        x.F_Code == listObj[0].F_Code && x.F_Part_No == listObj[0].F_Part_No &&
                        x.F_Ruibetsu == listObj[0].F_Ruibetsu);

                    if (isExisted)
                    {
                        throw new CustomHttpException(400, "Data Already Exist");
                    }

                    if (listObj[0] != null)
                    {
                        listObj[0].F_Create_By = _BearerClass.UserCode;
                        listObj[0].F_Create_Date = DateTime.Now;
                        listObj[0].F_Update_Date = DateTime.Now;
                        listObj[0].F_Update_By = _BearerClass.UserCode;

                        await _kbContext.TB_MS_PartCode.AddAsync(listObj[0]);
                        _log.WriteLogMsg($"Insert TB_MS_PartCode : {JsonConvert.SerializeObject(listObj[0])}");
                    }
                    else
                    {
                        throw new CustomHttpException(500, "Can't Convert Data to Object");
                    }

                }
                else if (action.ToLower() == "del")
                {
                    var removeList = await _kbContext.TB_MS_PartCode
                        .Where(x => x.F_Line == listObj[0].F_Line &&
                        x.F_Code == listObj[0].F_Code && x.F_Part_No == listObj[0].F_Part_No &&
                        x.F_Ruibetsu == listObj[0].F_Ruibetsu)
                        .ToListAsync();

                    _kbContext.TB_MS_PartCode.RemoveRange(removeList);
                    _log.WriteLogMsg($"Delete TB_MS_PartCode : {JsonConvert.SerializeObject(removeList)}");
                }
                else
                {
                    foreach (var item in listObj)
                    {
                        var obj = await _kbContext.TB_MS_PartCode
                            .Where(x => x.F_Line == item.F_Line &&
                            x.F_Code == item.F_Code && x.F_Part_No == item.F_Part_No &&
                            x.F_Ruibetsu == item.F_Ruibetsu)
                            .FirstOrDefaultAsync();

                        if (obj != null)
                        {
                            obj.F_Bridge = item.F_Bridge;
                            obj.F_Detail = item.F_Detail;
                            obj.F_Update_By = _BearerClass.UserCode;
                            obj.F_Update_Date = DateTime.Now;

                            _kbContext.TB_MS_PartCode.Update(obj);
                            _log.WriteLogMsg($"Update TB_MS_PartCode : {JsonConvert.SerializeObject(obj)}");
                        }
                        else
                        {
                            throw new CustomHttpException(404, "Data Not Found");
                        }
                    }
                }

                await _kbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_MS_PartCode>> CheckPairPart()
        {
            try
            {
                var dataEF = await _kbContext.TB_MS_PartCode
                    .Where(x => x.F_Line == "S")
                    .ToListAsync();

                return dataEF;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }
    }
}
