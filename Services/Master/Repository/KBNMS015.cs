using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master.ViewModel;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.PPM3;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS015 : IKBNMS015
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNMS015
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

        public async Task<List<T_Construction>> GetDropDownNew(string? Supplier, string? Kanban, string? StoreCode, string? PartNo)
        {
            try
            {
                var data = await _PPM3Context.T_Construction
                    .Where(x => x.F_Store_cd.StartsWith(_BearerClass.Plant)
                    && x.F_Local_Str.CompareTo(strDateNow) <= 0
                    && x.F_Local_End.CompareTo(strDateNow) >= 0).ToListAsync();

                if (!string.IsNullOrWhiteSpace(Supplier))
                {
                    data = data.Where(x => x.F_supplier_cd + "-" + x.F_plant == Supplier).ToList();
                }
                if (!string.IsNullOrWhiteSpace(Kanban))
                {
                    data = data.Where(x => "0" + x.F_Sebango == Kanban).ToList();
                }
                if (!string.IsNullOrWhiteSpace(StoreCode))
                {
                    data = data.Where(x => x.F_Store_cd == StoreCode).ToList();
                }
                if (!string.IsNullOrWhiteSpace(PartNo))
                {
                    data = data.Where(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == PartNo).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_MS_Label>> GetDropDownInq(string? Supplier, string? Kanban, string? StoreCode, string? PartNo)
        {
            try
            {
                var data = await _kbContext.TB_MS_Label
                    .Where(x => x.F_Plant == _BearerClass.Plant)
                    .ToListAsync();

                if (!string.IsNullOrWhiteSpace(Supplier))
                {
                    data = data.Where(x => x.F_Supplier_Cd + "-" + x.F_Supplier_Plant == Supplier).ToList();
                }
                if (!string.IsNullOrWhiteSpace(Kanban))
                {
                    data = data.Where(x => x.F_Kanban_No == Kanban).ToList();
                }
                if (!string.IsNullOrWhiteSpace(StoreCode))
                {
                    data = data.Where(x => x.F_Store_Code == StoreCode).ToList();
                }
                if (!string.IsNullOrWhiteSpace(PartNo))
                {
                    data = data.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == PartNo).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<T_Supplier_MS> SupplierChanged(string SupplierCode, string? StoreCode)
        {
            try
            {
                var data = await _PPM3Context.T_Supplier_MS
                    .Where(x => x.F_supplier_cd + "-" + x.F_Plant_cd == SupplierCode
                    && x.F_TC_Str.CompareTo(strDateNow) <= 0
                    && x.F_TC_End.CompareTo(strDateNow) >= 0
                    && x.F_Store_cd.StartsWith(_BearerClass.Plant)).ToListAsync();

                if (!string.IsNullOrWhiteSpace(StoreCode))
                {
                    data = data.Where(x => x.F_Store_cd == StoreCode).ToList();
                }

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> PartNoSelectedNew(string PartNo, string? SupplierCode, string? Kanban, string? StoreCode)
        {
            try
            {
                var data = await _PPM3Context.T_Construction
                    .Where(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == PartNo
                    && x.F_Store_cd.StartsWith(_BearerClass.Plant)
                    && x.F_Local_Str.CompareTo(strDateNow) <= 0
                    && x.F_Local_End.CompareTo(strDateNow) >= 0).ToListAsync();

                if (!string.IsNullOrWhiteSpace(SupplierCode))
                {
                    data = data.Where(x => x.F_supplier_cd + "-" + x.F_plant == SupplierCode).ToList();
                }
                if (!string.IsNullOrWhiteSpace(Kanban))
                {
                    data = data.Where(x => "0" + x.F_Sebango == Kanban).ToList();
                }
                if (!string.IsNullOrWhiteSpace(StoreCode))
                {
                    data = data.Where(x => x.F_Store_cd == StoreCode).ToList();
                }

                data = data.DistinctBy(x => new
                {
                    x.F_Part_no,
                    x.F_Ruibetsu,
                    x.F_Store_cd,
                    x.F_Sebango,
                    x.F_Part_nm,
                }).ToList();

                return JsonConvert.SerializeObject(data.FirstOrDefault());
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> PartNoSelectedInq(string PartNo, string? SupplierCode, string? Kanban, string? StoreCode)
        {
            try
            {
                var con = _PPM3Context.T_Construction.AsNoTracking().ToList();

                var data = await _kbContext.TB_MS_Label.AsNoTracking()
                    .Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == PartNo
                        && x.F_Plant == _BearerClass.Plant)
                    .SelectMany(label => con
                        .Where(con => con.F_Part_no.Trim() == label.F_Part_No.Trim()
                            && con.F_Ruibetsu.Trim() == label.F_Ruibetsu.Trim()
                            && con.F_Store_cd.Trim() == label.F_Store_Code.Trim()
                            && !string.IsNullOrEmpty(label.F_Kanban_No) && label.F_Kanban_No.Length >= 4 &&
                            con.F_Sebango!.Trim() == label.F_Kanban_No.Trim().Substring(1, 3)
                        ),
                        (label, con) => new { label, con }
                    ).ToListAsync();

                if (!string.IsNullOrWhiteSpace(SupplierCode))
                {
                    data = data.Where(x => x.label.F_Supplier_Cd + "-" + x.label.F_Supplier_Plant == SupplierCode).ToList();
                }

                if (!string.IsNullOrWhiteSpace(Kanban))
                {
                    data = data.Where(x => x.label.F_Kanban_No == Kanban).ToList();
                }

                if (!string.IsNullOrWhiteSpace(StoreCode))
                {
                    data = data.Where(x => x.label.F_Store_Code == StoreCode).ToList();
                }

                var selData = data.Select(x => new
                {
                    F_Part_nm = x.con.F_Part_nm.Trim(),
                    F_Start_Date = x.label.F_Start_Date,
                    F_End_Date = x.label.F_End_Date,
                    F_Type_Order = x.label.F_Type_Order,
                    F_Color = x.label.F_Color,
                    F_Text = x.label.F_Text,
                }).FirstOrDefault();

                return JsonConvert.SerializeObject(selData);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Save(List<VM_KBNMS015> listObj, string action)
        {
            try
            {
                var obj = listObj.FirstOrDefault();

                var existObj = await _kbContext.TB_MS_Label
                    .Where(x => x.F_Supplier_Cd + "-" + x.F_Supplier_Plant == obj.F_Supplier_Code
                    && x.F_Kanban_No == obj.F_Kanban_No && x.F_Store_Code == obj.F_Store_Code
                    && x.F_Part_No + "-" + x.F_Ruibetsu == obj.F_Part_No).ToListAsync();

                if (action.ToLower() == "new")
                {
                    if (existObj.Count > 0)
                    {
                        throw new CustomHttpException(400, "Data have exist in System!!");
                    }

                    TB_MS_Label addObj = new TB_MS_Label
                    {
                        F_Color = obj.F_Color,
                        F_Create_By = _BearerClass.UserCode,
                        F_Create_Date = DateTime.Now,
                        F_Update_By = _BearerClass.UserCode,
                        F_Update_Date = DateTime.Now,
                        F_Cycle = obj.F_Cycle,
                        F_Kanban_No = obj.F_Kanban_No,
                        F_Plant = _BearerClass.Plant,
                        F_Supplier_Cd = obj.F_Supplier_Code.Split("-")[0],
                        F_Supplier_Plant = obj.F_Supplier_Code.Split("-")[1],
                        F_End_Date = obj.F_End_Date,
                        F_Start_Date = obj.F_Start_Date,
                        F_Part_No = obj.F_Part_No.Split("-")[0],
                        F_Ruibetsu = obj.F_Part_No.Split("-")[1],
                        F_Store_Code = obj.F_Store_Code,
                        F_Text = obj.F_Description,
                        F_Type_Order = "",
                    };

                    await _kbContext.TB_MS_Label.AddAsync(addObj);
                    _log.WriteLogMsg("INSERT TB_MS_Label => " + JsonConvert.SerializeObject(addObj));
                }
                else if (action.ToLower() == "upd")
                {
                    if (existObj.Count == 0)
                    {
                        throw new CustomHttpException(404, "Not Found Data to Update in System");
                    }

                    var updObj = existObj.FirstOrDefault();

                    updObj.F_Cycle = obj.F_Cycle;
                    updObj.F_Color = obj.F_Color;
                    updObj.F_End_Date = obj.F_End_Date;
                    updObj.F_Text = obj.F_Description;
                    updObj.F_Update_By = _BearerClass.UserCode;
                    updObj.F_Update_Date = DateTime.Now;

                    _kbContext.TB_MS_Label.Update(updObj);
                    _log.WriteLogMsg("UPDATE TB_MS_Label => " + JsonConvert.SerializeObject(updObj));

                }
                else if (action.ToLower() == "del")
                {
                    if (existObj.Count == 0)
                    {
                        throw new CustomHttpException(404, "Not Found Data to Update in System");
                    }

                    foreach (var each in listObj)
                    {
                        var delObj = await _kbContext.TB_MS_Label
                           .Where(x => x.F_Supplier_Cd + "-" + x.F_Supplier_Plant == each.F_Supplier_Code
                           && x.F_Kanban_No == obj.F_Kanban_No && x.F_Store_Code == each.F_Store_Code
                           && x.F_Part_No + "-" + x.F_Ruibetsu == each.F_Part_No
                           && x.F_Plant == _BearerClass.Plant
                           && x.F_Start_Date == obj.F_Start_Date
                           && x.F_End_Date == obj.F_End_Date
                           ).ToListAsync();

                        _kbContext.TB_MS_Label.RemoveRange(delObj);
                        _log.WriteLogMsg("DELETE TB_MS_Label => " + JsonConvert.SerializeObject(delObj));
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

    }
}
