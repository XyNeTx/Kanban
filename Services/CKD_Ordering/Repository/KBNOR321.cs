using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.NewFolder;
using KANBAN.Models.KB3.OrderingProcess;
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

        public async Task<List<DataTable>> Onload(string _loginDate)
        {
            try
            {
                DateLogin = DateTime.ParseExact(_loginDate.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                ShiftLogin = _loginDate.Substring(10, 1);

                if (ShiftLogin == "D")
                {
                    DateLogin = new DateTime(DateLogin.Year, DateLogin.Month, DateLogin.Day, 7, 30, 0);
                }
                else
                {
                    DateLogin = new DateTime(DateLogin.Year, DateLogin.Month, DateLogin.Day, 19, 30, 0);
                }

                var sqlParams = new List<SqlParameter>
                {
                    new SqlParameter("@Plant",_BearerClass.Plant),
                    new SqlParameter("@getDate",DateLogin)
                };

                var dt = await _FillDT.ExecuteStoreSQLAsync("[CKD_Inhouse].[sp_getProcessDateTime]", sqlParams.ToArray());

                string sql = $@"SELECT F_value2, F_Value3
                    FROM TB_MS_Parameter
                    WHERE F_code ='CI_CKD'";

                var dtParam = await _FillDT.ExecuteSQLAsync(sql);

                return new List<DataTable> { dt, dtParam };

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
        private async Task Set_All_Data(string action, string F_Supplier_Code, string? F_KanbanFrom, int? intRow, string? F_KanbanTo, string? F_StoreFrom, string? F_StoreTo, string? F_PartFrom, string? F_PartTo)
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
                        await get_startDate(action, intRow);
                    }
                }

                if (intRow != null)
                {
                    sqlParam = new List<SqlParameter>
                    {
                        new SqlParameter("@StartDate",charStartDate),
                        new SqlParameter("@EndDate",charEndDate),
                        new SqlParameter("@Supplier_Code",F_Supplier_Code.Split("-")[0]),
                        new SqlParameter("@Supplier_Plant",F_Supplier_Code.Split("-")[1]),
                        new SqlParameter("@Part_No_FROM",DT_PartControl.Rows[intRow ?? 0]["F_Part_No"].ToString().Trim()),
                        new SqlParameter("@Part_No_TO",DT_PartControl.Rows[intRow ?? 0]["F_Part_No"].ToString().Trim()),
                        new SqlParameter("@Ruibetsu_FROM",DT_PartControl.Rows[intRow ?? 0]["F_Ruibetsu"].ToString().Trim()),
                        new SqlParameter("@Ruibetsu_TO",DT_PartControl.Rows[intRow ?? 0]["F_Ruibetsu"].ToString().Trim()),
                        new SqlParameter("@Store_Code_FROM",DT_PartControl.Rows[intRow ?? 0]["F_Store_Code"].ToString().Trim()),
                        new SqlParameter("@Store_Code_TO",DT_PartControl.Rows[intRow ?? 0]["F_Store_Code"].ToString().Trim()),
                        new SqlParameter("@Kanban_No_FROM",DT_PartControl.Rows[intRow ?? 0]["F_Kanban_No"].ToString().Trim()),
                        new SqlParameter("@Kanban_No_TO",DT_PartControl.Rows[intRow ?? 0]["F_Kanban_No"].ToString().Trim()),
                    };
                };

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

                sqlParam = new List<SqlParameter>
                    {
                        new SqlParameter("@StartDate",charStartDate),
                        new SqlParameter("@EndDate",charEndDate),
                        new SqlParameter("@Supplier_Code",F_Supplier_Code.Split("-")[0]),
                        new SqlParameter("@Supplier_Plant",F_Supplier_Code.Split("-")[1]),
                        new SqlParameter("@Part_No_FROM",DT_PartControl.Rows[intRow ?? 0]["F_Part_No"].ToString().Trim()),
                        new SqlParameter("@Part_No_TO",DT_PartControl.Rows[intRow ?? 0]["F_Part_No"].ToString().Trim()),
                        new SqlParameter("@Ruibetsu_FROM",DT_PartControl.Rows[intRow ?? 0]["F_Ruibetsu"].ToString().Trim()),
                        new SqlParameter("@Ruibetsu_TO",DT_PartControl.Rows[intRow ?? 0]["F_Ruibetsu"].ToString().Trim()),
                        new SqlParameter("@Store_Code_FROM",DT_PartControl.Rows[intRow ?? 0]["F_Store_Code"].ToString().Trim()),
                        new SqlParameter("@Store_Code_TO",DT_PartControl.Rows[intRow ?? 0]["F_Store_Code"].ToString().Trim()),
                    };

                DT_Actual_Receive = await _FillDT.ExecuteStoreSQLAsync("sp_DT_Actual_Receive", sqlParam.ToArray());

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<List<string>> Get_All_Data(string action, string F_Supplier_Code, int? intRow, string? F_KanbanFrom, string? F_KanbanTo, string? F_StoreFrom, string? F_StoreTo, string? F_PartFrom, string? F_PartTo)
        {
            try
            {
                await Find_StartEnd_Date(action, F_Supplier_Code);
                await Set_All_Data(action, F_Supplier_Code, F_KanbanFrom, intRow, F_KanbanTo, F_StoreFrom, F_StoreTo, F_PartFrom, F_PartTo);

                //var Filtered_DT_Header = DT_Header.Select($@"F_Part_No == '{DT_PartControl.Rows[intRow ?? 0]["F_Part_No"].ToString().Trim()}'
                //    && F_Ruibetsu == '{DT_PartControl.Rows[intRow ?? 0]["F_Ruibetsu"].ToString().Trim()}'
                //    && F_Supplier_Code == '{DT_PartControl.Rows[intRow ?? 0]["F_Supplier_Code"].ToString().Trim()}'
                //    && F_Supplier_Plant == '{DT_PartControl.Rows[intRow ?? 0]["F_Supplier_Plant"].ToString().Trim()}'
                //    && F_Store_Code == '{DT_PartControl.Rows[intRow ?? 0]["F_Store_Code"].ToString().Trim()}'
                //    && F_Kanban_No == '{DT_PartControl.Rows[intRow ?? 0]["F_Kanban_No"].ToString().Trim()}'
                //");

                //var Filtered_DT_Detail = DT_Detail.Select($@"F_Part_No == '{DT_PartControl.Rows[intRow ?? 0]["F_Part_No"].ToString().Trim()}'
                //    && F_Ruibetsu == '{DT_PartControl.Rows[intRow ?? 0]["F_Ruibetsu"].ToString().Trim()}'
                //    && F_Supplier_Code == '{DT_PartControl.Rows[intRow ?? 0]["F_Supplier_Code"].ToString().Trim()}'
                //    && F_Supplier_Plant == '{DT_PartControl.Rows[intRow ?? 0]["F_Supplier_Plant"].ToString().Trim()}'
                //    && F_Store_Code == '{DT_PartControl.Rows[intRow ?? 0]["F_Store_Code"].ToString().Trim()}'
                //    && F_Kanban_No == '{DT_PartControl.Rows[intRow ?? 0]["F_Kanban_No"].ToString().Trim()}'
                //");

                //var Filtered_DT_Volume = DT_Volume.Select($@"F_Part_No == '{DT_PartControl.Rows[intRow ?? 0]["F_Part_No"].ToString().Trim()}'
                //    && F_Ruibetsu == '{DT_PartControl.Rows[intRow ?? 0]["F_Ruibetsu"].ToString().Trim()}'
                //    && F_Supplier_Code == '{DT_PartControl.Rows[intRow ?? 0]["F_Supplier_Code"].ToString().Trim()}'
                //    && F_Supplier_Plant == '{DT_PartControl.Rows[intRow ?? 0]["F_Supplier_Plant"].ToString().Trim()}'
                //    && F_Store_Code == '{DT_PartControl.Rows[intRow ?? 0]["F_Store_Code"].ToString().Trim()}'
                //    && F_Kanban_No == '{DT_PartControl.Rows[intRow ?? 0]["F_Kanban_No"].ToString().Trim()}'
                //");

                //var Filtered_DT_AdjustOrder_Trip = DT_AdjustOrder_Trip.Select($@"F_Part_No == '{DT_PartControl.Rows[intRow ?? 0]["F_Part_No"].ToString().Trim()}'
                //    && F_Ruibetsu == '{DT_PartControl.Rows[intRow ?? 0]["F_Ruibetsu"].ToString().Trim()}'
                //    && F_Supplier_Code == '{DT_PartControl.Rows[intRow ?? 0]["F_Supplier_Code"].ToString().Trim()}'
                //    && F_Supplier_Plant == '{DT_PartControl.Rows[intRow ?? 0]["F_Supplier_Plant"].ToString().Trim()}'
                //    && F_Store_Code == '{DT_PartControl.Rows[intRow ?? 0]["F_Store_Code"].ToString().Trim()}'
                //    && F_Kanban_No == '{DT_PartControl.Rows[intRow ?? 0]["F_Kanban_No"].ToString().Trim()}'
                //");

                //var Filtered_DT_Actual_Receive = DT_Actual_Receive.Select($@"F_PART_No == '{DT_PartControl.Rows[intRow ?? 0]["F_Part_No"].ToString().Trim()}'
                //    && F_RUibetsu == '{DT_PartControl.Rows[intRow ?? 0]["F_Ruibetsu"].ToString().Trim()}'
                //    && F_Supplier_code == '{DT_PartControl.Rows[intRow ?? 0]["F_Supplier_Code"].ToString().Trim()}'
                //    && F_Supplier_Plant == '{DT_PartControl.Rows[intRow ?? 0]["F_Supplier_Plant"].ToString().Trim()}'
                //    && F_Store_Code == '{DT_PartControl.Rows[intRow ?? 0]["F_Store_Code"].ToString().Trim()}'
                //    && F_Kanban_No == '{DT_PartControl.Rows[intRow ?? 0]["F_Kanban_No"].ToString().Trim()}'
                //");

                //var ListData = new List<string>
                //{
                //    JsonConvert.SerializeObject(DT_DeliveryDate),
                //    JsonConvert.SerializeObject(DT_Date),
                //    JsonConvert.SerializeObject(DT_Period),
                //    JsonConvert.SerializeObject(DT_PartControl),
                //    JsonConvert.SerializeObject(Filtered_DT_Header),
                //    JsonConvert.SerializeObject(Filtered_DT_Detail),
                //    JsonConvert.SerializeObject(Filtered_DT_Volume),
                //    JsonConvert.SerializeObject(Filtered_DT_AdjustOrder_Trip),
                //    JsonConvert.SerializeObject(DT_Actual_Receive),
                //};

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

        public async Task Recalculate(string action, string F_Supplier_Code, int? intRow)
        {
            try
            {
                await Find_StartEnd_Date(action, F_Supplier_Code);
                await Set_All_Data(action, F_Supplier_Code, null, intRow, null, null, null, null, null);
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
                    new SqlParameter("@Plant",_BearerClass.Plant),
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

                string STD_B = decimal.TryParse(_dt.Rows[0]["STD_B"].ToString(), null, out decimal result1) ? Math.Round(result1).ToString() : 0m.ToString();
                string Safety_Stock = _dt.Rows[0]["Safety_Stock"].ToString();

                _dt = await _FillDT.ExecuteStoreSQLAsync("sp_getSTDStock", sqlParams.ToArray());
                string STD_Stock = decimal.TryParse(_dt.Rows[0]["STDStock"].ToString(), null, out decimal result2) ? Math.Round(result2).ToString() : 0m.ToString();

                _dt = await _FillDT.ExecuteStoreSQLAsync("sp_getMinStock", sqlParams.ToArray());
                string Min_Stock = decimal.TryParse(_dt.Rows[0]["Min_Stock"].ToString(), null, out decimal result3) ? Math.Round(result3).ToString() : 0m.ToString();

                string sql = $@"SELECT SUM(F_KB_CUT) AS F_KB_CUT, SUM(F_KB_ADD) AS F_KB_ADD, F_NON_STOP
                    FROM TB_Calculate_D_CKD
                    WHERE F_Supplier_Code = '{F_Supplier_Code.Split("-")[0]}' 
                    AND F_Supplier_Plant = '{F_Supplier_Code.Split("-")[1]}'
                    AND F_Store_Code = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}'
                    AND F_Kanban_No = '{DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim().Substring(1, 3)}'
                    AND F_Part_No = '{DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()}'
                    AND F_Ruibetsu = '{DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()}'
                    AND F_Process_Date = '{DateLogin}'
                    AND F_Process_Shift = '{ShiftLogin}'
                    GROUP BY F_Supplier_Code, F_Supplier_Plant, F_Store_Code, F_Part_No, F_Ruibetsu,
                    F_Kanban_No, F_Process_Date, F_Process_Shift, F_NON_STOP
                    ";

                _dt = await _FillDT.ExecuteSQLAsync(sql);
                string F_Non_Stop = "0";
                string F_KB_CUT = "0";
                string F_KB_ADD = "0";

                if (_dt.Rows.Count > 0)
                {
                    F_Non_Stop = _dt.Rows[0]["F_NON_STOP"].ToString().Trim();
                    F_KB_CUT = _dt.Rows[0]["F_KB_CUT"].ToString().Trim();
                    F_KB_ADD = _dt.Rows[0]["F_KB_ADD"].ToString().Trim();
                }

                sql = $@"SELECT * FROM TB_Kanban_SetOrder
                    WHERE F_Plant = '{_BearerClass.Plant}'
                    AND F_Supplier_Code = '{F_Supplier_Code.Split("-")[0]}'
                    AND F_Supplier_Plant = '{F_Supplier_Code.Split("-")[1]}'
                    AND F_Part_No = '{DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()}'
                    AND F_Ruibetsu = '{DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()}'
                    AND F_Kanban_No = '{DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim()}'
                    AND F_Store_Cd = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}'
                    AND CONVERT(INT,F_Trip1)+CONVERT(INT,F_Trip2)+CONVERT(INT,F_Trip3)+CONVERT(INT,F_Trip4)
                    +CONVERT(INT,F_Trip5)+CONVERT(INT,F_Trip6)+CONVERT(INT,F_Trip7)+CONVERT(INT,F_Trip8)
                    +CONVERT(INT,F_Trip9)+CONVERT(INT,F_Trip10)+CONVERT(INT,F_Trip11)+CONVERT(INT,F_Trip12)
                    +CONVERT(INT,F_Trip13)+CONVERT(INT,F_Trip14)+CONVERT(INT,F_Trip15)+CONVERT(INT,F_Trip16)
                    +CONVERT(INT,F_Trip17)+CONVERT(INT,F_Trip18)+CONVERT(INT,F_Trip19)+CONVERT(INT,F_Trip20)
                    +CONVERT(INT,F_Trip21)+CONVERT(INT,F_Trip22)+CONVERT(INT,F_Trip23)+CONVERT(INT,F_Trip24) > 0";

                _dt = await _FillDT.ExecuteSQLAsync(sql);
                bool _chkBoxKanbanOrder = false;
                bool _mrpLess20 = false;
                bool _mrpMore20 = false;

                if (_dt.Rows.Count > 0)
                {
                    _chkBoxKanbanOrder = true;
                }

                var dbTB_Cal_D_CKD = await _kbContext.TB_Calculate_D_CKD.AsNoTracking()
                    .Where(x => x.F_Supplier_Code == F_Supplier_Code.Substring(0, 4)
                    && x.F_Supplier_Plant == F_Supplier_Code.Substring(5, 1)
                    && x.F_Part_No == DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()
                    && x.F_Ruibetsu == DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()
                    && x.F_Store_Code == DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()
                    && x.F_Kanban_No == DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim()
                    && x.F_Process_Date == DateLogin.ToString("yyyyMMdd")
                    && x.F_Process_Shift == ShiftLogin
                    ).FirstOrDefaultAsync();

                if (dbTB_Cal_D_CKD != null)
                {

                    if ((dbTB_Cal_D_CKD.F_TMT_FO * 0.8) > dbTB_Cal_D_CKD.F_MRP)
                    {
                        _mrpLess20 = true;
                    }
                    else if ((dbTB_Cal_D_CKD.F_TMT_FO * 1.2) < dbTB_Cal_D_CKD.F_MRP)
                    {
                        _mrpMore20 = true;
                    }
                }

                sql = $@"SELECT A.Slide_Order + B.Slide_Order_Part AS SliceOrder
                    FROM 
                    (   SELECT COUNT(*) AS Slide_Order 
                        FROM TB_Slide_Order
                        WHERE F_Plant = '{_BearerClass.Plant}'
                        AND F_Supplier_CD = '{F_Supplier_Code.Split("-")[0]}'
                        AND F_Supplier_Plant = '{F_Supplier_Code.Split("-")[1]}'
                        AND F_Store_CD = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}'
                        AND F_Delivery_Date = '{DateDelivery.ToString("yyyyMMdd").Trim()}'
                        AND F_Delivery_Trip = {intDeliveryTrip}
                    ) A CROSS JOIN
                    (   SELECT COUNT(*) AS Slide_Order_Part
                        FROM TB_Slide_Order_Part
                        WHERE F_Plant = '{_BearerClass.Plant}'
                        AND F_Supplier_CD = '{F_Supplier_Code.Split("-")[0]}'
                        AND F_Supplier_Plant = '{F_Supplier_Code.Split("-")[1]}'
                        AND F_Store_CD = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}'
                        AND F_Part_No = '{DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()}'
                        AND F_Ruibetsu = '{DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()}'
                        AND F_Delivery_Date = '{DateDelivery.ToString("yyyyMMdd").Trim()}'
                        AND F_Delivery_Trip = {intDeliveryTrip}
                    ) B
                    WHERE A.Slide_Order + B.Slide_Order_Part > 0
                    ";

                _dt = await _FillDT.ExecuteSQLAsync(sql);
                bool _chkBoxSlideOrder = false;

                if (_dt.Rows.Count > 0)
                {
                    _chkBoxSlideOrder = true;
                }

                sql = $@"SELECT A.Rec_Slide_Order + B.Rec_Slide_Order_Part AS SliceOrder
                    FROM
                    (   SELECT COUNT(*) AS Rec_Slide_Order
                    FROM TB_Slide_Order
                    WHERE F_Plant = '{_BearerClass.Plant}'
                    AND F_Supplier_CD = '{F_Supplier_Code.Split("-")[0]}'
                    AND F_Supplier_Plant = '{F_Supplier_Code.Split("-")[1]}'
                    AND F_Store_CD = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}'
                    AND F_Slide_Date = '{DateDelivery.ToString("yyyyMMdd").Trim()}'
                    AND F_Slide_Trip = {intDeliveryTrip}
                    ) A CROSS JOIN
                    (   SELECT COUNT(*) AS Rec_Slide_Order_Part
                        FROM TB_Slide_Order_Part
                        WHERE F_Plant = '{_BearerClass.Plant}'
                        AND F_Supplier_CD = '{F_Supplier_Code.Split("-")[0]}'
                        AND F_Supplier_Plant = '{F_Supplier_Code.Split("-")[1]}'
                        AND F_Store_CD = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}'
                        AND F_Part_No = '{DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()}'
                        AND F_Ruibetsu = '{DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()}'
                        AND F_Slide_Date = '{DateDelivery.ToString("yyyyMMdd").Trim()}'
                        AND F_Slide_Trip = {intDeliveryTrip}
                    ) B
                    WHERE A.Rec_Slide_Order + B.Rec_Slide_Order_Part > 0
                    ";

                _dt = await _FillDT.ExecuteSQLAsync(sql);
                bool _chkBoxRecSlideOrder = false;

                if (_dt.Rows.Count > 0)
                {
                    _chkBoxRecSlideOrder = true;
                }

                sql = $@"SELECT * 
                    FROM( 
                        SELECT DISTINCT F_Supplier_Code, F_Supplier_Plant, F_Start_Order_Date AS F_Start_Date, F_Start_Date AS F_End_Date 
                        FROM TB_MS_DeliveryTime 
                        WHERE F_Start_Date <> F_Start_Order_Date
                        AND F_Supplier_Code = '{F_Supplier_Code.Split("-")[0]}'
                        AND F_Supplier_Plant = '{F_Supplier_Code.Split("-")[1]}'
                        AND F_Start_Order_Date <> '') A
                        WHERE A.F_Start_Date <= '{DateLogin.ToString("yyyyMMdd")}'
                        AND A.F_End_Date >= '{DateLogin.ToString("yyyyMMdd")}'
                        ";

                _dt = await _FillDT.ExecuteSQLAsync(sql);
                bool _chkBoxDeliveryTime = false;

                if (_dt.Rows.Count > 0)
                {
                    _chkBoxDeliveryTime = true;
                }

                var result = new List<string>
                {
                    lineName,
                    partName,
                    supName,
                    maxArea,
                    STD_B,
                    Safety_Stock,
                    forecastMax,
                    STD_Stock,
                    Min_Stock,
                    F_Non_Stop,
                    F_KB_ADD,
                    F_KB_CUT,
                    _mrpLess20.ToString(),
                    _mrpMore20.ToString(),
                    _chkBoxKanbanOrder.ToString(),
                    _chkBoxSlideOrder.ToString(),
                    _chkBoxRecSlideOrder.ToString(),
                    _chkBoxDeliveryTime.ToString(),
                };

                return result;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<List<string>> GetBL(string strDate, string Row_Num, int intRow)
        {
            try
            {
                int intAbnormal = 0;
                string strCycle = DT_Date.Rows[DT_Date.Rows.Count - 1]["F_Cycle"].ToString().Trim();
                int intCycleB = int.Parse(strCycle.Substring(2, 2));

                DateTime _Date = DateTime.ParseExact(strDate, "yyyyMMdd", null);
                List<SqlParameter> sqlParams = new List<SqlParameter>
                {
                    new SqlParameter("@Supplier_Code", DT_PartControl.Rows[intRow]["F_Supplier_Code"].ToString()),
                    new SqlParameter("@Supplier_Plant", DT_PartControl.Rows[intRow]["F_Supplier_Plant"].ToString()),
                    new SqlParameter("@Part_No", DT_PartControl.Rows[intRow]["F_Part_No"].ToString()),
                    new SqlParameter("@Ruibetsu", DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString()),
                    new SqlParameter("@Kanban_No", DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString()),
                    new SqlParameter("@Store_Code", DT_PartControl.Rows[intRow]["F_Store_Code"].ToString()),
                };

                if (Row_Num == "2")
                {
                    sqlParams.Add(new SqlParameter("@Date", _Date.ToString("yyyyMMdd")));
                    sqlParams.Add(new SqlParameter("@Shift", "D"));
                }
                else
                {
                    sqlParams.Add(new SqlParameter("@Date", _Date.AddDays(-1).ToString("yyyyMMdd")));
                    sqlParams.Add(new SqlParameter("@Shift", "N"));
                }

                var DT_Last = await _FillDT.ExecuteStoreSQLAsync("[CKD_Inhouse].sp_autoRecalculateBL_First", sqlParams.ToArray());

                if (DT_Last.Rows.Count > 0)
                {
                    List<string> result = new List<string>
                    {
                        (DT_Last.Rows[0]["F_BL_SET_Plan"].ToString().Trim()),
                        (DT_Last.Rows[0]["F_BL_SET_Actual"].ToString().Trim()),
                        (DT_Last.Rows[0]["F_Not_Recalculate"].ToString().Trim()),
                    };

                    return result;
                }
                else
                {
                    List<string> result = new List<string>
                    {
                        "0",
                        "0",
                        "0",
                    };

                    return result;
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<TB_MS_Inform_News> GetInformNews(string F_Supplier_Code, string F_Kanban, string F_Store, string F_Part)
        {
            try
            {
                var dbNews = await _kbContext.TB_MS_Inform_News.AsNoTracking()
                    .Where(x => x.F_Supplier_Code + "-" + x.F_Supplier_Plant == F_Supplier_Code
                    && x.F_Store_Code == F_Store && x.F_Kanban_No.Trim() == F_Kanban
                    && x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == F_Part).FirstOrDefaultAsync();

                if (dbNews == null)
                {
                    throw new CustomHttpException(500, "Inform News Not Found");
                }

                return dbNews;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task get_startDate(string action, int? intRun)
        {
            try
            {
                if (action.ToLower() == "re-calculate bl")
                {
                    //for (int rowIndex = 0; rowIndex < DT_PartControl.Rows.Count; rowIndex++)
                    //{
                    //    await set_startDate(rowIndex);
                    //}
                    await set_startDate(intRun ?? 0);
                }
                else if (action.ToLower() == "re-calculate")
                {
                    await set_startDate(intRun ?? 0);
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException.Message ?? ex.Message);
            }
        }

        public async Task set_startDate(int RowIndex)
        {
            try
            {
                string start_Date; string end_Date;

                var dbCalD = await _kbContext.TB_Calculate_D_CKD.AsNoTracking()
                    .Where(x => x.F_Part_No == DT_PartControl.Rows[RowIndex]["F_Part_No"].ToString().Trim()
                    && x.F_Ruibetsu == DT_PartControl.Rows[RowIndex]["F_Ruibetsu"].ToString().Trim()
                    && x.F_Supplier_Code == DT_PartControl.Rows[RowIndex]["F_Supplier_Code"].ToString().Trim()
                    && x.F_Supplier_Plant == DT_PartControl.Rows[RowIndex]["F_Supplier_Plant"].ToString().Trim()
                    && x.F_Store_Code == DT_PartControl.Rows[RowIndex]["F_Store_Code"].ToString().Trim()
                    && x.F_Kanban_No == DT_PartControl.Rows[RowIndex]["F_Kanban_No"].ToString().Trim()
                    && x.Flag_Chg_BL_Stock == true
                    ).OrderBy(x => x.F_Process_Date).FirstOrDefaultAsync();

                if (dbCalD != null)
                {
                    start_Date = dbCalD.F_Process_Date;
                }
                else
                {
                    start_Date = DateTime.Now.ToString("yyyyMMdd");
                }

                string Dynamic_Store = _BearerClass.Plant switch
                {
                    "1" => "1A",
                    "2" => "2B",
                    "3" => "3C",
                    _ => "3C"
                };

                var sqlParams = new List<SqlParameter>
                {
                    new SqlParameter("@Plant",_BearerClass.Plant),
                    new SqlParameter("@Supplier_Code",DT_PartControl.Rows[RowIndex]["F_Supplier_Code"].ToString().Trim()),
                    new SqlParameter("@Supplier_Plant",DT_PartControl.Rows[RowIndex]["F_Supplier_Plant"].ToString().Trim()),
                    new SqlParameter("@Store_Code",Dynamic_Store),
                    new SqlParameter("@Date",DateTime.Now.ToString("yyyyMMdd")),
                };

                var dt = await _FillDT.ExecuteStoreSQLAsync("sp_NumberOfDayToPreview", sqlParams.ToArray());

                if (start_Date == DateTime.Now.ToString("yyyyMMdd"))
                {
                    start_Date = dt.Rows[0]["Start_Date"].ToString().Trim();
                }
                end_Date = dt.Rows[0]["End_Date"].ToString().Trim();

                await re_Calculate_Trail(start_Date, end_Date, RowIndex);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException.Message ?? ex.Message);
            }
        }

        public async Task re_Calculate_Trail(string start_Date, string end_Date, int RowIndex)
        {
            try
            {
                int Last_BL_Plan = 0; int Last_BL_Actual = 0;
                bool blnFromSetStock;
                DateECI dateECI = await get_ECIDate(start_Date, end_Date, RowIndex);
                var dateLast_Trip = DateTime.ParseExact(start_Date, "yyyyMMdd", CultureInfo.InvariantCulture);

                var sqlParams = new List<SqlParameter>
                {
                    new SqlParameter("@Date",dateLast_Trip.AddDays(-1).ToString("yyyyMMdd")),
                    new SqlParameter("@Supplier_Code",DT_PartControl.Rows[RowIndex]["F_Supplier_Code"].ToString().Trim()),
                    new SqlParameter("@Supplier_Plant",DT_PartControl.Rows[RowIndex]["F_Supplier_Plant"].ToString().Trim()),
                    new SqlParameter("@Part_No",DT_PartControl.Rows[RowIndex]["F_Part_No"].ToString().Trim()),
                    new SqlParameter("@Ruibetsu",DT_PartControl.Rows[RowIndex]["F_Ruibetsu"].ToString().Trim()),
                    new SqlParameter("@Kanban_No",DT_PartControl.Rows[RowIndex]["F_Kanban_No"].ToString().Trim()),
                    new SqlParameter("@Store_Code",DT_PartControl.Rows[RowIndex]["F_Store_Code"].ToString().Trim()),
                };

                var _DT = await _FillDT.ExecuteStoreSQLAsync("[CKD_Inhouse].sp_autoRecalculateBL_First", sqlParams.ToArray());

                if (_DT.Rows.Count > 0)
                {
                    Last_BL_Plan = int.Parse(_DT.Rows[0]["F_BL_SET_Plan"].ToString().Trim());
                    Last_BL_Actual = int.Parse(_DT.Rows[0]["F_BL_SET_Actual"].ToString().Trim());
                    blnFromSetStock = bool.Parse(_DT.Rows[0]["F_Not_Recalculate"].ToString().Trim());
                }
                else
                {
                    Last_BL_Plan = 0;
                    Last_BL_Actual = 0;
                    blnFromSetStock = false;
                }

                sqlParams = new List<SqlParameter>
                {
                    new SqlParameter("@StartDate",dateLast_Trip.ToString("yyyyMMdd")),
                    new SqlParameter("@EndDate",end_Date),
                    new SqlParameter("@Supplier_Code",DT_PartControl.Rows[RowIndex]["F_Supplier_Code"].ToString().Trim()),
                    new SqlParameter("@Supplier_Plant",DT_PartControl.Rows[RowIndex]["F_Supplier_Plant"].ToString().Trim()),
                    new SqlParameter("@Part_No",DT_PartControl.Rows[RowIndex]["F_Part_No"].ToString().Trim()),
                    new SqlParameter("@Ruibetsu",DT_PartControl.Rows[RowIndex]["F_Ruibetsu"].ToString().Trim()),
                    new SqlParameter("@Kanban_No",DT_PartControl.Rows[RowIndex]["F_Kanban_No"].ToString().Trim()),
                    new SqlParameter("@Store_Code",DT_PartControl.Rows[RowIndex]["F_Store_Code"].ToString().Trim()),
                };

                var DT = await _FillDT.ExecuteStoreSQLAsync("[CKD_Inhouse].sp_autoRecalculateBL_Second", sqlParams.ToArray());
                var DT_Actual = await _FillDT.ExecuteStoreSQLAsync("[CKD_Inhouse].sp_autoRecalculateBL_Third", sqlParams.ToArray());
                var DT_Adjust = await _FillDT.ExecuteStoreSQLAsync("[CKD_Inhouse].sp_autoRecalculateBL_Fourth", sqlParams.ToArray());

                if (DT.Rows.Count > 0)
                {
                    using var kbTrans = await _kbContext.Database.BeginTransactionAsync();
                    try
                    {
                        int InRec = 0; int InActual = 0;
                        int BlPlan = 0; int BlActual = 0;
                        DateTime dateDelivery = new DateTime();
                        string BLPlan_Solution, BLActual_Solution = "";
                        //DataRow DR_Receive = null;

                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            BLPlan_Solution = ""; BLActual_Solution = "";

                            if (i > 0)
                            {
                                if (DT.Rows[i]["F_Process_Round"].ToString().Trim() == DT.Rows[i - 1]["F_Process_Round"].ToString().Trim()
                                    && DT.Rows[i]["F_Process_Date"].ToString().Trim() == DT.Rows[i - 1]["F_Process_Date"].ToString().Trim())
                                {
                                    dateDelivery = DateTime.ParseExact(DT.Rows[i]["F_Process_Date"].ToString().Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);

                                    sqlParams = new List<SqlParameter>
                            {
                                new SqlParameter("@Date",dateDelivery.AddDays(-1).ToString("yyyyMMdd")),
                                new SqlParameter("@Supplier_Code",DT_PartControl.Rows[RowIndex]["F_Supplier_Code"].ToString().Trim()),
                                new SqlParameter("@Supplier_Plant",DT_PartControl.Rows[RowIndex]["F_Supplier_Plant"].ToString().Trim()),
                                new SqlParameter("@Part_No",DT_PartControl.Rows[RowIndex]["F_Part_No"].ToString().Trim()),
                                new SqlParameter("@Ruibetsu",DT_PartControl.Rows[RowIndex]["F_Ruibetsu"].ToString().Trim()),
                                new SqlParameter("@Kanban_No",DT_PartControl.Rows[RowIndex]["F_Kanban_No"].ToString().Trim()),
                                new SqlParameter("@Store_Code",DT_PartControl.Rows[RowIndex]["F_Store_Code"].ToString().Trim()),
                            };

                                    var DT_LastBL = await _FillDT.ExecuteStoreSQLAsync("sp_autoRecalculateBL_First", sqlParams.ToArray());

                                    if (DT_LastBL.Rows.Count > 0)
                                    {
                                        Last_BL_Plan = int.Parse(DT_LastBL.Rows[0]["F_BL_SET_Plan"].ToString().Trim());
                                        Last_BL_Actual = int.Parse(DT_LastBL.Rows[0]["F_BL_SET_Actual"].ToString().Trim());
                                    }
                                    else
                                    {
                                        Last_BL_Plan = 0;
                                        Last_BL_Actual = 0;
                                    }
                                }
                            }

                            var DR_Receive = DT_Actual.Select($@"F_Delivery_trip = '{DT.Rows[i]["F_Process_Round"].ToString().Trim()}' 
                                AND F_Receive_date = '{DT.Rows[i]["F_Process_Date"].ToString().Trim()}' 
                                AND F_Supplier_Code = '{DT.Rows[i]["F_Supplier_Code"].ToString().Trim()}' 
                                AND F_Supplier_Plant = '{DT.Rows[i]["F_Supplier_Plant"].ToString().Trim()}' 
                                AND F_Part_No = '{DT.Rows[i]["F_Part_No"].ToString().Trim()}' 
                                AND F_Ruibetsu = '{DT.Rows[i]["F_Ruibetsu"].ToString().Trim()}' 
                                AND F_Store_CD = '{DT.Rows[i]["F_Store_Code"].ToString().Trim()}' ");

                            if (DR_Receive.Length > 0)
                            {
                                InActual = int.Parse(DR_Receive[0]["IN_ACTUAL"].ToString());
                            }
                            else
                            {
                                InActual = 0;
                            }

                            if (DT.Rows[i]["F_Flag_Pattern"].ToString().Trim() == "True")
                            {
                                InRec = int.Parse(DT.Rows[i]["F_Adj_Pattern"].ToString());
                            }
                            else
                            {
                                InRec = int.Parse(DT.Rows[i]["IN_Plan"].ToString());
                            }

                            if (DT.Rows[i]["F_Process_Date"].ToString().CompareTo(dateECI.Begining_Date) < 0)
                            {
                                if (DT.Rows[i]["F_Process_Round"].ToString() == "1")
                                {
                                    if (blnFromSetStock)
                                    {
                                        BLPlan_Solution = "BL = ( BF + In(Rec) ) + Urgent \n";
                                        BlPlan = (Last_BL_Plan + InRec) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                        BLPlan_Solution = $@"{BLPlan_Solution} BLPlan : {BlPlan.ToString()}
                                    = ({Last_BL_Plan.ToString()} + {InRec.ToString()}) + {DT.Rows[i]["F_Urgent_Order"].ToString()}";

                                        BlActual = (Last_BL_Actual + InActual);
                                        BLActual_Solution = $@"BLActual : {BlActual} = ({Last_BL_Actual} + {InActual})";

                                        if (DT.Rows[i]["Flag_HalfChg_BL_Stock"].ToString() == "False")
                                        {
                                            BLPlan_Solution = "BL = (BF + In(Rec)) - MRP + Urgent \n";
                                            BlPlan = (Last_BL_Plan + InRec) - int.Parse(DT.Rows[i]["F_MRP"].ToString()) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                            BLPlan_Solution = $@"{BLPlan_Solution} BLPlan : {BlPlan} = ({Last_BL_Plan} + {InRec}) - 
                                    {DT.Rows[i]["F_MRP"].ToString()} + {DT.Rows[i]["F_Urgent_Order"].ToString()}";

                                            BlActual = (Last_BL_Actual + InActual) - int.Parse(DT.Rows[i]["F_MRP"].ToString());
                                            BLActual_Solution = $@"BLActual : {BlActual} = ({Last_BL_Actual} + {InActual}) - {DT.Rows[i]["F_MRP"].ToString()}";
                                        }
                                        else if (DT.Rows[i]["Flag_HalfChg_BL_Stock"].ToString() == "True")
                                        {
                                            if (DT.Rows[i]["F_Process_Shift"].ToString().Trim().ToUpper() == "D")
                                            {
                                                BLPlan_Solution = "BL = BF - MRP/2 \n";
                                                BlPlan = Last_BL_Plan - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2);
                                                BLPlan_Solution = $@"{BLPlan_Solution} BLPlan : {BlPlan} = {Last_BL_Plan} - ({int.Parse(DT.Rows[i]["F_MRP"].ToString())}/2)";

                                                BlActual = Last_BL_Actual - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2);
                                                BLActual_Solution = $@"BLActual : {BlActual} = {Last_BL_Actual} - ({int.Parse(DT.Rows[i]["F_MRP"].ToString())}/2)";
                                            }
                                            else
                                            {
                                                BLPlan_Solution = "BL = (BF + In(Rec)) - MRP/2 + Urgent \n";
                                                BlPlan = (Last_BL_Plan + InRec) - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                                BLPlan_Solution = $@"{BLPlan_Solution} BLPlan : {BlPlan} = ({Last_BL_Plan} + {InRec}) - ({int.Parse(DT.Rows[i]["F_MRP"].ToString())}/2) + {DT.Rows[i]["F_Urgent_Order"].ToString()}";

                                                BlActual = (Last_BL_Plan + InRec) - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                                BLActual_Solution = $@"BLActual : {BlActual} = ({Last_BL_Plan} + {InRec}) - ({DT.Rows[i]["F_MRP"].ToString()}/2) + {DT.Rows[i]["F_Urgent_Order"].ToString()}";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        BLPlan_Solution = "BL = ( BF + In(Rec) ) + Urgent - Abnormal \n";
                                        BlPlan = (Last_BL_Plan + InRec) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString()) - int.Parse(DT.Rows[i]["F_AbNormal_Part"].ToString());
                                        BLPlan_Solution = $@"{BLPlan_Solution} BLPlan : {BlPlan} = ({Last_BL_Plan} + {InRec}) + 
                                {DT.Rows[i]["F_Urgent_Order"].ToString()} - {DT.Rows[i]["F_AbNormal_Part"].ToString()}";

                                        BlActual = (Last_BL_Actual + InActual) - int.Parse(DT.Rows[i]["F_AbNormal_Part"].ToString());
                                        BLActual_Solution = $@"BLActual : {BlActual} = ({Last_BL_Actual} + {InActual}) - {DT.Rows[i]["F_AbNormal_Part"].ToString()}";
                                    }
                                    Last_BL_Plan = BlPlan;
                                    Last_BL_Actual = BlActual;
                                }
                                else
                                {
                                    BLPlan_Solution = "BL = ( BF + In(Rec) ) + Urgent \n";
                                    BlPlan = (Last_BL_Plan + InRec) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                    BLPlan_Solution += $"BLPlan : {BlPlan} = ({Last_BL_Plan} + {InRec}) + {DT.Rows[i]["F_Urgent_Order"].ToString()}";

                                    BlActual = (Last_BL_Actual + InActual);
                                    BLActual_Solution = $"BLActual : {BlActual} = ({Last_BL_Actual} + {InActual})";

                                    Last_BL_Plan = BlPlan;
                                    Last_BL_Actual = BlActual;
                                }
                            }
                            else
                            {
                                if (DT.Rows[i]["F_Process_Round"].ToString() == "1")
                                {
                                    if (blnFromSetStock)
                                    {
                                        if (DT.Rows[i]["Flag_HalfChg_BL_Stock"].ToString() == "False")
                                        {
                                            BLPlan_Solution = "BL = (BF + In(Rec)) - MRP + Urgent \n";
                                            BlPlan = (Last_BL_Plan + InRec) - int.Parse(DT.Rows[i]["F_MRP"].ToString()) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                            BLPlan_Solution += $@"BLPlan : {BlPlan} = ({Last_BL_Plan} + {InRec}) - {DT.Rows[i]["F_MRP"].ToString()} + {DT.Rows[i]["F_Urgent_Order"].ToString()}";

                                            BlActual = (Last_BL_Actual + InActual) - int.Parse(DT.Rows[i]["F_MRP"].ToString());
                                            BLActual_Solution = $@"BLActual : {BlActual} = ({Last_BL_Actual} + {InActual}) - {DT.Rows[i]["F_MRP"].ToString()}";
                                        }
                                        else if (DT.Rows[i]["Flag_HalfChg_BL_Stock"].ToString() == "True")
                                        {
                                            if (DT.Rows[i]["F_Proccess_Shift"].ToString().Trim() == "D")
                                            {
                                                BLPlan_Solution = "BL = BF - MRP/2 \n";
                                                BlPlan = Last_BL_Plan - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2);
                                                BLPlan_Solution += $@"BLPlan : {BlPlan} = {Last_BL_Plan} - ({int.Parse(DT.Rows[i]["F_MRP"].ToString())}/2)";

                                                BlActual = Last_BL_Actual - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2);
                                                BLActual_Solution = $@"BLActual : {BlActual} = {Last_BL_Actual} - ({int.Parse(DT.Rows[i]["F_MRP"].ToString())}/2)";
                                            }
                                            else
                                            {
                                                BLPlan_Solution = "BL = (BF + In(Rec)) - MRP/2 + Urgent \n";
                                                BlPlan = (Last_BL_Plan + InRec) - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                                BLPlan_Solution += $@"BLPlan : {BlPlan} = ({Last_BL_Plan} + {InRec}) - ({int.Parse(DT.Rows[i]["F_MRP"].ToString())}/2) + {DT.Rows[i]["F_Urgent_Order"].ToString()}";

                                                BlActual = (Last_BL_Plan + InRec) - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                                BLActual_Solution = $@"BLActual : {BlActual} = ({Last_BL_Actual} + {InActual}) - ({DT.Rows[i]["F_MRP"].ToString()}/2) - {DT.Rows[i]["F_AbNormal_Part"].ToString()}";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        BLPlan_Solution = "BL = ( BF + In(Rec) ) - MRP + Urgent - Abnormal \n";
                                        BlPlan = (Last_BL_Plan + InRec) - int.Parse(DT.Rows[i]["F_MRP"].ToString()) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString()) - int.Parse(DT.Rows[i]["F_AbNormal_Part"].ToString());
                                        BLPlan_Solution += $@"BLPlan : {BlPlan} = ({Last_BL_Plan} + {InRec}) - {DT.Rows[i]["F_MRP"].ToString()} + {DT.Rows[i]["F_Urgent_Order"].ToString()} - {DT.Rows[i]["F_AbNormal_Part"].ToString()}";

                                        BlActual = (Last_BL_Actual + InActual) - int.Parse(DT.Rows[i]["F_MRP"].ToString()) - int.Parse(DT.Rows[i]["F_AbNormal_Part"].ToString());
                                        BLActual_Solution = $@"BLActual : {BlActual} = ({Last_BL_Actual} + {InActual}) - {DT.Rows[i]["F_MRP"].ToString()} - {DT.Rows[i]["F_AbNormal_Part"].ToString()}";
                                    }

                                    Last_BL_Plan = BlPlan;
                                    Last_BL_Actual = BlActual;
                                }
                                else
                                {
                                    BLPlan_Solution = "BL = ( BF + In(Rec) ) - MRP + Urgent \n";
                                    BlPlan = (Last_BL_Plan + InRec) - int.Parse(DT.Rows[i]["F_MRP"].ToString()) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                    BLPlan_Solution += $@"BLPlan : {BlPlan} = ({Last_BL_Plan} + {InRec}) - {DT.Rows[i]["F_MRP"].ToString()} + {DT.Rows[i]["F_Urgent_Order"].ToString()}";

                                    BlActual = (Last_BL_Actual + InActual) - int.Parse(DT.Rows[i]["F_MRP"].ToString());
                                    BLActual_Solution = $@"BLActual : {BlActual} = ({Last_BL_Actual} + {InActual}) - {DT.Rows[i]["F_MRP"].ToString()}";

                                    Last_BL_Plan = BlPlan;
                                    Last_BL_Actual = BlActual;
                                }
                            }

                            sqlParams = new List<SqlParameter>
                            {
                                new SqlParameter("@Part_No", DT.Rows[i]["F_Part_No"].ToString().Trim()),
                                new SqlParameter("@Ruibetsu", DT.Rows[i]["F_Ruibetsu"].ToString().Trim()),
                                new SqlParameter("@Supplier_Code", DT.Rows[i]["F_Supplier_Code"].ToString().Trim()),
                                new SqlParameter("@Supplier_Plant", DT.Rows[i]["F_Supplier_Plant"].ToString().Trim()),
                                new SqlParameter("@Store_Code", DT.Rows[i]["F_Store_Code"].ToString().Trim()),
                                new SqlParameter("@Kanban_No", DT.Rows[i]["F_Kanban_No"].ToString().Trim()),
                                new SqlParameter("@Process_Date", DT.Rows[i]["F_Process_Date"].ToString().Trim()),
                                new SqlParameter("@Process_Round", DT.Rows[i]["F_Process_Round"].ToString().Trim()),
                                new SqlParameter("@Process_Shift", DT.Rows[i]["F_Process_Shift"].ToString().Trim()),
                                new SqlParameter("@BL_Plan",BlPlan),
                                new SqlParameter("@BL_Actual",BlActual),
                                new SqlParameter("@Not_Recalculate",DT.Rows[i]["F_Not_Recalculate"].ToString().Trim())
                            };

                            string query = (RowIndex + 1) + "/" + (i + 1);
                            query += " [CKD_Inhouse].sp_autoRecalculateBL_UpdateBL";
                            foreach (var p in sqlParams)
                            {
                                query += $" {p.ParameterName} {p.Value}, ";
                            }


                            int intResult = await _kbContext.Database.ExecuteSqlRawAsync(
                             "[CKD_Inhouse].sp_autoRecalculateBL_UpdateBL @Process_Date,@Process_Shift,@Process_Round,@Supplier_Code,@Supplier_Plant,@Part_No,@Ruibetsu,@Kanban_No,@Store_Code,@BL_Plan,@BL_Actual,@Not_Recalculate",
                             new SqlParameter("@Process_Date", DT.Rows[i]["F_Process_Date"].ToString().Trim()),
                             new SqlParameter("@Process_Shift", DT.Rows[i]["F_Process_Shift"].ToString().Trim()),
                             new SqlParameter("@Process_Round", DT.Rows[i]["F_Process_Round"].ToString().Trim()),
                             new SqlParameter("@Supplier_Code", DT.Rows[i]["F_Supplier_Code"].ToString().Trim()),
                             new SqlParameter("@Supplier_Plant", DT.Rows[i]["F_Supplier_Plant"].ToString().Trim()),
                             new SqlParameter("@Part_No", DT.Rows[i]["F_Part_No"].ToString().Trim()),
                             new SqlParameter("@Ruibetsu", DT.Rows[i]["F_Ruibetsu"].ToString().Trim()),
                             new SqlParameter("@Kanban_No", DT.Rows[i]["F_Kanban_No"].ToString().Trim()),
                             new SqlParameter("@Store_Code", DT.Rows[i]["F_Store_Code"].ToString().Trim()),
                             new SqlParameter("@BL_Plan", BlPlan),
                             new SqlParameter("@BL_Actual", BlActual),
                             new SqlParameter("@Not_Recalculate", DT.Rows[i]["F_Not_Recalculate"].ToString().Trim())
                            );


                            if (intResult > 0)
                            {
                                _log.WriteLogMsg($"Update TB_Calculate_D : Complete {query} {BLPlan_Solution} {BLActual_Solution}");
                            }
                            else
                            {
                                _log.WriteLogMsg($"Update TB_Calculate_D : Not Complete {query} {BLPlan_Solution} {BLActual_Solution}");
                            }

                            if (DT.Rows[i]["F_Not_Recalculate"].ToString() == "True")
                            {
                                Last_BL_Plan = int.Parse(DT.Rows[i]["F_BL_SET_Plan"].ToString());
                                Last_BL_Actual = int.Parse(DT.Rows[i]["F_BL_SET_Actual"].ToString());
                                blnFromSetStock = DT.Rows[i]["F_Not_Recalculate"].ToString() == "True";
                            }
                            else
                            {
                                blnFromSetStock = false;
                            }

                            if (DT.Rows[i]["F_Process_Date"].ToString() == dateECI.Begining_Calculate
                                && DT.Rows[i]["F_Process_Round"].ToString() == "1")
                            {
                                dateDelivery = DateTime.ParseExact(DT.Rows[i]["F_Process_Date"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                                sqlParams.Clear();
                                sqlParams.Add(new SqlParameter("@Part_No", DT.Rows[i]["F_Part_No"].ToString().Trim()));
                                sqlParams.Add(new SqlParameter("@Ruibetsu", DT.Rows[i]["F_Ruibetsu"].ToString().Trim()));
                                sqlParams.Add(new SqlParameter("@Supplier_Code", DT.Rows[i]["F_Supplier_Code"].ToString().Trim()));
                                sqlParams.Add(new SqlParameter("@Supplier_Plant", DT.Rows[i]["F_Supplier_Plant"].ToString().Trim()));
                                sqlParams.Add(new SqlParameter("@Store_Code", DT.Rows[i]["F_Store_Code"].ToString().Trim()));
                                sqlParams.Add(new SqlParameter("@Kanban_No", DT.Rows[i]["F_Kanban_No"].ToString().Trim()));
                                sqlParams.Add(new SqlParameter("@Date", dateDelivery.AddDays(-1).ToString("yyyyMMdd")));

                                var DT_LastBL = await _FillDT.ExecuteStoreSQLAsync("sp_autoRecalculateBL_First", sqlParams.ToArray());

                                if (DT_LastBL.Rows.Count > 0)
                                {
                                    Last_BL_Plan = int.TryParse(DT_LastBL.Rows[0]["F_BL_SET_Plan"].ToString(), out int iBL_Plan) ? iBL_Plan : 0;
                                    Last_BL_Actual = int.TryParse(DT_LastBL.Rows[0]["F_BL_SET_Actual"].ToString(), out int iBL_Actual) ? iBL_Actual : 0;
                                }
                                else
                                {
                                    Last_BL_Plan = 0;
                                    Last_BL_Actual = 0;
                                }
                            }
                        }
                        await kbTrans.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await kbTrans.RollbackAsync();
                        _log.WriteErrorLogMsg("Update TB_Calculate_D Not Complete index => " + RowIndex + " " + ex.ToString());
                        throw new CustomHttpException(500, ex.InnerException.Message ?? ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.WriteErrorLogMsg("Update TB_Calculate_D Not Complete index => " + RowIndex + " " + ex.ToString());
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException.Message ?? ex.Message);
            }
        }

        public async Task<DateECI> get_ECIDate(string start_Date, string end_Date, int RowIndex)
        {
            try
            {
                DateECI dateECI = new DateECI();
                DateTime dateLast_Trip = new DateTime();

                dateLast_Trip = DateTime.ParseExact(start_Date, "yyyyMMdd", CultureInfo.InvariantCulture);

                var dbConstruction = await _PPM3Context.T_Construction.AsNoTracking()
                    .Where(x => x.F_Part_no == DT_PartControl.Rows[RowIndex]["F_Part_No"].ToString().Trim()
                    && x.F_Ruibetsu == DT_PartControl.Rows[RowIndex]["F_Ruibetsu"].ToString().Trim()
                    && x.F_Store_cd == DT_PartControl.Rows[RowIndex]["F_Store_Code"].ToString().Trim()
                    && x.F_supplier_cd == DT_PartControl.Rows[RowIndex]["F_Supplier_Code"].ToString().Trim()
                    && x.F_plant == DT_PartControl.Rows[RowIndex]["F_Supplier_Plant"].ToString().Trim()[0]
                    && x.F_Sebango == DT_PartControl.Rows[RowIndex]["F_Kanban_No"].ToString().Trim().Substring(1, 3)
                    ).FirstOrDefaultAsync();

                if (dbConstruction != null)
                {
                    dateECI.Begining_Date = dbConstruction.F_Local_Str;
                }
                else
                {
                    dateECI.Begining_Date = dateLast_Trip.ToString("yyyyMMdd");
                }

                var dbCalculateD = await _kbContext.TB_Calculate_D_CKD.AsNoTracking()
                    .Where(x => x.F_Part_No == DT_PartControl.Rows[RowIndex]["F_Part_No"].ToString().Trim()
                    && x.F_Ruibetsu == DT_PartControl.Rows[RowIndex]["F_Ruibetsu"].ToString().Trim()
                    && x.F_Supplier_Code == DT_PartControl.Rows[RowIndex]["F_Supplier_Code"].ToString().Trim()
                    && x.F_Supplier_Plant == DT_PartControl.Rows[RowIndex]["F_Supplier_Plant"].ToString().Trim()
                    && x.F_Store_Code == DT_PartControl.Rows[RowIndex]["F_Store_Code"].ToString().Trim()
                    && x.F_Kanban_No == DT_PartControl.Rows[RowIndex]["F_Kanban_No"].ToString().Trim())
                    .OrderBy(x => x.F_Process_Date).FirstOrDefaultAsync();

                if (dbCalculateD != null)
                {
                    dateECI.Begining_Calculate = dbCalculateD.F_Process_Date;
                }
                else
                {
                    dateECI.Begining_Calculate = dateLast_Trip.ToString("yyyyMMdd");
                }

                return dateECI;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLogMsg(ex.ToString());
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException.Message ?? ex.Message);
            }
        }

    }
}
