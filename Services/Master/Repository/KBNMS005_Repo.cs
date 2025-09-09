using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS005_Repo : IKBNMS005_Repo
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KBNMS005_Repo(KB3Context kbContext, BearerClass bearerClass, 
            PPM3Context ppm3Context, FillDataTable fillDT,
            SerilogLibs log, IEmailService emailService,
            IAutoMapService automapService, IHttpContextAccessor httpContextAccessor)
        {
            _kbContext = kbContext;
            _BearerClass = bearerClass;
            _PPM3Context = ppm3Context;
            _FillDT = fillDT;
            _log = log;
            _emailService = emailService;
            _automapService = automapService;
            _httpContextAccessor = httpContextAccessor;
        }

        public static string strDateNow = DateTime.Now.ToString("yyyyMMdd");
        public static DataTable DT_Daily = new DataTable();
        public static DataTable DT_Header = new DataTable();
        public static DataTable DT_Period = new DataTable();

        public async Task<List<List<string>>> GetListOption(string? Sup, string? Part, string? PartT, string? Store, string? StoreT)
        {
            try
            {
                List<string> storeList = new List<string>();
                List<string> kbList = new List<string>();
                List<string> partList = new List<string>();

                var data = await _kbContext.TB_MS_PartOrder.AsNoTracking()
                    .Where(x => x.F_Start_Date.CompareTo(strDateNow) <= 0
                    && x.F_End_Date.CompareTo(strDateNow) >= 0
                    && x.F_Store_Code.StartsWith(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value)).ToListAsync();

                var supList = data.Select(x => x.F_Supplier_Cd.Trim() + "-" + x.F_Supplier_Plant).Distinct().OrderBy(x => x).ToList();

                if (!string.IsNullOrWhiteSpace(Sup)) {
                    kbList = data.Where(x => x.F_Supplier_Cd.Trim() == Sup.Substring(0,4))
                        .Select(x => x.F_Kanban_No).Distinct().OrderBy(x => x).ToList();
                }
                else
                {
                    kbList = data.Select(x => x.F_Kanban_No).Distinct().OrderBy(x => x).ToList();
                }

                if (!string.IsNullOrWhiteSpace(Store))
                {
                    partList = data.Where(x => x.F_Store_Code.Trim().CompareTo(Store) >= 0 && x.F_Store_Code.Trim().CompareTo(StoreT) <= 0)
                        .Select(x => x.F_Part_No.Trim() + "-" +x.F_Ruibetsu).Distinct().OrderBy(x => x).ToList();
                }
                else
                {
                    partList = data.Select(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu).Distinct().OrderBy(x => x).ToList();
                }

                if (!string.IsNullOrWhiteSpace(Part))
                {
                    storeList = data.Where(x => (x.F_Part_No.Trim() + "-" + x.F_Ruibetsu).CompareTo(Part) >= 0 
                        && (x.F_Part_No.Trim() + "-" + x.F_Ruibetsu).CompareTo(PartT) <= 0)
                        .Select(x => x.F_Store_Code.Trim()).Distinct().OrderBy(x => x).ToList();
                }
                else
                {
                    storeList = data.Select(x => x.F_Store_Code.Trim()).Distinct().OrderBy(x => x).ToList();
                }

                return new List<List<string>>
                {
                    supList,
                    kbList,
                    partList,
                    storeList
                };
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<List<string>> GetAllData(string Supplier,string? Kanban,string? KanbanTo,string? Store,string? StoreTo,string? Part,string? PartTo,string? Date,string DateTo, bool isOk, bool isNo, bool isPartShort)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@StartDate", Date),
                    new SqlParameter("@EndDate", DateTo),
                    new SqlParameter("@Supplier_Code", Supplier.Split("-")[0]),
                    new SqlParameter("@Supplier_Plant", Supplier.Split("-")[1]),
                    new SqlParameter("@Kanban_No_FROM", Kanban ?? (object)DBNull.Value),
                    new SqlParameter("@Kanban_No_TO", KanbanTo ?? (object)DBNull.Value),
                    new SqlParameter("@Store_Code_FROM", Store ?? (object)DBNull.Value),
                    new SqlParameter("@Store_Code_TO", StoreTo ?? (object)DBNull.Value),
                    new SqlParameter("@Part_No_FROM", Part == null ? (object)DBNull.Value : Part.Split("-")[0]),
                    new SqlParameter("@Part_No_TO", PartTo == null ? (object)DBNull.Value : Part.Split("-")[0]),
                    new SqlParameter("@Ruibetsu_FROM", Part == null ?(object) DBNull.Value : Part.Split("-")[1]),
                    new SqlParameter("@Ruibetsu_TO", PartTo == null ?(object) DBNull.Value : Part.Split("-")[1])
                };

                DT_Daily = await _FillDT.ExecuteStoreSQLAsync("sp_DT_DailyBalance",sqlParameters.ToArray());
                
                if(DT_Daily.Rows.Count == 0)
                {
                    throw new CustomHttpException(404, "No data found");
                }
                
                DT_Header = await _FillDT.ExecuteStoreSQLAsync("sp_DT_Header", sqlParameters.ToArray());

                if (DT_Header.Rows.Count == 0)
                {
                    throw new CustomHttpException(404, "No header data found");
                }

                await _kbContext.Database.ExecuteSqlRawAsync("DELETE FROM TB_BL_Data Where F_UpdateBy = @p0", _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value);
                await Add_Head();

                await _kbContext.Database.ExecuteSqlRawAsync("DELETE FROM TB_BL_Grid Where F_UpdateBy = @p0", _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value);
                await Add_Grid(isOk,isNo,isPartShort);

                string sqlQ = $@"Select * From TB_BL_Grid 
                    Where F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' 
                    Order By F_No,F_Arrange";

                var DT_Grid = await _FillDT.ExecuteSQLAsync(sqlQ);

                sqlQ = $@"Select * From TB_BL_Data 
                    Where F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' 
                    Order By F_No";

                var DT_Data = await _FillDT.ExecuteSQLAsync(sqlQ);

                if(DT_Grid.Rows.Count == 0)
                {
                    throw new CustomHttpException(404, "No grid data found");
                }
                if (DT_Data.Rows.Count == 0)
                {
                    throw new CustomHttpException(404, "No data found");
                }

                var resultList = new List<string>
                {
                    JsonConvert.SerializeObject(DT_Grid, Formatting.Indented),
                    JsonConvert.SerializeObject(DT_Data, Formatting.Indented)
                };

                return resultList;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task Add_Head()
        {
            try
            {
                int intFC_Max, intCycleB, intQty_Pack, intCRUse, intAvgTrip, intSTDStock = 0, intBF = 0;
                uint intFirstTrip_Night = 0;
                double dblSafetyStock = 0;
                string strPartName = "", strSupName = "";
                string chrShift_SetStock = "", chrBF_State = "";
                DateTime dateDate = new DateTime();

                for (int intRow = 0; intRow < DT_Daily.Rows.Count; intRow++)
                {
                    intCycleB = int.TryParse(DT_Daily.Rows[intRow]["F_Cycle"].ToString().Substring(2, 2), out int intCycle) ? intCycle : 0;

                    List<SqlParameter> sqlParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@Plant", _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value),
                        new SqlParameter("@Supplier_Code", DT_Daily.Rows[intRow]["Supplier_Code"].ToString().Trim()),
                        new SqlParameter("@Supplier_Plant", DT_Daily.Rows[intRow]["Supplier_Plant"].ToString().Trim()),
                        new SqlParameter("@Part_No", DT_Daily.Rows[intRow]["Part_No"].ToString().Trim()),
                        new SqlParameter("@Ruibetsu", DT_Daily.Rows[intRow]["Ruibetsu"].ToString().Trim()),
                        new SqlParameter("@Kanban_No", DT_Daily.Rows[intRow]["Kanban_No"].ToString().Trim().Substring(1,3)),
                        new SqlParameter("@Store_Code", DT_Daily.Rows[intRow]["Store_Code"].ToString().Trim()),
                        new SqlParameter("@Date", DT_Daily.Rows[intRow]["Date"].ToString().Trim()),
                    };

                    var DT = await _FillDT.ExecuteStoreSQLAsync("sp_getForecastMax", sqlParameters.ToArray());

                    if (DT.Rows.Count > 0 && DT.Rows[0]["ForecastMax"].ToString().Trim() != "")
                    {
                        intFC_Max = int.TryParse(DT.Rows[0]["ForecastMax"].ToString().Trim(), out int intMax) ? intMax : 0;
                    }
                    else intFC_Max = 0;

                    var DR_GetData = DT_Header.Select($@"F_Process_Date = '{DT_Daily.Rows[intRow]["Date"].ToString().Trim()}'
                        AND F_Store_Code = '{DT_Daily.Rows[intRow]["Store_Code"].ToString().Trim()}'
                        AND F_Part_No = '{DT_Daily.Rows[intRow]["Part_No"].ToString().Trim()}'
                        AND F_Ruibetsu = '{DT_Daily.Rows[intRow]["Ruibetsu"].ToString().Trim()}'
                        AND F_Kanban_No = '{DT_Daily.Rows[intRow]["Kanban_No"].ToString().Trim()}'
                    ");

                    if (DR_GetData.Length > 0)
                    {
                        if ((int.TryParse(DR_GetData[0]["F_Qty_Box"].ToString().Trim(), out int intQtyBox) ? intQtyBox : 0) == 0)
                        {
                            intQty_Pack = 1;
                            intCRUse = int.Parse(DR_GetData[0]["F_TMT_FO"].ToString().Trim());
                        }
                        else
                        {
                            intQty_Pack = int.TryParse(DR_GetData[0]["F_Qty_Box"].ToString().Trim(), out int intQtyBox2) ? intQtyBox : 1;
                            intCRUse = int.Parse(DR_GetData[0]["F_TMT_FO"].ToString().Trim());
                        }
                    }
                    else
                    {
                        intQty_Pack = 1;
                        intCRUse = 0;
                    }

                    intAvgTrip = (int)Math.Floor(Math.Ceiling((double)intFC_Max / (double)intCycleB) / intQty_Pack) * intQty_Pack;

                    sqlParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@Plant", _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value),
                        new SqlParameter("@Supplier_Code", DT_Daily.Rows[intRow]["Supplier_Code"].ToString().Trim()),
                        new SqlParameter("@Supplier_Plant", DT_Daily.Rows[intRow]["Supplier_Plant"].ToString().Trim()),
                        new SqlParameter("@dateNow", DateTime.Now.AddDays(-1).ToString("yyyyMMdd")),
                        new SqlParameter("@UserName", "AutoRun")
                    };

                    DT_Period = await _FillDT.ExecuteStoreSQLAsync("sp_findPeriod", sqlParameters.ToArray());

                    if (DT_Period.Rows.Count > 0)
                    {
                        intFirstTrip_Night = uint.Parse(DT_Period.Rows[0]["F_Period"].ToString()) + 1;
                    }

                    dateDate = DateTime.ParseExact(DT_Daily.Rows[intRow]["Date"].ToString().Trim(), "yyyyMMdd", null);

                    string sqlQ = $@"SELECT TOP 1 ISNULL(F_BL,0) AS F_BL, F_Shift, F_Date+F_Shift AS Sort 
                            FROM TB_BL_SET 
                            WHERE F_Date = '{dateDate.AddDays(-1).ToString("yyyyMMdd")}'
                            AND F_Part_No = '{DT_Daily.Rows[intRow]["Part_No"].ToString().Trim()}'
                            AND F_Ruibetsu = '{DT_Daily.Rows[intRow]["Ruibetsu"].ToString().Trim()}'
                            AND F_Sebango = '{DT_Daily.Rows[intRow]["Kanban_No"].ToString().Trim().Substring(1,3)}'
                            AND F_Store_Cd = '{DT_Daily.Rows[intRow]["Store_Code"].ToString().Trim()}'
                            AND F_Sup_Cd = '{DT_Daily.Rows[intRow]["Supplier_Code"].ToString().Trim()}'
                            AND F_Sup_Plant = '{DT_Daily.Rows[intRow]["Supplier_Plant"].ToString().Trim()}'
                            ORDER BY Sort DESC";
                    
                    DT = await _FillDT.ExecuteSQLAsync(sqlQ);

                    if(DT.Rows.Count == 1)
                    {
                        intBF = int.Parse(DT.Rows[0]["F_BL"].ToString().Trim());
                        chrShift_SetStock = DT.Rows[0]["F_Shift"].ToString().Trim();
                        chrBF_State = "Y";
                    }
                    else
                    {
                        sqlQ = $@"SELECT ISNULL(F_BL,0) AS F_BL 
                            FROM TB_BL 
                            WHERE F_Delivery_Date = '{dateDate.AddDays(-1).ToString("yyyyMMdd")}'
                            AND F_Part_No = '{DT_Daily.Rows[intRow]["Part_No"].ToString().Trim()}{DT_Daily.Rows[intRow]["Ruibetsu"].ToString().Trim()}'
                            AND F_Kanban_No = '{DT_Daily.Rows[intRow]["Kanban_No"].ToString().Trim().Substring(1,3)}'
                            AND F_Store_Cd = '{DT_Daily.Rows[intRow]["Store_Code"].ToString().Trim()}'
                            AND F_Supplier_Cd = '0{DT_Daily.Rows[intRow]["Supplier_Code"].ToString().Trim()}'
                            AND F_Supplier_Plant = '{DT_Daily.Rows[intRow]["Supplier_Plant"].ToString().Trim()}'";

                        DT = await _FillDT.ExecuteSQLAsync(sqlQ);

                        if (DT.Rows.Count > 0)
                        {
                            intBF = int.Parse(DT.Rows[0]["F_BL"].ToString().Trim());
                            chrBF_State = "N";
                        }
                        else
                        {
                            //intBF = 0;
                            chrBF_State = "N";
                        }
                    }

                    var kbCon = _FillDT.kbnConnect();

                    sqlQ = $@"SELECT DISTINCT RTRIM(C.F_Part_nm) AS F_Part_nm, C.F_Safety_Stk 
                        , RTRIM(S.F_short_name) AS F_short_name, P.F_Address 
                        FROM T_Construction C INNER JOIN T_Supplier_ms S 
                        ON C.F_supplier_cd = S.F_supplier_cd 
                        AND C.F_plant =  S.F_Plant_cd 
                        AND C.F_Store_cd = S.F_Store_cd 
                        LEFT JOIN {kbCon}.[dbo].TB_MS_Kanban P 
                        ON C.F_supplier_cd = P.F_Supplier_Code collate THAI_CS_AS 
                        AND C.F_plant = P.F_Supplier_Plant collate THAI_CS_AS 
                        AND C.F_Store_cd = P.F_Store_Code collate THAI_CS_AS 
                        AND RIGHT('0000'+C.F_Sebango,4) = P.F_Kanban_No collate THAI_CS_AS 
                        AND C.F_Part_no = P.F_Part_No collate THAI_CS_AS 
                        AND C.F_Ruibetsu = P.F_Ruibetsu collate THAI_CS_AS 
                        WHERE C.F_Part_no = '{DT_Daily.Rows[intRow]["Part_No"].ToString().Trim()}'
                        AND C.F_Ruibetsu = '{DT_Daily.Rows[intRow]["Ruibetsu"].ToString().Trim()}' 
                        AND C.F_Sebango = '{DT_Daily.Rows[intRow]["Kanban_No"].ToString().Trim().Substring(1, 3)}'
                        AND C.F_supplier_cd = '{DT_Daily.Rows[intRow]["Supplier_Code"].ToString().Trim()}'
                        AND C.F_plant = '{DT_Daily.Rows[intRow]["Supplier_Plant"].ToString().Trim()}'
                        AND C.F_Store_cd = '{DT_Daily.Rows[intRow]["Store_Code"].ToString().Trim()}'
                        AND C.F_Local_Str <= convert(char(8),getdate(),112) 
                        AND C.F_Local_End >= convert(char(8),getdate(),112) 
                        AND S.F_TC_Str <= convert(char(8),getdate(),112) 
                        AND S.F_TC_End >= convert(char(8),getdate(),112) ";

                    DT = await _FillDT.ExecuteSQLAsyncPPMDB(sqlQ);

                    if(DT.Rows.Count > 0 && DT.Rows[0]["F_Part_nm"].ToString().Trim() != "")
                    {
                        strPartName = DT.Rows[0]["F_Part_nm"].ToString().Trim();
                        strSupName = DT.Rows[0]["F_short_name"].ToString().Trim();
                        dblSafetyStock = double.TryParse(DT.Rows[0]["F_Safety_Stk"].ToString().Trim(), out double dblSafety) ? dblSafety : 0;
                    }
                    else
                    {
                        strPartName = "";
                        dblSafetyStock = 0;
                        strSupName = "";
                    }

                    sqlParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@Plant", _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value),
                        new SqlParameter("@Supplier_Code", DT_Daily.Rows[intRow]["Supplier_Code"].ToString().Trim()),
                        new SqlParameter("@Supplier_Plant", DT_Daily.Rows[intRow]["Supplier_Plant"].ToString().Trim()),
                        new SqlParameter("@Part_No", DT_Daily.Rows[intRow]["Part_No"].ToString().Trim()),
                        new SqlParameter("@Ruibetsu", DT_Daily.Rows[intRow]["Ruibetsu"].ToString().Trim()),
                        new SqlParameter("@Kanban_No", DT_Daily.Rows[intRow]["Kanban_No"].ToString().Trim()),
                        new SqlParameter("@Store_Code", DT_Daily.Rows[intRow]["Store_Code"].ToString().Trim()),
                        new SqlParameter("@Date", DT_Daily.Rows[intRow]["Date"].ToString().Trim()),
                    };

                    DT = await _FillDT.ExecuteStoreSQLAsync("sp_getSTDStock", sqlParameters.ToArray());

                    if(DT.Rows.Count > 0 && DT.Rows[0]["STDStock"].ToString().Trim() != "")
                    {
                        if (int.TryParse(DT.Rows[0]["STDStock"].ToString().Trim(), out intSTDStock))
                        {
                            intSTDStock = (int)Math.Round(decimal.Parse(DT.Rows[0]["STDStock"].ToString().Trim()));
                        }
                        else
                        {
                            intSTDStock = 0;
                        }
                    }

                    sqlQ = $@"INSERT INTO TB_BL_Data(F_No, F_Date, F_Part_No, F_Ruibetsu, F_Store_Cd, F_Part_NM 
                        ,F_Short_Name, F_Supplier_Cd, F_Supplier_Plant, F_Sebango, F_Cycle, F_Qty 
                        ,F_Safety_Stock, F_FC_Max, F_CR_Use, F_AVG, F_Std_Stock 
                        ,F_BF1,F_BF_State, F_FC, F_PD, F_Order, F_Status
                        ,F_UpdateDate, F_UpdateBy 
                        ,F_Order1,F_Order2, F_Order3, F_Order4, F_Order5, F_Order6, F_Order7, F_Order8, F_Order9, F_Order10 
                        ,F_Order11, F_Order12, F_Order13, F_Order14, F_Order15, F_Order16, F_Order17, F_Order18, F_Order19, F_Order20 
                        ,F_Order21, F_Order22, F_Order23, F_Order24) 
                        VALUES ('{intRow + 1}','{DT_Daily.Rows[intRow]["Date"].ToString().Trim()}'
                        , '{DT_Daily.Rows[intRow]["Part_No"].ToString().Trim()}'
                        , '{DT_Daily.Rows[intRow]["Ruibetsu"].ToString().Trim()}'
                        , '{DT_Daily.Rows[intRow]["Store_Code"].ToString().Trim()}'
                        , '{strPartName}'
                        , '{strSupName}'
                        , '{DT_Daily.Rows[intRow]["Supplier_Code"].ToString().Trim()}'
                        , '{DT_Daily.Rows[intRow]["Supplier_Plant"].ToString().Trim()}'
                        , '{DT_Daily.Rows[intRow]["Kanban_No"].ToString().Trim().Substring(1,3)}'
                        , '{DT_Daily.Rows[intRow]["F_Cycle"].ToString().Trim().Substring(0,2)}-{DT_Daily.Rows[intRow]["F_Cycle"].ToString().Trim().Substring(2, 2)}-{DT_Daily.Rows[intRow]["F_Cycle"].ToString().Trim().Substring(4,2)}'
                        , '{intQty_Pack}'
                        , '{dblSafetyStock}'
                        , '{intFC_Max}','{intCRUse}','{intAvgTrip}','{intSTDStock}'
                        , '{intBF}','{chrBF_State}{(chrShift_SetStock == "D" ? intFirstTrip_Night : 1)}','{Math.Round((double)intCRUse/intCycleB,0)}'
                        , '{intCRUse/2}','','','{DateTime.Now}','{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'
                        ,'','','','','','','','','',''
                        ,'','','','','','','','','',''
                        ,'','','','')";

                    await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                    _log.WriteLogMsg("Insert TB_BL_Data " + sqlQ);

                }

                await Add_Order();
                await Add_BL();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task Add_Order()
        {
            try
            {
                int intSum = 0;
                string sqlQ = $@"SELECT * FROM TB_BL_Data 
                    WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' 
                    ORDER BY F_No,F_Date ";

                var DT = await _FillDT.ExecuteSQLAsync(sqlQ);

                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    intSum = 0;

                    sqlQ = $@"SELECT SUM(D.F_Unit_Amount) AS F_Amount, F_Delivery_Trip 
                        FROM  TB_REC_Header H INNER JOIN TB_REC_Detail D 
                        ON H.F_OrderNo = D.F_OrderNo 
                        WHERE (H.F_Status = 'N' ) AND H.F_OrderType IN ('N','U') 
                        AND H.F_Supplier_Code = '{DT.Rows[i]["F_Supplier_Cd"].ToString().Trim()}'
                        AND H.F_Supplier_Plant = '{DT.Rows[i]["F_Supplier_Plant"].ToString().Trim()}'
                        AND H.F_Delivery_Date = '{DT.Rows[i]["F_Date"].ToString().Trim()}'
                        AND D.F_Part_No = '{DT.Rows[i]["F_Part_No"].ToString().Trim()}'
                        AND D.F_Ruibetsu = '{DT.Rows[i]["F_Ruibetsu"].ToString().Trim()}'
                        AND H.F_Delivery_Dock = '{DT.Rows[i]["F_Store_Cd"].ToString().Trim()}'
                        AND D.F_Kanban_No = '0{DT.Rows[i]["F_Sebango"].ToString().Trim()}'
                        GROUP BY F_Delivery_Trip ORDER BY F_Delivery_Trip ";

                    var DT2 = await _FillDT.ExecuteSQLAsync(sqlQ);

                    if(DT2.Rows.Count > 0)
                    {
                        for (int j = 0; j < DT2.Rows.Count; j++)
                        {
                            intSum += int.Parse(DT2.Rows[j]["F_Amount"].ToString().Trim());

                            sqlQ = $@"UPDATE TB_BL_Data SET 
                                F_Order{int.Parse(DT2.Rows[j]["F_Delivery_Trip"].ToString().Trim())}
                                = '{int.Parse(DT2.Rows[j]["F_Amount"].ToString().Trim())}'
                                , F_Order = '{intSum}' 
                                WHERE F_Date = '{DT.Rows[i]["F_Date"].ToString().Trim()}'
                                AND F_Part_No = '{DT.Rows[i]["F_Part_No"].ToString().Trim()}'
                                AND F_Ruibetsu = '{DT.Rows[i]["F_Ruibetsu"].ToString().Trim()}'
                                AND F_Store_Cd = '{DT.Rows[i]["F_Store_Cd"].ToString().Trim()}'
                                AND F_Supplier_Cd = '{DT.Rows[i]["F_Supplier_Cd"].ToString().Trim()}'
                                AND F_Supplier_Plant = '{DT.Rows[i]["F_Supplier_Plant"].ToString().Trim()}'
                                AND F_Sebango = '{DT.Rows[i]["F_Sebango"].ToString().Trim()}'
                                AND F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'
                                ";
                            await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                            _log.WriteLogMsg("Update TB_BL_Data " + sqlQ);
                        }
                    
                    }

                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task Add_BL()
        {
            try
            {
                int intBL = 0;
                int intCycleB = 0,intOrder = 0,intCRUse = 0;
                double dblFC = 0, dblPD = 0;

                var DT = await _FillDT.ExecuteSQLAsync($@"SELECT * 
                    FROM TB_BL_Data WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' 
                    ORDER BY F_No, F_Date ");

                if(DT.Rows.Count > 0)
                {
                    intBL = int.Parse(DT.Rows[0]["F_BF1"].ToString().Trim());
                    for(int i = 0; i < DT.Rows.Count; i++)
                    {
                        if (DT.Rows[i]["F_BF_State"].ToString().Trim().StartsWith("Y"))
                        {
                            intBL = int.Parse(DT.Rows[i]["F_BF1"].ToString().Trim());
                        }
                        intCycleB = int.TryParse(DT.Rows[i]["F_Cycle"].ToString().Trim().Substring(3, 2), out int intCycle) ? intCycle : 0;
                        intCRUse = int.Parse(DT.Rows[i]["F_CR_Use"].ToString().Trim());

                        for (int c = 1; c <= intCycleB; c++)
                        { 
                            dblFC = c != intCycleB ? Math.Ceiling(double.Parse(DT.Rows[i]["F_FC"].ToString().Trim())) : intCRUse - ((intCycleB - 1) * Math.Ceiling(double.Parse(DT.Rows[i]["F_FC"].ToString().Trim())));
                            string OrderC = "F_Order" + c.ToString().Trim();
                            intOrder = int.Parse(DT.Rows[i][OrderC].ToString().Trim());
                            intBL = (intBL - (int)dblFC) + intOrder;

                            string sqlQ = $@"UPDATE TB_BL_Data SET F_BF{c + 1} = '{(c == intCycleB ? DBNull.Value : intBL)}'
                                ,F_BL{c} = '{intBL}' 
                                ,F_FC{c} = '{dblFC}'
                                WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' 
                                AND F_No = '{i + 1}'";

                            await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                            _log.WriteLogMsg("Update TB_BL_Data " + sqlQ);

                            if (c == intCycleB && i + 2 <= DT.Rows.Count && DT.Rows[i]["F_Sebango"].ToString().Trim() == DT.Rows[i + 1]["F_Sebango"].ToString().Trim())
                            {
                                sqlQ = $@"UPDATE TB_BL_Data SET F_BF1 = '{intBL}' 
                                    WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' 
                                    AND F_No = '{i + 2}'
                                    AND F_BF_State NOT LIKE '%Y%'
                                    ";

                                await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                                _log.WriteLogMsg("Update TB_BL_Data " + sqlQ);

                            }

                            int percent = (int)(dblFC * 0.4);
                            percent = (int)dblFC - percent;

                            if(intBL > percent)
                            {
                                sqlQ = $@"UPDATE TB_BL_Data SET F_Status = 'OK'
                                    , F_BL = '{intBL}'
                                    WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' 
                                    AND F_No = '{i + 1}'";

                                await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                                _log.WriteLogMsg("Update F_Status " + sqlQ);
                            }
                            else
                            {
                                sqlQ = $@"UPDATE TB_BL_Data SET F_Status = 'NO'
                                    , F_BL = '{intBL}'
                                    WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' 
                                    AND F_No = '{i + 1}'";

                                await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                                _log.WriteLogMsg("Update F_Status " + sqlQ);
                            }

                            dblPD = double.Parse(DT.Rows[i]["F_PD"].ToString().Trim());

                            sqlQ = $@"UPDATE TB_BL_Data SET F_PD1 = '{Math.Floor(dblPD)}'
                                , F_PD2 = '{Math.Ceiling(dblPD)}'   
                                WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' 
                                AND F_No = '{i + 1}'";

                            await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                            _log.WriteLogMsg("Update TB_BL_Data " + sqlQ);

                            for(int j = intCycleB +1; j <= 24; j++)
                            {
                                sqlQ = $@"UPDATE TB_BL_Data SET F_Order{j} = NULL 
                                    WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' 
                                    AND F_No = '{i + 1}'";

                                await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                                _log.WriteLogMsg("Update TB_BL_Data " + sqlQ);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task Add_Grid(bool isOk,bool isNo,bool isPartShort)
        {
            try
            {
                string sqlQ = $@"INSERT INTO TB_BL_Grid(F_No,F_TRIP,F_R1,F_R2,F_R3,F_R4,F_R5,F_R6,F_R7,F_R8,F_R9,F_R10
                    ,F_R11,F_R12,F_R13,F_R14,F_R15,F_R16,F_R17,F_R18,F_R19,F_R20,F_R21,F_R22,F_R23,F_R24
                    ,F_UpdateBy,F_UpdateDate,F_Arrange)
                    SELECT F_No,'B/F' AS F_TRIP,F_BF1,F_BF2,F_BF3,F_BF4,F_BF5,F_BF6,F_BF7,F_BF8,F_BF9,F_BF10
                    ,F_BF11,F_BF12,F_BF13,F_BF14,F_BF15,F_BF16,F_BF17,F_BF18,F_BF19,F_BF20,F_BF21,F_BF22,F_BF23,F_BF24
                    ,F_UpdateBy,getDate(),'1'
                    FROM TB_BL_Data
                    WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'";

                if(isOk && !isNo && !isPartShort)
                {
                    sqlQ += " AND F_Status = 'OK' ";
                }
                else if (!isOk && isNo && !isPartShort)
                {
                    sqlQ += " AND F_Status = 'NO' ";
                }
                else if (!isOk && !isNo && isPartShort)
                {
                    sqlQ += " AND F_BL < 0 ";
                }
                else if (!isOk &&  isNo && isPartShort)
                {
                    sqlQ += "AND (F_Status = 'NO' OR F_BL < 0 ) ";
                }
                else if (isOk && !isNo && isPartShort)
                {
                    sqlQ += " AND (F_Status = 'OK' OR F_BL < 0 ) ";
                }

                sqlQ += "ORDER BY F_No ";

                await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                _log.WriteLogMsg("Insert TB_BL_Grid " + sqlQ);

                sqlQ = $@"INSERT INTO TB_BL_Grid(F_No,F_TRIP,F_R1,F_R2,F_R3,F_R4,F_R5,F_R6,F_R7,F_R8,F_R9,F_R10
                    ,F_R11,F_R12,F_R13,F_R14,F_R15,F_R16,F_R17,F_R18,F_R19,F_R20,F_R21,F_R22,F_R23,F_R24
                    ,F_UpdateBy,F_UpdateDate,F_Arrange) 
                    SELECT F_No,'Forecast [Pcs.]' AS F_TRIP,F_FC1,F_FC2,F_FC3,F_FC4,F_FC5,F_FC6,F_FC7,F_FC8,F_FC9,F_FC10
                    ,F_FC11,F_FC12,F_FC13,F_FC14,F_FC15,F_FC16,F_FC17,F_FC18,F_FC19,F_FC20,F_FC21,F_FC22,F_FC23,F_FC24
                    ,F_UpdateBy,getDate(),'2' 
                    FROM TB_BL_Data 
                    WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'";

                if (isOk && !isNo && !isPartShort)
                {
                    sqlQ += " AND F_Status = 'OK' ";
                }
                else if (!isOk && isNo && !isPartShort)
                {
                    sqlQ += " AND F_Status = 'NO' ";
                }
                else if (!isOk && !isNo && isPartShort)
                {
                    sqlQ += " AND F_BL < 0 ";
                }
                else if (!isOk && isNo && isPartShort)
                {
                    sqlQ += "AND (F_Status = 'NO' OR F_BL < 0 ) ";
                }
                else if (isOk && !isNo && isPartShort)
                {
                    sqlQ += " AND (F_Status = 'OK' OR F_BL < 0 ) ";
                }

                sqlQ += "ORDER BY F_No ";
                await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                _log.WriteLogMsg("Insert TB_BL_Grid " + sqlQ);

                sqlQ = $@"INSERT INTO TB_BL_Grid(F_No,F_TRIP,F_R1,F_R2,F_UpdateBy,F_UpdateDate,F_Arrange) 
                    SELECT F_No,'PD/Day [Pcs.]' AS F_TRIP,F_PD1, F_PD2,F_UpdateBy,getDate(),'3' 
                    FROM TB_BL_Data 
                    WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'";

                if (isOk && !isNo && !isPartShort)
                {
                    sqlQ += " AND F_Status = 'OK' ";
                }
                else if (!isOk && isNo && !isPartShort)
                {
                    sqlQ += " AND F_Status = 'NO' ";
                }
                else if (!isOk && !isNo && isPartShort)
                {
                    sqlQ += " AND F_BL < 0 ";
                }
                else if (!isOk && isNo && isPartShort)
                {
                    sqlQ += "AND (F_Status = 'NO' OR F_BL < 0 ) ";
                }
                else if (isOk && !isNo && isPartShort)
                {
                    sqlQ += " AND (F_Status = 'OK' OR F_BL < 0 ) ";
                }

                sqlQ += "ORDER BY F_No ";
                await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                _log.WriteLogMsg("Insert TB_BL_Grid " + sqlQ);

                sqlQ = $@"INSERT INTO TB_BL_Grid(F_No,F_TRIP,F_R1,F_R2,F_R3,F_R4,F_R5,F_R6,F_R7,F_R8,F_R9,F_R10
                    ,F_R11,F_R12,F_R13,F_R14,F_R15,F_R16,F_R17,F_R18,F_R19,F_R20,F_R21,F_R22,F_R23,F_R24
                    ,F_UpdateBy,F_UpdateDate,F_Arrange) 
                    SELECT F_No,'Order [Pcs.]' AS F_TRIP,F_Order1,F_Order2,F_Order3,F_Order4,F_Order5,F_Order6,F_Order7,F_Order8,F_Order9,F_Order10
                    ,F_Order11,F_Order12,F_Order13,F_Order14,F_Order15,F_Order16,F_Order17,F_Order18,F_Order19,F_Order20,F_Order21,F_Order22,F_Order23,F_Order24
                    ,F_UpdateBy,getDate(),'4' 
                    FROM TB_BL_Data 
                    WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'";

                if (isOk && !isNo && !isPartShort)
                {
                    sqlQ += " AND F_Status = 'OK' ";
                }
                else if (!isOk && isNo && !isPartShort)
                {
                    sqlQ += " AND F_Status = 'NO' ";
                }
                else if (!isOk && !isNo && isPartShort)
                {
                    sqlQ += " AND F_BL < 0 ";
                }
                else if (!isOk && isNo && isPartShort)
                {
                    sqlQ += "AND (F_Status = 'NO' OR F_BL < 0 ) ";
                }
                else if (isOk && !isNo && isPartShort)
                {
                    sqlQ += " AND (F_Status = 'OK' OR F_BL < 0 ) ";
                }

                sqlQ += "ORDER BY F_No ";
                await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                _log.WriteLogMsg("Insert TB_BL_Grid " + sqlQ);

                sqlQ = $@"INSERT INTO TB_BL_Grid(F_No,F_TRIP,F_R1,F_UpdateBy,F_UpdateDate,F_Arrange) 
                    SELECT F_No,'Order Total' AS F_TRIP,F_Order,F_UpdateBy,getDate(),'5' 
                    FROM TB_BL_Data 
                    WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'";

                if (isOk && !isNo && !isPartShort)
                {
                    sqlQ += " AND F_Status = 'OK' ";
                }
                else if (!isOk && isNo && !isPartShort)
                {
                    sqlQ += " AND F_Status = 'NO' ";
                }
                else if (!isOk && !isNo && isPartShort)
                {
                    sqlQ += " AND F_BL < 0 ";
                }
                else if (!isOk && isNo && isPartShort)
                {
                    sqlQ += "AND (F_Status = 'NO' OR F_BL < 0 ) ";
                }
                else if (isOk && !isNo && isPartShort)
                {
                    sqlQ += " AND (F_Status = 'OK' OR F_BL < 0 ) ";
                }

                sqlQ += "ORDER BY F_No ";
                await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                _log.WriteLogMsg("Insert TB_BL_Grid " + sqlQ);

                sqlQ = $@"INSERT INTO TB_BL_Grid(F_No,F_TRIP,F_R1,F_R2,F_R3,F_R4,F_R5,F_R6,F_R7,F_R8,F_R9,F_R10
                    ,F_R11,F_R12,F_R13,F_R14,F_R15,F_R16,F_R17,F_R18,F_R19,F_R20,F_R21,F_R22,F_R23,F_R24
                    ,F_UpdateBy,F_UpdateDate,F_Arrange) 
                    SELECT F_No,'B/L' AS F_TRIP,F_BL1,F_BL2,F_BL3,F_BL4,F_BL5,F_BL6,F_BL7,F_BL8,F_BL9,F_BL10
                    ,F_BL11,F_BL12,F_BL13,F_BL14,F_BL15,F_BL16,F_BL17,F_BL18,F_BL19,F_BL20,F_BL21,F_BL22,F_BL23,F_BL24
                    ,F_UpdateBy,getDate(),'6' 
                    FROM TB_BL_Data 
                    WHERE F_UpdateBy = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'";

                if (isOk && !isNo && !isPartShort)
                {
                    sqlQ += " AND F_Status = 'OK' ";
                }
                else if (!isOk && isNo && !isPartShort)
                {
                    sqlQ += " AND F_Status = 'NO' ";
                }
                else if (!isOk && !isNo && isPartShort)
                {
                    sqlQ += " AND F_BL < 0 ";
                }
                else if (!isOk && isNo && isPartShort)
                {
                    sqlQ += "AND (F_Status = 'NO' OR F_BL < 0 ) ";
                }
                else if (isOk && !isNo && isPartShort)
                {
                    sqlQ += " AND (F_Status = 'OK' OR F_BL < 0 ) ";
                }
                sqlQ += "ORDER BY F_No ";

                await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                _log.WriteLogMsg("Insert TB_BL_Grid " + sqlQ);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
