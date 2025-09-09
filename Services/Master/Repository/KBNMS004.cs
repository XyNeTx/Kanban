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
using System.Security.Claims;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS004 : IKBNMS004
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public KBNMS004
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

        public string strDate = DateTime.Now.ToString("yyyyMMdd");

        public async Task<List<string>> GetSelectList(string? kanban,string? storecd,string? partno,string? supplier,bool isNew)
        {
            try
            {
                string stringDate = DateTime.Now.ToString("yyyyMMdd");

                List<string> listResult = new List<string>();

                if (isNew)
                {
                    var data = await _PPM3Context.T_Construction.AsNoTracking()
                        .Where(x => x.F_Local_Str.CompareTo(stringDate) <= 0
                        && x.F_Local_End.CompareTo(stringDate) >= 0
                        && x.F_Store_cd.StartsWith(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value))
                        .ToListAsync();

                    if (!string.IsNullOrWhiteSpace(kanban))
                    {
                        data = data.Where(x => x.F_Sebango == kanban.Substring(1, 3)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(storecd))
                    {
                        data = data.Where(x => x.F_Store_cd == storecd).ToList();
                    }

                    if (!string.IsNullOrWhiteSpace(partno))
                    {
                        data = data.Where(x => x.F_Part_no + "-" +x.F_Ruibetsu == partno).ToList();
                    }

                    if (!string.IsNullOrWhiteSpace(supplier))
                    {
                        data = data.Where(x => x.F_supplier_cd + "-" + x.F_plant == supplier).ToList();
                    }

                    var supplierList = data.Select(x => new
                    {
                        F_Supplier = x.F_supplier_cd + "-" + x.F_plant,
                    }).Distinct().OrderBy(x=>x.F_Supplier).ToList();

                    var partList = data.Select(x => new
                    {
                        F_PartNo = x.F_Part_no.Trim() + "-" + x.F_Ruibetsu,
                    }).Distinct().OrderBy(x=>x.F_PartNo).ToList();

                    var storeList = data.Select(x => new
                    {
                        F_StoreCD = x.F_Store_cd,
                    }).Distinct().OrderBy(x=>x.F_StoreCD).ToList();

                    var kanbanList = data.Select(x => new
                    {
                        F_KanbanNo = "0" + x.F_Sebango,
                    }).Distinct().OrderBy(x=>x.F_KanbanNo).ToList();

                    listResult.Add(JsonConvert.SerializeObject(supplierList));
                    listResult.Add(JsonConvert.SerializeObject(partList));
                    listResult.Add(JsonConvert.SerializeObject(storeList));
                    listResult.Add(JsonConvert.SerializeObject(kanbanList));

                    //return JsonConvert.SerializeObject(data);
                }
                else
                {
                    var MsPartSpecial = await _kbContext.TB_MS_PartSpecial.AsNoTracking()
                        .Where(x=>x.F_Start_Date.CompareTo(stringDate) <= 0
                        && x.F_End_Date.CompareTo(stringDate) >= 0
                        && x.F_Plant == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value)
                        .ToListAsync();

                    if(!string.IsNullOrWhiteSpace(kanban))
                    {
                        MsPartSpecial = MsPartSpecial.Where(x => x.F_Kanban_No == kanban).ToList();
                    }

                    if (!string.IsNullOrWhiteSpace(storecd))
                    {
                        MsPartSpecial = MsPartSpecial.Where(x => x.F_Store_Code == storecd).ToList();
                    }

                    if (!string.IsNullOrWhiteSpace(partno))
                    {
                        MsPartSpecial = MsPartSpecial.Where(x => x.F_Part_No + "-" + x.F_Ruibetsu == partno).ToList();
                    }

                    if (!string.IsNullOrWhiteSpace(supplier))
                    {
                        MsPartSpecial = MsPartSpecial.Where(x => x.F_Supplier_Cd + "-" + x.F_Supplier_Plant == supplier).ToList();
                    }

                    var supplierList = MsPartSpecial.Select(x => new
                    {
                        F_Supplier = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant,
                    }).Distinct().OrderBy(x=>x.F_Supplier).ToList();

                    var partList = MsPartSpecial.Select(x => new
                    {
                        F_PartNo = x.F_Part_No.Trim() + "-" + x.F_Ruibetsu,
                    }).Distinct().OrderBy(x=>x.F_PartNo).ToList();

                    var storeList = MsPartSpecial.Select(x => new
                    {
                        F_StoreCD = x.F_Store_Code,
                    }).Distinct().OrderBy(x=>x.F_StoreCD).ToList();

                    var kanbanList = MsPartSpecial.Select(x => new
                    {
                        F_KanbanNo = x.F_Kanban_No,
                    }).Distinct().OrderBy(x=>x.F_KanbanNo).ToList();

                    listResult.Add(JsonConvert.SerializeObject(supplierList));
                    listResult.Add(JsonConvert.SerializeObject(partList));
                    listResult.Add(JsonConvert.SerializeObject(storeList));
                    listResult.Add(JsonConvert.SerializeObject(kanbanList));
                }

                return listResult;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public string GetListData(string? kanban, string? storecd, string? partno, string? supplier,string? type)
        {
            try
            {
                string ppmCon = _FillDT.ppmConnect();

                string sqlQuery = $@"SELECT RTRIM(K.F_Plant) AS F_Plant,RTRIM(K.F_Supplier_Cd)+'-'+RTRIM(K.F_Supplier_Plant) AS F_Supplier_Code 
                    ,RTRIM(S.F_name) AS F_name ,RIGHT('00'+ CONVERT(VARCHAR,S.F_Cycle_A),2) 
                    +'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_B),2) 
                    +'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_C),2) AS F_Cycle 
                    ,RTRIM(K.F_Part_No)+'-'+RTRIM(K.F_Ruibetsu) AS F_Part_No 
                    ,RTRIM(K.F_Kanban_No) AS F_Kanban_No 
                    ,RTRIM(C.F_Part_nm) AS F_Part_nm 
                    ,RTRIM(K.F_Store_Code) AS F_Store_Code 
                    ,SUBSTRING(K.F_Start_Date,7,2)+'/'+SUBSTRING(K.F_Start_Date,5,2)+'/'+SUBSTRING(K.F_Start_Date,1,4) AS F_Start_Date 
                    ,SUBSTRING(K.F_End_Date,7,2)+'/'+SUBSTRING(K.F_End_Date,5,2)+'/'+SUBSTRING(K.F_End_Date,1,4) AS F_End_Date 
                    ,RTRIM(K.F_Type_Special) AS F_Type_Special 
                    FROM TB_MS_PartSpecial K INNER JOIN {ppmCon}.[dbo].[T_Construction] C 
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
                    AND K.F_Start_Date <= convert(char(8),getdate(),112) 
                    AND K.F_End_Date >= convert(char(8),getdate(),112) ";

                if(!string.IsNullOrWhiteSpace(kanban))
                {
                    sqlQuery += $" AND K.F_Kanban_No = '{kanban}' ";
                }

                if(!string.IsNullOrWhiteSpace(supplier))
                {
                    sqlQuery += $" AND K.F_Supplier_Cd+'-'+K.F_Supplier_Plant = '{supplier}' ";
                }

                if (!string.IsNullOrWhiteSpace(partno))
                {
                    sqlQuery += $" AND K.F_Part_No+'-'+K.F_Ruibetsu = '{partno}' ";
                }

                if (!string.IsNullOrWhiteSpace(storecd))
                {
                    sqlQuery += $" AND K.F_Store_Code = '{storecd}' ";
                }

                if (!string.IsNullOrWhiteSpace(type))
                {
                    sqlQuery += $" AND K.F_Type_Special = '{type}' ";
                }

                sqlQuery += @"GROUP BY RTRIM(K.F_Plant),RTRIM(K.F_Supplier_Cd)+'-'+RTRIM(K.F_Supplier_Plant) ,RTRIM(S.F_name) 
                        ,RIGHT('00'+ CONVERT(VARCHAR,S.F_Cycle_A),2) +'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_B),2) +'-'+RIGHT('00'+ CONVERT(VARCHAR,S.F_cycle_C),2) 
                        ,RTRIM(K.F_Part_No)+'-'+RTRIM(K.F_Ruibetsu) 
                        ,RTRIM(K.F_Kanban_No) ,RTRIM(C.F_Part_nm) ,RTRIM(K.F_Kanban_No),RTRIM(K.F_Store_Code) 
                        ,SUBSTRING(K.F_Start_Date,7,2)+'/'+SUBSTRING(K.F_Start_Date,5,2)+'/'+SUBSTRING(K.F_Start_Date,1,4) 
                        ,SUBSTRING(K.F_End_Date,7,2)+'/'+SUBSTRING(K.F_End_Date,5,2)+'/'+SUBSTRING(K.F_End_Date,1,4) 
                        ,RTRIM(K.F_Type_Special) 
                        ORDER BY F_Supplier_Code, F_name, F_Cycle, F_Part_No, F_Kanban_No
                        ,F_Part_nm, F_Store_Code, F_Start_Date, F_End_Date, F_Type_Special ";

                var data = _FillDT.ExecuteSQL(sqlQuery);

                Console.WriteLine(sqlQuery);

                return JsonConvert.SerializeObject(data);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<T_Supplier_MS> SelectedSupplier(string supplier,string? storecd)
        {
            try
            {
                var data = await _PPM3Context.T_Supplier_MS.AsNoTracking()
                    .Where(x => x.F_supplier_cd + "-" + x.F_Plant_cd == supplier
                    && x.F_TC_Str.CompareTo(strDate) <= 0
                    && x.F_TC_End.CompareTo(strDate) >= 0
                    && x.F_Store_cd.StartsWith(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value))
                    .ToListAsync();

                if(!string.IsNullOrWhiteSpace(storecd))
                {
                    data = data.Where(x => x.F_Store_cd == storecd).ToList();
                }

                if(data.Count == 0)
                {
                    throw new CustomHttpException(404, "Data not found");
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

        public async Task<T_Construction> SelectedPartNo(string partno,string? supplier,string? kanban,string? storecd)
        {
            try
            {
                var data = await _PPM3Context.T_Construction.AsNoTracking()
                    .Where(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == partno
                    && x.F_Local_Str!.Trim().CompareTo(strDate) <= 0
                    && x.F_Local_End!.Trim().CompareTo(strDate) >= 0
                    && x.F_Store_cd.StartsWith(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value))
                    .ToListAsync();

                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    data = data.Where(x => x.F_supplier_cd + "-" + x.F_plant == supplier).ToList();
                }
                if(!string.IsNullOrWhiteSpace(kanban))
                {
                    data = data.Where(x => x.F_Sebango == kanban.Substring(1, 3)).ToList();
                }
                if(!string.IsNullOrWhiteSpace(storecd))
                {
                    data = data.Where(x => x.F_Store_cd == storecd).ToList();
                }

                if(data.Count == 0)
                {
                    throw new CustomHttpException(404, "Data not found");
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
        
        public async Task Save(List<VM_SAVE_KBNMS004> listObj)
        {
            try
            {
                var obj = listObj.FirstOrDefault();

                if (obj.Action.ToLower().Contains("new"))
                {
                    if (obj.StartDate.CompareTo(obj.EndDate) > 0)
                    {
                        throw new CustomHttpException(400, "Please Select Start Date Less than End Date Before Process Data");
                    }
                    if (strDate.CompareTo(obj.StartDate) < 0)
                    {
                        throw new CustomHttpException(400, "Please Select Start Date From Present OR More Before Process Data");
                    }

                    var existData = await _kbContext.TB_MS_PartSpecial.AsNoTracking()
                        .Where(x => x.F_Kanban_No == obj.Kanban
                        && x.F_Store_Code == obj.StoreCD
                        && x.F_Part_No + "-" + x.F_Ruibetsu == obj.PartNo
                        && x.F_Supplier_Cd + "-" + x.F_Supplier_Plant == obj.Supplier
                        && x.F_Start_Date.CompareTo(strDate) <= 0
                        && x.F_End_Date.CompareTo(strDate) >= 0)
                        .AnyAsync();

                    if (existData)
                    {
                        throw new CustomHttpException(400, "Data Already Exist");
                    }

                    await _kbContext.TB_MS_PartSpecial.AddAsync(new TB_MS_PartSpecial
                    {
                        F_Kanban_No = obj.Kanban,
                        F_Store_Code = obj.StoreCD,
                        F_Part_No = obj.PartNo.Split("-")[0],
                        F_Ruibetsu = obj.PartNo.Split("-")[1],
                        F_Supplier_Cd = obj.Supplier.Split("-")[0],
                        F_Supplier_Plant = obj.Supplier.Split("-")[1],
                        F_Type_Special = obj.TypeOrder,
                        F_Start_Date = obj.StartDate,
                        F_End_Date = obj.EndDate,
                        F_Plant = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value,
                        F_Create_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value,
                        F_Create_Date = DateTime.Now,
                        F_Update_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value,
                        F_Update_Date = DateTime.Now,
                        F_Cycle = obj.Cycle.Replace("-", string.Empty),
                    });

                    _log.WriteLogMsg("Insert TB_MS_PartSpecial Data : " + JsonConvert.SerializeObject(obj));

                }
                else if (obj.Action.ToLower().Contains("upd"))
                {
                    await _kbContext.TB_MS_PartSpecial.Where(x => x.F_Kanban_No == obj.Kanban
                        && x.F_Store_Code == obj.StoreCD
                        && x.F_Part_No + "-" + x.F_Ruibetsu == obj.PartNo
                        && x.F_Supplier_Cd + "-" + x.F_Supplier_Plant == obj.Supplier
                        && x.F_Start_Date == obj.StartDate
                        && x.F_Plant == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value)
                        .ExecuteUpdateAsync(x => x.SetProperty(x => x.F_Cycle, obj.Cycle.Replace("-", string.Empty))
                        .SetProperty(x => x.F_Type_Special, obj.TypeOrder)
                        .SetProperty(x => x.F_End_Date, obj.EndDate)
                        .SetProperty(x => x.F_Update_By, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value)
                        .SetProperty(x => x.F_Update_Date, DateTime.Now));

                    _log.WriteLogMsg("Update TB_MS_PartSpecial Data : " + JsonConvert.SerializeObject(obj));
                }
                else if (obj.Action.ToLower().Contains("del"))
                {
                    foreach (var each in listObj)
                    {
                        await _kbContext.TB_MS_PartSpecial.Where(x => x.F_Kanban_No == each.Kanban
                            && x.F_Store_Code == each.StoreCD
                            && x.F_Part_No + "-" + x.F_Ruibetsu == each.PartNo
                            && x.F_Supplier_Cd + "-" + x.F_Supplier_Plant == each.Supplier
                            && x.F_Start_Date == each.StartDate
                            && x.F_Plant == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value)
                            .ExecuteDeleteAsync();

                        _log.WriteLogMsg("Delete TB_MS_PartSpecial Data : " + JsonConvert.SerializeObject(each));
                    }
                }
                else
                {
                    throw new CustomHttpException(400, "Please Select Action Before Process Data");
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
