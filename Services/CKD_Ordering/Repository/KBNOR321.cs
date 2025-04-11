using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;

namespace KANBAN.Services.CKD_Ordering.Repository
{
    public class KBNOR321 : IKBNOR321
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNOR321
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

        public static DateTime DateLogin;
        public static DateTime DateDelivery;

        private static string ShiftLogin;
        private static string charStartDate;
        private static string charEndDate;

        private static int intAmountShow;
        private static int intDeliveryTrip;

        private static DataTable DT_DeliveryDate = new DataTable();
        private static DataTable DT_Date = new DataTable();
        private static DataTable DT_Period = new DataTable();
        private static DataTable DT_PartControl = new DataTable();
        private static DataTable DT_Header = new DataTable();
        private static DataTable DT_Detail = new DataTable();
        private static DataTable DT_Volume = new DataTable();
        private static DataTable DT_AdjustOrder_Trip = new DataTable();
        private static DataTable DT_Actual_Receive = new DataTable();

        public async Task Onload(string _loginDate)
        {
            try
            {
                DateLogin = DateTime.ParseExact(_loginDate.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                ShiftLogin = _loginDate.Substring(10, 1);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<List<List<string>>> GetDropDownData(string? F_Supplier_Code, string? F_Store_Code)
        {
            try
            {
                List<List<string>> DataList = new List<List<string>>
                {
                    await GetSupplier(),
                    await GetKanban(F_Supplier_Code),
                    await GetStore(F_Supplier_Code),
                    await GetPartNo(F_Supplier_Code,F_Store_Code),
                };

                return DataList;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task<List<string>> GetSupplier()
        {
            try
            {
                string Dy_Store = _BearerClass.Plant switch
                {
                    "1" => "1D",
                    "2" => "2D",
                    "3" => "3C",
                    _ => "3C"
                };

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Plant", _BearerClass.Plant),
                    new SqlParameter("@Store_Code_FROM", Dy_Store),
                    new SqlParameter("@Store_Code_TO", Dy_Store),
                };

                var dt = await _FillDT.ExecuteStoreSQLAsync($"sp_NormalOrder_getSupplier", parameters);

                List<string> Supplier = new List<string> { "All" };

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Supplier.Add(dt.Rows[i]["F_Supplier"].ToString());
                }

                return Supplier;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        private async Task<List<string>> GetKanban(string? F_Supplier_Code)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Plant", _BearerClass.Plant),
                    new SqlParameter("@OrderType", "Daily"),
                };

                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    Array.Resize(ref parameters, parameters.Length + 2);
                    parameters[2] = new SqlParameter("@Supplier_Code", F_Supplier_Code.Split("-")[0]);
                    parameters[3] = new SqlParameter("@Supplier_Plant", F_Supplier_Code.Split("-")[1]);
                }

                var dt = await _FillDT.ExecuteStoreSQLAsync($"sp_NormalOrder_getKanban", parameters);

                List<string> Kanban = new List<string> { "All" };

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Kanban.Add(dt.Rows[i]["F_Kanban_No"].ToString());
                }

                return Kanban;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        private async Task<List<string>> GetStore(string? F_Supplier_Code)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Plant", _BearerClass.Plant),
                    new SqlParameter("@OrderType", "Daily"),
                };

                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    Array.Resize(ref parameters, parameters.Length + 2);
                    parameters[2] = new SqlParameter("@Supplier_Code", F_Supplier_Code.Split("-")[0]);
                    parameters[3] = new SqlParameter("@Supplier_Plant", F_Supplier_Code.Split("-")[1]);
                }

                var dt = await _FillDT.ExecuteStoreSQLAsync($"sp_NormalOrder_getStoreCode", parameters);

