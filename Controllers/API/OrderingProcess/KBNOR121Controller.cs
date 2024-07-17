using HINOSystem.Context;
using HINOSystem.Controllers.KBN;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.KB3.OrderingProcess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

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
        private static readonly DateTime dateLogin = DateTime.Now.Date;
        private static string Txt_Shift = "Day";
        private static string Txt_MRPStatus = "MRP : " + Now.Date.ToString();
        private static string UserCode = "";
        private static string Plant = "";

        private static string Start_Date = "";
        private static string End_Date = "";
        private static int intAmountShow = 0;
        private static string dateDelivery = "";
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
                        storeCalendar, ProcessDate.ToString("yyyyMMdd"));

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

                dateDelivery = DT_DeliveryDate.Rows[0]["F_Delivery_Date"].ToString().Trim();
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

        //[HttpPost]
        //public async Task<IActionResult> Detail_Data(int Page)
        //{
        //    try
        //    {
        //        if (dateDelivery == "")
        //        {
        //            dateDelivery = ProcessDate 
        //        }
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            status = "500",
        //            response = "Internal Server Error",
        //            title = "Error",
        //            message = "Unexpected Error !!",
        //            error = ex.Message
        //        });
        //    }
        //}
    }
}
