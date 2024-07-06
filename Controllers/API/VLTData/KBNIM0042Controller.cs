using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNIM0042Controller : ControllerBase
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

        public KBNIM0042Controller(
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



        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                if (pData != null) _json = JsonConvert.DeserializeObject(pData);

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_MS_Factory"" : " + _jsTB_MS_Factory + @"
                            }
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerCode()
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

                DataTable dt = _FillDT.ExecuteSQL("Select Distinct F_Customer_Cd From TB_MS_VLT_CUSTOMER Order by F_Customer_Cd");
                if(dt.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Not Found",
                        message = "Data Not Found!"
                    });
                }
                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found!",
                    data = JsonConvert.SerializeObject(dt)
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStartJigIn(string F_Customer_Cd)
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

                DataTable Frame = _FillDT.ExecuteSQL("EXEC [exec].[spKBNIM0042_GetSeq] @p0,@p1,@p2,@p3"
                    , _BearerClass.Plant, "VLT", F_Customer_Cd, "Frame");
                
                DataTable Dedion = _FillDT.ExecuteSQL("EXEC [exec].[spKBNIM0042_GetSeq] @p0,@p1,@p2,@p3"
                    , _BearerClass.Plant, "VLT", F_Customer_Cd, "DEDION");
                
                DataTable TG = _FillDT.ExecuteSQL("EXEC [exec].[spKBNIM0042_GetSeq] @p0,@p1,@p2,@p3"
                    , _BearerClass.Plant, "VLT", F_Customer_Cd, "Tail Gate");

                DataTable RR = _FillDT.ExecuteSQL("EXEC [exec].[spKBNIM0042_GetSeq] @p0,@p1,@p2,@p3"
                    ,_BearerClass.Plant,"VLT",F_Customer_Cd,"RR Axle");

                DataTable FrameCKD = _FillDT.ExecuteSQL("EXEC [exec].[spKBNIM0042_GetSeq_CKD] @p0,@p1,@p2,@p3"
                    , _BearerClass.Plant, "VLT", F_Customer_Cd, "Frame");
                
                DataTable DedionCKD = _FillDT.ExecuteSQL("EXEC [exec].[spKBNIM0042_GetSeq_CKD] @p0,@p1,@p2,@p3"
                    , _BearerClass.Plant, "VLT", F_Customer_Cd, "DEDION");
                
                DataTable TGCKD = _FillDT.ExecuteSQL("EXEC [exec].[spKBNIM0042_GetSeq_CKD] @p0,@p1,@p2,@p3"
                    , _BearerClass.Plant, "VLT", F_Customer_Cd, "Tail Gate");

                DataTable RRCKD = _FillDT.ExecuteSQL("EXEC [exec].[spKBNIM0042_GetSeq_CKD] @p0,@p1,@p2,@p3"
                    , _BearerClass.Plant,"VLT",F_Customer_Cd,"RR Axle");

                if (Frame.Rows.Count == 0 && Dedion.Rows.Count == 0 && TG.Rows.Count == 0 && RR.Rows.Count == 0 && 
                    FrameCKD.Rows.Count == 0 && DedionCKD.Rows.Count == 0 && TGCKD.Rows.Count == 0 && RRCKD.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Not Found",
                        message = "Data Not Found!"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found!",
                    data = new
                    {
                        frame = JsonConvert.SerializeObject(Frame),
                        dedion = JsonConvert.SerializeObject(Dedion),
                        tg = JsonConvert.SerializeObject(TG),
                        rr = JsonConvert.SerializeObject(RR),
                        frameCKD = JsonConvert.SerializeObject(FrameCKD),
                        dedionCKD = JsonConvert.SerializeObject(DedionCKD),
                        tgCKD = JsonConvert.SerializeObject(TGCKD),
                        rrCKD = JsonConvert.SerializeObject(RRCKD)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEndJigIn(string F_Customer_Cd,string F_Part_Type,int F_Remain_Unit)
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

                DataTable EndSeq = _FillDT.ExecuteSQL("EXEC [exec].[spKBNIM0042_GetEndSeq] @p0,@p1,@p2,@p3,@p4",
                    _BearerClass.Plant,"VLT",F_Customer_Cd,F_Part_Type,F_Remain_Unit);

                DataTable EndSeqCKD = _FillDT.ExecuteSQL("EXEC [exec].[spKBNIM0042_GetEndSeq_CKD] @p0,@p1,@p2,@p3,@p4",
                                       _BearerClass.Plant, "VLT", F_Customer_Cd, F_Part_Type, F_Remain_Unit);

                if (EndSeq.Rows.Count == 0 && EndSeqCKD.Rows.Count == 0)
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Not Found",
                        message = "Data Not Found!"
                    });

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found!",
                    data = new
                    {
                        EndSeq = JsonConvert.SerializeObject(EndSeq),
                        EndSeqCKD = JsonConvert.SerializeObject(EndSeqCKD)
                    }
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
            }
        }
    }
}