                List<string> Store = new List<string> { "All" };

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Store.Add(dt.Rows[i]["F_Store_CD"].ToString());
                }

                return Store;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        private async Task<List<string>> GetPartNo(string? F_Supplier_Code, string? F_Store_Code)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Plant", _BearerClass.Plant),
                    new SqlParameter("@OrderType", "Daily"),
                };

                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    Array.Resize(ref parameters, parameters.Length + 2);
                    parameters[2] = new SqlParameter("@Supplier_Code", F_Supplier_Code.Split("-")[0]);
                    parameters[3] = new SqlParameter("@Supplier_Plant", F_Supplier_Code.Split("-")[1]);
                }

                if (!string.IsNullOrWhiteSpace(F_Store_Code) && F_Store_Code != "All")
                {
                    Array.Resize(ref parameters, parameters.Length + 2);
                    parameters[4] = new SqlParameter("@Store_Code_FROM", F_Store_Code);
                    parameters[5] = new SqlParameter("@Store_Code_TO", F_Store_Code);
                }

                var dt = await _FillDT.ExecuteStoreSQLAsync($"sp_NormalOrder_getPartNo", parameters);

                List<string> Part = new List<string> { "All" };

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Part.Add(dt.Rows[i]["F_Part_No"].ToString());
                }

                return Part;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task Find_StartEnd_Date(string action, string F_Supplier_Code)
        {
            try
            {
                string Dy_Store = _BearerClass.Plant switch
                {
                    "1" => "1D",
                    "2" => "2D",
                    "3" => "3C",
                    _ => "3C"
                };
                DataTable DT;
                SqlParameter[] sqlParams = new SqlParameter[] { };

                if (action.ToLower() == "preview")
                {
                    sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("@Plant",_BearerClass.Plant),
                        new SqlParameter("@Supplier_Code",F_Supplier_Code.Split("-")[0]),
                        new SqlParameter("@Supplier_Plant",F_Supplier_Code.Split("-")[1]),
                        new SqlParameter("@Store_Code",Dy_Store),
                        new SqlParameter("@Date",DateLogin.ToString("yyyyMMdd"))
                    };

                    DT = await _FillDT.ExecuteStoreSQLAsync("[CKD_Inhouse].[sp_NumberOfDayToPreview]", sqlParams);
                    charStartDate = DT.Rows[0]["Start_Date"].ToString();
                    charEndDate = DT.Rows[0]["End_Date"].ToString();
                    intAmountShow = int.Parse(DT.Rows[0]["Display_Date"].ToString());

                }
                else if (action.ToLower() == "process")
                {
                    sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("@Plant",_BearerClass.Plant),
                        new SqlParameter("@Supplier_Code",F_Supplier_Code.Split("-")[0]),
                        new SqlParameter("@Supplier_Plant",F_Supplier_Code.Split("-")[1]),
                        new SqlParameter("@Shift",ShiftLogin),
                        new SqlParameter("@UserName",_BearerClass.UserCode),
                        new SqlParameter("@Date",DateTime.Now.ToString("yyyyMMdd"))
                    };

                    DT = await _FillDT.ExecuteStoreSQLAsync("[CKD_Inhouse].[sp_NumberOfDayToSearch]", sqlParams);
                    charStartDate = DT.Rows[0]["Start_Date"].ToString();
                    charEndDate = DT.Rows[0]["End_Date"].ToString();
                    intAmountShow = int.Parse(DT.Rows[0]["Display_Date"].ToString());
                }

                sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@Plant",_BearerClass.Plant),
                    new SqlParameter("@Supplier_Code",F_Supplier_Code.Split("-")[0]),
                    new SqlParameter("@Supplier_Plant",F_Supplier_Code.Split("-")[1]),
                    new SqlParameter("@ProcessDate",DateTime.Now.ToString("yyyyMMdd")),
                    new SqlParameter("@ProcessShift",ShiftLogin)
                };

                DT_DeliveryDate = await _FillDT.ExecuteStoreSQLAsync("[CKD_Inhouse].[sp_getDeliveryDateTrip]", sqlParams);
                if (DT_DeliveryDate.Rows.Count > 0)
                {
                    DateDelivery = DateTime.ParseExact(DT_DeliveryDate.Rows[0]["F_Delivery_Date"].ToString().Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
                    intDeliveryTrip = int.Parse(DT_DeliveryDate.Rows[0]["F_Delivery_Trip"].ToString().Trim());
                }

                sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@Supplier_Code",F_Supplier_Code.Split("-")[0]),
                    new SqlParameter("@Supplier_Plant",F_Supplier_Code.Split("-")[1]),
                    new SqlParameter("@StartDate",charStartDate),
                    new SqlParameter("@EndDate",charEndDate),
                };

                DT_Date = await _FillDT.ExecuteStoreSQLAsync("sp_getCycleTime", sqlParams);

                if (charStartDate != DT_Date.Rows[0]["F_Date"].ToString())
                {
                    charStartDate = DT_Date.Rows[0]["F_Date"].ToString();
                }
                if (charEndDate != DT_Date.Rows[DT_Date.Rows.Count - 1]["F_Date"].ToString())
                {
                    charEndDate = DT_Date.Rows[DT_Date.Rows.Count - 1]["F_Date"].ToString();
                }
                intAmountShow = DT_Date.Rows.Count;

                // Initialize parameters outside the loop
                sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@Plant", _BearerClass.Plant),
                    new SqlParameter("@Supplier_Code", F_Supplier_Code.Split("-")[0]),
                    new SqlParameter("@Supplier_Plant", F_Supplier_Code.Split("-").Length > 1 ? F_Supplier_Code.Split("-")[1] : ""),
                    new SqlParameter("@dateNow", DBNull.Value), // Placeholder value
                    new SqlParameter("@UserName", _BearerClass.UserCode),
                };
                var DT_Temp = await _FillDT.ExecuteStoreSQLAsync("sp_findPeriod", sqlParams);

                DT_Period.Clear();
                DT_Period = DT_Temp.Clone();

                for (int i = 0; i < intAmountShow; i++)
                {
                    sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("@Plant", _BearerClass.Plant),
                        new SqlParameter("@Supplier_Code", F_Supplier_Code.Split("-")[0]),
                        new SqlParameter("@Supplier_Plant", F_Supplier_Code.Split("-").Length > 1 ? F_Supplier_Code.Split("-")[1] : ""),
                        new SqlParameter("@dateNow", DT_Date.Rows[i]["F_Date"].ToString().Trim()), // Placeholder value
                        new SqlParameter("@UserName", _BearerClass.UserCode),
                    };

                    DT_Temp = await _FillDT.ExecuteStoreSQLAsync("sp_findPeriod", sqlParams);

                    foreach (DataRow row in DT_Temp.Rows)
                    {
                        DT_Period.ImportRow(row);
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        private async Task Set_All_Data(string action, string F_Supplier_Code, string? F_KanbanFrom, string? F_KanbanTo, string? F_StoreFrom, string? F_StoreTo, string? F_PartFrom, string? F_PartTo)
        {
            try
            {
                var sqlParam = new List<SqlParameter>
                {
                    new SqlParameter("@StartDate",charStartDate),
                    new SqlParameter("@EndDate",charEndDate),
                    new SqlParameter("@Supplier_Code",F_Supplier_Code.Split("-")[0]),
                    new SqlParameter("@Supplier_Plant",F_Supplier_Code.Split("-")[1]),
                };

                if (!string.IsNullOrWhiteSpace(F_KanbanFrom) && !string.IsNullOrWhiteSpace(F_KanbanTo)
                    && F_KanbanFrom.ToUpper() != "ALL" && F_KanbanTo.ToUpper() != "ALL")
                {
                    sqlParam.Add(new SqlParameter("@Kanban_No_FROM", F_KanbanFrom));
                    sqlParam.Add(new SqlParameter("@Kanban_No_TO", F_KanbanTo));
                }

                if (!string.IsNullOrWhiteSpace(F_StoreFrom) && !string.IsNullOrWhiteSpace(F_StoreTo)
                    && F_StoreFrom.ToUpper() != "ALL" && F_StoreTo.ToUpper() != "ALL")
                {
                    sqlParam.Add(new SqlParameter("@Store_Code_FROM", F_StoreFrom));
                    sqlParam.Add(new SqlParameter("@Store_Code_TO", F_StoreTo));
                }

                if (!string.IsNullOrWhiteSpace(F_PartFrom) && !string.IsNullOrWhiteSpace(F_PartTo)
                    && F_PartFrom.ToUpper() != "ALL" && F_PartTo.ToUpper() != "ALL")
                {
                    sqlParam.Add(new SqlParameter("@Part_No_FROM", F_PartFrom.Split("-")[0]));
                    sqlParam.Add(new SqlParameter("@Part_No_TO", F_PartTo.Split("-")[0]));
                    sqlParam.Add(new SqlParameter("@Ruibetsu_FROM", F_PartFrom.Split("-")[1]));
                    sqlParam.Add(new SqlParameter("@Ruibetsu_TO", F_PartTo.Split("-")[1]));
                }

                DT_PartControl = await _FillDT.ExecuteStoreSQLAsync("[CKD_Inhouse].[sp_DT_PartControl]", sqlParam.ToArray());

                if (DT_PartControl.Rows.Count == 0)
                {
                    throw new CustomHttpException(404, "No Data PartControl");
                }
                else
                {
                    if (action.ToLower() == "re-calculate bl" || action.ToLower() == "re-calculate")
                    {
                        //get_startDate();
                    }
                }

                DT_Header = await _FillDT.ExecuteStoreSQLAsync("[CKD_Inhouse].[sp_DT_Header]", sqlParam.ToArray());

                if (DT_Header.Rows.Count == 0)
                {
                    throw new CustomHttpException(404, "No Data Header");
                }

                DT_Detail = await _FillDT.ExecuteStoreSQLAsync("[CKD_Inhouse].[sp_DT_Detail]", sqlParam.ToArray());

                if (DT_Detail.Rows.Count == 0)
                {
                    throw new CustomHttpException(404, "No Data Detail");
                }

                DT_Volume = await _FillDT.ExecuteStoreSQLAsync("[CKD_Inhouse].[sp_DT_Volume]", sqlParam.ToArray());

                if (DT_Volume.Rows.Count == 0)
                {
                    throw new CustomHttpException(404, "No Data Volume");
                }

                DT_AdjustOrder_Trip = await _FillDT.ExecuteStoreSQLAsync("sp_DT_AdjustOder_Trip", sqlParam.ToArray());

                DT_Actual_Receive = await _FillDT.ExecuteStoreSQLAsync("sp_DT_Actual_Receive", sqlParam.ToArray());

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<List<string>> Get_All_Data(string action, string F_Supplier_Code, string? F_KanbanFrom, string? F_KanbanTo, string? F_StoreFrom, string? F_StoreTo, string? F_PartFrom, string? F_PartTo)
        {
            try
            {
                await Find_StartEnd_Date(action, F_Supplier_Code);
                await Set_All_Data(action, F_Supplier_Code, F_KanbanFrom, F_KanbanTo, F_StoreFrom, F_StoreTo, F_PartFrom, F_PartTo);

                var ListData = new List<string>
                {
                    JsonConvert.SerializeObject(DT_DeliveryDate),
                    JsonConvert.SerializeObject(DT_Date),
                    JsonConvert.SerializeObject(DT_Period),
                    JsonConvert.SerializeObject(DT_PartControl),
                    JsonConvert.SerializeObject(DT_Header),
                    JsonConvert.SerializeObject(DT_Detail),
                    JsonConvert.SerializeObject(DT_Volume),
                    JsonConvert.SerializeObject(DT_AdjustOrder_Trip),
                    JsonConvert.SerializeObject(DT_Actual_Receive),
                };

                return ListData;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<List<string>> Detail_Data(int intRow, string F_Supplier_Code)
        {
            try
            {
                int intFC_Max = 0;
                string avgTrip = "";
                string forecastMax = "";

                var sqlParams = new List<SqlParameter>
                {
                    new SqlParameter("@Plant",charStartDate),
                    new SqlParameter("@Supplier_Code",F_Supplier_Code.Split("-")[0]),
                    new SqlParameter("@Supplier_Plant",F_Supplier_Code.Split("-")[1]),
                    new SqlParameter("@Part_No",DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()),
                    new SqlParameter("@Ruibetsu",DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()),
                    new SqlParameter("@Kanban_No",DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim().Substring(1,3)),
                    new SqlParameter("@Store_Code",DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()),
                    new SqlParameter("@Date",DateDelivery),
                };

                var DT = await _FillDT.ExecuteStoreSQLAsync("sp_getForecastMax", sqlParams.ToArray());

                if (DT.Rows.Count > 0 && !string.IsNullOrWhiteSpace(DT.Rows[0]["ForecastMax"].ToString().Trim()))
                {
                    intFC_Max = int.TryParse(DT.Rows[0]["ForecastMax"].ToString().Trim(), out int intResult) ? intResult : 0;
                    forecastMax = intFC_Max.ToString();
                }
                else
                {
                    forecastMax = "0";
                }

                string ppmCon = _FillDT.ppmConnect();

                string sqlQuery = $@"SELECT DISTINCT RTRIM(C.F_Part_nm) AS F_Part_nm, RTRIM(S.F_short_name) AS F_short_name, P.F_Address 
                    FROM {ppmCon}.[dbo].[T_Construction] C INNER JOIN {ppmCon}.[dbo].[T_Supplier_ms] S 
                    ON C.F_supplier_cd = S.F_supplier_cd 
                    AND C.F_plant =  S.F_Plant_cd 
                    AND C.F_Store_cd = S.F_Store_cd 
                    LEFT JOIN TB_MS_Kanban P 
                    ON C.F_supplier_cd = P.F_Supplier_Code collate THAI_CS_AS 
                    AND C.F_plant = P.F_Supplier_Plant collate THAI_CS_AS 
                    AND C.F_Store_cd = P.F_Store_Code collate THAI_CS_AS 
                    AND RIGHT('0000'+C.F_Sebango,4) = P.F_Kanban_No collate THAI_CS_AS 
                    AND C.F_Part_no = P.F_Part_No collate THAI_CS_AS 
                    AND C.F_Ruibetsu = P.F_Ruibetsu collate THAI_CS_AS 
                    WHERE C.F_Part_no = '{DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()}' 
                    AND C.F_Ruibetsu = '{DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()}' 
                    AND C.F_Store_cd = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}' 
                    AND C.F_supplier_cd = '{DT_PartControl.Rows[intRow]["F_Supplier_Code"].ToString().Trim()}' 
                    AND C.F_plant = '{DT_PartControl.Rows[intRow]["F_Supplier_Plant"].ToString().Trim()}' 
                    AND C.F_Sebango = '{DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim().Substring(1, 3)}' 
                    AND C.F_Local_Str <= convert(char(8),getdate(),112) 
                    AND C.F_Local_End >= convert(char(8),getdate(),112) 
                    AND S.F_TC_Str <= convert(char(8),getdate(),112) 
                    AND S.F_TC_End >= convert(char(8),getdate(),112) ";

                var _dt = await _FillDT.ExecuteSQLAsync(sqlQuery);

                string partName = _dt.Rows[0]["F_Part_nm"].ToString().Trim();
                string supName = _dt.Rows[0]["F_short_name"].ToString().Trim();
                string lineName = _dt.Rows[0]["F_Address"].ToString().Trim();

                var stMaxArea = await _kbContext.TB_MS_MaxArea_Stock.AsNoTracking()
                    .Where(x => x.F_Plant == _BearerClass.Plant
                    && x.F_Supplier_Cd == DT_PartControl.Rows[intRow]["F_Supplier_Code"].ToString().Trim()
                    && x.F_Supplier_Plant == DT_PartControl.Rows[intRow]["F_Supplier_Plant"].ToString().Trim()
                    && x.F_Part_No == DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()
                    && x.F_Ruibetsu == DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()
                    && x.F_Store_Cd == DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()
                    && x.F_Kanban_No == DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim()
                    ).FirstOrDefaultAsync();

                string maxArea = stMaxArea?.F_Max_Area == null ? "0" : stMaxArea.F_Max_Area.ToString();

                _dt = await _FillDT.ExecuteStoreSQLAsync("sp_getSTD_B", sqlParams.ToArray());

                string STD_B = Math.Round(decimal.Parse(_dt.Rows[0]["STD_B"].ToString())).ToString();
                string Safety_Stock = _dt.Rows[0]["Safety_Stock"].ToString();

                var result = new List<string>
                {
                    lineName,
                    partName,
                    supName,
                    maxArea,
                };

                return result;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
