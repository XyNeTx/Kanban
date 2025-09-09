using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS014 : IKBNMS014
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public KBNMS014
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

        public async Task<string> GetSupplierCode(bool isNew)
        {
            try
            {
                string result = string.Empty;
                if (isNew)
                {
                    var data = await _PPM3Context.T_Supplier_MS
                        .Select(x => new
                        {
                            F_Supplier_Code = x.F_supplier_cd.Trim()
                        })
                        .AsNoTracking()
                        .ToListAsync();

                    data = data.DistinctBy(x => x.F_Supplier_Code)
                        .OrderBy(x => x.F_Supplier_Code).ToList();

                    result = JsonConvert.SerializeObject(data);
                }
                else
                {
                    var data = await _kbContext.TB_MS_PrintKanban
                        .Select(x => new
                        {
                            F_Supplier_Code = x.F_Supplier_Code.Trim()
                        })
                        .AsNoTracking()
                        .ToListAsync();

                    data = data.DistinctBy(x => x.F_Supplier_Code)
                        .OrderBy(x => x.F_Supplier_Code).ToList();

                    result = JsonConvert.SerializeObject(data);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> GetSupplierPlant(bool isNew, string SupplierCode)
        {
            try
            {
                string result = string.Empty;
                if (isNew)
                {
                    var data = await _PPM3Context.T_Supplier_MS
                        .Where(x => x.F_supplier_cd.Trim() == SupplierCode.Trim())
                        .Select(x => new
                        {
                            F_Supplier_Plant = x.F_Plant_cd
                        })
                        .AsNoTracking()
                        .ToListAsync();

                    data = data.DistinctBy(x => x.F_Supplier_Plant)
                        .OrderBy(x => x.F_Supplier_Plant).ToList();

                    result = JsonConvert.SerializeObject(data);
                }
                else
                {
                    var data = await _kbContext.TB_MS_PrintKanban
                        .Where(x => x.F_Supplier_Code.Trim() == SupplierCode.Trim())
                        .Select(x => new
                        {
                            F_Supplier_Plant = x.F_Supplier_Plant
                        })
                        .AsNoTracking()
                        .ToListAsync();

                    data = data.DistinctBy(x => x.F_Supplier_Plant)
                        .OrderBy(x => x.F_Supplier_Plant).ToList();

                    result = JsonConvert.SerializeObject(data);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> GetShortName(string SupplierCode, string SupplierPlant, bool isNew)
        {
            try
            {
                string result = string.Empty;

                if (isNew)
                {
                    var data = await _PPM3Context.T_Supplier_MS
                        .Where(x => x.F_supplier_cd == SupplierCode.Trim()
                            && x.F_Plant_cd == SupplierPlant[0])
                        .Select(x => new
                        {
                            F_Short_Name = "(" + x.F_short_name.Trim() + ") " + x.F_name.Trim(),
                        })
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    if (data == null)
                    {
                        throw new CustomHttpException(400, "Data not found in System");
                    }

                    result = JsonConvert.SerializeObject(data);

                }
                else
                {
                    var data = await _kbContext.TB_MS_PrintKanban
                        .Where(x => x.F_Supplier_Code == SupplierCode.Trim()
                            && x.F_Supplier_Plant == SupplierPlant)
                        .Select(x => new
                        {
                            F_Short_Name = x.F_Short_Name.Trim(),
                        })
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    if (data == null)
                    {
                        throw new CustomHttpException(400, "Data not found in System");
                    }

                    result = JsonConvert.SerializeObject(data);

                }

                return result;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Save(List<VM_Save_KBNMS014> listobj, string action)
        {
            try
            {
                var obj = listobj.FirstOrDefault();
                if (action.ToLower() == "new")
                {
                    var isAny = await _kbContext.TB_MS_PrintKanban
                        .AnyAsync(x => x.F_Supplier_Code == obj.SupplierCode.Trim()
                        && x.F_Supplier_Plant == obj.SupplierPlant
                        && x.F_Plant == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value);

                    if (isAny)
                    {
                        throw new CustomHttpException(400, "Data already exist in System");
                    }

                    TB_MS_PrintKanban addObj = new TB_MS_PrintKanban
                    {
                        F_Plant = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value,
                        F_Supplier_Code = obj.SupplierCode.Trim(),
                        F_Supplier_Plant = obj.SupplierPlant,
                        F_Flag_Print = obj.FlagPrint,
                        F_Short_Name = obj.SupplierName.Trim(),
                        F_Create_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value,
                        F_Create_Date = DateTime.Now,
                        F_Update_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value,
                        F_Update_Date = DateTime.Now
                    };

                    _kbContext.TB_MS_PrintKanban.Add(addObj);
                    _log.WriteLogMsg($"Insert TB_MS_PrintKanban : {JsonConvert.SerializeObject(addObj)}");

                }
                else if (action.ToLower() == "upd")
                {
                    var data = await _kbContext.TB_MS_PrintKanban
                        .Where(x => x.F_Supplier_Code == obj.SupplierCode.Trim()
                        && x.F_Supplier_Plant == obj.SupplierPlant
                        && x.F_Plant == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value)
                        .FirstOrDefaultAsync();

                    if (data == null)
                    {
                        throw new CustomHttpException(400, "Data not found in System");
                    }

                    data.F_Flag_Print = obj.FlagPrint;
                    data.F_Update_Date = DateTime.Now;
                    data.F_Update_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value;

                    _kbContext.TB_MS_PrintKanban.Update(data);
                    _log.WriteLogMsg($"Update TB_MS_PrintKanban : {JsonConvert.SerializeObject(data)}");
                }
                else
                {
                    foreach (var delObj in listobj)
                    {

                        var data = await _kbContext.TB_MS_PrintKanban
                            .Where(x => x.F_Supplier_Code == delObj.SupplierCode.Trim()
                            && x.F_Supplier_Plant == delObj.SupplierPlant
                            && x.F_Plant == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value)
                            .FirstOrDefaultAsync();

                        if (data == null)
                        {
                            throw new CustomHttpException(400, "Data not found in System");
                        }

                        _kbContext.TB_MS_PrintKanban.Remove(data);
                        _log.WriteLogMsg($"Delete TB_MS_PrintKanban : {JsonConvert.SerializeObject(data)}");

                    }

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

        public async Task<List<TB_MS_PrintKanban>> GetListData(string? SupplierCode, string? SupplierPlant)
        {
            try
            {
                var data = await _kbContext.TB_MS_PrintKanban
                    .AsNoTracking()
                    .Where(x => x.F_Plant == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value)
                    .ToListAsync();

                if (!string.IsNullOrWhiteSpace(SupplierCode))
                {
                    data = data.Where(x => x.F_Supplier_Code.Contains(SupplierCode)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(SupplierPlant))
                {
                    data = data.Where(x => x.F_Supplier_Plant.Contains(SupplierPlant)).ToList();
                }

                return data.OrderBy(x => x.F_Supplier_Code).ThenBy(x => x.F_Supplier_Plant).ToList();

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
