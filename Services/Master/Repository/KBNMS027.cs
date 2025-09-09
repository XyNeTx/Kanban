using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS027 : IKBNMS027
    {

        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public KBNMS027
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMapService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _automapService = autoMapService;
            _httpContextAccessor = httpContextAccessor;
        }

        public static string strDateNow = DateTime.Now.ToString("yyyyMMdd");

        public async Task<List<TB_MS_Matching_Supplier>> GetShortLogistic()
        {
            try
            {
                var data = await _kbContext.TB_MS_Matching_Supplier
                    .AsNoTracking()
                    .ToListAsync();

                data = data.DistinctBy(x => x.F_short_Logistic.Trim())
                    .OrderBy(x => x.F_short_Logistic).ToList();

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<T_Supplier_MS>> GetShortName()
        {
            try
            {
                var data = await _PPM3Context.T_Supplier_MS
                    .AsNoTracking()
                    .Where(x => x.F_TC_Str.CompareTo(strDateNow) <= 0
                    && x.F_TC_End.CompareTo(strDateNow) >= 0)
                    .ToListAsync();

                data = data.DistinctBy(x => x.F_short_name.Trim())
                    .OrderBy(x => x.F_short_name).ToList();

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_MS_Matching_Supplier>> GetListData(string? F_Short_Logistic)
        {
            try
            {
                var data = await _kbContext.TB_MS_Matching_Supplier
                    .AsNoTracking()
                    .ToListAsync();

                if (!string.IsNullOrEmpty(F_Short_Logistic))
                {
                    data = data.Where(x => x.F_short_Logistic.Trim() == F_Short_Logistic).ToList();
                }

                data = data.DistinctBy(x => new
                {
                    x.F_short_Logistic,
                    x.F_short_name,
                    x.F_Supplier_CD,
                    x.F_Supplier_Plant,
                    x.F_name
                }).OrderBy(x => x.F_short_Logistic)
                .ThenBy(x => x.F_short_name)
                .ThenBy(x => x.F_Supplier_CD)
                .ThenBy(x => x.F_Supplier_Plant)
                .ThenBy(x => x.F_name)
                .ToList();

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<T_Supplier_MS>> SupOrderSelected(string F_Short_Name)
        {
            try
            {
                var data = await _PPM3Context.T_Supplier_MS
                    .AsNoTracking()
                    .Where(x => x.F_short_name.Trim() == F_Short_Name
                    && x.F_TC_Str.CompareTo(strDateNow) <= 0
                    && x.F_TC_End.CompareTo(strDateNow) >= 0)
                    .OrderBy(x => x.F_supplier_cd)
                    .ThenBy(x => x.F_Plant_cd)
                    .ToListAsync();

                data = data.DistinctBy(x => new
                {
                    x.F_supplier_cd,
                    x.F_Plant_cd,
                    x.F_short_name,
                    //x.F_name
                }).ToList();

                return data == null ? throw new CustomHttpException(404, "Data not found") : data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Save(List<VM_KBNMS027> listObj, string action)
        {
            try
            {
                VM_KBNMS027 obj = listObj!.FirstOrDefault();

                if (action.ToLower() == "new")
                {
                    var existed = _kbContext.TB_MS_Matching_Supplier
                        .Where(x => x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim() == obj.F_Supplier_CD)
                        .ToList();

                    if (existed.Count > 0)
                    {
                        throw new CustomHttpException(400, "Data already existed");
                    }

                    TB_MS_Matching_Supplier addObj = new TB_MS_Matching_Supplier
                    {
                        F_short_Logistic = obj.F_Short_Logistic,
                        F_short_name = obj.F_Short_Name,
                        F_Supplier_CD = obj.F_Supplier_CD.Split("-")[0],
                        F_Supplier_Plant = obj.F_Supplier_CD.Split("-")[1],
                        F_name = obj.F_name,
                        F_Update_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value,
                        F_Update_Date = DateTime.Now
                    };

                    await _kbContext.TB_MS_Matching_Supplier.AddAsync(addObj);

                    _log.WriteLogMsg("INSERT TB_MS_Matching_Supplier " + JsonConvert.SerializeObject(addObj));
                }
                else if (action.ToLower() == "upd")
                {
                    var data = await _kbContext.TB_MS_Matching_Supplier
                        .Where(x => x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim() == obj.F_Supplier_CD + "-" + obj.F_Supplier_Plant)
                        .ExecuteUpdateAsync(set => set.SetProperty(x => x.F_short_Logistic, obj.F_Short_Logistic)
                        .SetProperty(x => x.F_Update_By, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value)
                        .SetProperty(x => x.F_Update_Date, DateTime.Now));

                    _log.WriteLogMsg($@"UPDATE TB_MS_Matching_Supplier
                        _kbContext.TB_MS_Matching_Supplier
                        .Where(x=>x.F_Supplier_CD + ""-"" + x.F_Supplier_Plant == {obj.F_Supplier_CD})
                        .ExecuteUpdateAsync(set=>set.SetProperty(x=>x.F_short_Logistic,{obj.F_Short_Logistic})
                        .SetProperty(x => x.F_Update_By, {_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value})
                        .SetProperty(x => x.F_Update_Date, {DateTime.Now}))
                        ");
                }
                else if (action.ToLower() == "del")
                {
                    foreach (var delObj in listObj)
                    {

                        var data = await _kbContext.TB_MS_Matching_Supplier
                            .Where(x => x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim() == delObj.F_Supplier_CD + "-" + delObj.F_Supplier_Plant
                            && x.F_short_Logistic.Trim() == delObj.F_Short_Logistic
                            && x.F_short_name.Trim() == delObj.F_Short_Name)
                            .ExecuteDeleteAsync();

                        _log.WriteLogMsg($@"DELETE TB_MS_Matching_Supplier
                        _kbContext.TB_MS_Matching_Supplier
                        .Where(x => x.F_Supplier_CD + ""-"" + x.F_Supplier_Plant == {delObj.F_Supplier_CD}
                        && x.F_short_Logistic == {delObj.F_Short_Logistic}
                        && x.F_short_name == {delObj.F_Short_Name})
                        .ExecuteDeleteAsync()");

                    }

                }
                else
                {
                    throw new CustomHttpException(400, "Invalid action");
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
