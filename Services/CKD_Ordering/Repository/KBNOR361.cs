using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.CKD_Ordering;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.PPM3;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using TB_MS_PartOrder = HINOSystem.Models.KB3.Master.TB_MS_PartOrder;

namespace KANBAN.Services.CKD_Ordering.Repository
{
    public class KBNOR361 : IKBNOR361
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly CKDWH_Context _CKDContext;
        private readonly CKDUSA_Context _CKDUSAContext;


        public KBNOR361
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMapService,
            CKDWH_Context CKDContext,
            CKDUSA_Context CKDUSAContext
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _automapService = autoMapService;
            _CKDContext = CKDContext;
            _CKDUSAContext = CKDUSAContext;
        }

        public async Task<List<List<string>>> GetDataList(string? Supplier_Code, string? Kanban_No, string? Store_Code, string? Part_No,bool IsNew)
        {
            try
            {
                List<List<string>> result = new List<List<string>>();
                if (IsNew)
                {
                    var data = await GetDataListNew(Supplier_Code, Kanban_No, Store_Code, Part_No);

                    if (data.Count > 0)
                    {
                        var supplier = data.Select(x => x.F_supplier_cd.Trim() + "-" + x.F_plant)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList();
                        var kanban = data.Select(x => "0" + x.F_Sebango)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList();
                        var store = data.Select(x => x.F_Store_cd)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList();
                        var part = data.Select(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList();

                        result.Add(supplier);
                        result.Add(kanban);
                        result.Add(store);
                        result.Add(part);
                    }
                }
                else
                {
                    var data = await GetDataListInq(Supplier_Code, Kanban_No, Store_Code, Part_No);

                    if (data.Count > 0)
                    {
                        var supplier = data.Select(x => x.F_Supplier_Cd.Trim() + "-" + x.F_Supplier_Plant)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList();
                        var kanban = data.Select(x => x.F_Kanban_No)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList();
                        var store = data.Select(x => x.F_Store_Code)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList();
                        var part = data.Select(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList();

                        result.Add(supplier);
                        result.Add(kanban);
                        result.Add(store);
                        result.Add(part);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task<List<TB_MS_PartOrder>> GetDataListInq(string? Supplier_Code,string? Kanban_No,string? Store_Code,string? Part_No)
        {
            try
            {
                var now = DateTime.Now.ToString("yyyyMMdd");
                var data = await _kbContext.TB_MS_PartOrder.AsNoTracking()
                    .Where(x => x.F_Start_Date.CompareTo(now) <= 0
                    && x.F_End_Date.CompareTo(now) >= 0
                    && (x.F_Supplier_Cd == "9999" || x.F_Supplier_Cd == "9995")
                    && x.F_Plant == _BearerClass.Plant).ToListAsync();

                if(!string.IsNullOrEmpty(Supplier_Code))
                {
                    data = data.Where(x => x.F_Supplier_Cd + "-" +x.F_Supplier_Plant == Supplier_Code).ToList();
                }
                if (!string.IsNullOrEmpty(Kanban_No))
                {
                    data = data.Where(x => x.F_Kanban_No == Kanban_No).ToList();
                }
                if (!string.IsNullOrEmpty(Store_Code))
                {
                    data = data.Where(x => x.F_Store_Code == Store_Code).ToList();
                }
                if (!string.IsNullOrEmpty(Part_No))
                {
                    data = data.Where(x => x.F_Part_No + "-" + x.F_Ruibetsu == Part_No).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task<List<T_Construction>> GetDataListNew(string? Supplier_Code, string? Kanban_No, string? Store_Code, string? Part_No)
        {
            try
            {
                var now = DateTime.Now.ToString("yyyyMMdd");
                var data = await _PPM3Context.T_Construction.AsNoTracking()
                    .Where(x => x.F_Local_Str.CompareTo(now) <= 0
                    && x.F_Local_End.CompareTo(now) >= 0
                    && (x.F_supplier_cd == "9999" || x.F_supplier_cd == "9995")
                    && x.F_Store_cd.StartsWith(_BearerClass.Plant)).ToListAsync();

                if (!string.IsNullOrEmpty(Supplier_Code))
                {
                    data = data.Where(x => x.F_supplier_cd + "-" + x.F_plant == Supplier_Code).ToList();
                }
                if (!string.IsNullOrEmpty(Kanban_No))
                {
                    data = data.Where(x => "0" + x.F_Sebango == Kanban_No).ToList();
                }
                if (!string.IsNullOrEmpty(Store_Code))
                {
                    data = data.Where(x => x.F_Store_cd == Store_Code).ToList();
                }
                if (!string.IsNullOrEmpty(Part_No))
                {
                    data = data.Where(x => x.F_Part_no + "-" + x.F_Ruibetsu == Part_No).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<T_Supplier_MS> GetSupplier(string Supplier_Code,string? Store_Code)
        {
            try
            {
                var now = DateTime.Now.ToString("yyyyMMdd");
                var data = await _PPM3Context.T_Supplier_MS.AsNoTracking()
                    .Where(x => x.F_supplier_cd + "-" + x.F_Plant_cd == Supplier_Code
                    && x.F_Store_cd.StartsWith(_BearerClass.Plant)
                    && x.F_TC_Str.CompareTo(now) <= 0
                    && x.F_TC_End.CompareTo(now) >= 0).ToListAsync();

                if(!string.IsNullOrEmpty(Store_Code))
                {
                    data = data.Where(x => x.F_Store_cd == Store_Code).ToList();
                }

                if (data.Count == 0)
                {
                    throw new CustomHttpException(400, "Supplier not found");
                }
                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<T_Construction> GetPartNo(string Part_No, string? Supplier_Code, string? Kanban_No, string? Store_Code)
        {
            try
            {
                var now = DateTime.Now.ToString("yyyyMMdd");
                var data = await _PPM3Context.T_Construction.AsNoTracking()
                    .Where(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == Part_No
                    && x.F_Store_cd.StartsWith(_BearerClass.Plant)
                    && x.F_Local_Str.CompareTo(now) <= 0
                    && x.F_Local_End.CompareTo(now) >= 0
                    && x.F_Cycle_A == '1')
                    .ToListAsync();

                if (!string.IsNullOrEmpty(Supplier_Code))
                {
                    data = data.Where(x => x.F_supplier_cd + "-" + x.F_plant == Supplier_Code).ToList();
                }
                if (!string.IsNullOrEmpty(Kanban_No))
                {
                    data = data.Where(x => "0" + x.F_Sebango == Kanban_No).ToList();
                }
                if (!string.IsNullOrEmpty(Store_Code))
                {
                    data = data.Where(x => x.F_Store_cd == Store_Code).ToList();
                }

                if (data.Count == 0)
                {
                    throw new CustomHttpException(400, "Part No not found");
                }

                return data.FirstOrDefault();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<string> GetList(string? Supplier_Code, string? Kanban_No, string? Store_Code, string? Part_No)
        {
            try
            {
                string ppmCon = _FillDT.ppmConnect();
                string _sql = $@"SELECT  K.F_Flg_ClearModule, RTRIM(K.F_Plant) AS F_Plant, RTRIM(K.F_Supplier_Cd)+'-'+RTRIM(K.F_Supplier_Plant) AS F_Supplier_Code 
                    ,RTRIM(S.F_name) AS F_name 
                    ,RIGHT('00'+ CONVERT(VARCHAR,S.F_Cycle_A),2) 
                    +'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_B),2) 
                    +'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_C),2) AS F_Cycle 
                    ,RTRIM(K.F_Part_No)+'-'+RTRIM(K.F_Ruibetsu) AS F_Part_No 
                    ,RTRIM(K.F_Kanban_No) AS F_Kanban_No 
                    ,RTRIM(C.F_Part_nm) AS F_Part_nm 
                    ,RTRIM(K.F_Store_Code) AS F_Store_Code 
                    ,SUBSTRING(K.F_Start_Date,7,2)+'/'+SUBSTRING(K.F_Start_Date,5,2)+'/'+SUBSTRING(K.F_Start_Date,1,4) AS F_Start_Date 
                    ,SUBSTRING(K.F_End_Date,7,2)+'/'+SUBSTRING(K.F_End_Date,5,2)+'/'+SUBSTRING(K.F_End_Date,1,4) AS F_End_Date 
                    ,RTRIM(K.F_Type_Order) AS F_Type_Order 
                    FROM TB_MS_PartOrder K INNER JOIN {ppmCon}.[dbo].[T_Construction] C 
                    ON K.F_Part_No = C.F_Part_no collate Thai_CI_AS 
                    AND K.F_Ruibetsu = C. F_Ruibetsu collate Thai_CI_AS 
                    AND K.F_Store_Code = C.F_Store_cd collate Thai_CI_AS
                    INNER JOIN {ppmCon}.[dbo].[T_Supplier_ms] S 
                    ON K.F_Supplier_Cd = S.F_supplier_cd COLLATE SQL_Latin1_General_CP1_CI_AI 
                    AND K.F_Supplier_Plant = S.F_Plant_cd COLLATE SQL_Latin1_General_CP1_CI_AI 
                    AND K.F_Store_Code = S.F_Store_cd collate Thai_CI_AS 
                    WHERE S.F_TC_Str <= convert(char(8),getdate(),112) 
                    AND S.F_TC_End >= convert(char(8),getdate(),112)  
                    AND C.F_Local_Str <= convert(char(8),getdate(),112) 
                    AND C.F_Local_End >= convert(char(8),getdate(),112) 
                    ";

                if (!string.IsNullOrEmpty(Supplier_Code))
                {
                    _sql += $" AND RTRIM(K.F_Supplier_Cd)+'-'+RTRIM(K.F_Supplier_Plant) = '{Supplier_Code}'";
                }
                if (!string.IsNullOrEmpty(Kanban_No))
                {
                    _sql += $" AND RTRIM(K.F_Kanban_No) = '{Kanban_No}'";
                }
                if (!string.IsNullOrEmpty(Store_Code))
                {
                    _sql += $" AND RTRIM(K.F_Store_Code) = '{Store_Code}'";
                }
                if (!string.IsNullOrEmpty(Part_No))
                {
                    _sql += $" AND RTRIM(K.F_Part_No)+'-'+RTRIM(K.F_Ruibetsu) = '{Part_No}'";
                }

                _sql += $@"
                        GROUP BY K.F_Flg_ClearModule, RTRIM(K.F_Plant),RTRIM(K.F_Supplier_Cd)+'-'+RTRIM(K.F_Supplier_Plant) ,RTRIM(S.F_name) 
                        ,RIGHT('00'+ CONVERT(VARCHAR,S.F_Cycle_A),2) +'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_B),2) +'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_C),2) 
                        ,RTRIM(K.F_Part_No)+'-'+RTRIM(K.F_Ruibetsu) 
                        ,RTRIM(K.F_Kanban_No),RTRIM(C.F_Part_nm),RTRIM(K.F_Store_Code)
                        ,SUBSTRING(K.F_Start_Date,7,2)+'/'+SUBSTRING(K.F_Start_Date,5,2)+'/'+SUBSTRING(K.F_Start_Date,1,4)
                        ,SUBSTRING(K.F_End_Date,7,2)+'/'+SUBSTRING(K.F_End_Date,5,2)+'/'+SUBSTRING(K.F_End_Date,1,4)
                        ,RTRIM(K.F_Type_Order)
                        ORDER BY F_Supplier_Code, F_name, F_Cycle, F_Part_No, F_Kanban_No
                        ,F_Part_nm, F_Store_Code, F_Start_Date, F_End_Date, F_Type_Order 
                        ";

                var data = await _FillDT.ExecuteSQLAsync(_sql);

                if (data.Rows.Count == 0)
                {
                    throw new CustomHttpException(400, "Data not found");
                }

                return JsonConvert.SerializeObject(data, Formatting.Indented);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task UpdateFlgClearModule(List<VM_KBNOR361_Save> listObj)        
        {
            try
            {
                foreach(var obj in listObj)
                {
                    var updObj = await _kbContext.TB_MS_PartOrder.FirstOrDefaultAsync(x => x.F_Plant == obj.F_Plant
                        && x.F_Supplier_Cd + "-" + x.F_Supplier_Plant == obj.F_Supplier_Code
                        && x.F_Kanban_No == obj.F_Kanban_No
                        && x.F_Part_No + "-" + x.F_Ruibetsu == obj.F_Part_No
                        && x.F_Store_Code == obj.F_Store_Code);

                    if (updObj != null)
                    {
                        updObj.F_Flg_ClearModule = obj.F_Flg_ClearModule;
                        _kbContext.Entry(updObj).State = EntityState.Modified;
                    }

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
