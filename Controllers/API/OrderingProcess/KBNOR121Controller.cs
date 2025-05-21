using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.KB3.OrderingProcess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;

namespace KANBAN.Controllers.API.OrderingProcess
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR121Controller : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly KB3Context _KB3Context;
        private readonly SerilogLibs _Log;
        private readonly FillDataTable _FillDT;

        public KBNOR121Controller(
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context,
            PPM3Context pPM3Context,
            SerilogLibs serilogLibs,
            FillDataTable fillDataTable
            )
        {
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _PPM3Context = pPM3Context;
            _Log = serilogLibs;
            _FillDT = fillDataTable;
        }

        private readonly string Txt_OrderType = "Daily Order";
        private readonly string Type_Import = "N";
        private static readonly DateTime Now = DateTime.Now;
        private static DateTime LoginDate = DateTime.Now.Date;
        private static string Proc_Shift = "Day";
        private static string Login_Shift = "Day";
        private static string Txt_MRPStatus = "MRP : " + Now.Date.ToString();
        private static string UserCode = "";
        private static string Plant = "";
        private static string strAction = "";

        private static string Start_Date = "";
        private static string End_Date = "";
        private static int intAmountShow = 0;
        private static DateTime dateDelivery = new DateTime();
        private static string DeliveryTrip = "";
        private static DateTime ProcessDate = new DateTime();


        private static int intRun = 0;

        private static DataTable DT_Period = new DataTable();
        private static DataTable DT_Date = new DataTable();
        private static DataTable DT_DeliveryDate = new DataTable();
        private static DataTable DT_PartControl = new DataTable();
        private static DataTable DT_Header = new DataTable();
        private static DataTable DT_Detail = new DataTable();
        private static DataTable DT_Volume = new DataTable();
        private static DataTable DT_Actual_Receive = new DataTable();
        private static DataTable DT_AdjustOrder_Trip = new DataTable();

        public class DateECI
        {
            public string Begining_Date { get; set; }
            public string Begining_Calculate { get; set; }
        }


        [HttpGet]
        public IActionResult OnLoad(string Login_Date, string Shift, string Process_Date)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });

                Proc_Shift = (Shift.Substring(0, 1) == "1") ? "Day" : "Night";
                Login_Shift = (Login_Date.Substring(10, 1) == "D") ? "Day" : "Night";
                UserCode = _BearerClass.UserCode;
                Plant = _BearerClass.Plant;
                ProcessDate = DateTime.ParseExact(Process_Date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                LoginDate = DateTime.ParseExact(Login_Date.Substring(0, 10), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Onloading is success.",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = ex.InnerException?.Message ?? ex.Message

                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> SupplierDropDown()
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });

                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@Plant", Plant),
                };

                var dt = _FillDT.ExecuteSQL_Param("EXEC [dbo].[sp_NormalOrder_getSupplier] @Plant", sqlParameters);

                if (dt.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No Supplier Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Supplier List",
                    data = JsonConvert.SerializeObject(dt)
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = ex.InnerException?.Message ?? ex.Message

                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> KanbanDropDown(string Supplier)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });

                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@Plant", Plant),
                    new SqlParameter("@OrderType", DBNull.Value), // Assuming "Daily" is always required
                    new SqlParameter("@Supplier_Code", !string.IsNullOrWhiteSpace(Supplier) ? Supplier.Substring(0, 4) : (object)DBNull.Value),
                    new SqlParameter("@Supplier_Plant", !string.IsNullOrWhiteSpace(Supplier) ? Supplier.Substring(5, 1) : (object)DBNull.Value),
                    // Add other parameters similarly if they are provided, else set to DBNull.Value
                    new SqlParameter("@Part_No_FROM", (object)DBNull.Value),
                    new SqlParameter("@Part_No_TO", (object) DBNull.Value),
                    new SqlParameter("@Ruibetsu_FROM", (object) DBNull.Value),
                    new SqlParameter("@Ruibetsu_TO", (object) DBNull.Value),
                    new SqlParameter("@Kanban_No_FROM", (object) DBNull.Value),
                    new SqlParameter("@Kanban_No_TO", (object) DBNull.Value),
                    new SqlParameter("@Store_Code_FROM", (object) DBNull.Value),
                    new SqlParameter("@Store_Code_TO", (object) DBNull.Value)
                };

                var dt = _FillDT.ExecuteSQL_Param("EXEC [dbo].[sp_NormalOrder_getKanban] @Plant, @Supplier_Code, @Supplier_Plant, @Part_No_FROM, @Part_No_TO, @Ruibetsu_FROM, @Ruibetsu_TO, @Kanban_No_FROM, @Kanban_No_TO, @Store_Code_FROM, @Store_Code_TO, @OrderType", sqlParameters);

                if (dt.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No Kanban Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Kanban List",
                    data = JsonConvert.SerializeObject(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = ex.InnerException?.Message ?? ex.Message

                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> StoreDropDown()
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });


                var dt = _FillDT.ExecuteSQL("EXEC [dbo].[sp_NormalOrder_getStoreCode] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11",
                    Plant, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
                    DBNull.Value);

                if (dt.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No Store Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Store List",
                    data = JsonConvert.SerializeObject(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = ex.InnerException?.Message ?? ex.Message

                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PartNoDropDown(string? Store_Cd_From = null, string? Store_Cd_To = null)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });


                var dt = _FillDT.ExecuteSQL("EXEC [dbo].[sp_NormalOrder_getPartNo] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11",
                    Plant, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
                    string.IsNullOrWhiteSpace(Store_Cd_From) ? DBNull.Value : Store_Cd_From, string.IsNullOrWhiteSpace(Store_Cd_To) ? DBNull.Value : Store_Cd_To,
                    DBNull.Value);

                if (dt.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No Part Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Part List",
                    data = JsonConvert.SerializeObject(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = ex.InnerException?.Message ?? ex.Message

                });
            }
        }

        private void Find_StartEnd_Date(VMKBNOR121_Preview obj)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) throw new Exception("Unauthorized");

                if (obj.Action == "Preview")
                {
                    strAction = "Preview";
                    intRun = 0;
                    var storeCalendar = Plant switch
                    {
                        "1" => "1A",
                        "2" => "2B",
                        "3" => "3C",
                        _ => "3C"
                    };

                    var NoDayPreview = _FillDT.ExecuteSQL("exec [dbo].[sp_NumberOfDayToPreview] @p0,@p1,@p2,@p3,@p4",
                        Plant, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                            storeCalendar, LoginDate.ToString("yyyyMMdd"));

                    if (NoDayPreview.Rows.Count == 0)
                    {

                        throw new Exception("NumberOfDayToPreview Not Found");
                    }

                    Start_Date = NoDayPreview.Rows[0]["Start_Date"].ToString();
                    End_Date = NoDayPreview.Rows[0]["End_Date"].ToString();
                    intAmountShow = int.Parse(NoDayPreview.Rows[0]["Display_Date"].ToString());

                }

                else
                {
                    strAction = "Process";
                    intRun = 0;

                    var NoDayPreview = _FillDT.ExecuteSQL("exec [dbo].[sp_NumberOfDayToSearch] @p0,@p1,@p2,@p3,@p4,@p5",
                        Plant, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                            ProcessDate.ToString("yyyyMMdd"), Proc_Shift.Substring(0, 1), UserCode);

                    if (NoDayPreview.Rows.Count == 0)
                    {

                        throw new Exception("NumberOfDayToPreview Not Found");
                    }

                    Start_Date = NoDayPreview.Rows[0]["Start_Date"].ToString();
                    End_Date = NoDayPreview.Rows[0]["End_Date"].ToString();
                    intAmountShow = int.Parse(NoDayPreview.Rows[0]["Display_Date"].ToString());

                }

                //DT_DeliveryDate = _FillDT.ExecuteSQL("exec [dbo].[sp_getDeliveryDateTrip] @p0,@p1,@p2,@p3,@p4,@p5,@p6",
                //    Plant, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1), ProcessDate.ToString("yyyyMMdd"), Proc_Shift.Substring(0, 1),
                //    string.IsNullOrWhiteSpace(obj.Store) ? DBNull.Value : obj.Store, string.IsNullOrWhiteSpace(obj.StoreTo) ? DBNull.Value : obj.StoreTo);
                var tmpProcessDate = ProcessDate;

                do
                {
                    DT_DeliveryDate = _FillDT.ExecuteSQL("exec [dbo].[sp_getDeliveryDateTrip] @p0,@p1,@p2,@p3,@p4,@p5,@p6",
                        Plant, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1), tmpProcessDate.ToString("yyyyMMdd"), Proc_Shift.Substring(0, 1),
                        string.IsNullOrWhiteSpace(obj.Store) ? DBNull.Value : obj.Store, string.IsNullOrWhiteSpace(obj.StoreTo) ? DBNull.Value : obj.StoreTo);

                    tmpProcessDate = tmpProcessDate.AddDays(+1);
                }
                while (DT_DeliveryDate.Rows.Count == 0);

                dateDelivery = DateTime.TryParseExact(DT_DeliveryDate.Rows[0]["F_Delivery_Date"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result) ? result : DateTime.Now;
                DeliveryTrip = DT_DeliveryDate.Rows[0]["F_Delivery_Trip"].ToString().Trim();

                DT_Date = _FillDT.ExecuteSQL("exec [dbo].[sp_getCycleTime] @p0,@p1,@p2,@p3",
                                     obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1), Start_Date, End_Date);

                if (DT_Date.Rows.Count == 0)
                {

                    throw new Exception("Cycle Time Not Found");
                }

                if (Start_Date != DT_Date.Rows[0]["F_Date"]) Start_Date = DT_Date.Rows[0]["F_Date"].ToString();
                if (End_Date != DT_Date.Rows[DT_Date.Rows.Count - 1]["F_Date"]) End_Date = DT_Date.Rows[DT_Date.Rows.Count - 1]["F_Date"].ToString();
                intAmountShow = int.Parse(DT_Date.Rows.Count.ToString());

                // Clear the destination DataTable
                DT_Period.Clear();

                for (int i = 0; i < intAmountShow; i++)
                {
                    // Execute the stored procedure and get the result in dtAdd
                    var dtAdd = _FillDT.ExecuteSQL("exec [dbo].[sp_findPeriod] @p0,@p1,@p2,@p3,@p4",
                        Plant, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                        DT_Date.Rows[i]["F_Date"], UserCode);


                    // Merge the result to the destination DataTable
                    DT_Period.Merge(dtAdd);

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Get_All_Data(VMKBNOR121_Preview obj)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });

                Find_StartEnd_Date(obj);

                DT_PartControl.Clear();
                DT_Header.Clear();
                DT_Detail.Clear();
                DT_Volume.Clear();
                DT_AdjustOrder_Trip.Clear();
                DT_Actual_Receive.Clear();


                string _SQL = "exec [dbo].[sp_DT_PartControl] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12";

                DT_PartControl = _FillDT.ExecuteSQL(_SQL,
                    ProcessDate.ToString("yyyyMMdd"), ProcessDate.ToString("yyyyMMdd"), obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                    string.IsNullOrWhiteSpace(obj.Kanban) ? DBNull.Value : obj.Kanban,
                    string.IsNullOrWhiteSpace(obj.KanbanTo) ? DBNull.Value : obj.KanbanTo,
                    string.IsNullOrWhiteSpace(obj.Store) ? DBNull.Value : obj.Store,
                    string.IsNullOrWhiteSpace(obj.StoreTo) ? DBNull.Value : obj.StoreTo,
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(11, 2),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(11, 2), DBNull.Value);

                if (DT_PartControl.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No Data Found"
                    });
                }

                else if (obj.Action == "Re-Calculate BL" || obj.Action == "Re-Calculate")
                {
                    var _intRow = 0;
                    for (int j = 0; j < DT_PartControl.Rows.Count; j++)
                    {
                        if (DT_PartControl.Rows[j]["F_Kanban_No"].ToString().Trim() == obj.Kanban
                            && DT_PartControl.Rows[j]["F_Part_No"].ToString().Trim() == obj.PartNo.Split("-")[0]
                            && DT_PartControl.Rows[j]["F_Ruibetsu"].ToString().Trim() == obj.PartNo.Split("-")[1]
                            && DT_PartControl.Rows[j]["F_Store_Code"].ToString().Trim() == obj.Store)
                        {
                            _intRow = j;
                            break;
                        }
                    }

                    await get_startDate(_intRow, obj.Action);
                }

                _SQL = "exec [dbo].[sp_DT_Header] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12";

                if (obj.intRow != null)
                {
                    DT_Header = _FillDT.ExecuteSQL(_SQL,
                        Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Part_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Part_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Ruibetsu"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Ruibetsu"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Kanban_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Kanban_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Store_Code"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Store_Code"].ToString().Trim(),
                        DBNull.Value
                        );
                }
                else
                {
                    DT_Header = _FillDT.ExecuteSQL(_SQL,
                        Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                        string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(0, 10),
                        string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(0, 10),
                        string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(11, 2),
                        string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(11, 2),
                        string.IsNullOrWhiteSpace(obj.Kanban) ? DBNull.Value : obj.Kanban,
                        string.IsNullOrWhiteSpace(obj.KanbanTo) ? DBNull.Value : obj.KanbanTo,
                        string.IsNullOrWhiteSpace(obj.Store) ? DBNull.Value : obj.Store,
                        string.IsNullOrWhiteSpace(obj.StoreTo) ? DBNull.Value : obj.StoreTo, DBNull.Value);
                }


                if (DT_Header.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No Header Found"
                    });
                }

                _SQL = "exec [dbo].[sp_DT_Detail] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12";

                if (obj.intRow != null)
                {
                    DT_Detail = _FillDT.ExecuteSQL(_SQL,
                        Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Part_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Part_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Ruibetsu"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Ruibetsu"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Kanban_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Kanban_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Store_Code"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Store_Code"].ToString().Trim(),
                        DBNull.Value
                        );
                }
                else
                {
                    DT_Detail = _FillDT.ExecuteSQL(_SQL,
                        Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                        string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(0, 10),
                        string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(0, 10),
                        string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(11, 2),
                        string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(11, 2),
                        string.IsNullOrWhiteSpace(obj.Kanban) ? DBNull.Value : obj.Kanban,
                        string.IsNullOrWhiteSpace(obj.KanbanTo) ? DBNull.Value : obj.KanbanTo,
                        string.IsNullOrWhiteSpace(obj.Store) ? DBNull.Value : obj.Store,
                        string.IsNullOrWhiteSpace(obj.StoreTo) ? DBNull.Value : obj.StoreTo, DBNull.Value);
                }
                if (DT_Detail.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No Detail Found"
                    });
                }

                _SQL = "exec [dbo].[sp_DT_Volume] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12";

                if (obj.intRow != null)
                {
                    DT_Volume = _FillDT.ExecuteSQL(_SQL,
                        Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Part_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Part_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Ruibetsu"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Ruibetsu"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Kanban_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Kanban_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Store_Code"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Store_Code"].ToString().Trim(),
                        DBNull.Value
                        );
                }
                else
                {
                    DT_Volume = _FillDT.ExecuteSQL(_SQL,
                        Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                        string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(0, 10),
                        string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(0, 10),
                        string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(11, 2),
                        string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(11, 2),
                        string.IsNullOrWhiteSpace(obj.Kanban) ? DBNull.Value : obj.Kanban,
                        string.IsNullOrWhiteSpace(obj.KanbanTo) ? DBNull.Value : obj.KanbanTo,
                        string.IsNullOrWhiteSpace(obj.Store) ? DBNull.Value : obj.Store,
                        string.IsNullOrWhiteSpace(obj.StoreTo) ? DBNull.Value : obj.StoreTo, DBNull.Value);
                }

                if (DT_Volume.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No Volume Found"
                    });
                }

                _SQL = "exec [dbo].[sp_DT_AdjustOder_Trip] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12";

                if (obj.intRow != null)
                {
                    DT_AdjustOrder_Trip = _FillDT.ExecuteSQL(_SQL,
                        Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Part_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Part_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Ruibetsu"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Ruibetsu"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Kanban_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Kanban_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Store_Code"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Store_Code"].ToString().Trim(),
                        DBNull.Value
                        );
                }
                else
                {
                    DT_AdjustOrder_Trip = _FillDT.ExecuteSQL(_SQL,
                        Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                        string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(0, 10),
                        string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(0, 10),
                        string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(11, 2),
                        string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(11, 2),
                        string.IsNullOrWhiteSpace(obj.Kanban) ? DBNull.Value : obj.Kanban,
                        string.IsNullOrWhiteSpace(obj.KanbanTo) ? DBNull.Value : obj.KanbanTo,
                        string.IsNullOrWhiteSpace(obj.Store) ? DBNull.Value : obj.Store,
                        string.IsNullOrWhiteSpace(obj.StoreTo) ? DBNull.Value : obj.StoreTo, DBNull.Value);
                }

                if (DT_AdjustOrder_Trip.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No AdjustOrder_Trip Found"
                    });
                }

                _SQL = "exec [dbo].[sp_DT_Actual_Receive] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9";

                if (obj.intRow != null)
                {
                    DT_Actual_Receive = _FillDT.ExecuteSQL(_SQL,
                        Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Part_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Part_No"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Ruibetsu"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Ruibetsu"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Store_Code"].ToString().Trim(),
                        DT_PartControl.Rows[obj.intRow ?? 0]["F_Store_Code"].ToString().Trim()
                        );
                }
                else
                {
                    DT_Actual_Receive = _FillDT.ExecuteSQL(_SQL,
                        Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                        string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(0, 10),
                        string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(0, 10),
                        string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(11, 2),
                        string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(11, 2),
                        string.IsNullOrWhiteSpace(obj.Store) ? DBNull.Value : obj.Store,
                        string.IsNullOrWhiteSpace(obj.StoreTo) ? DBNull.Value : obj.StoreTo);
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = new
                    {
                        DT_Date = JsonConvert.SerializeObject(DT_Date),
                        DT_DeliveryDate = JsonConvert.SerializeObject(DT_DeliveryDate),
                        DT_Period = JsonConvert.SerializeObject(DT_Period),
                        DT_PartControl = JsonConvert.SerializeObject(DT_PartControl),
                        DT_Header = JsonConvert.SerializeObject(DT_Header),
                        DT_Detail = JsonConvert.SerializeObject(DT_Detail),
                        DT_Volume = JsonConvert.SerializeObject(DT_Volume),
                        DT_AdjustOrder_Trip = JsonConvert.SerializeObject(DT_AdjustOrder_Trip),
                        DT_Actual_Receive = JsonConvert.SerializeObject(DT_Actual_Receive)
                    }
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail_Data(int intRow, string F_Supplier_Cd)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });

                int ForecastMaxInt = 0;
                var ForecastMax = _FillDT.ExecuteSQL("EXEC [dbo].[sp_getForecastMax_New] " +
                    "@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7", Plant, F_Supplier_Cd.Substring(0, 4), F_Supplier_Cd.Substring(5, 1),
                    DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim(), DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString(),
                    DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Substring(1, 3), DT_PartControl.Rows[intRow]["F_Store_Code"].ToString(),
                    Now.ToString("yyyyMMdd"));

                if (ForecastMax.Rows.Count == 0 || ForecastMax == null)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No Forecast Max Found"
                    });
                }
                else
                {
                    ForecastMaxInt = int.TryParse(ForecastMax.Rows[0]["ForecastMax"].ToString(), out int ResultForecast) ? ResultForecast : 0;
                }

                DataRow dr = DT_Date.Select("F_Date = '" + dateDelivery.ToString("yyyyMMdd") + "'")[0];
                string strCycle = DT_Date.Select("F_Date = '" + dateDelivery.ToString("yyyyMMdd") + "'")[0]["F_Cycle"].ToString().Trim();
                int CycleB = int.Parse(strCycle.Substring(2, 2));

                var VM_dt = DT_Header.Select($@"F_Process_Date = '{dateDelivery.ToString("yyyyMMdd")}' 
                        AND F_Store_Code = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}' 
                        AND F_Kanban_No = '{DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim()}' 
                        AND F_Part_No = '{DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()}' 
                        AND F_Ruibetsu = '{DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()}' 
                        ");

                if (VM_dt.Length == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data not found Please Select Other Data"
                    });
                }

                dr = VM_dt[0];

                int QtyPack = int.Parse(dr["F_Qty_Box"].ToString().Trim()) == 0 ? 1 : int.Parse(dr["F_Qty_Box"].ToString().Trim());

                if (dr == null)
                {
                    QtyPack = 0;
                }
                var IsDevelop = Request.Cookies["IsDev"];
                string PlantAndDev = Plant + IsDevelop;
                string sourceKB = PlantAndDev switch
                {
                    "1" => "[hmmt-E_Kanban].[New_Kanban]",
                    "2" => "[hmmt-E_Kanban].[New_Kanban_F2]",
                    "3" => "[New_Kanban_F3]",
                    "11" => "[New_Kanban]",
                    "21" => "[New_Kanban_F2]",
                    "31" => "[New_Kanban_F3]",
                    _ => "[New_Kanban_F3]"
                };

                string _SQL = $@"SELECT DISTINCT RTRIM(C.F_Part_nm) AS F_Part_nm, RTRIM(S.F_short_name) AS F_short_name, P.F_Address 
                    FROM T_Construction C INNER JOIN T_Supplier_ms S ON C.F_supplier_cd = S.F_supplier_cd 
                    AND C.F_plant =  S.F_Plant_cd AND C.F_Store_cd = S.F_Store_cd 
                    LEFT JOIN {sourceKB}.[dbo].TB_MS_Kanban P ON C.F_supplier_cd = P.F_Supplier_Code collate THAI_CS_AS 
                    AND C.F_plant = P.F_Supplier_Plant collate THAI_CS_AS AND C.F_Store_cd = P.F_Store_Code collate THAI_CS_AS 
                    AND RIGHT('0000'+C.F_Sebango,4) = P.F_Kanban_No collate THAI_CS_AS 
                    AND C.F_Part_no = P.F_Part_No collate THAI_CS_AS AND C.F_Ruibetsu = P.F_Ruibetsu collate THAI_CS_AS 
                    WHERE C.F_Part_no = '" + DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim() + "'" +
                    "AND C.F_Ruibetsu = '" + DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim() + "'" +
                    "AND C.F_Store_cd = '" + DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim() + "'" +
                    "AND C.F_supplier_cd = '" + DT_PartControl.Rows[intRow]["F_Supplier_Code"].ToString().Trim() + "'" +
                    "AND C.F_plant = '" + DT_PartControl.Rows[intRow]["F_Supplier_Plant"].ToString().Trim() + "'" +
                    "AND C.F_Sebango = '" + DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim().Substring(1, 3) + "'" +
                    "AND C.F_Local_Str <= convert(char(8),getdate(),112) " +
                    "AND C.F_Local_End >= convert(char(8),getdate(),112) " +
                    "AND S.F_TC_Str <= convert(char(8),getdate(),112) " +
                    "AND S.F_TC_End >= convert(char(8),getdate(),112) ";

                var dtNameAndLine = _FillDT.ExecuteSQLPPMDB(_SQL);
                string F_Part_nm = "";
                string F_short_name = "";
                string F_Address = "";
                if (dtNameAndLine.Rows.Count != 0)
                {
                    F_Part_nm = dtNameAndLine.Rows[0]["F_Part_nm"].ToString().Trim();
                    F_short_name = dtNameAndLine.Rows[0]["F_short_name"].ToString().Trim();
                    F_Address = dtNameAndLine.Rows[0]["F_Address"].ToString().Trim();
                }

                _SQL = $@"SELECT F_Max_Area FROM TB_MS_MaxArea_Stock WHERE F_Plant = '" + Plant + "' " +
                        "AND F_Part_no = '" + DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim() + "' " +
                        "AND F_Ruibetsu = '" + DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim() + "' " +
                        "AND F_Store_cd = '" + DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim() + "' " +
                        "AND F_Supplier_Cd = '" + DT_PartControl.Rows[intRow]["F_Supplier_Code"].ToString().Trim() + "' " +
                        "AND F_Supplier_Plant = '" + DT_PartControl.Rows[intRow]["F_Supplier_Plant"].ToString().Trim() + "' " +
                        "AND F_Kanban_No = '" + DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim() + "' ";

                var dtMaxArea = _FillDT.ExecuteSQL(_SQL);
                string Max_Area = "";
                if (dtMaxArea.Rows.Count == 0)
                {
                    Max_Area = "0";
                }
                else Max_Area = dtMaxArea.Rows[0]["F_Max_Area"].ToString();

                _SQL = $@"Exec [dbo].[sp_getSTD_B] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7";
                var dtSTD_B = _FillDT.ExecuteSQL(_SQL, Plant, F_Supplier_Cd.Substring(0, 4), F_Supplier_Cd.Substring(5, 1),
                            DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim(), DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString(),
                            DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Substring(1, 3), DT_PartControl.Rows[intRow]["F_Store_Code"].ToString(),
                            dateDelivery.ToString("yyyyMMdd"));

                if (dtSTD_B.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No STD_B Found"
                    });
                }

                string STD_B = string.IsNullOrWhiteSpace(dtSTD_B.Rows[0]["STD_B"].ToString()) ? "0" : (Math.Round(float.Parse(dtSTD_B.Rows[0]["STD_B"].ToString()))).ToString();
                string Safety_Stock = string.IsNullOrWhiteSpace(dtSTD_B.Rows[0]["Safety_Stock"].ToString()) ? "0" : dtSTD_B.Rows[0]["Safety_Stock"].ToString();

                _SQL = $@"Exec [dbo].[sp_getSTDStock] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7";
                var dtSTDStock = _FillDT.ExecuteSQL(_SQL, Plant, F_Supplier_Cd.Substring(0, 4), F_Supplier_Cd.Substring(5, 1),
                            DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim(), DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString(),
                            DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Substring(1, 3), DT_PartControl.Rows[intRow]["F_Store_Code"].ToString(),
                            dateDelivery.ToString("yyyyMMdd"));

                if (dtSTDStock.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No STDStock Found"
                    });
                }

                string STDStock = string.IsNullOrWhiteSpace(dtSTDStock.Rows[0]["STDStock"].ToString()) ? "0" : (Math.Round(float.Parse(dtSTDStock.Rows[0]["STDStock"].ToString()))).ToString();

                _SQL = $@"Exec [dbo].[sp_getMinStock] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7";
                var dtMinStock = _FillDT.ExecuteSQL(_SQL, Plant, F_Supplier_Cd.Substring(0, 4), F_Supplier_Cd.Substring(5, 1),
                            DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim(), DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString(),
                            DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Substring(1, 3), DT_PartControl.Rows[intRow]["F_Store_Code"].ToString(),
                            dateDelivery.ToString("yyyyMMdd"));

                if (dtMinStock.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No MinStock Found"
                    });
                }

                string MinStock = string.IsNullOrWhiteSpace(dtMinStock.Rows[0]["Min_Stock"].ToString()) ? "0" : (Math.Round(float.Parse(dtMinStock.Rows[0]["Min_Stock"].ToString()))).ToString();

                _SQL = $@"SELECT F_MRP, F_HMMT_Prod FROM TB_CALCULATE_H WHERE F_Supplier_Code = '" + F_Supplier_Cd.Substring(0, 4) + "'" +
                    "AND F_Supplier_Plant = '" + F_Supplier_Cd.Substring(5, 1) + "'" +
                    "AND F_Part_No = '" + DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim() + "'" +
                    "AND F_Ruibetsu = '" + DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim() + "'" +
                    "AND F_Store_Code = '" + DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim() + "'" +
                    "AND F_Kanban_No = '" + DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim() + "'" +
                    "AND F_Process_Date = '" + LoginDate.ToString("yyyyMMdd") + "'";

                var dtMRP = _FillDT.ExecuteSQL(_SQL);

                float MRP = 0f;
                float HMMT_Prod = 0f;

                if (dtMRP.Rows.Count > 0)
                {
                    float.TryParse(dtMRP.Rows[0]["F_MRP"]?.ToString(), out MRP);
                    float.TryParse(dtMRP.Rows[0]["F_HMMT_Prod"]?.ToString(), out MRP);
                }

                string MRPCheck = "";
                if (HMMT_Prod * 0.8 > MRP)
                {
                    MRPCheck = "Less20Check";
                }
                else if (HMMT_Prod * 1.2 < MRP)
                {
                    MRPCheck = "More20Check";
                }
                if (dtMRP.Rows.Count == 0)
                {
                    MRPCheck = "";
                }

                _SQL = $@"SELECT F_KB_CUT, F_KB_ADD, F_KB_STOP FROM TB_Calculate_H 
                       WHERE F_Supplier_Code = '{F_Supplier_Cd.Substring(0, 4)}'
                       AND F_Supplier_Plant = '{F_Supplier_Cd.Substring(5, 1)}'
                       AND F_Store_Code = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}'
                       AND F_Part_No = '{DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()}'
                       AND F_Ruibetsu = '{DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()}'
                       AND F_Kanban_No = '{DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim()}'
                       AND F_Process_Date = '{ProcessDate.ToString("yyyyMMdd")}'";

                var dtKB = _FillDT.ExecuteSQL(_SQL);
                if (dtKB.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "No KB Found"
                    });
                }

                int KB_Cut = int.Parse(dtKB.Rows[0]["F_KB_CUT"].ToString());
                int KB_Add = int.Parse(dtKB.Rows[0]["F_KB_ADD"].ToString());
                int KB_Stop = int.Parse(dtKB.Rows[0]["F_KB_STOP"].ToString());

                _SQL = $@"SELECT * FROM TB_Kanban_SetOrder WHERE F_Plant = '{Plant}' 
                        AND F_Supplier_Code = '{F_Supplier_Cd.Substring(0, 4)}' 
                        AND F_Supplier_Plant = '{F_Supplier_Cd.Substring(5, 1)}' 
                        AND F_Part_No = '{DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()}' 
                        AND F_Ruibetsu = '{DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()}' 
                        AND F_Kanban_No = '{DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim()}' 
                        AND F_Store_Cd = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}' 
                        AND CONVERT(INT,F_Trip1)+CONVERT(INT,F_Trip2)+CONVERT(INT,F_Trip3)+CONVERT(INT,F_Trip4) 
                        +CONVERT(INT,F_Trip5)+CONVERT(INT,F_Trip6)+CONVERT(INT,F_Trip7)+CONVERT(INT,F_Trip8) 
                        +CONVERT(INT,F_Trip9)+CONVERT(INT,F_Trip10)+CONVERT(INT,F_Trip11)+CONVERT(INT,F_Trip12) 
                        +CONVERT(INT,F_Trip13)+CONVERT(INT,F_Trip14)+CONVERT(INT,F_Trip15)+CONVERT(INT,F_Trip16) 
                        +CONVERT(INT,F_Trip17)+CONVERT(INT,F_Trip18)+CONVERT(INT,F_Trip19)+CONVERT(INT,F_Trip20) 
                        +CONVERT(INT,F_Trip21)+CONVERT(INT,F_Trip22)+CONVERT(INT,F_Trip23)+CONVERT(INT,F_Trip24) > 0 ";

                var dtSetOrder = _FillDT.ExecuteSQL(_SQL);
                string SetOrderCheck = "1";

                if (dtSetOrder.Rows.Count == 0)
                {
                    SetOrderCheck = "0";
                }

                _SQL = $@"SELECT * FROM( Select Distinct F_Supplier_Code, F_Supplier_Plant, F_Start_Order_Date AS F_Start_Date, F_Start_Date AS F_End_Date 
                        From TB_MS_DeliveryTime 
                        Where F_Start_Date <> F_Start_Order_Date 
                        And F_Supplier_Code = '{F_Supplier_Cd.Substring(0, 4)}' 
                        And F_Supplier_Plant = '{F_Supplier_Cd.Substring(5, 1)}' 
                        And F_Start_Order_Date <> '') A 
                        WHERE A.F_Start_Date <= '{ProcessDate.ToString("yyyyMMdd")}' 
                        AND A.F_End_Date >= '{ProcessDate.ToString("yyyyMMdd")}' ";

                var dtDeliveryTime = _FillDT.ExecuteSQL(_SQL);
                string ChgCycleCheck = "0";

                if (dtDeliveryTime.Rows.Count > 0)
                {
                    ChgCycleCheck = "1";
                }

                _SQL = $@"SELECT A.Slide_Order + B.Slide_Order_Part AS SliceOrder 
                        FROM  (   SELECT COUNT(*) AS Slide_Order FROM TB_Slide_Order 
                        WHERE F_Plant = '{Plant}' AND F_Supplier_CD = '{F_Supplier_Cd.Substring(0, 4)}'
                        AND F_Supplier_Plant = '{F_Supplier_Cd.Substring(5, 1)}' 
                        AND F_Store_CD = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}' 
                        AND F_Delivery_Date = '{dateDelivery.ToString("yyyyMMdd")}' 
                        AND F_Delivery_Trip = '{DeliveryTrip}' ) A CROSS JOIN 
                        (   SELECT COUNT(*) AS Slide_Order_Part 
                        FROM TB_Slide_Order_Part WHERE F_Plant = '{Plant}' 
                        AND F_Supplier_CD = '{F_Supplier_Cd.Substring(0, 4)}' 
                        AND F_Supplier_Plant = '{F_Supplier_Cd.Substring(5, 1)}' 
                        AND F_Part_No = '{DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()}' 
                        AND F_Ruibetsu = '{DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()}' 
                        AND F_Store_CD = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}' 
                        AND F_Delivery_Date = '{dateDelivery.ToString("yyyyMMdd")}' 
                        AND F_Delivery_Trip = '{DeliveryTrip}'  ) B
                        WHERE A.Slide_Order + B.Slide_Order_Part > 0 ";

                var dtSlideOrder = _FillDT.ExecuteSQL(_SQL);
                string SlideOrderCheck = "0";
                if (dtSlideOrder.Rows.Count > 0)
                {
                    SlideOrderCheck = "1";
                }

                _SQL = $@"SELECT A.Rec_Slide_Order + B.Rec_Slide_Order_Part AS SliceOrder 
                        FROM ( SELECT COUNT(*) AS Rec_Slide_Order FROM TB_Slide_Order 
                        WHERE F_Plant = '{Plant}' AND F_Supplier_CD = '{F_Supplier_Cd.Substring(0, 4)}'
                        AND F_Supplier_Plant = '{F_Supplier_Cd.Substring(5, 1)}' 
                        AND F_Store_CD = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}' 
                        AND F_Slide_Date = '{dateDelivery.ToString("yyyyMMdd")}' 
                        AND F_Slide_Trip = '{DeliveryTrip}' ) A CROSS JOIN 
                        (   SELECT COUNT(*) AS Rec_Slide_Order_Part FROM TB_Slide_Order_Part 
                        WHERE F_Plant = '{Plant}' 
                        AND F_Supplier_CD = '{F_Supplier_Cd.Substring(0, 4)}' 
                        AND F_Supplier_Plant = '{F_Supplier_Cd.Substring(5, 1)}' 
                        AND F_Part_No = '{DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()}' 
                        AND F_Ruibetsu = '{DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()}'                         AND F_Store_CD = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}' 
                        AND F_Slide_Date = '{dateDelivery.ToString("yyyyMMdd")}' 
                        AND F_Slide_Trip = '{DeliveryTrip}'  ) B
                        WHERE A.Rec_Slide_Order + B.Rec_Slide_Order_Part > 0 ";

                var dtRecSlideOrder = _FillDT.ExecuteSQL(_SQL);
                string RecSlideOrderCheck = "0";
                if (dtRecSlideOrder.Rows.Count > 0)
                {
                    RecSlideOrderCheck = "1";
                }

                string AvgTrip = (Math.Floor(((decimal)ForecastMaxInt / CycleB) / QtyPack) * QtyPack).ToString();

                var _InformNews = _KB3Context.TB_MS_Inform_News.Where(x =>
                    x.F_Supplier_Code == F_Supplier_Cd.Substring(0, 4) &&
                    x.F_Supplier_Plant == F_Supplier_Cd.Substring(5, 1) &&
                    x.F_Part_No == DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim() &&
                    x.F_Ruibetsu == DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim() &&
                    x.F_Store_Code == DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim() &&
                    x.F_Kanban_No == DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim())
                    .Select(x => x.F_Text).FirstOrDefault();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = new
                    {
                        forecastMaxInt = ForecastMaxInt,
                        cycleB = CycleB,
                        f_Part_nm = F_Part_nm,
                        f_short_name = F_short_name,
                        f_Address = F_Address,
                        max_Area = Max_Area,
                        std_B = STD_B,
                        safety_Stock = Safety_Stock,
                        stdStock = STDStock,
                        minStock = MinStock,
                        mrp = MRP,
                        hmmt_Prod = HMMT_Prod,
                        mrpCheck = MRPCheck,
                        kb_Cut = KB_Cut,
                        kb_Add = KB_Add,
                        kb_Stop = KB_Stop,
                        setOrderCheck = SetOrderCheck,
                        chgCycleCheck = ChgCycleCheck,
                        slideOrderCheck = SlideOrderCheck,
                        recSlideOrderCheck = RecSlideOrderCheck,
                        avgTrip = AvgTrip,
                        informNews = _InformNews
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Get_BL(VMKBNOR121_GetBL obj)
        {
            try
            {
                //await _KB3Context.Database.ExecuteSqlRawAsync("Exec dbo.sp_autoRecalculateBL_First @p0,@p1,@p2,@p3,@p4,@p5,@p6",
                //        obj.CurrentDate.Date.AddDays(-1).ToString("yyyy-MM-dd"),
                //        obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                //        obj.PartNo.Split("-")[0], obj.PartNo.Split("-")[1],
                //        obj.Kanban, obj.Store);

                var _autoRecalBl = _FillDT.ExecuteSQL("Exec dbo.sp_autoRecalculateBL_First @p0,@p1,@p2,@p3,@p4,@p5,@p6",
                    obj.CurrentDate.Date.AddDays(-1).ToString("yyyy-MM-dd"),
                    obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                    obj.PartNo.Split("-")[0], obj.PartNo.Split("-")[1],
                    obj.Kanban, obj.Store);


                if (_autoRecalBl.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "BL Not Found"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = JsonConvert.SerializeObject(_autoRecalBl),
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = ex.InnerException?.Message ?? ex.Message

                });
            }
        }

        [HttpPost]
        public IActionResult Chk_Status_CCR(VMKBNOR121_Preview obj)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });

                var _msParameter = _KB3Context.TB_MS_Parameter.Where(x => x.F_Code == "CI")
                    .Select(x => new
                    {
                        x.F_Value2,
                        x.F_Value3
                    }).ToList();

                string _btnCheck = "";

                if (_msParameter.Count > 0)
                {
                    if (_msParameter[0].F_Value2 == 0 || _msParameter[0].F_Value2 == 3
                        || _msParameter[0].F_Value3 != LoginDate.ToString("yyyyMMdd") + Login_Shift.Substring(0, 1))
                    {
                        _btnCheck = "Preview";
                    }
                    else
                    {
                        _btnCheck = "Search,Preview";

                        if (obj.Action == "Process")
                        {
                            _btnCheck += ",Recalculate";
                        }
                    }
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = _btnCheck
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = ex.InnerException?.Message ?? ex.Message

                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Bl_Recalculate(VMKBNOR121_Recal obj)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });

                VMKBNOR121_Preview _nObj = new VMKBNOR121_Preview
                {
                    Supplier = obj.Supplier,
                    PartNo = obj.PartNo,
                    Kanban = obj.Kanban,
                    Store = obj.Store,
                    Action = obj.Action
                };

                if (strAction == "Process")
                {
                    try
                    {
                        var dymStore = Plant switch
                        {
                            "1" => "1A",
                            "2" => "2B",
                            "3" => "3C",
                            _ => "3C"
                        };

                        int _loginDate = int.Parse(LoginDate.ToString("dd"));

                        string _SQL = $@"SELECT CONVERT(Integer, F_workCd_D{_loginDate}) 
                                 + CONVERT(Integer, F_workCd_N{_loginDate}) AS F_Work 
                                FROM TB_Calendar WHERE F_Store_cd = '{dymStore}' 
                                AND F_YM = '{LoginDate.ToString("yyyyMM")}' ";

                        var _workingDay = _FillDT.ExecuteSQL(_SQL);

                        if (_workingDay.Rows.Count > 0)
                        {
                            if (_workingDay.Rows[0]["F_Work"].ToString() == "" || _workingDay.Rows[0]["F_Work"].ToString() == "0")
                            {
                                return BadRequest(new
                                {
                                    status = "400",
                                    response = "Bad Request",
                                    title = "Error",
                                    message = "Working Day = 0"
                                });
                            }
                        }

                        else if (_workingDay.Rows.Count == 0)
                        {
                            return NotFound(new
                            {
                                status = "404",
                                response = "Not Found",
                                title = "Error",
                                message = "No Working Day Found"
                            });
                        }
                        //await Find_StartEnd_Date(_nObj);
                        return await Get_All_Data(_nObj);
                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                    }

                    finally
                    {
                        Chk_Status_CCR(_nObj);
                    }

                }
                else
                {
                    try
                    {

                        //await Find_StartEnd_Date(_nObj);
                        return await Get_All_Data(_nObj);

                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                    }

                    finally
                    {
                        Chk_Status_CCR(_nObj);
                    }

                }

                //return Ok(new
                //{
                //    status = "200",
                //    response = "OK",
                //    title = "Success",
                //    message = "Calculate Complete!",
                //});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = ex.InnerException?.Message ?? ex.Message

                });
            }
        }


        private async Task get_startDate(int _intRow, string Action)
        {
            try
            {
                if (Action == "Re-Calculate BL")
                {
                    for (int i = 0; i < DT_PartControl.Rows.Count; i++)
                    {
                        await set_startDate(i);
                    }
                }
                else if (Action == "Re-Calculate")
                {
                    await set_startDate(_intRow);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task set_startDate(int _intRow)
        {
            try
            {
                string start_Date, End_Date = "";
                string storeCode = Plant switch
                {
                    "1" => "1A",
                    "2" => "2B",
                    "3" => "3C",
                    _ => "3C"
                };

                var _ProcessDate = await _KB3Context.TB_Calculate_D.Where(x =>
                    x.F_Supplier_Code == DT_PartControl.Rows[_intRow]["F_Supplier_Code"].ToString().Trim() &&
                    x.F_Supplier_Plant == DT_PartControl.Rows[_intRow]["F_Supplier_Plant"].ToString().Trim() &&
                    x.F_Part_No == DT_PartControl.Rows[_intRow]["F_Part_No"].ToString().Trim() &&
                    x.F_Ruibetsu == DT_PartControl.Rows[_intRow]["F_Ruibetsu"].ToString().Trim() &&
                    x.F_Store_Code == DT_PartControl.Rows[_intRow]["F_Store_Code"].ToString().Trim() &&
                    x.F_Kanban_No == DT_PartControl.Rows[_intRow]["F_Kanban_No"].ToString().Trim() &&
                    (x.Flag_Chg_BL_Stock == true || x.Flag_Cancel_PDS == true || x.Flag_Chg_MRP == true ||
                    x.Flag_Chg_Urgent == true || x.Flag_Adjust_Order == true)
                    ).OrderBy(x => x.F_Process_Date).FirstOrDefaultAsync();

                //var _ProcessDateList = await _KB3Context.TB_Calculate_D.Where(x =>
                //    x.F_Supplier_Code == DT_PartControl.Rows[_intRow]["F_Supplier_Code"].ToString().Trim() &&
                //    x.F_Supplier_Plant == DT_PartControl.Rows[_intRow]["F_Supplier_Plant"].ToString().Trim() &&
                //    x.F_Part_No == DT_PartControl.Rows[_intRow]["F_Part_No"].ToString().Trim() &&
                //    x.F_Ruibetsu == DT_PartControl.Rows[_intRow]["F_Ruibetsu"].ToString().Trim() &&
                //    x.F_Store_Code == DT_PartControl.Rows[_intRow]["F_Store_Code"].ToString().Trim() &&
                //    x.F_Kanban_No == DT_PartControl.Rows[_intRow]["F_Kanban_No"].ToString().Trim() &&
                //    (x.Flag_Chg_BL_Stock == true || x.Flag_Cancel_PDS == true || x.Flag_Chg_MRP == true ||
                //    x.Flag_Chg_Urgent == true || x.Flag_Adjust_Order == true)
                //    ).OrderBy(x => x.F_Process_Date).ToListAsync();

                if (_ProcessDate != null)
                {
                    start_Date = _ProcessDate.F_Process_Date;
                }
                else
                {
                    start_Date = LoginDate.ToString("yyyyMMdd");
                }

                string _execSQL = "exec [dbo].[sp_NumberOfDayToPreview] @p0,@p1,@p2,@p3,@p4";
                var _dt = _FillDT.ExecuteSQL(_execSQL, Plant, DT_PartControl.Rows[_intRow]["F_Supplier_Code"].ToString().Trim(),
                    DT_PartControl.Rows[_intRow]["F_Supplier_Plant"].ToString().Trim(), storeCode, LoginDate.ToString("yyyyMMdd"));

                if (start_Date == ProcessDate.ToString("yyyyMMdd"))
                {
                    start_Date = _dt.Rows[0]["Start_Date"].ToString();
                }
                End_Date = _dt.Rows[0]["End_Date"].ToString();


                await re_Calculate_Trail(start_Date, End_Date, _intRow);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task re_Calculate_Trail(string start_date, string end_date, int _intRow)
        {
            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                DateTime dateLast_Trip = new DateTime();
                int Last_BL_Plan, Last_BL_Actual = 0;
                bool blnFromSetStock = false;
                DataTable DT_Adjust, DT_Actual, DT_LastBL, DT = new DataTable();

                dateLast_Trip = DateTime.ParseExact(start_date, "yyyyMMdd", CultureInfo.InvariantCulture);
                DateECI dateECI = await get_ECIDate(start_date, end_date, _intRow);

                string _execSQL = "exec [dbo].[sp_autoRecalculateBL_First] @p0,@p1,@p2,@p3,@p4,@p5,@p6";

                DT = _FillDT.ExecuteSQL(_execSQL, dateLast_Trip.AddDays(-1).ToString("yyyyMMdd"),
                    DT_PartControl.Rows[_intRow]["F_Supplier_Code"].ToString().Trim(), DT_PartControl.Rows[_intRow]["F_Supplier_Plant"].ToString().Trim(),
                    DT_PartControl.Rows[_intRow]["F_Part_No"].ToString().Trim(), DT_PartControl.Rows[_intRow]["F_Ruibetsu"].ToString().Trim(),
                    DT_PartControl.Rows[_intRow]["F_Kanban_No"].ToString().Trim(), DT_PartControl.Rows[_intRow]["F_Store_Code"].ToString().Trim());

                if (DT.Rows.Count > 0)
                {
                    Last_BL_Plan = int.TryParse(DT.Rows[0]["F_BL_SET_Plan"].ToString().Trim(), out int F_BL_SET_Plan) ? F_BL_SET_Plan : 0;
                    Last_BL_Actual = int.TryParse(DT.Rows[0]["F_BL_SET_Actual"].ToString().Trim(), out int F_BL_SET_Actual) ? F_BL_SET_Actual : 0;
                    blnFromSetStock = DT.Rows[0]["F_Not_Recalculate"].ToString().Trim() == "1" ? true : false;
                }
                else
                {
                    Last_BL_Plan = 0;
                    Last_BL_Actual = 0;
                    blnFromSetStock = false;
                }

                _execSQL = "exec [dbo].[sp_autoRecalculateBL_Second] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7";
                DT = _FillDT.ExecuteSQL(_execSQL, dateLast_Trip.ToString("yyyyMMdd"),
                    end_date, DT_PartControl.Rows[_intRow]["F_Supplier_Code"].ToString(), DT_PartControl.Rows[_intRow]["F_Supplier_Plant"].ToString(),
                    DT_PartControl.Rows[_intRow]["F_Part_No"].ToString(), DT_PartControl.Rows[_intRow]["F_Ruibetsu"].ToString(),
                    DT_PartControl.Rows[_intRow]["F_Kanban_No"].ToString(), DT_PartControl.Rows[_intRow]["F_Store_Code"].ToString());

                _execSQL = "exec [dbo].[sp_autoRecalculateBL_Third] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7";
                DT_Actual = _FillDT.ExecuteSQL(_execSQL, dateLast_Trip.ToString("yyyyMMdd"),
                            end_date, DT_PartControl.Rows[_intRow]["F_Supplier_Code"].ToString(), DT_PartControl.Rows[_intRow]["F_Supplier_Plant"].ToString(),
                            DT_PartControl.Rows[_intRow]["F_Part_No"].ToString(), DT_PartControl.Rows[_intRow]["F_Ruibetsu"].ToString(),
                            DT_PartControl.Rows[_intRow]["F_Kanban_No"].ToString(), DT_PartControl.Rows[_intRow]["F_Store_Code"].ToString());

                _execSQL = "exec [dbo].[sp_autoRecalculateBL_Fourth] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7";
                DT_Adjust = _FillDT.ExecuteSQL(_execSQL, dateLast_Trip.ToString("yyyyMMdd"),
                            end_date, DT_PartControl.Rows[_intRow]["F_Supplier_Code"].ToString(), DT_PartControl.Rows[_intRow]["F_Supplier_Plant"].ToString(),
                            DT_PartControl.Rows[_intRow]["F_Part_No"].ToString(), DT_PartControl.Rows[_intRow]["F_Ruibetsu"].ToString(),
                            DT_PartControl.Rows[_intRow]["F_Kanban_No"].ToString(), DT_PartControl.Rows[_intRow]["F_Store_Code"].ToString());

                if (DT.Rows.Count > 0)
                {
                    int InRec, InActual, BLActual = 0;
                    int BLPlan = 0;
                    DateTime dateDelivery = new DateTime();
                    string BLPlan_Solution, BLActual_Solution = "";
                    DataRow DR_Receive, DR_Adjust = null;

                    _KB3Transaction.CreateSavepoint("BL_Solution");
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        BLPlan_Solution = "";
                        BLActual_Solution = "";

                        if (i > 0)
                        {
                            if (DT.Rows[i]["F_Process_Round"].ToString() == DT.Rows[i - 1]["F_Process_Round"].ToString()
                                && DT.Rows[i]["F_Process_Date"].ToString() == DT.Rows[i - 1]["F_Process_Date"].ToString())
                            {
                                dateDelivery = DateTime.TryParseExact(DT.Rows[i]["F_Process_Date"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateDelivery) ? dateDelivery : new DateTime();

                                if (dateDelivery == new DateTime())
                                {
                                    throw new Exception("Date Delivery Not Found");
                                }

                                _execSQL = "exec [dbo].[sp_autoRecalculateBL_First] @p0,@p1,@p2,@p3,@p4,@p5,@p6";
                                DT_LastBL = _FillDT.ExecuteSQL(_execSQL, dateDelivery.AddDays(-1).ToString("yyyyMMdd"),
                                            DT_PartControl.Rows[_intRow]["F_Supplier_Code"].ToString(), DT_PartControl.Rows[_intRow]["F_Supplier_Plant"].ToString(),
                                            DT_PartControl.Rows[_intRow]["F_Part_No"].ToString(), DT_PartControl.Rows[_intRow]["F_Ruibetsu"].ToString(),
                                            DT_PartControl.Rows[_intRow]["F_Kanban_No"].ToString(), DT_PartControl.Rows[_intRow]["F_Store_Code"].ToString());

                                if (DT_LastBL.Rows.Count > 0)
                                {
                                    Last_BL_Plan = int.TryParse(DT_LastBL.Rows[0]["F_BL_SET_Plan"].ToString(), out int F_BL_SET_Plan) ? F_BL_SET_Plan : 0;
                                    Last_BL_Actual = int.TryParse(DT_LastBL.Rows[0]["F_BL_SET_Actual"].ToString(), out int F_BL_SET_Actual) ? F_BL_SET_Actual : 0;
                                }
                                else
                                {
                                    Last_BL_Plan = 0;
                                    Last_BL_Actual = 0;
                                }
                            }
                        }

                        DR_Receive = DT_Actual.Select(
                            "F_Delivery_trip = '" + DT.Rows[i]["F_Process_Round"].ToString() + "' " +
                            "AND F_Receive_date = '" + DT.Rows[i]["F_Process_Date"].ToString() + "' " +
                            "AND F_Supplier_Code = '" + DT.Rows[i]["F_Supplier_Code"].ToString() + "' " +
                            "AND F_Supplier_Plant = '" + DT.Rows[i]["F_Supplier_Plant"].ToString() + "' " +
                            "AND F_Part_No = '" + DT.Rows[i]["F_Part_No"].ToString() + "' " +
                            "AND F_Ruibetsu = '" + DT.Rows[i]["F_Ruibetsu"].ToString() + "' " +
                            "AND F_Store_CD = '" + DT.Rows[i]["F_Store_Code"].ToString() + "'"
                        ).FirstOrDefault();


                        if (DR_Receive != null)
                        {
                            InActual = int.TryParse(DR_Receive["IN_ACTUAL"].ToString(), out int F_In_Actual) ? F_In_Actual : 0;
                        }
                        else
                        {
                            InActual = 0;
                        }

                        if (DT.Rows[i]["F_Flag_Pattern"].ToString() == "1")
                        {
                            InRec = int.TryParse(DT.Rows[i]["F_Adj_Pattern"].ToString(), out int F_Adj_Pattern) ? F_Adj_Pattern : 0;
                        }
                        else
                        {
                            InRec = int.TryParse(DT.Rows[i]["IN_Plan"].ToString(), out int IN_Plan) ? IN_Plan : 0;
                        }

                        DR_Adjust = DT_Adjust.Select("F_Delivery_trip = '" + DT.Rows[i]["F_Process_Round"].ToString() + "' " +
                            "AND F_Delivery_Date = '" + DT.Rows[i]["F_Process_Date"].ToString() + "' " +
                            "AND F_Supplier_Code = '" + DT.Rows[i]["F_Supplier_Code"].ToString() + "' " +
                            "AND F_Supplier_Plant = '" + DT.Rows[i]["F_Supplier_Plant"].ToString() + "' " +
                            "AND F_Part_No = '" + DT.Rows[i]["F_Part_No"].ToString() + "' " +
                            "AND F_Ruibetsu = '" + DT.Rows[i]["F_Ruibetsu"].ToString() + "' " +
                            "AND F_Store_Code = '" + DT.Rows[i]["F_Store_Code"].ToString() + "' ",
                            "F_Process_Date DESC, F_Process_Round DESC").FirstOrDefault();

                        if (DR_Adjust != null)
                        {
                            if (DR_Adjust["F_Flag_Pattern"].ToString() == "1")
                            {
                                InRec = int.TryParse(DR_Adjust["F_Adj_Pattern"].ToString(), out int F_Adj_Pattern) ? F_Adj_Pattern : 0;
                            }
                            else if (DR_Adjust["F_Flag_Adj"].ToString() == "1")
                            {
                                InRec = int.TryParse(DR_Adjust["F_Adj_Qty"].ToString(), out int IN_Plan) ? IN_Plan * int.Parse(DT.Rows[i]["F_Qty_Box"].ToString()) : 0;
                            }
                        }

                        if (DT.Rows[i]["F_Process_Date"].ToString().CompareTo(dateECI.Begining_Date) < 0)
                        {
                            if (DT.Rows[i]["F_Process_Round"].ToString() == "1")
                            {
                                if (blnFromSetStock)
                                {
                                    BLPlan_Solution = "BL = (BL + In(Rec) ) + Urgent";
                                    BLPlan = (Last_BL_Plan + InRec) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                    BLPlan_Solution = BLPlan_Solution + "BLPlan : " + BLPlan + " = (" + Last_BL_Plan.ToString() + " + " +
                                        InRec + ") + " + DT.Rows[i]["F_Urgent_Order"].ToString();
                                    BLActual = (Last_BL_Actual + InActual);
                                    BLActual_Solution = "BLActual : " + BLActual.ToString() + " = (" + Last_BL_Actual.ToString() + " + " + InActual.ToString() + ")";

                                    if (DT.Rows[i]["Flag_HalfChg_BL_Stock"].ToString() == "False")
                                    {
                                        BLPlan_Solution = "BL = ( BL + In(Rec) ) - MRP + Urgent";
                                        BLPlan = (Last_BL_Plan + InRec) - int.Parse(DT.Rows[i]["F_MRP"].ToString()) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                        BLPlan_Solution = BLPlan_Solution + "BLPlan : " + BLPlan + " = (" + Last_BL_Plan.ToString() + " + " + InRec.ToString() + ") - " + DT.Rows[i]["F_MRP"].ToString() + " + " + DT.Rows[i]["F_Urgent_Order"].ToString();
                                        BLActual = (Last_BL_Actual + InActual) - int.Parse(DT.Rows[i]["F_MRP"].ToString());
                                        BLActual_Solution = "BLActual : " + BLActual.ToString() + " = (" + Last_BL_Actual.ToString() + " + " + InActual.ToString() + ") - " + DT.Rows[i]["F_MRP"].ToString();

                                    }
                                    else if (DT.Rows[i]["Flag_HalfChg_BL_Stock"].ToString() == "True")
                                    {
                                        if (DT.Rows[i]["F_Process_Shift"].ToString() == "D")
                                        {
                                            BLPlan_Solution = "BL = BF - MRP/2";
                                            BLPlan = Last_BL_Plan - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2);
                                            BLPlan_Solution = BLPlan_Solution + "BLPlan : " + BLPlan + " = " + Last_BL_Plan.ToString() + " - " + (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2).ToString();

                                            BLActual = Last_BL_Actual - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2);
                                            BLActual_Solution = "BLActual : " + BLActual + " = " + Last_BL_Actual.ToString() + " - " + (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2).ToString();
                                        }
                                        else
                                        {
                                            BLPlan_Solution = "BL = (BF + In(Rec)) - MRP/2 + Urgent";
                                            BLPlan = (Last_BL_Plan + InRec) - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                            BLPlan_Solution = BLPlan_Solution + "BLPlan : " + BLPlan.ToString() + " = (" + Last_BL_Plan.ToString() + " + " + InRec.ToString() + ") - " + (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2).ToString() + " + " + DT.Rows[i]["F_Urgent_Order"].ToString();

                                            BLActual = (Last_BL_Actual + InRec) - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                            BLActual_Solution = "BLActual : " + BLActual.ToString() + " = (" + Last_BL_Actual.ToString() + " + " + InRec.ToString() + ") - " + (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2).ToString() + " + " + DT.Rows[i]["F_Urgent_Order"].ToString();
                                        }
                                    }

                                }
                                else
                                {
                                    BLPlan_Solution = "BL = (BL + In(Rec)) + Urgent - Abnormal";
                                    BLPlan = (Last_BL_Plan + InRec)
                                        + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString())
                                        - int.Parse(DT.Rows[i]["F_AbNormal_Part"].ToString());

                                    BLPlan_Solution = BLPlan_Solution + "BLPlan : " + BLPlan.ToString() + " = (" + Last_BL_Plan.ToString() + " + " + InRec.ToString() + ") + " + DT.Rows[i]["F_Urgent_Order"].ToString() + " - " + DT.Rows[i]["F_AbNormal_Part"].ToString();

                                    BLActual = (Last_BL_Actual + InActual) - int.Parse(DT.Rows[i]["F_AbNormal_Part"].ToString());
                                    BLActual_Solution = "BLActual : " + BLActual.ToString() + " = (" + Last_BL_Actual.ToString() + " + " + InActual.ToString() + ") - " + DT.Rows[i]["F_AbNormal_Part"].ToString();

                                }
                                Last_BL_Actual = BLActual;
                                Last_BL_Plan = BLPlan;
                            }
                            else
                            {
                                BLPlan_Solution = "BL = (BL + In(Rec)) + Urgent";
                                BLPlan = (Last_BL_Plan + InRec) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                BLPlan_Solution = BLPlan_Solution + "BLPlan : " + BLPlan.ToString() + " = (" + Last_BL_Plan.ToString() + " + " + InRec.ToString() + ") + " + DT.Rows[i]["F_Urgent_Order"].ToString();

                                BLActual = (Last_BL_Actual + InActual);
                                BLActual_Solution = "BLActual : " + BLActual.ToString() + " = (" + Last_BL_Actual.ToString() + " + " + InActual.ToString() + ")";

                                Last_BL_Actual = BLActual;
                                Last_BL_Plan = BLPlan;
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

                                        BLPlan_Solution = "BL = (BL + In(Rec)) - MRP + Urgent";
                                        BLPlan = (Last_BL_Plan + InRec) - int.Parse(DT.Rows[i]["F_MRP"].ToString())
                                            + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                        BLPlan_Solution = BLPlan_Solution + "BLPlan : " + BLPlan.ToString() + " = (" + Last_BL_Plan.ToString() + " + " + InRec.ToString() + ") - " + DT.Rows[i]["F_MRP"].ToString() + " + " + DT.Rows[i]["F_Urgent_Order"].ToString();

                                        BLActual = (Last_BL_Actual + InActual) - int.Parse(DT.Rows[i]["F_MRP"].ToString());
                                        BLActual_Solution = "BLActual : " + BLActual.ToString() + " = (" + Last_BL_Actual.ToString() + " + " + InActual.ToString() + ") - " + DT.Rows[i]["F_MRP"].ToString();

                                    }
                                    else if (DT.Rows[i]["Flag_HalfChg_BL_Stock"].ToString() == "True")
                                    {
                                        if (DT.Rows[i]["F_Process_Shift"].ToString() == "D")
                                        {
                                            BLPlan_Solution = "BL = BF - MRP/2";
                                            BLPlan = Last_BL_Plan - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2);
                                            BLPlan_Solution = BLPlan_Solution + "BLPlan : " + BLPlan.ToString() + " = " + Last_BL_Plan.ToString() + " - " + (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2).ToString();

                                            BLActual = Last_BL_Actual - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2);
                                            BLActual_Solution = "BLActual : " + BLActual.ToString() + " = " + Last_BL_Actual.ToString() + " - " + (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2).ToString();

                                        }
                                        else
                                        {

                                            BLPlan_Solution = "BL = (BF + In(Rec)) - MRP/2 + Urgent";
                                            BLPlan = (Last_BL_Plan + InRec) - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                            BLPlan_Solution = BLPlan_Solution + "BLPlan : " + BLPlan + " = (" + Last_BL_Plan.ToString() + " + " + InRec + ") - " + (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2).ToString() + " + " + DT.Rows[i]["F_Urgent_Order"].ToString();

                                            BLActual = (Last_BL_Actual + InRec) - (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2) + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                            BLActual_Solution = "BLActual : " + BLActual + " = (" + Last_BL_Actual.ToString() + " + " + InRec + ") - " + (int.Parse(DT.Rows[i]["F_MRP"].ToString()) / 2).ToString() + " + " + DT.Rows[i]["F_Urgent_Order"].ToString();

                                        }

                                    }
                                }
                                else
                                {

                                    //สูตร BL T1 = [ BL(Last Trip) + In(Rec) Pcs ] - MRP + Urgent - Abnormal
                                    BLPlan_Solution = "BL = (BL + In(Rec)) - MRP + Urgent - Abnormal";
                                    BLPlan = (Last_BL_Plan + InRec) - int.Parse(DT.Rows[i]["F_MRP"].ToString())
                                        + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString())
                                        - int.Parse(DT.Rows[i]["F_AbNormal_Part"].ToString());
                                    BLPlan_Solution = BLPlan_Solution + "BLPlan : " + BLPlan + " = (" + Last_BL_Plan.ToString() + " + " + InRec + ") - " + DT.Rows[i]["F_MRP"].ToString() + " + " + DT.Rows[i]["F_Urgent_Order"].ToString() + " - " + DT.Rows[i]["F_AbNormal_Part"].ToString();

                                    BLActual = (Last_BL_Actual + InActual) - int.Parse(DT.Rows[i]["F_MRP"].ToString())
                                    - int.Parse(DT.Rows[i]["F_AbNormal_Part"].ToString());
                                    BLActual_Solution = "BLActual : " + BLActual + " = (" + Last_BL_Actual.ToString() + " + " + InActual + ") - " + DT.Rows[i]["F_MRP"].ToString() + " - " + DT.Rows[i]["F_AbNormal_Part"].ToString();

                                }

                                Last_BL_Actual = BLActual;
                                Last_BL_Plan = BLPlan;

                            }
                            else
                            {
                                BLPlan_Solution = "BL = (BL + In(Rec)) - MRP + Urgent";
                                BLPlan = (Last_BL_Plan + InRec) - int.Parse(DT.Rows[i]["F_MRP"].ToString())
                                    + int.Parse(DT.Rows[i]["F_Urgent_Order"].ToString());
                                BLPlan_Solution = BLPlan_Solution + "BLPlan : " + BLPlan + " = (" + Last_BL_Plan.ToString() + " + " + InRec + ") - " + DT.Rows[i]["F_MRP"].ToString() + " + " + DT.Rows[i]["F_Urgent_Order"].ToString();

                                BLActual = (Last_BL_Actual + InActual) - int.Parse(DT.Rows[i]["F_MRP"].ToString());
                                BLActual_Solution = "BLActual : " + BLActual + " = (" + Last_BL_Actual.ToString() + " + " + InActual + ") - " + DT.Rows[i]["F_MRP"].ToString();

                                Last_BL_Actual = BLActual;
                                Last_BL_Plan = BLPlan;
                            }
                        }

                        //Update BL ของวันนั้นๆ
                        _execSQL = $@"exec [dbo].[sp_autoRecalculateBL_UpdateBL] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11";

                        int _RowAffected = _KB3Context.Database.ExecuteSqlRaw(_execSQL, DT.Rows[i]["F_Process_Date"].ToString(), DT.Rows[i]["F_Process_Shift"].ToString(),
                            DT.Rows[i]["F_Process_Round"].ToString(), DT.Rows[i]["F_Supplier_Code"].ToString(), DT.Rows[i]["F_Supplier_Plant"].ToString(),
                            DT.Rows[i]["F_Part_No"].ToString(), DT.Rows[i]["F_Ruibetsu"].ToString(),
                            DT.Rows[i]["F_Kanban_No"].ToString(), DT.Rows[i]["F_Store_Code"].ToString(),
                            BLPlan, BLActual, DT.Rows[i]["F_Not_Recalculate"].ToString());

                        if (_RowAffected > 0)
                        {
                            _Log.WriteLogMsg($"Update TB_Calculate_D : Complete | Query : {_execSQL} | BLPlan_Solution : {BLPlan_Solution} | BLActual_Solution : {BLActual_Solution} " +
                                $"Data : {JsonConvert.SerializeObject(DT.Rows[i])}");
                        }
                        else
                        {
                            _Log.WriteLogMsg($"Update TB_Calculate_D : Not Complete | Query : {_execSQL} | BLPlan_Solution : {BLPlan_Solution} | BLActual_Solution : {BLActual_Solution}" +
                                $"Data : {JsonConvert.SerializeObject(DT.Rows[i])}");
                        }
                        if (_intRow == 5)
                        {
                            Console.WriteLine("5");
                        }
                        if (DT.Rows[i]["F_Not_Recalculate"].ToString() == "True")
                        {
                            Last_BL_Plan = int.TryParse(DT.Rows[i]["F_BL_SET_Plan"].ToString(), out int F_BL_SET_Plan) ? F_BL_SET_Plan : 0;
                            Last_BL_Actual = int.TryParse(DT.Rows[i]["F_BL_SET_Actual"].ToString(), out int F_BL_SET_Actual) ? F_BL_SET_Actual : 0;
                            blnFromSetStock = true;
                        }
                        else
                        {
                            blnFromSetStock = false;
                        }

                        if (DT.Rows[i]["F_Process_Date"].ToString() == dateECI.Begining_Calculate
                            && DT.Rows[i]["F_Process_Round"].ToString() == "1")
                        {
                            dateDelivery = DateTime.TryParseExact(DT.Rows[i]["F_Process_Date"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateDelivery) ? dateDelivery : new DateTime();

                            _execSQL = "exec [dbo].[sp_autoRecalculateBL_First] @p0,@p1,@p2,@p3,@p4,@p5,@p6";
                            DT_LastBL = _FillDT.ExecuteSQL(_execSQL, dateDelivery.AddDays(-1).ToString("yyyyMMdd"),
                                        DT_PartControl.Rows[_intRow]["F_Supplier_Code"].ToString(), DT_PartControl.Rows[_intRow]["F_Supplier_Plant"].ToString(),
                                        DT_PartControl.Rows[_intRow]["F_Part_No"].ToString(), DT_PartControl.Rows[_intRow]["F_Ruibetsu"].ToString(),
                                        DT_PartControl.Rows[_intRow]["F_Kanban_No"].ToString(), DT_PartControl.Rows[_intRow]["F_Store_Code"].ToString());

                            if (DT_LastBL.Rows.Count > 0)
                            {
                                Last_BL_Plan = int.TryParse(DT_LastBL.Rows[0]["F_BL_SET_Plan"].ToString(), out int F_BL_SET_Plan) ? F_BL_SET_Plan : 0;
                                Last_BL_Actual = int.TryParse(DT_LastBL.Rows[0]["F_BL_SET_Actual"].ToString(), out int F_BL_SET_Actual) ? F_BL_SET_Actual : 0;
                            }
                            else
                            {
                                Last_BL_Plan = 0;
                                Last_BL_Actual = 0;
                            }
                        }
                    }
                    _KB3Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _KB3Transaction.Rollback();
                throw new Exception(ex.Message);
            }

        }

        private async Task<DateECI> get_ECIDate(string start_date, string end_date, int _intRow)
        {
            DateECI dateECI = new DateECI();
            DataTable DT = new DataTable();
            DateTime dateLast_Trip = new DateTime();

            try
            {
                dateLast_Trip = DateTime.TryParseExact(start_date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateLast_Trip) ? dateLast_Trip : new DateTime();

                string _SQL = $@"Select F_Local_Str FROM T_Construction 
                    WHERE F_Part_no = '{DT_PartControl.Rows[_intRow]["F_Part_No"].ToString()}' 
                    AND F_Ruibetsu = '{DT_PartControl.Rows[_intRow]["F_Ruibetsu"].ToString()}' 
                    AND F_Store_cd = '{DT_PartControl.Rows[_intRow]["F_Store_Code"].ToString()}' 
                    AND F_supplier_cd = '{DT_PartControl.Rows[_intRow]["F_Supplier_Code"].ToString()}' 
                    AND F_plant = '{DT_PartControl.Rows[_intRow]["F_Supplier_Plant"].ToString()}' 
                    AND F_Sebango = '{DT_PartControl.Rows[_intRow]["F_Kanban_No"].ToString().Substring(1, 3)}' ";

                DT = _FillDT.ExecuteSQLPPMDB(_SQL);

                if (DT.Rows.Count > 0)
                {
                    dateECI.Begining_Date = DT.Rows[0]["F_Local_Str"].ToString();

                }
                else
                {
                    dateECI.Begining_Date = dateLast_Trip.ToString("yyyyMMdd");
                }

                _SQL = $"SELECT MIN(F_Process_Date) F_Process_Date " +
                    $"FROM TB_Calculate_D WHERE F_Supplier_Code = '{DT_PartControl.Rows[_intRow]["F_Supplier_Code"].ToString()}' " +
                    $"AND F_Supplier_Plant = '{DT_PartControl.Rows[_intRow]["F_Supplier_Plant"].ToString()}' " +
                    $"AND F_Part_No = '{DT_PartControl.Rows[_intRow]["F_Part_No"].ToString()}' " +
                    $"AND F_Ruibetsu = '{DT_PartControl.Rows[_intRow]["F_Ruibetsu"].ToString()}' " +
                    $"AND F_Store_Code = '{DT_PartControl.Rows[_intRow]["F_Store_Code"].ToString()}' " +
                    $"AND F_Kanban_No = '{DT_PartControl.Rows[_intRow]["F_Kanban_No"].ToString()}' ";

                DT = _FillDT.ExecuteSQL(_SQL);
                if (DT.Rows.Count > 0)
                {
                    dateECI.Begining_Calculate = DT.Rows[0]["F_Process_Date"].ToString();
                }
                else
                {
                    dateECI.Begining_Calculate = dateLast_Trip.ToString("yyyyMMdd");
                }

                return dateECI;
            }
            catch (Exception ex)
            {
                _Log.WriteLogMsg("Update TB_Calculate_D :  Not Complete");
                throw new Exception(ex.Message);
            }


        }

        [HttpPost]
        public async Task<IActionResult> Recalculate(VMKBNOR121_Recal obj)
        {
            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                _KB3Transaction.CreateSavepoint("Start Recalculate");
                string _sql = "SELECT CASE WHEN F_Value2 = 5 THEN 1 ELSE 0 END F_GENERATING " +
                    "FROM TB_MS_Parameter WHERE F_Code = 'ST' ";
                DataTable _dt = _FillDT.ExecuteSQL(_sql);

                if (_dt.Rows.Count > 0)
                {
                    if (_dt.Rows[0]["F_GENERATING"].ToString() == "1")
                    {
                        return BadRequest(new
                        {
                            status = "400",
                            response = "Bad Request",
                            title = "Error",
                            message = "กำลัง Generate PDS for Normal Order Data ไม่สามารถ Re-Calculate ได้"
                        });
                    }
                }

                _sql = "SELECT * FROM TB_PDS_HEADER WHERE F_OrderType = 'N' ";
                _dt = _FillDT.ExecuteSQL(_sql);
                if (_dt.Rows.Count > 0)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "ขณะนี้ Generate PDS for Normal Order Data ไปแล้ว กรุณา Generate PDS for Normal Order Data อีกครั้ง"
                    });
                }

                if (Login_Shift.ToUpper() == "DAY")
                {
                    _sql = $"exec [dbo].[SP_CALCULATE_KBNOR120] '{ProcessDate.ToString("yyyyMMdd")}'" +
                        $",'{obj.Supplier.Substring(0, 4)}','{obj.Supplier.Substring(5, 1)}'" +
                        $",'{obj.Store}','{obj.Kanban}','{obj.PartNo.Split("-")[0]}','{obj.PartNo.Split("-")[1]}'";

                    var count = await _KB3Context.Database.ExecuteSqlRawAsync(_sql);

                    if (count > 0)
                    {
                        _Log.WriteLogMsg("message : Update TB_Calculate_D : SP_CALCULATE_KBNOR120");
                    }

                }
                else
                {
                    string sEndDate = "";
                    string sLastDate = "";

                    sEndDate = _KB3Context.Database.SqlQueryRaw<string>($"select dbo.FN_GET14Day('{ProcessDate.ToString("yyyyMMdd")}') AS VALUE").FirstOrDefault();
                    _sql = "exec [dbo].[sp_Calculate_kanban] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7";
                    _dt = _FillDT.ExecuteSQL(_sql, obj.Supplier.Split("-")[0], obj.Supplier.Split("-")[1],
                        obj.PartNo.Split("-")[0], obj.PartNo.Split("-")[1], obj.Store, obj.Kanban, ProcessDate.ToString("yyyyMMdd"), sEndDate);

                    _Log.WriteLogMsg("message : Update TB_Calculate_D : sp_Calculate_kanban");

                    _sql = $"UPDATE TB_Calculate_Volume SET F_QTY = D.F_Actual_Order " +
                    $"FROM TB_Calculate_D D INNER JOIN TB_Calculate_Volume V " +
                    $"ON D.F_Supplier_Code = V.F_Supplier_Code AND D.F_Supplier_Plant = V.F_Supplier_Plant " +
                    $"AND D.F_Part_No = V.F_Part_No AND D.F_Ruibetsu = V.F_Ruibetsu " +
                    $"AND D.F_Store_Code = V.F_Store_Code AND D.F_Kanban_No = V.F_Kanban_No " +
                    $"AND D.F_Process_Date = V.F_Process_Date AND D.F_Process_Round = V.F_Process_Round " +
                    $"WHERE V.F_Lock = '0' AND D.F_Process_Date >= '{ProcessDate.ToString("yyyyMMdd")}' " +
                    $"AND D.F_Supplier_Code = '{obj.Supplier.Split("-")[0]}' " +
                    $"AND D.F_Supplier_Plant = '{obj.Supplier.Split("-")[1]}' " +
                    $"AND D.F_Part_No = '{obj.PartNo.Split("-")[0]}' " +
                    $"AND D.F_Ruibetsu = '{obj.PartNo.Split("-")[1]}' " +
                    $"AND D.F_Kanban_No = '{obj.Kanban}' " +
                    $"AND D.F_Store_Code = '{obj.Store}' ";

                    await _KB3Context.Database.ExecuteSqlRawAsync(_sql);

                    sLastDate = _KB3Context.Database.SqlQueryRaw<string>($"select dbo.FN_GetLastDate('{ProcessDate.ToString("yyyyMMdd")}') AS VALUE").FirstOrDefault();

                    _sql = $"UPDATE TB_Calculate_D SET F_Urgent_Order = U.F_Unit_Amount ,Flag_Chg_Urgent = '1' " +
                        $"FROM TB_Calculate_D D INNER JOIN (Select H.F_Supplier_Code, H.F_Supplier_Plant, H.F_Delivery_Date, H.F_Delivery_Trip " +
                        $" , H.F_Delivery_Dock, D.F_Part_No, D.F_Ruibetsu, D.F_Kanban_No " +
                        $", SUM(D.F_Unit_Amount) AS F_Unit_Amount From TB_Rec_Header H INNER JOIN TB_REC_Detail D " +
                        $"On H.F_OrderNo = D.F_OrderNo Where H.F_orderType = 'U' And SUBSTRING(H.F_OrderNo,11,1) = 'U' " +
                        $"And CONVERT(char(8),F_Issued_Date,112) >= '{sLastDate}' And F_Status = 'N' " +
                        $"GROUP BY H.F_Supplier_Code, H.F_Supplier_Plant , H.F_Delivery_Date, H.F_Delivery_Trip, H.F_Delivery_Dock " +
                        $", D.F_Part_No, D.F_Ruibetsu, D.F_kanban_No) U ON D.F_Supplier_Code = U.F_Supplier_Code " +
                        $"AND D.F_Supplier_Plant = U.F_Supplier_Plant collate Thai_CI_AS " +
                        $"AND D.F_Process_Date = U.F_Delivery_date AND D.F_Process_Round = REPLACE(U.F_Delivery_Trip,'0','1') " +
                        $"AND D.F_Store_Code = U.F_Delivery_Dock AND D.F_Part_No = U.F_Part_No " +
                        $"AND D.F_Ruibetsu = U.F_Ruibetsu AND D.F_kanban_No = U.F_Kanban_No ";

                    await _KB3Context.Database.ExecuteSqlRawAsync(_sql);

                    _sql = $"UPDATE TB_Calculate_H SET F_Urgent_order = D.F_Urgent_Order " +
                        $"FROM (Select F_Supplier_Code, F_Supplier_Plant " +
                        $" , F_Part_No, F_Ruibetsu, F_Store_Code, F_Kanban_No " +
                        $", F_Process_Date, SUM(F_Urgent_order) AS F_Urgent_order " +
                        $"From TB_Calculate_D " +
                        $"Group By F_Supplier_Code, F_Supplier_Plant, F_Part_No, F_Ruibetsu " +
                        $", F_Store_Code, F_Kanban_No, F_Process_Date ) D " +
                        $"INNER JOIN TB_Calculate_H H " +
                        $"ON D.F_Supplier_Code = H.F_Supplier_Code " +
                        $"AND D.F_Supplier_Plant = H.F_Supplier_Plant  " +
                        $"AND D.F_Part_No = H.F_Part_No AND D.F_Ruibetsu = H.F_Ruibetsu " +
                        $"AND D.F_Store_Code = H.F_Store_Code AND D.F_Kanban_No = H.F_Kanban_No " +
                        $"AND D.F_Process_Date = H.F_Process_Date WHERE D.F_Urgent_Order > 0 " +
                        $"AND H.F_Process_Date >= '{sLastDate}' ";

                    await _KB3Context.Database.ExecuteSqlRawAsync(_sql);

                    _sql = $"exec [dbo].[SP_RecalBL_Night] '{ProcessDate.ToString("yyyyMMdd")}'," +
                        $"'{obj.Supplier.Split("-")[0]}','{obj.Supplier.Split("-")[1]}'," +
                        $"'{obj.Store}','{obj.Kanban}'";

                    await _KB3Context.Database.ExecuteSqlRawAsync(_sql);

                    _Log.WriteLogMsg("message : Update TB_Calculate_D : SP_RecalBL_Night");

                    _sql = $"exec [dbo].[SP_CALCULATE_OTHER_CONDITION] '{ProcessDate.ToString("yyyyMMdd")}'";
                    await _KB3Context.Database.ExecuteSqlRawAsync(_sql);

                    _Log.WriteLogMsg("message : Update TB_Calculate_D : SP_CALCULATE_OTHER_CONDITION");
                }

                _KB3Transaction.Commit();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Recalculate Success"
                });
            }
            catch (Exception ex)
            {
                _KB3Transaction.RollbackToSavepoint("Start Recalculate");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = ex.InnerException?.Message ?? ex.Message

                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInformNews(TB_MS_Inform_News obj)
        {
            try
            {
                var isExisted = _KB3Context.TB_MS_Inform_News.Any(x => x.Equals(obj));

                if (isExisted)
                {
                    _KB3Context.TB_MS_Inform_News.Update(obj);
                }
                else
                {
                    _KB3Context.TB_MS_Inform_News.Add(obj);
                }

                await _KB3Context.SaveChangesAsync();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Update Inform News Success"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = ex.InnerException?.Message ?? ex.Message

                });
            }
        }
    }
}
