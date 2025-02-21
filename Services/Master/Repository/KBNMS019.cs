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
    public class KBNMS019 : IKBNMS019
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _autoMap;


        public KBNMS019
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

        public async Task<List<T_Supplier_MS>> GetSupplierNew()
        {
            try
            {
                var data = await _PPM3Context.T_Supplier_MS
                    .AsNoTracking()
                    .ToListAsync();

                return data.DistinctBy(x => new
                {
                    F_Supplier_Code = x.F_supplier_cd + "-" + x.F_Plant_cd
                }).OrderBy(x => x.F_supplier_cd)
                    .ThenBy(x => x.F_Plant_cd)
                    .ToList();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<List<TB_MS_MAXAREA>> GetSupplierInq()
        {
            try
            {
                var data = await _kbContext.TB_MS_MAXAREA
                    .AsNoTracking()
                    .Where(x => x.F_Plant == _BearerClass.Plant)
                    .ToListAsync();

                return data.DistinctBy(x => new
                {
                    F_Supplier_Code = x.F_Supplier_Code + "-" + x.F_Supplier_Plant
                }).OrderBy(x => x.F_Supplier_Code)
                    .ThenBy(x => x.F_Supplier_Plant)
                    .ToList();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<TB_MS_MAXAREA> GetSupplierDetail(string SupplierCode)
        {
            try
            {
                var data = await _kbContext.TB_MS_MAXAREA
                    .AsNoTracking().FirstOrDefaultAsync(
                    x => x.F_Supplier_Code + "-" + x.F_Supplier_Plant == SupplierCode
                    && x.F_Part_No == "" && x.F_Ruibetsu == ""
                    && x.F_Kanban_No == "" && x.F_Store_CD == "");

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<List<T_Construction>> GetPartNew()
        {
            try
            {
                var data = await _PPM3Context.T_Construction
                    .AsNoTracking()
                    .Where(x => x.F_Local_Str.CompareTo(strDateNow) <= 0
                    && x.F_Local_End.CompareTo(strDateNow) >= 0
                    && x.F_Store_cd.StartsWith(_BearerClass.Plant)).ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<List<TB_MS_MAXAREA>> GetPartInq(string SupplierCode)
        {
            try
            {
                var data = await _kbContext.TB_MS_MAXAREA
                    .AsNoTracking()
                    .Where(x => x.F_Plant == _BearerClass.Plant
                    && x.F_Supplier_Code + "-" + x.F_Supplier_Plant == SupplierCode)
                    .ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<T_Construction> GetPartName(string PartNo)
        {
            try
            {
                var data = await _PPM3Context.T_Construction
                    .AsNoTracking().FirstOrDefaultAsync(
                    x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == PartNo);

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<TB_MS_MAXAREA> GetMaxTrip(string SupplierCode, string PartNo, string StoreCode, string KanbanNo)
        {
            try
            {
                var data = await _kbContext.TB_MS_MAXAREA.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.F_Plant == _BearerClass.Plant
                    && x.F_Supplier_Code + "-" + x.F_Supplier_Plant == SupplierCode
                    && x.F_Part_No + "-" + x.F_Ruibetsu == PartNo
                    && x.F_Kanban_No == KanbanNo && x.F_Store_CD == StoreCode);

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<List<VM_KBNMS019>> GetListMaxArea(string? SupplierCode, string? PartNo, string? StoreCode, string? KanbanNo)
        {
            try
            {
                string ppmCon = _FillDT.ppmConnect();
                string sqlQuery = $@"SELECT RTRIM(M.F_Supplier_Code)+'-'+ RTRIM(M.F_Supplier_Plant) AS F_Supplier 
                    ,CASE WHEN LEN(RTRIM(M.F_Part_No)) > 0 THEN RTRIM(M.F_Part_No)+'-'+RTRIM(M.F_Ruibetsu) 
                    ELSE COALESCE(M.F_Part_No, '') END  AS F_PartNo 
                    ,RTRIM(M.F_Kanban_No) AS F_KanbanNo 
                    ,ISNULL(RTRIM(C.F_Part_nm),'') AS F_PartName 
                    ,RTRIM(M.F_Store_CD) AS F_StoreCD, RTRIM(M.F_Max_Trip) As F_Max_Trip 
                    FROM TB_MS_MAXAREA M LEFT OUTER JOIN {ppmCon}.[dbo].[T_Construction] C 
                    ON M.F_Supplier_Code = C.F_supplier_cd collate Thai_CI_AS 
                    AND M.F_Supplier_Plant = C.F_plant collate Thai_CI_AS 
                    AND M.F_Part_No = C.F_Part_no collate Thai_CI_AS 
                    AND M.F_Ruibetsu = C.F_Ruibetsu collate Thai_CI_AS 
                    AND M.F_Store_CD = C.F_Store_cd collate Thai_CI_AS 
                    AND M.F_Kanban_No = C.F_Sebango collate Thai_CI_AS 
                    WHERE M.F_Plant = '{_BearerClass.Plant}' ";

                if (!string.IsNullOrWhiteSpace(SupplierCode))
                {
                    sqlQuery += $"AND M.F_Supplier_Code + '-' + M.F_Supplier_Plant = '{SupplierCode}'";
                }
                if (!string.IsNullOrWhiteSpace(PartNo))
                {
                    sqlQuery += $"AND M.F_Part_No + '-' + M.F_Ruibetsu = '{PartNo}' ";
                }
                if (!string.IsNullOrWhiteSpace(KanbanNo))
                {
                    sqlQuery += $"AND M.F_Kanban_No = '{KanbanNo}'";
                }
                if (!string.IsNullOrWhiteSpace(StoreCode))
                {
                    sqlQuery += $"AND M.F_Store_CD = '{StoreCode}'";
                }

                var dt = _FillDT.ExecuteSQL(sqlQuery);

                List<VM_KBNMS019> listObj = new List<VM_KBNMS019>();

                listObj = JsonConvert.DeserializeObject<List<VM_KBNMS019>>(JsonConvert.SerializeObject(dt));

                return listObj;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task Save(List<VM_KBNMS019> listObj, string action)
        {
            try
            {
                using var transaction = _kbContext.Database.BeginTransaction();
                var obj = listObj.FirstOrDefault();
                var existObj = await _kbContext.TB_MS_MAXAREA.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.F_Supplier_Code + "-" + x.F_Supplier_Plant == obj.F_Supplier
                    && x.F_Part_No + "-" + x.F_Ruibetsu == obj.F_PartNo && x.F_Kanban_No == obj.F_KanbanNo
                    && x.F_Store_CD == obj.F_StoreCD);

                string _logObj = "";

                if (existObj != null)
                {
                    int totalMax = existObj.F_Max_Trip;
                    if (obj.F_Max_Trip > totalMax)
                    {
                        throw new CustomHttpException(400, "Please Input Max Trip Less Than Total Max/Trip Integer Before Process Data");
                    }
                }

                var existT_Con = await _PPM3Context.T_Construction.AsNoTracking()
                    .Where(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == obj.F_PartNo
                    && x.F_supplier_cd + "-" + x.F_plant == obj.F_Supplier
                    && x.F_Store_cd == obj.F_StoreCD && x.F_Sebango == obj.F_KanbanNo)
                    .ToListAsync();

                if (existT_Con.Count == 0)
                {
                    throw new CustomHttpException(404, "Not found Data in Construction");
                }

                if (action.ToLower() == "new")
                {
                    if (existObj == null)
                    {
                        TB_MS_MAXAREA addObj = new TB_MS_MAXAREA
                        {
                            F_Create_By = _BearerClass.UserCode,
                            F_Create_Date = DateTime.Now,
                            F_Kanban_No = obj.F_KanbanNo,
                            F_Max_Trip = obj.F_Max_Trip,
                            F_Part_No = obj.F_PartNo.Split("-")[0],
                            F_Ruibetsu = obj.F_PartNo.Split("-")[1],
                            F_Plant = _BearerClass.Plant,
                            F_Store_CD = obj.F_StoreCD,
                            F_Supplier_Code = obj.F_Supplier.Split("-")[0],
                            F_Supplier_Plant = obj.F_Supplier.Split("-")[1],
                            F_Update_By = _BearerClass.UserCode,
                            F_Update_Date = DateTime.Now,
                        };

                        await _kbContext.TB_MS_MAXAREA.AddAsync(addObj);
                        _logObj = "INSERT TB_MS_MAXAREA " + JsonConvert.SerializeObject(addObj);
                    }
                    else
                    {
                        throw new CustomHttpException(400, "Data already exist in system");
                    }
                }
                else if (action.ToLower() == "upd")
                {
                    if (existObj != null)
                    {
                        _logObj = "Update TB_MS_MAXAREA Before => " + JsonConvert.SerializeObject(existObj);

                        _kbContext.TB_MS_MAXAREA.Remove(existObj);
                        await _kbContext.SaveChangesAsync();

                        existObj.F_Max_Trip = obj.F_Max_Trip;
                        existObj.F_Part_No = obj.F_PartNo.Split('-')[0];
                        existObj.F_Ruibetsu = obj.F_PartNo.Split('-')[1];
                        existObj.F_Store_CD = obj.F_StoreCD;
                        existObj.F_Kanban_No = obj.F_KanbanNo;
                        existObj.F_Update_By = _BearerClass.UserCode;
                        existObj.F_Update_Date = DateTime.Now;

                        await _kbContext.TB_MS_MAXAREA.AddAsync(existObj);

                        _logObj += " After => " + JsonConvert.SerializeObject(existObj);
                    }
                    else
                    {
                        throw new CustomHttpException(404, "Data not found in system");
                    }
                }
                else if (action.ToLower() == "del")
                {
                    if (existObj != null)
                    {
                        _kbContext.TB_MS_MAXAREA.Remove(existObj);

                        _logObj = "Delete TB_MS_MAXAREA => " + JsonConvert.SerializeObject(existObj);
                    }
                    else
                    {
                        throw new CustomHttpException(404, "Data not found in system");
                    }
                }
                else
                {
                    throw new CustomHttpException(400, "Please Select action before Proceed Save");
                }

                await _kbContext.SaveChangesAsync();
                transaction.Commit();
                _log.WriteLogMsg(_logObj);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

    }
}
