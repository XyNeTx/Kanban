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
    public class KBNMS020 : IKBNMS020
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _autoMap;


        public KBNMS020
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

        public async Task<List<T_Construction>> GetDropDownList(string? F_Supplier, string? F_KanbanNo, string? F_StoreCD, string? F_PartNo)
        {
            try
            {
                var data = await _PPM3Context.T_Construction.AsNoTracking()
                    .Where(x => x.F_Local_Str.CompareTo(strDateNow) <= 0
                    && x.F_Local_End.CompareTo(strDateNow) >= 0
                    && x.F_Store_cd.StartsWith(_BearerClass.Plant))
                    .ToListAsync();

                if (!string.IsNullOrWhiteSpace(F_Supplier))
                {
                    data = data.Where(x => x.F_supplier_cd + "-" + x.F_plant == F_Supplier).ToList();
                }
                if (!string.IsNullOrWhiteSpace(F_PartNo))
                {
                    data = data.Where(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == F_PartNo).ToList();
                }
                if (!string.IsNullOrWhiteSpace(F_KanbanNo))
                {
                    data = data.Where(x => x.F_Sebango == F_KanbanNo.Substring(1, 3)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(F_StoreCD))
                {
                    data = data.Where(x => x.F_Store_cd == F_StoreCD).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<T_Supplier_MS> GetSupplierName(string F_Supplier, string? F_StoreCD)
        {
            try
            {
                var data = await _PPM3Context.T_Supplier_MS.AsNoTracking()
                    .Where(x => x.F_supplier_cd + "-" + x.F_Plant_cd == F_Supplier
                    && x.F_TC_Str.CompareTo(strDateNow) <= 0
                    && x.F_TC_End.CompareTo(strDateNow) >= 0
                    && x.F_Store_cd.StartsWith(_BearerClass.Plant))
                    .ToListAsync();

                if (!string.IsNullOrWhiteSpace(F_StoreCD))
                {
                    data = data.Where(x => x.F_Store_cd == F_StoreCD).ToList();
                }

                return data.FirstOrDefault();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task Save(List<VM_KBNMS020> listObj, string action)
        {
            try
            {
                var obj = listObj.FirstOrDefault();
                var existObj = await _kbContext.TB_MS_MaxArea_Stock.AsNoTracking()
                    .Where(x => x.F_Plant == _BearerClass.Plant
                    && x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == obj.F_PartNo
                    && x.F_Supplier_Cd + "-" + x.F_Supplier_Plant == obj.F_Supplier
                    && x.F_Kanban_No == obj.F_KanbanNo && x.F_Store_Cd == obj.F_StoreCD)
                    .FirstOrDefaultAsync();

                string logMessage = "";

                decimal STD_Stock = 0.0m;

                if (action.ToLower() == "new")
                {
                    if (existObj == null)
                    {
                        string sql = $"exec sp_getSTDStock '{_BearerClass.Plant}'" +
                            $",'{obj.F_Supplier.Split("-")[0]}'" +
                            $",'{obj.F_Supplier.Split("-")[1]}'" +
                            $",'{obj.F_PartNo.Split("-")[0]}'" +
                            $",'{obj.F_PartNo.Split("-")[1]}'" +
                            $",'{obj.F_KanbanNo.Substring(1, 3)}'" +
                            $",'{obj.F_StoreCD}'" +
                            $",'{strDateNow}'";

                        var dt = _FillDT.ExecuteSQL(sql);

                        if (dt.Rows.Count > 0)
                        {
                            STD_Stock = decimal.TryParse(dt.Rows[0]["STDStock"].ToString(), out STD_Stock) ? STD_Stock : 0;
                        }
                        else
                        {
                            STD_Stock = 0.0m;
                        }

                        TB_MS_MaxArea_Stock addObj = new TB_MS_MaxArea_Stock
                        {
                            F_Create_By = _BearerClass.UserCode,
                            F_Create_Date = DateTime.Now,
                            F_Update_By = _BearerClass.UserCode,
                            F_Update_Date = DateTime.Now,
                            F_Kanban_No = obj.F_KanbanNo,
                            F_Part_No = obj.F_PartNo.Split("-")[0],
                            F_Ruibetsu = obj.F_PartNo.Split("-")[1],
                            F_Supplier_Cd = obj.F_Supplier.Split("-")[0],
                            F_Supplier_Plant = obj.F_Supplier.Split('-')[1],
                            F_Plant = _BearerClass.Plant,
                            F_Store_Cd = obj.F_StoreCD,
                            F_Max_Area = obj.F_Max_Area,
                            F_STD_Stock = STD_Stock,
                            F_Box_Qty = obj.F_BoxQty
                        };

                        await _kbContext.TB_MS_MaxArea_Stock.AddAsync(addObj);

                        logMessage = "INSERT INTO TB_MS_MaxArea_Stock => " + JsonConvert.SerializeObject(addObj);
                    }
                    else
                    {
                        throw new CustomHttpException(400, "Already have data in System");
                    }
                }
                else if (action.ToLower() == "upd")
                {
                    if (existObj != null)
                    {
                        logMessage = "UPDATE TO TB_MS_MaxArea_Stock BEFORE => " + JsonConvert.SerializeObject(existObj);

                        string sql = $"exec sp_getSTDStock '{_BearerClass.Plant}'" +
                            $",'{obj.F_Supplier.Split("-")[0]}'" +
                            $",'{obj.F_Supplier.Split("-")[1]}'" +
                            $",'{obj.F_PartNo.Split("-")[0]}'" +
                            $",'{obj.F_PartNo.Split("-")[1]}'" +
                            $",'{obj.F_KanbanNo.Substring(1, 3)}'" +
                            $",'{obj.F_StoreCD}'" +
                            $",'{strDateNow}'";

                        var dt = _FillDT.ExecuteSQL(sql);

                        if (dt.Rows.Count > 0)
                        {
                            STD_Stock = decimal.TryParse(dt.Rows[0]["STDStock"].ToString(), out STD_Stock) ? STD_Stock : 0;
                        }
                        else
                        {
                            STD_Stock = 0.0m;
                        }

                        _kbContext.Attach(existObj);

                        existObj.F_Max_Area = obj.F_Max_Area;
                        existObj.F_STD_Stock = STD_Stock;
                        existObj.F_Update_By = _BearerClass.UserCode;
                        existObj.F_Update_Date = DateTime.Now;

                        _kbContext.Update(existObj);

                        logMessage += " UPDATE TO TB_MS_MaxArea_Stock AFTER => " + JsonConvert.SerializeObject(existObj);


                    }
                }
                else if (action.ToLower() == "del")
                {
                    foreach (var each in listObj)
                    {
                        var delObj = await _kbContext.TB_MS_MaxArea_Stock.AsNoTracking()
                            .Where(x => x.F_Plant == _BearerClass.Plant
                            && x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == each.F_PartNo
                            && x.F_Supplier_Cd + "-" + x.F_Supplier_Plant == each.F_Supplier
                            && x.F_Kanban_No == each.F_KanbanNo && x.F_Store_Cd == each.F_StoreCD)
                            .FirstOrDefaultAsync();

                        if (delObj != null)
                        {
                            _kbContext.Remove(delObj);

                            logMessage = "DELETE TO TB_MS_MaxArea_Stock => " + JsonConvert.SerializeObject(delObj);
                        }
                        else
                        {
                            throw new CustomHttpException(400, "Data not found to Delete");
                        }

                    }
                }
                else
                {
                    throw new CustomHttpException(400, "Invalid Action. Please Select an action");
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

        public async Task<string> GetListData(string? F_Supplier, string? F_KanbanNo, string? F_StoreCD, string? F_PartNo)
        {
            try
            {
                string ppmConnect = _FillDT.ppmConnect();

                string sqlQuery = $@"SELECT M.F_Part_No+'-'+M.F_Ruibetsu AS F_PartNo, M.F_Store_Cd AS F_StoreCD , C.F_Part_nm 
                    ,M.F_Supplier_Cd+'-'+M.F_Supplier_Plant AS F_Supplier, TRIM(S.F_name)+' ('+TRIM(S.F_short_name)+')' AS F_name 
                    , M.F_Kanban_No AS F_KanbanNo, M.F_Box_Qty AS F_BoxQty, K.F_Address AS Group_Part, M.F_STD_Stock 
                    , K.F_Supply_Code AS Dock 
                    , P.F_Package_Type , M.F_Max_Area
                    FROM TB_MS_MaxArea_Stock M LEFT JOIN TB_MS_PartPackage P 
                    ON M.F_Plant = P.F_Plant AND M.F_Supplier_Cd = P.F_Supplier_Cd 
                    AND M.F_Supplier_Plant = P.F_Supplier_Plant AND M.F_Part_No = P.F_Part_No 
                    AND M.F_Ruibetsu = P.F_Ruibetsu AND M.F_Store_Cd = P.F_Store_Cd 
                    LEFT JOIN TB_MS_Kanban K 
                    ON  M.F_Plant = K.F_Plant AND M.F_Supplier_Cd = K.F_Supplier_Code 
                    AND M.F_Supplier_Plant = K.F_Supplier_Plant AND M.F_Store_Cd = K.F_Store_Code 
                    AND M.F_Kanban_No = K.F_Kanban_No AND M.F_Part_No = K.F_Part_No
                    AND M.F_Ruibetsu = K.F_Ruibetsu 
                    INNER JOIN {ppmConnect}.[dbo].[T_Construction] C 
                    ON M.F_Part_No = C.F_Part_no collate Thai_CI_AS 
                    AND M.F_Ruibetsu = C.F_Ruibetsu collate Thai_CI_AS 
                    AND M.F_Store_Cd = C.F_Store_cd collate Thai_CI_AS 
                    AND M.F_Supplier_Cd = C.F_supplier_cd collate Thai_CI_AS 
                    AND M.F_Supplier_Plant = C.F_plant collate Thai_CI_AS 
                    INNER JOIN {ppmConnect}.[dbo].[T_Supplier_ms] S 
                    ON M.F_Supplier_Cd = S.F_supplier_cd collate Thai_CI_AS 
                    AND M.F_Supplier_Plant = S.F_Plant_cd collate Thai_CI_AS 
                    WHERE S.F_name = (Select Top 1 F_name From {ppmConnect}.[dbo].[T_Supplier_ms] ST 
                    Where ST.F_supplier_cd = S.F_supplier_cd And ST.F_Plant_cd = S.F_Plant_cd) ";

                if (!string.IsNullOrWhiteSpace(F_Supplier))
                {
                    sqlQuery += $"AND M.F_Supplier_Cd +'-'+ M.F_Supplier_Plant = '{F_Supplier}'";
                }
                if (!string.IsNullOrWhiteSpace(F_PartNo))
                {
                    sqlQuery += $"AND M.F_Part_No +'-'+ M.F_Ruibetsu = '{F_PartNo}'";
                }
                if (!string.IsNullOrWhiteSpace(F_StoreCD))
                {
                    sqlQuery += $"AND M.F_Store_Cd = '{F_StoreCD}'";
                }
                if (!string.IsNullOrWhiteSpace(F_KanbanNo))
                {
                    sqlQuery += $"AND M.F_Kanban_No = '{F_KanbanNo}'";
                }

                sqlQuery += $@"GROUP BY M.F_Part_No,M.F_Ruibetsu,M.F_Store_Cd,C.F_Part_nm,M.F_Supplier_Cd,M.F_Supplier_Plant,S.F_name,S.F_short_name
                    ,M.F_Kanban_No,M.F_Box_Qty,K.F_Address,K.F_Supply_Code,P.F_Package_Type , M.F_Max_Area, M.F_STD_Stock";

                var dt = _FillDT.ExecuteSQL(sqlQuery);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        decimal STD_Stock = 0.0m;

                        string sql = $"exec sp_getSTDStock '{_BearerClass.Plant}'" +
                                    $",'{dt.Rows[i]["F_Supplier"].ToString().Split("-")[0]}'" +
                                    $",'{dt.Rows[i]["F_Supplier"].ToString().Split("-")[1]}'" +
                                    $",'{dt.Rows[i]["F_PartNo"].ToString().Split("-")[0]}'" +
                                    $",'{dt.Rows[i]["F_PartNo"].ToString().Split("-")[1]}'" +
                                    $",'{dt.Rows[i]["F_KanbanNo"].ToString().Substring(1, 3)}'" +
                                    $",'{dt.Rows[i]["F_StoreCD"].ToString()}'" +
                                    $",'{strDateNow}'";

                        var dtSTD = _FillDT.ExecuteSQL(sql);

                        if (dtSTD.Rows.Count > 0)
                        {
                            STD_Stock = decimal.TryParse(dtSTD.Rows[0]["STDStock"].ToString(), out STD_Stock) ? STD_Stock : 0;
                        }
                        else
                        {
                            STD_Stock = 0.0m;
                        }

                        string dtSuppCode = dt.Rows[i]["F_Supplier"].ToString().Split("-")[0];
                        string dtSuppPlant = dt.Rows[i]["F_Supplier"].ToString().Split("-")[1];
                        string dtPartNo = dt.Rows[i]["F_PartNo"].ToString().Split("-")[0];
                        string dtRuibetsu = dt.Rows[i]["F_PartNo"].ToString().Split("-")[1];

                        await _kbContext.TB_MS_MaxArea_Stock
                            .Where(x => x.F_Supplier_Cd == dtSuppCode
                            && x.F_Supplier_Plant == dtSuppPlant
                            && x.F_Part_No == dtPartNo
                            && x.F_Ruibetsu == dtRuibetsu
                            && x.F_Kanban_No == dt.Rows[i]["F_KanbanNo"].ToString()
                            && x.F_Store_Cd == dt.Rows[i]["F_StoreCD"].ToString())
                            .ExecuteUpdateAsync(x => x.SetProperty(y => y.F_STD_Stock, STD_Stock));
                    }
                }

                var updatedDT = _FillDT.ExecuteSQL(sqlQuery);


                return JsonConvert.SerializeObject(updatedDT);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

    }
}
