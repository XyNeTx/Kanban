using HINOSystem.Context;
using HINOSystem.Controllers.KBN;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.KB3.OrderingProcess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Net;

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
            IConfiguration configuration,
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
            _configuration = configuration;
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
        private static DateTime dateLogin = DateTime.Now.Date;
        private static string Txt_Shift = "Day";
        private static string Txt_MRPStatus = "MRP : " + Now.Date.ToString();
        private static string UserCode = "";
        private static string Plant = "";

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

        [HttpGet]
        public IActionResult OnLoad(string Shift , string Process_Date)
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

                Txt_Shift = (Shift.Substring(0, 1) == "1") ? "Day" : "Night";
                UserCode = _BearerClass.UserCode;
                Plant = _BearerClass.Plant;
                ProcessDate = DateTime.ParseExact(Process_Date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                dateLogin = ProcessDate.AddDays(-1).Date;

                string appUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                string UserIPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string UserIPAddress2 = Request.HttpContext.Connection.LocalIpAddress.ToString();


                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Onloading is success.",
                    data = new
                    {
                        UserIPAddress = UserIPAddress,
                        UserIPAddress2 = UserIPAddress2,
                        appUserName = appUserName
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
                    message = "Onloading is error.",
                    error = ex.Message
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

                var dt = _FillDT.ExecuteSQL_Param("EXEC [dbo].[sp_NormalOrder_getSupplier] @Plant",sqlParameters);

                if(dt.Rows.Count == 0)
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
                    message = "Unexpected Error !!",
                    error = ex.Message
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
                    new SqlParameter("@OrderType", "Daily"), // Assuming "Daily" is always required
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

                if(dt.Rows.Count == 0)
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
                    message = "Unexpected Error !!",
                    error = ex.Message
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
                    "Daily");

                if(dt.Rows.Count == 0)
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
                    message = "Unexpected Error !!",
                    error = ex.Message
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
                    "Daily");

                if(dt.Rows.Count == 0)
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
                    message = "Unexpected Error !!",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Find_StartEnd_Date(VMKBNOR121_Preview obj)
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

                if (obj.Action == "Preview")
                {
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
                        storeCalendar, dateLogin.ToString("yyyyMMdd"));

                    if (NoDayPreview.Rows.Count == 0)
                    {
                        return NotFound(new
                        {
                            status = "404",
                            response = "Not Found",
                            title = "Error",
                            message = "NumberOfDayToPreview Not Found"
                        });
                    }

                    Start_Date = NoDayPreview.Rows[0]["Start_Date"].ToString();
                    End_Date = NoDayPreview.Rows[0]["End_Date"].ToString();
                    intAmountShow = int.Parse(NoDayPreview.Rows[0]["Display_Date"].ToString());

                }

                DT_DeliveryDate = _FillDT.ExecuteSQL("exec [dbo].[sp_getDeliveryDateTrip] @p0,@p1,@p2,@p3,@p4,@p5,@p6",
                    Plant, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1), ProcessDate.ToString("yyyyMMdd"), Txt_Shift.Substring(0, 1),
                    string.IsNullOrWhiteSpace(obj.Store) ? DBNull.Value : obj.Store, string.IsNullOrWhiteSpace(obj.StoreTo) ? DBNull.Value : obj.StoreTo);

                if (DT_DeliveryDate.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Delivery Date Not Found"
                    });
                }

                dateDelivery = DateTime.TryParseExact(DT_DeliveryDate.Rows[0]["F_Delivery_Date"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result) ? result : DateTime.Now;
                DeliveryTrip = DT_DeliveryDate.Rows[0]["F_Delivery_Trip"].ToString().Trim();

                DT_Date = _FillDT.ExecuteSQL("exec [dbo].[sp_getCycleTime] @p0,@p1,@p2,@p3",
                                     obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1), Start_Date, End_Date);

                if (DT_Date.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Cycle Time Not Found"
                    });
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

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Start and End Date Complete",
                    data = JsonConvert.SerializeObject(DT_Period)
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error !",
                    message = "Unexpected Error !",
                    error = ex.Message
                });
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

                DT_PartControl.Clear();
                DT_Header.Clear();
                DT_Detail.Clear();
                DT_Volume.Clear();
                DT_AdjustOrder_Trip.Clear();
                DT_Actual_Receive.Clear();


                string _SQL = "exec [dbo].[sp_DT_PartControl] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12";

                DT_PartControl = _FillDT.ExecuteSQL(_SQL,
                    Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                    obj.Kanban, obj.KanbanTo, obj.Store, obj.StoreTo,
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(11, 2),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(11, 2), "Daily");

                if(DT_PartControl.Rows.Count == 0)
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

                }

                _SQL = "exec [dbo].[sp_DT_Header] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12";

                DT_Header = _FillDT.ExecuteSQL(_SQL,
                    Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                    obj.Kanban, obj.KanbanTo, obj.Store, obj.StoreTo,
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(11, 2),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(11, 2), "Daily");

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

                DT_Detail = _FillDT.ExecuteSQL(_SQL,
                    Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                    obj.Kanban, obj.KanbanTo, obj.Store, obj.StoreTo,
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(11, 2),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(11, 2), "Daily");

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

                DT_Volume = _FillDT.ExecuteSQL(_SQL,
                    Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                    obj.Kanban, obj.KanbanTo, obj.Store, obj.StoreTo,
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(11, 2),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(11, 2), "Daily");

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

                DT_AdjustOrder_Trip = _FillDT.ExecuteSQL(_SQL,
                    Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                    obj.Kanban, obj.KanbanTo, obj.Store, obj.StoreTo,
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(11, 2),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(11, 2), "Daily");

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

                DT_Actual_Receive = _FillDT.ExecuteSQL(_SQL,
                    Start_Date, End_Date, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                    obj.Store, obj.StoreTo,
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(0, 10),
                    string.IsNullOrWhiteSpace(obj.PartNo) ? DBNull.Value : obj.PartNo.Substring(11, 2),
                    string.IsNullOrWhiteSpace(obj.PartNoTo) ? DBNull.Value : obj.PartNoTo.Substring(11, 2));


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
                    message = "Unexpected Error !!",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail_Data(int intRow,string F_Supplier_Cd)
        {
            try
            {
                int ForecastMaxInt = 0;
                var ForecastMax = _FillDT.ExecuteSQL("EXEC [dbo].[sp_getForecastMax_New] " +
                    "@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7",Plant,F_Supplier_Cd.Substring(0,4),F_Supplier_Cd.Substring(5,1),
                    DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim(), DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString(),
                    DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Substring(1,3), DT_PartControl.Rows[intRow]["F_Store_Code"].ToString(),
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

                dr = DT_Header.Select($@"F_Process_Date = '{dateDelivery.ToString("yyyyMMdd")}' 
                        AND F_Store_Code = '{DT_PartControl.Rows[intRow]["F_Store_Code"].ToString().Trim()}' 
                        AND F_Kanban_No = '{DT_PartControl.Rows[intRow]["F_Kanban_No"].ToString().Trim()}' 
                        AND F_Part_No = '{DT_PartControl.Rows[intRow]["F_Part_No"].ToString().Trim()}' 
                        AND F_Ruibetsu = '{DT_PartControl.Rows[intRow]["F_Ruibetsu"].ToString().Trim()}' 
                        ")[0];

                int QtyPack = int.Parse(dr["F_Qty_Box"].ToString().Trim()) == 0 ? 1 : int.Parse(dr["F_Qty_Box"].ToString().Trim());
                
                if(dr == null)
                {
                    QtyPack = 0;
                }

                string sourceKB = Plant switch
                {
                    "1" => "[hmmt-E_Kanban].[New_Kanban]",
                    "2" => "[hmmt-E_Kanban].[New_Kanban_F2]",
                    "3" => "[New_Kanban_F3]",
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

                _SQL = $@"SELECT F_Max_Area FROM TB_MS_MaxArea_Stock WHERE F_Plant = '"+ Plant + "' " +
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

                string STDStock = string.IsNullOrWhiteSpace(dtSTDStock.Rows[0]["STDStock"].ToString()) ? "0" :  (Math.Round(float.Parse(dtSTDStock.Rows[0]["STDStock"].ToString()))).ToString();

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
                    "AND F_Process_Date = '" + dateLogin.ToString("yyyyMMdd") + "'";

                var dtMRP = _FillDT.ExecuteSQL(_SQL);

                float MRP = float.Parse(dtMRP.Rows[0]["F_MRP"].ToString());
                float HMMT_Prod = float.Parse(dtMRP.Rows[0]["F_HMMT_Prod"].ToString());
                string MRPCheck = "";
                if(HMMT_Prod * 0.8 > MRP)
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
                string SetOrderCheck = "KanbanOrderChecked";

                if (dtSetOrder.Rows.Count == 0)
                {
                    SetOrderCheck = "";
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
                string ChgCycleCheck = "";

                if (dtDeliveryTime != null)
                {
                    ChgCycleCheck = "ChgCycleChecked";
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
                        AND F_Delivery_Trip = '{DeliveryTrip}  ) B '
                        WHERE A.Slide_Order + B.Slide_Order_Part > 0 ";

                var dtSlideOrder = _FillDT.ExecuteSQL(_SQL);
                string SlideOrderCheck = "";
                if (dtSlideOrder.Rows.Count > 0)
                {
                    SlideOrderCheck = "SlideOrderChecked";
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
                        WHERE A.Slide_Order + B.Slide_Order_Part > 0 ";

                var dtRecSlideOrder = _FillDT.ExecuteSQL(_SQL);
                string RecSlideOrderCheck = "";
                if (dtRecSlideOrder.Rows.Count > 0)
                {
                    RecSlideOrderCheck = "RecSlideOrderChecked";
                }

                string AvgTrip = (Math.Floor(((decimal)ForecastMaxInt / CycleB) / QtyPack) * QtyPack).ToString();

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
                        avgTrip = AvgTrip
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
                    message = "Unexpected Error !!",
                    error = ex.Message
                });
            }
        }
    }
}
