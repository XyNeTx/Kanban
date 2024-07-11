using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace KANBAN.Controllers.API.OrderingProcess
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR120Controller : ControllerBase
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

        public KBNOR120Controller(
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

        public readonly string Txt_Type = "Normal Ordering";
        public readonly string Type_Import = "N";
        public readonly DateTime Now = DateTime.Now;
        public readonly DateTime Txt_Date = DateTime.Now.Date;
        public static string Txt_Shift = "Day";
        public static string UserCode = "";
        public static string Plant = "";
        public static decimal ProcessCount = 0.00m;

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

                Txt_Shift = (Shift.Substring(0,1) == "1") ? "Day" : "Night";
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
        public async Task<IActionResult> Calculate()
        {
            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
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


                string dir = Directory.GetCurrentDirectory();

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "\\\\hmmta-tpcap\\kanban\\wwwroot\\Storage\\New_Kanban_AutoRun_RecalculateBL.exe";
                startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Verb = "runas";
                Process.Start(startInfo);

                await _KB3Context.Database.ExecuteSqlRawAsync($@"UPDATE TB_MS_PARAMETER SET F_VALUE2 ='2',F_Update_Date=Getdate()
                    ,F_Update_By='{UserCode}' where F_COde='ST'");

                await _KB3Context.Database.ExecuteSqlRawAsync($@"UPDATE TB_MS_PARAMETER SET F_VALUE2 ='2',F_Update_Date=Getdate()
                    ,F_Update_By='{UserCode}' where F_COde='CI'");

                _KB3Transaction.Commit();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Calculate is success.",
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Calculate is error.",
                    error = ex.Message
                });
            }
        }

        public async Task<object> Process_Order(string sDate)
        {
            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
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

                await _KB3Context.Database.ExecuteSqlRawAsync($"UPDATE TB_MS_PARAMETER " +
                    $"SET F_VALUE2 ='3',F_Update_Date=Getdate(),F_Update_By='{UserCode}' " +
                    $" WHERE F_CODE='CI' ");

                sDate = sDate.Replace("-", string.Empty);

                _KB3Transaction.CreateSavepoint("BeforeProcessOrder");
                _Log.WriteLog("Start Process Calculate Normal ON : "+sDate, UserCode, _BearerClass.Device);

                string _SQL = $@"INSERT INTO TB_KANBAN_NG(F_Process_Date, F_Process_Shift, 
                        F_Plant, F_Refer_Doc, F_Part_No, F_Ruibetsu, f_Qty, F_Supplier_Code, F_Supplier_Plant 
                        ,F_Store_Cd,F_Location, F_WorkingDay, F_Update_By, F_Update_Date)  
                         select '{sDate}' as F_Process_Date,'D' as F_Process_SHift, '{Plant}' as F_Plant, 
                        I.F_Refer_Doc,I.F_PartNo,I.F_Ruibetsu,I.F_Qty,I.F_Supplier_Code,I.F_Supplier_Plant, 
                         C.F_Store_Cd,I.F_Location, I.F_Workingdate, '{UserCode}' as F_Update_BY,getdate() as F_Update_Date 
                        from TB_Kanban_NG NG RIGHT OUTER JOIN [HMMT-PPM].[Inventory].[dbo].[T_Transaction_D_{sDate.Substring(0, 6)}] I  
                         ON NG.F_Refer_Doc = I.F_Refer_Doc collate Thai_CI_AS and NG.F_Part_No = I.F_PartNo collate Thai_CI_AS 
                         and NG.F_Ruibetsu = I.F_Ruibetsu collate Thai_CI_AS and NG.F_Supplier_Code = I.F_Supplier_Code collate Thai_CI_AS
                         and NG.F_Supplier_Plant = I.F_Supplier_Plant collate Thai_CI_AS 
                         INNER JOIN VW_SYSCTL C ON I.F_Location = C.F_Value 
                        Where I.F_Tran_CD = '41' and I.F_ADJ_CD in ('A01','A02','A03','A04','A05','A06','A07','A08','A10','A11','A17','A18') 
                         and NG.F_Part_No is null ";

                await _KB3Context.Database.ExecuteSqlRawAsync(_SQL);

                _Log.WriteLog("Interface NG Data : " + _SQL,UserCode,_BearerClass.Device);

                _SQL = $@"Select distinct F_Supplier_Code, F_Supplier_Plant, F_Store_Code, F_Kanban_No ,F_Part_No, F_Ruibetsu 
                         From TB_Calculate_H  Where (F_Process_Date = '{sDate}') and F_Supplier_Code <> '9999' and F_Supplier_Code <> '9995' Order by 1,2,3,4,5,6";

                var _DT = _FillDT.ExecuteSQL(_SQL);

                if (_DT.Rows.Count == 0)
                {
                    _KB3Transaction.RollbackToSavepoint("BeforeProcessOrder");
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Data in TB_Calculate_H not found.",
                    });
                }

                _Log.WriteLog("Get Data Calculate_H : " + _DT.Rows.Count + "Records | " + _SQL, UserCode, _BearerClass.Device);

                for(int i = 0; i < _DT.Rows.Count; i++)
                {
                    await _KB3Context.Database.ExecuteSqlRawAsync("EXEC dbo.SP_CALCULATE_KBNOR120 @p0,@p1,@p2,@p3,@p4,@p5,@p6",
                        sDate, _DT.Rows[i]["F_Supplier_Code"].ToString(), _DT.Rows[i]["F_Supplier_Plant"].ToString(),
                        _DT.Rows[i]["F_Store_Code"].ToString(), _DT.Rows[i]["F_Kanban_No"].ToString(),
                        _DT.Rows[i]["F_Part_No"].ToString(), _DT.Rows[i]["F_Ruibetsu"].ToString());
                    
                    ProcessCount = ((decimal)i / _DT.Rows.Count) * 90;
                }

                _Log.WriteLog("End Process Calculate Normal ON : " + sDate, UserCode, _BearerClass.Device);

                await _KB3Context.Database.ExecuteSqlRawAsync("EXEC dbo.SP_CALCULATE_OTHER_CONDITION @p0", sDate);

                _Log.WriteLog("End Process Calculate Other Condition : " + sDate, UserCode, _BearerClass.Device);

                ProcessCount = 100;

                _KB3Transaction.Commit();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Process Order is success.",
                });
            }
            catch (Exception ex)
            {
                _KB3Transaction.RollbackToSavepoint("BeforeProcessOrder");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Process Order is error.",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult GetProcessCount()
        {
            return Ok(new
            {
                status = "200",
                response = "OK",
                title = "Success",
                message = "Get Process Count is success.",
                data = ProcessCount
            });
        }

    }
}
