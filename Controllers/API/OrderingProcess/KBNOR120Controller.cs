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

        private readonly string Txt_Type = "Normal Ordering";
        private readonly string Type_Import = "N";
        private readonly DateTime Now = DateTime.Now;
        private readonly DateTime Txt_Date = DateTime.Now.Date;
        private static string Txt_Shift = "Day";
        private static string UserCode = "";
        private static string Plant = "";
        private static decimal ProcessCount = 0.00m;

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
                startInfo.FileName = "\\\\156.71.5.160\\hcst\\Source\\New_Kanban_F3\\5.Program\\New_Kanban_F3_AutoRun\\New_Kanban_F3_AutoRun_RecalculateBL\\New_Kanban_AutoRun_RecalculateBL\\bin\\Debug\\New_Kanban_F3_AutoRun_RecalculateBL.exe";
                startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(startInfo);

                await _KB3Context.Database.ExecuteSqlRawAsync($@"UPDATE TB_MS_PARAMETER SET F_VALUE2 ='2',F_Update_Date=Getdate()
                    ,F_Update_By='{UserCode}' where F_COde='ST'");

                await _KB3Context.Database.ExecuteSqlRawAsync($@"UPDATE TB_MS_PARAMETER SET F_VALUE2 ='2',F_Update_Date=Getdate()
                    ,F_Update_By='{UserCode}' where F_COde='CI'");

                ProcessCount = 0;

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

        [HttpGet]
        public async Task<IActionResult> Process_Order(string sDate)
        {
            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                ProcessCount = 0;
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
                         From TB_Calculate_H  Where (F_Process_Date = '{sDate}')  Order by 1,2,3,4,5,6"; //and F_Supplier_Code <> '9999' and F_Supplier_Code <> '9995'

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

                    ProcessCount = decimal.Round(((decimal)i / _DT.Rows.Count) * 90.00m,2, MidpointRounding.AwayFromZero);
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
        public async Task<IActionResult> Process_Order_Night(string sDate)
        {
            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                ProcessCount = 0;
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

                _KB3Transaction.CreateSavepoint("BeforeProcessOrderNight");
                _Log.WriteLog("Start Process Order for Night Shift ON : " + sDate, UserCode, _BearerClass.Device);

                var sEndDate = _KB3Context.Database.SqlQueryRaw<string>($"select dbo.FN_GET14Day('{sDate}') AS VALUE").FirstOrDefault();

                string _SQL = $@"SELECT distinct F_Supplier_Code,F_Supplier_plant,F_PART_no,F_Ruibetsu,F_Kanban_No,F_Store_Code 
                                FROM TB_CALCULATE_D  WHERE F_Process_Shift ='N' and F_Process_Date=@p0 
                                 Order by 1,2,3,4,5,6"; //and F_Supplier_Code <> '9999' and F_Supplier_Code <>'9995'

                var _DT = _FillDT.ExecuteSQL(_SQL,sDate);

                if (_DT.Rows.Count == 0)
                {
                    _KB3Transaction.RollbackToSavepoint("BeforeProcessOrderNight");
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Data in TB_CALCULATE_D not found.",
                    });
                }

                _SQL = $@"exec dbo.sp_Calculate_kanban @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7";

                for (int i = 0; i < _DT.Rows.Count; i++)
                {
                    await _KB3Context.Database.ExecuteSqlRawAsync(_SQL,_DT.Rows[i]["F_Supplier_Code"].ToString(),
                                    _DT.Rows[i]["F_Supplier_plant"].ToString(), _DT.Rows[i]["F_PART_no"].ToString(),
                                    _DT.Rows[i]["F_Ruibetsu"].ToString(), _DT.Rows[i]["F_Kanban_No"].ToString(),
                                    _DT.Rows[i]["F_Store_Code"].ToString(), sDate, sEndDate);

                    _SQL = $@"Update TB_Calculate_Volume Set F_QTY = D.F_Actual_Order 
                                 From TB_Calculate_D D INNER JOIN TB_Calculate_Volume V ON 
                                 D.F_Supplier_Code = V.F_Supplier_Code And D.F_Supplier_Plant = V.F_Supplier_Plant
                                 and D.F_Part_No = V.F_Part_No and D.F_Ruibetsu = V.F_Ruibetsu and D.F_Store_Code = V.F_Store_Code
                                 and D.F_Kanban_No = V.F_Kanban_No and D.F_Process_Date = V.F_Process_Date 
                                 AND D.F_Process_Round = V.F_Process_Round 
                                 Where V.F_Lock = '0' and D.F_Process_Date >= @p0
                                 AND D.F_Supplier_Code = @p1  AND D.F_Supplier_Plant = @p2
                                 AND D.F_PART_no = @p3 AND D.F_Ruibetsu = @p4 AND D.F_Kanban_No = @p5 AND D.F_Store_Code = @p6";

                    await _KB3Context.Database.ExecuteSqlRawAsync(_SQL, sDate, _DT.Rows[i]["F_Supplier_Code"].ToString(),
                        _DT.Rows[i]["F_Supplier_plant"].ToString(), _DT.Rows[i]["F_PART_no"].ToString(), _DT.Rows[i]["F_Ruibetsu"].ToString(),
                        _DT.Rows[i]["F_Kanban_No"].ToString(), _DT.Rows[i]["F_Store_Code"].ToString());

                    ProcessCount = decimal.Round(((decimal)i / _DT.Rows.Count) * 70.00m, 2, MidpointRounding.AwayFromZero);
                }

                string sLastDate = _KB3Context.Database.SqlQueryRaw<string>($"select dbo.FN_GetDateV2V('{sDate}',2) AS VALUE").FirstOrDefault();

                string _SQL2 = $@"Update TB_Calculate_H Set F_Urgent_order = D.F_Urgent_Order 
                                 From (Select F_Supplier_Code,F_Supplier_Plant,F_Part_No,F_Ruibetsu,F_Store_Code,F_Kanban_No,F_Process_Date, 
                                 sum(F_Urgent_order) as F_Urgent_order from TB_Calculate_D 
                                 Group by F_Supplier_Code,F_Supplier_Plant,F_Part_No,F_Ruibetsu,F_Store_Code,F_Kanban_No,F_Process_Date)D INNER JOIN TB_Calculate_H H ON 
                                 D.F_Supplier_Code = H.F_Supplier_Code And D.F_Supplier_Plant = H.F_Supplier_Plant 
                                 and D.F_Part_No = H.F_Part_No and D.F_Ruibetsu = H.F_Ruibetsu and D.F_Store_Code = H.F_Store_Code 
                                 and D.F_Kanban_No = H.F_Kanban_No and D.F_Process_Date = H.F_Process_Date 
                                 Where D.F_Urgent_Order > 0 and H.F_Process_Date >= @p0";

                await _KB3Context.Database.ExecuteSqlRawAsync(_SQL2, sLastDate);

                ProcessCount = 85.00m;

                await _KB3Context.Database.ExecuteSqlRawAsync("EXEC dbo.SP_CALCULATE_OTHER_CONDITION_NIGHT @p0", sDate);

                ProcessCount = 100.00m;

                _Log.WriteLog("Process Completed All NIGHT SHIFT : " + sDate, UserCode, _BearerClass.Device);
                _KB3Transaction.Commit();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Process Order Night is success.",
                });
            }
            catch (Exception ex)
            {
                _KB3Transaction.RollbackToSavepoint("BeforeProcessOrderNight");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Process Order Night is error.",
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
