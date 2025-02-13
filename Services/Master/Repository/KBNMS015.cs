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
                var con = await _PPM3Context.T_Construction.AsNoTracking()
                    .Where(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == PartNo)
                    .ToListAsync();

                var label = await _kbContext.TB_MS_Label.AsNoTracking()
                    .Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == PartNo)
                    .ToListAsync();

                var data = con.SelectMany(con => label
                    .Where(label => label.F_Part_No.Trim() == con.F_Part_no.Trim()
                    && label.F_Ruibetsu.Trim() == con.F_Ruibetsu.Trim()
                    && label.F_Store_Code.Trim() == con.F_Store_cd.Trim()
                    && !string.IsNullOrEmpty(label.F_Kanban_No) && label.F_Kanban_No.Length >= 4 &&
                    con.F_Sebango!.Trim() == label.F_Kanban_No.Trim().Substring(1, 3)),
                    (con, label) => new { con, label }).ToList();

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
                    && x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == obj.F_Part_No).ToListAsync();

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
                    _log.WriteLogMsg("UPDATE TB_MS_Label BEFORE => " + JsonConvert.SerializeObject(existObj));

                    updObj.F_Cycle = obj.F_Cycle;
                    updObj.F_Color = obj.F_Color;
                    updObj.F_End_Date = obj.F_End_Date;
                    updObj.F_Text = obj.F_Description;
                    updObj.F_Update_By = _BearerClass.UserCode;
                    updObj.F_Update_Date = DateTime.Now;

                    _kbContext.TB_MS_Label.Update(updObj);
                    _log.WriteLogMsg("UPDATE TB_MS_Label AFTER => " + JsonConvert.SerializeObject(updObj));

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
                           && x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == each.F_Part_No
                           && x.F_Plant == _BearerClass.Plant
                           && x.F_Start_Date == obj.F_Start_Date
                           && x.F_End_Date == obj.F_End_Date
                           ).ToListAsync();

                        _kbContext.TB_MS_Label.RemoveRange(delObj);
                        _log.WriteLogMsg("DELETE TB_MS_Label => " + JsonConvert.SerializeObject(delObj));
                    }
                }
                else if (action.ToLower() == "imp")
                {

                    var labels = _kbContext.TB_MS_Label
                        .AsNoTracking()
                        .ToList(); // Materialize data from the database

                    var isAnyExist = labels.Any(x =>
                        listObj.Any(y =>
                            (x.F_Supplier_Cd + "-" + x.F_Supplier_Plant) == y.F_Supplier_Code &&
                            (x.F_Part_No + "-" + x.F_Ruibetsu) == y.F_Part_No &&
                             x.F_Kanban_No == y.F_Kanban_No &&
                             x.F_Store_Code == y.F_Store_Code &&
                             x.F_Start_Date == y.F_Start_Date));


                    if (isAnyExist)
                    {
                        throw new CustomHttpException(400, "Can't insert exist data to Database");
                    }

                    foreach (var each in listObj)
                    {
                        var objSupplier = _PPM3Context.T_Supplier_MS
                            .AsNoTracking()
                            .FirstOrDefault(x => x.F_supplier_cd + "-" + x.F_Plant_cd == each.F_Supplier_Code
                            && x.F_TC_Str.CompareTo(strDateNow) <= 0
                            && x.F_TC_End.CompareTo(strDateNow) >= 0
                            && x.F_Store_cd == each.F_Store_Code);

                        if (objSupplier == null)
                        {
                            throw new CustomHttpException(404, "Not found Supplier In Supplier Master " + JsonConvert.SerializeObject(each));
                        }

                        var cycle = objSupplier.F_Cycle_A.Length == 1 ? "0" + objSupplier.F_Cycle_A + objSupplier.F_Cycle_B + objSupplier.F_Cycle_C
                            : objSupplier.F_Cycle_A + objSupplier.F_Cycle_B + objSupplier.F_Cycle_C;

                        var objPart = _PPM3Context.T_Construction
                            .AsNoTracking()
                            .FirstOrDefault(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == each.F_Part_No
                            && x.F_Store_cd == each.F_Store_Code
                            && x.F_Local_Str.CompareTo(strDateNow) <= 0
                            && x.F_Local_End.CompareTo(strDateNow) >= 0
                            && x.F_supplier_cd + "-" + x.F_plant == each.F_Supplier_Code
                            && x.F_Sebango == each.F_Kanban_No.Substring(1, 3));

                        if (objPart == null)
                        {
                            throw new CustomHttpException(404, "Not found Part In BOM " + _log.SerializeErrObjHTML(each));
                        }


                        TB_MS_Label addObj = new TB_MS_Label
                        {
                            F_Color = each.F_Color,
                            F_Create_By = _BearerClass.UserCode,
                            F_Create_Date = DateTime.Now,
                            F_Update_By = _BearerClass.UserCode,
                            F_Update_Date = DateTime.Now,
                            F_Cycle = cycle,
                            F_Kanban_No = each.F_Kanban_No,
                            F_Plant = _BearerClass.Plant,
                            F_Supplier_Cd = each.F_Supplier_Code.Split("-")[0],
                            F_Supplier_Plant = each.F_Supplier_Code.Split("-")[1],
                            F_End_Date = each.F_End_Date,
                            F_Start_Date = each.F_Start_Date,
                            F_Part_No = each.F_Part_No.Split("-")[0],
                            F_Ruibetsu = each.F_Part_No.Split("-")[1],
                            F_Store_Code = each.F_Store_Code,
                            F_Text = each.F_Description,
                            F_Type_Order = "",
                        };


                        await _kbContext.TB_MS_Label.AddAsync(addObj);
                    }
                    _log.WriteLogMsg("IMPORT INSERT TO TB_MS_Label => " + JsonConvert.SerializeObject(listObj));
                }
                else
                {
                    throw new CustomHttpException(400, "Please Select Action to proceed");
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

        public async Task<string> GetListData(string? PartNo, string? SupplierCode, string? Kanban, string? StoreCode)
        {
            try
            {
                return await Task.Run(() =>
                {

                    string ppmConnect = _FillDT.ppmConnect();

                    string query = $@"SELECT RTRIM(K.F_Supplier_Cd)+'-'+RTRIM(K.F_Supplier_Plant) AS F_Supplier_Code 
                    ,RTRIM(S.F_name) AS F_name 
                    ,RIGHT('00000'+ CONVERT(VARCHAR,S.F_Cycle_A),2) 
                    +'-'+RIGHT('00000'+ CONVERT(VARCHAR,S.F_cycle_B),2) 
                    +'-'+RIGHT('00000'+ CONVERT(VARCHAR,S.F_cycle_C),2) AS F_Cycle 
                    ,RTRIM(K.F_Part_No)+'-'+RTRIM(K.F_Ruibetsu) AS F_Part_No 
                    ,RTRIM(C.F_Part_nm) AS F_Part_nm 
                    ,RTRIM(K.F_Kanban_No) AS F_Kanban_No 
                    ,RTRIM(K.F_Store_Code) AS F_Store_Code 
                    ,SUBSTRING(K.F_Start_Date,7,2)+'/'+SUBSTRING(K.F_Start_Date,5,2)+'/'+SUBSTRING(K.F_Start_Date,1,4) AS F_Start_Date 
                    ,SUBSTRING(K.F_End_Date,7,2)+'/'+SUBSTRING(K.F_End_Date,5,2)+'/'+SUBSTRING(K.F_End_Date,1,4) AS F_End_Date 
                    ,RTRIM(K.F_Type_Order) AS F_Type_Order 
                    ,ISNULL(RTRIM(K.F_Color),0) AS F_Color 
                    ,RTRIM(K.F_Text) AS F_Text 
                    FROM  TB_MS_Label K INNER JOIN {ppmConnect}.[dbo].[T_Construction] C 
                    ON K.F_Part_No = C.F_Part_no collate Thai_CI_AS 
                    AND K.F_Ruibetsu = C. F_Ruibetsu collate Thai_CI_AS 
                    AND K.F_Store_Code = C.F_Store_cd collate Thai_CI_AS 
                    AND SUBSTRING(K.F_Kanban_No,2,3) = C.F_Sebango collate Thai_CI_AS 
                    INNER JOIN {ppmConnect}.[dbo].[T_Supplier_ms] S 
                    ON K.F_Supplier_Cd = S.F_supplier_cd COLLATE SQL_Latin1_General_CP1_CI_AI 
                    AND K.F_Supplier_Plant = S.F_Plant_cd COLLATE SQL_Latin1_General_CP1_CI_AI 
                    AND K.F_Store_Code = S.F_Store_cd collate Thai_CI_AS 
                    WHERE K.F_Plant = '{_BearerClass.Plant}' 
                    AND S.F_TC_Str <= convert(char(8),getdate(),112) 
                    AND S.F_TC_End >= convert(char(8),getdate(),112) 
                    AND S.F_store_cd LIKE '{_BearerClass.Plant}%' ";

                    if (!string.IsNullOrWhiteSpace(PartNo))
                    {
                        query += $" AND K.F_Part_No + '-' + K.F_Ruibetsu = '{PartNo}' ";
                    }
                    if (!string.IsNullOrWhiteSpace(SupplierCode))
                    {
                        query += $" AND K.F_Supplier_Cd + '-' + K.F_Supplier_Plant = '{SupplierCode}' ";
                    }
                    if (!string.IsNullOrWhiteSpace(Kanban))
                    {
                        query += $" AND K.F_Kanban_No = '{Kanban}' ";
                    }
                    if (!string.IsNullOrWhiteSpace(StoreCode))
                    {
                        query += $" AND K.F_Store_Code = '{StoreCode}' ";
                    }

                    query += @"GROUP BY RTRIM(K.F_Supplier_Cd)+'-'+RTRIM(K.F_Supplier_Plant)  ,RTRIM(K.F_Store_Code) 
                    ,RTRIM(K.F_Kanban_No),RTRIM(K.F_Part_No)+'-'+RTRIM(K.F_Ruibetsu) 
                    ,RTRIM(S.F_name),RIGHT('00000'+ CONVERT(VARCHAR,S.F_Cycle_A),2) +'-'+RIGHT('00000'+ CONVERT(VARCHAR,S.F_cycle_B),2) +'-'+RIGHT('00000'+ CONVERT(VARCHAR,S.F_cycle_C),2) 
                    ,RTRIM(C.F_Part_nm) ,SUBSTRING(K.F_Start_Date,7,2)+'/'+SUBSTRING(K.F_Start_Date,5,2)+'/'+SUBSTRING(K.F_Start_Date,1,4) 
                    ,SUBSTRING(K.F_End_Date,7,2)+'/'+SUBSTRING(K.F_End_Date,5,2)+'/'+SUBSTRING(K.F_End_Date,1,4) 
                    ,RTRIM(K.F_Type_Order) ,RTRIM(K.F_Color) ,RTRIM(K.F_Text) 
                    ORDER BY F_Supplier_Code,F_Store_Code,F_Kanban_No,F_Part_No,F_Type_Order,F_name 
                    ,F_Cycle,F_Part_nm ,F_Start_Date,F_End_Date ";

                    var data = _FillDT.ExecuteSQL(query);


                    return JsonConvert.SerializeObject(data);

                });


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
