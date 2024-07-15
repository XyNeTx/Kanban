using HINOSystem.Context;
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

        public readonly string Txt_OrderType = "Daily Order";
        public readonly string Type_Import = "N";
        public static readonly DateTime Now = DateTime.Now;
        public static readonly DateTime Txt_Date = DateTime.Now.Date;
        public static string Txt_Shift = "Day";
        public static string Txt_MRPStatus = "MRP : " + Txt_Date.ToString();
        public static string UserCode = "";
        public static string Plant = "";

        public static string Start_Date = "";
        public static string End_Date = "";
        public static string DisplayAmount = "";
        public static string dateDelivery = "";
        public static string DeliveryTrip = "";
        public static DataTable DT_Period = new DataTable();

        [HttpGet]
        public IActionResult OnLoad(string Shift)
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
                    var NoDayPreview = _FillDT.ExecuteSQL("exec [dbo].[sp_NumberOfDayToPreview] @p0,@p1,@p2,@p3,@p4",
                        Plant, obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1),
                        "3C", Txt_Date.ToString("yyyyMMdd"));
                    
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
                    DisplayAmount = NoDayPreview.Rows[0]["Display_Date"].ToString();

                }

                var DelieryDateTrip = _FillDT.ExecuteSQL("exec [dbo].[sp_getDeliveryDateTrip] @p0,@p1,@p2,@p3,@p4,@p5,@p6",
                    Plant,obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1), Txt_Date.ToString("yyyyMMdd"),Txt_Shift.Substring(0,1),
                    string.IsNullOrWhiteSpace(obj.Store) ? DBNull.Value : obj.Store, string.IsNullOrWhiteSpace(obj.StoreTo) ? DBNull.Value : obj.StoreTo);
                
                if(DelieryDateTrip.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Delivery Date Not Found"
                    });
                }

                dateDelivery = DelieryDateTrip.Rows[0]["F_Delivery_Date"].ToString().Trim();
                DeliveryTrip = DelieryDateTrip.Rows[0]["F_Delivery_Trip"].ToString().Trim();

                var getCycleTime = _FillDT.ExecuteSQL("exec [dbo].[sp_getCycleTime] @p0,@p1,@p2,@p3",
                                     obj.Supplier.Substring(0, 4), obj.Supplier.Substring(5, 1), Start_Date, End_Date);

                if(getCycleTime.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Cycle Time Not Found"
                    });
                }

                if (Start_Date != getCycleTime.Rows[0]["F_Date"]) Start_Date = getCycleTime.Rows[0]["F_Date"].ToString();
                if (End_Date != getCycleTime.Rows[getCycleTime.Rows.Count - 1]["F_Date"]) End_Date = getCycleTime.Rows[getCycleTime.Rows.Count - 1]["F_Date"].ToString();
                DisplayAmount = getCycleTime.Rows.Count.ToString();

                DT_Period.Clear();
                for (int i = 0; i < getCycleTime.Rows.Count; i++)
                {
                    var dtAdd = _FillDT.ExecuteSQL("exec [dbo].[sp_findPeriod] @p0,@p1,@p2,@p3,@p4",
                        Plant,obj.Supplier.Substring(0,4),obj.Supplier.Substring(5,1),
                        getCycleTime.Rows[i]["F_Date"],UserCode);

                    
                    foreach (DataRow dr in dtAdd.Rows)
                    {
                        DT_Period.ImportRow(dr);
                    }

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
    }
}
