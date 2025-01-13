using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS025 : IKBNMS025
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNMS025
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

        public string strDateNow = DateTime.Now.ToString("yyyyMMdd");

        public async Task<List<TB_MS_LPSupplier>> GetLogisticSupplier(string? TruckType)
        {
            try
            {
                var data = await _kbContext.TB_MS_LPSupplier
                    .AsNoTracking()
                    .Where(x => x.F_Plant == _BearerClass.Plant
                    && x.F_Start_Date.CompareTo(strDateNow) <= 0
                    && x.F_End_Date.CompareTo(strDateNow) >= 0)
                    .ToListAsync();

                if (!string.IsNullOrEmpty(TruckType))
                {
                    data = data.Where(x => x.F_Truck_Type.Trim() == TruckType.Trim()).ToList();
                }

                return data.DistinctBy(x => x.F_Logistic).OrderBy(x => x.F_Logistic).ToList();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> GetTruckType(bool isNew, string? Logistic)
        {
            try
            {
                if (isNew)
                {
                    var data = await _kbContext.TB_MS_TruckType
                        .AsNoTracking()
                        .OrderBy(x => x.F_Truck_Type)
                        .ToListAsync();

                    return JsonConvert.SerializeObject(data);
                }
                else
                {
                    var data = await _kbContext.TB_MS_LPSupplier
                        .AsNoTracking()
                        .Where(x => x.F_Plant == _BearerClass.Plant
                        && x.F_Start_Date.CompareTo(strDateNow) <= 0
                        && x.F_End_Date.CompareTo(strDateNow) >= 0)
                        .ToListAsync();

                    if (!string.IsNullOrEmpty(Logistic))
                    {
                        data = data.Where(x => x.F_Logistic.Trim() == Logistic.Trim()).ToList();
                    }

                    data = data.DistinctBy(x => new
                    {
                        x.F_Truck_Type
                    }).OrderBy(x => x.F_Truck_Type).ToList();


                    return JsonConvert.SerializeObject(data.DistinctBy(x => x.F_Truck_Type).OrderBy(x => x.F_Truck_Type).ToList());
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> TruckTypeSelected(bool isNew, string? Logistic, string? TruckType)
        {
            try
            {
                if (isNew)
                {
                    var data = await _kbContext.TB_MS_TruckType
                        .AsNoTracking()
                        .Where(x => x.F_Truck_Type.Trim() == TruckType)
                        .ToListAsync();

                    var firstData = data.FirstOrDefault();

                    var cal = ((firstData!.F_Width == null ? 0 : firstData.F_Width)
                        * (firstData!.F_Long == null ? 0 : firstData.F_Long)
                        * (firstData!.F_High == null ? 0 : firstData.F_High)) / 1000000;

                    cal = cal + 0.005f;

                    var m3 = cal.ToString("0.000");

                    var selData = data.Select(x => new
                    {
                        x.F_High,
                        x.F_Long,
                        x.F_Value,
                        x.F_Truck_Type,
                        x.F_Weight,
                        x.F_Width,
                        F_Start_Date = DateTime.ParseExact(strDateNow, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                        F_End_Date = "31/12/2999",
                        M3 = m3,
                    }).ToList();

                    return JsonConvert.SerializeObject(selData);

                }
                else
                {
                    var data = await _kbContext.TB_MS_LPSupplier
                        .AsNoTracking()
                        .Where(x => x.F_Plant == _BearerClass.Plant
                        && x.F_Start_Date.CompareTo(strDateNow) <= 0
                        && x.F_End_Date.CompareTo(strDateNow) >= 0
                        && x.F_Truck_Type.Trim() == TruckType)
                        .Join(_kbContext.TB_MS_TruckType
                        .AsNoTracking(),
                        a => a.F_Truck_Type,
                        b => b.F_Truck_Type,
                        (a, b) => new { a, b }).ToListAsync();

                    if (!string.IsNullOrEmpty(Logistic))
                    {
                        data = data.Where(x => x.a.F_Logistic.Trim() == Logistic.Trim()).ToList();
                    }

                    if (data.Count == 0) throw new CustomHttpException(404, "Data Not Found");

                    var firstData = data.FirstOrDefault();

                    var cal = ((firstData!.b.F_Width == null ? 0 : firstData.b.F_Width)
                        * (firstData!.b.F_Long == null ? 0 : firstData.b.F_Long)
                        * (firstData!.b.F_High == null ? 0 : firstData.b.F_High)) / 1000000;

                    cal = cal + 0.005f;

                    var m3 = cal.ToString("0.000");

                    var selData = data.Select(x => new
                    {
                        x.b.F_High,
                        x.b.F_Long,
                        x.b.F_Value,
                        x.b.F_Truck_Type,
                        x.b.F_Weight,
                        x.b.F_Width,
                        F_Start_Date = DateTime.ParseExact(x.a.F_Start_Date, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                        F_End_Date = DateTime.ParseExact(x.a.F_End_Date, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                        M3 = m3,
                    }).ToList();

                    return JsonConvert.SerializeObject(selData);

                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> GetListData(string? Logistic, string? TruckType)
        {
            try
            {
                var data = await _kbContext.TB_MS_LPSupplier
                    .AsNoTracking()
                    .Where(x => x.F_Plant == _BearerClass.Plant
                    && x.F_Start_Date.CompareTo(strDateNow) <= 0
                    && x.F_End_Date.CompareTo(strDateNow) >= 0)
                    .Join(_kbContext.TB_MS_TruckType,
                    a => a.F_Truck_Type,
                    b => b.F_Truck_Type,
                    (a, b) => new { a, b })
                    .Select(x => new
                    {
                        x.a.F_Logistic,
                        F_Start_Date = DateTime.ParseExact(x.a.F_Start_Date, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                        F_End_Date = DateTime.ParseExact(x.a.F_End_Date, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                        x.a.F_Truck_Type,
                        x.a.F_Weight,
                        F_Weight_TT = x.b.F_Weight,
                        x.b.F_Width,
                        x.b.F_High,
                        x.b.F_Long,
                    })
                    .ToListAsync();

                if (!string.IsNullOrEmpty(Logistic))
                {
                    data = data.Where(x => x.F_Logistic.Trim() == Logistic.Trim()).ToList();
                }

                if (!string.IsNullOrEmpty(TruckType))
                {
                    data = data.Where(x => x.F_Truck_Type.Trim() == TruckType.Trim()).ToList();
                }

                return JsonConvert.SerializeObject(data);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }


        public async Task Save(List<VM_Save_KBNMS025> listObj, string action)
        {
            try
            {
                var obj = listObj.FirstOrDefault();

                if (obj!.f_Start_Date.CompareTo(obj.f_End_Date) > 0)
                {
                    throw new CustomHttpException(400, "Please Select Start Date Less than End Date Before Process Data");
                }

                if (action.ToLower() == "new")
                {
                    Console.WriteLine(strDateNow);
                    if (obj!.f_Start_Date.CompareTo(strDateNow) < 0)
                    {
                        throw new CustomHttpException(400, "Please Select Start Date From Present OR More Before Process Data");
                    }

                    var data = await _kbContext.TB_MS_LPSupplier
                        .AsNoTracking()
                        .Where(x => x.F_Plant == _BearerClass.Plant
                        && x.F_Logistic == obj.f_Logistic
                        && x.F_Truck_Type == obj.f_Truck_Type
                        && x.F_Start_Date.CompareTo(strDateNow) <= 0
                        && x.F_End_Date.CompareTo(strDateNow) >= 0)
                        .ToListAsync();

                    if (data.Count > 0)
                    {
                        throw new CustomHttpException(400, "Data Already Exist");
                    }

                    var addObj = new TB_MS_LPSupplier
                    {
                        F_Create_By = _BearerClass.UserCode,
                        F_Create_Date = DateTime.Now,
                        F_Update_By = _BearerClass.UserCode,
                        F_Update_Date = DateTime.Now,
                        F_Plant = _BearerClass.Plant,
                        F_End_Date = obj.f_End_Date,
                        F_Logistic = obj.f_Logistic,
                        F_Start_Date = obj.f_Start_Date,
                        F_Truck_Type = obj.f_Truck_Type,
                        F_Weight = obj.F_Weight,
                    };

                    await _kbContext.TB_MS_LPSupplier.AddAsync(addObj);

                    _log.WriteLogMsg("INSERT TB_MS_LPSupplier " + JsonConvert.SerializeObject(obj));
                }
                else if (action.ToLower() == "upd")
                {
                    var data = await _kbContext.TB_MS_LPSupplier
                        .AsNoTracking()
                        .Where(x => x.F_Plant == _BearerClass.Plant
                        && x.F_Logistic == obj.f_Logistic
                        && x.F_Truck_Type == obj.f_Truck_Type
                        && x.F_Start_Date.CompareTo(strDateNow) <= 0
                        && x.F_End_Date.CompareTo(strDateNow) >= 0
                        && x.F_Start_Date == obj.f_Start_Date)
                        .ToListAsync();

                    if (data.Count == 0)
                    {
                        throw new CustomHttpException(400, "Data Didn't Exist in System");
                    }

                    await _kbContext.TB_MS_LPSupplier.Where(x => x.F_Plant == _BearerClass.Plant
                        && x.F_Logistic == obj.f_Logistic
                        && x.F_Start_Date == obj.f_Start_Date)
                        .ExecuteUpdateAsync(set => set.SetProperty(x => x.F_End_Date, obj.f_End_Date)
                        .SetProperty(x => x.F_Truck_Type, obj.f_Truck_Type)
                        .SetProperty(x => x.F_Weight, obj.F_Weight)
                        .SetProperty(x => x.F_Update_By, _BearerClass.UserCode)
                        .SetProperty(x => x.F_Update_Date, DateTime.Now));

                    _log.WriteLogMsg("UPDATE TB_MS_LPSupplier " + JsonConvert.SerializeObject(obj));

                }
                else if (action.ToLower() == "del")
                {
                    foreach (var each in listObj)
                    {
                        await _kbContext.TB_MS_LPSupplier.Where(x => x.F_Plant == _BearerClass.Plant
                            && x.F_Logistic == each.f_Logistic
                            && x.F_Start_Date == each.f_Start_Date
                            && x.F_End_Date == each.f_End_Date
                            && x.F_Truck_Type == each.f_Truck_Type
                            && x.F_Weight == each.F_Weight)
                            .ExecuteDeleteAsync();

                        _log.WriteLogMsg("DELETE TB_MS_LPSupplier " + JsonConvert.SerializeObject(each));
                    }
                }
                else throw new CustomHttpException(400, "Action Not Found");

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
