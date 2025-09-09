using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Libs;
using KANBAN.Models.ERP;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    public class KBNOR460Controller : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;
        private readonly IEmailService _IEmailService;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly BearerClass _Bearer;

        private Dictionary<string, string[]> SumDigit = new Dictionary<string, string[]>();


        private int[] _value = new int[43];
        private string[] _data = new string[43];

        public KBNOR460Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            KB3Context kB3Context,
            IEmailService iEmailService,
            FillDataTable fillDT,
            BearerClass Bearer
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
            _KB3Context = kB3Context;
            _IEmailService = iEmailService;
            _Bearer = Bearer;
            _FillDT = fillDT;

            // Initialize the value
            this.SumDigit["value"] = new string[43];
            this.SumDigit["data"] = new string[43];
        }



        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";

            _BearerClass.Authentication();
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            try
            {
                

                _SQL = @" EXEC [exec].[spKBNOR460_INITIAL] 
                    'U',
                    '" + User.FindFirst(ClaimTypes.Locality).Value + @"',
                    '" + User.FindFirst(ClaimTypes.UserData).Value + @"',
                    '' ";
                string _jsPDSNo = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""PDSNo"" : " + _jsPDSNo + @"
                            }
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailZeroPrice(SendEmailZeroPrice obj)
        {
            try
            {
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401)
                {
                    return Unauthorized(new
                    {
                        status = "401",
                        response = "Unauthorized",
                        title = "Error",
                        message = "Unauthorized"
                    });
                }


                string nDetail = "";
                string User_Logon = User.FindFirst(ClaimTypes.UserData).Value;
                obj.ProcessShift = obj.ProcessShift == "D" ? "1 : Day Shift" : "2 : Night Shift";

                string _sql = $"Select Distinct H.F_Supplier_Code +'-' + H.F_SUpplier_plant as F_Supplier,rtrim(S.F_SHort_name)+' : '+S.F_Name as F_Supplier_name, " +
                    $"rtrim(D.F_Part_NO) +'-' + D.F_Ruibetsu as F_Part_No, C.F_Part_Nm as F_Part_Name,H.F_Delivery_Date " +
                    $"From TB_REC_Header H Inner Join TB_REC_Detail D ON H.F_OrderNO = D.F_orderNo " +
                    $"INNER JOIN dbo.FN_GETPriceZeroUrgent('{User_Logon}','{obj.User_Name.Substring(0,10)}') Z " +
                    $"ON H.F_OrderNO = Z.F_ORDERNO and D.F_Part_No = Z.F_PART_NO and D.F_Ruibetsu = Z.F_Ruibetsu " +
                    $"Left Outer Join [HMMT-PPM].[PPMDB_TOTAL].dbo.T_Construction C ON " +
                    $"H.F_SUpplier_Code = C.F_Supplier_Cd collate Thai_CI_AS and H.F_Supplier_Plant = C.F_Plant collate Thai_CI_AS " +
                    $"and H.F_Delivery_Dock = C.F_Store_Cd collate Thai_CI_AS " +
                    $"and D.F_Part_no = C.F_Part_NO collate Thai_CI_AS and D.F_Ruibetsu = C.F_Ruibetsu collate Thai_CI_AS " +
                    $"and H.F_Delivery_Date >= C.F_TC_STR collate Thai_CI_AS and H.F_Delivery_Date <= C.F_TC_END collate Thai_CI_AS " +
                    $"Left Outer Join [HMMT-PPM].[PPMDB_TOTAL].dbo.T_Supplier_MS S ON " +
                    $"H.F_SUpplier_Code = S.F_Supplier_Cd collate Thai_CI_AS and H.F_SUpplier_Plant = S.F_Plant_Cd collate Thai_CI_AS " +
                    $"and H.F_Delivery_Dock = S.F_Store_Cd collate Thai_CI_AS " +
                    $"and H.F_Delivery_Date >= S.F_TC_STR collate Thai_CI_AS and H.F_Delivery_Date <= S.F_TC_END collate Thai_CI_AS " +
                    $" Where H.F_OrderTYpe='U' and H.F_Status='P' and H.F_Flg_EPro='9' " +
                    $" and H.F_Issued_BY ='{obj.User_Name.Substring(0,10)}' and H.F_Supplier_Code <> '0000' " +
                    $" and COnvert(CHar(8),H.F_issued_Date,112) = convert(Char(8),getdate(),112) " +
                    $"Order by 1,2,3,4,5 ";

                var _dt = _FillDT.ExecuteSQL(_sql);
                if(_dt.Rows.Count > 0)
                {
                    for(int i = 0; i < _dt.Rows.Count; i++) 
                    {
                        nDetail = $"{nDetail} {i + 1} Supplier Code : {_dt.Rows[i]["F_Supplier"].ToString().Trim()} " +
                            $"Supplier Name : {_dt.Rows[i]["F_Supplier_name"].ToString().Trim()} " +
                            $"Part No : {_dt.Rows[i]["F_Part_NO"].ToString().Trim()} " +
                            $"Part Name : {_dt.Rows[i]["F_Part_Name"].ToString().Trim()} " +
                            $"Delivery Date : {_dt.Rows[i]["F_Delivery_Date"].ToString().Trim()} <br/><br/>";
                    }
                    await _IEmailService.SendEmailAsync("Urgent Ordering", obj.Program, nDetail, obj.ProcessDate, obj.ProcessShift);
                }
                else
                {
                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        title = "Success",
                        message = "Not Found Data With Zero Price"
                    });
                }


                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Email sent successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Internal Server Error",
                    error = ex.Message
                });
            }
        }
    }
}
