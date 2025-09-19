using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Security.Claims;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT280Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _Serilog;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNRT280Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            PPMInvenContext pPMInvenContext,
            PPM3Context pPM3Context,
            KB3Context kB3Context,
            FillDataTable fillDataTable,
            SerilogLibs serilog
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPM3Context = pPM3Context;
            _PPMInvenContext = pPMInvenContext;
            _Serilog = serilog;
            _FillDT = fillDataTable;
        }

        
        public async Task<IActionResult> PrintReportSummary([FromBody] string data)
        {
            try
            {
                
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string orderChecked = _json["orderChecked"];
                bool delayChecked = _json["delayChecked"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                string UserName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value.ToString();
                string HostName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.WindowsDeviceClaim).Value.ToString();
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }
                string lastMonth = DateTime.ParseExact(dateFrom, "yyyyMMdd", CultureInfo.InvariantCulture).AddMonths(-1).ToString("yyyyMMdd");
                string Plant = HttpContext.Request.Cookies["plantCode"].ToString();
                string appendSql = "";

                await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC [dbo].[SP_RT280_SUM] '{Plant}','{orderChecked}','{dateFrom}','{dateTo}','{UserName}','{lastMonth}'");

                if (delayChecked)
                {
                    appendSql = " AND F_Delay > 0";
                }
                string sql = $"Select F_Plant, F_OrderTYpe, F_Start_Date, F_End_Date, F_Supplier_Code, F_Supplier_Plant, F_Short_Name, F_Name, F_Total_PDS, F_OnTime, F_Delay, F_Delay_Percent,F_Cancel From TMP_RT280_SUM Where F_Update_By = '{UserName}' ";
                DataTable rptDT = _FillDT.ExecuteSQL(sql + appendSql);

                if (rptDT.Rows.Count == 0)
                {
                    _result = @"{
                                    ""status"":""404"",
                                    ""response"":""OK"",
                                    ""title"":""Report Data Not Found"",
                                    ""message"": ""Please Try Other Option!""
                                    }";

                    return Ok(_result);
                }
                else
                {
                    string _jsondata = JsonConvert.SerializeObject(UserName);
                    string _jsondata2 = JsonConvert.SerializeObject(Plant);
                    _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsondata + @",
                                    ""data2"": " + _jsondata2 + @" 
                                    }";
                    return Ok(_result);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
        public async Task<IActionResult> PrintReportDetail([FromBody] string data)
        {
            try
            {
                
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string orderChecked = _json["orderChecked"];
                bool delayChecked = _json["delayChecked"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                string UserName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value.ToString();
                string HostName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.WindowsDeviceClaim).Value.ToString();
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }
                string lastMonth = DateTime.ParseExact(dateFrom, "yyyyMMdd", CultureInfo.InvariantCulture).AddMonths(-1).ToString("yyyyMMdd");
                string Plant = HttpContext.Request.Cookies["plantCode"].ToString();
                string appendSql = "";

                await _KB3Context.Database.ExecuteSqlRawAsync("EXEC dbo.SP_RT280_Detail '{0}','{1}','{2}','{3}','{4}','{5}'", Plant, orderChecked, dateFrom, dateTo, UserName, lastMonth);

                DataTable rptDT = _FillDT.ExecuteSQL($"SELECT F_Plant, F_OrderType, F_Start_Date, F_End_Date, F_Supplier_Code, F_Supplier_Plant, F_Short_Name, F_Name, F_Survey_No, F_OrderNo, F_Delivery_Date, " +
                    $" F_Receive_Date, F_QTY From TMP_RT280_Detail Where F_Update_By = '{UserName}' Order by F_Short_name,F_OrderNO");

                if (rptDT.Rows.Count == 0)
                {
                    _result = @"{
                                    ""status"":""404"",
                                    ""response"":""OK"",
                                    ""title"":""Report Data Not Found"",
                                    ""message"": ""Please Try Other Option!""
                                    }";

                    return Ok(_result);
                }
                else
                {
                    string _jsondata = JsonConvert.SerializeObject(UserName);
                    string _jsondata2 = JsonConvert.SerializeObject(Plant);
                    _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsondata + @",
                                    ""data2"": " + _jsondata2 + @" 
                                    }";
                    return Ok(_result);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
    }
}
