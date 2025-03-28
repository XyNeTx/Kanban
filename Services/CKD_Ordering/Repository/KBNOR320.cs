using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace KANBAN.Services.CKD_Ordering.Repository
{
    public class KBNOR320 : IKBNOR320
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;

        public KBNOR320
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

        public static string strDateNow = DateTime.Now.ToString("yyyyMMdd");
        public static DataTable DT_HeaderInProcess = new DataTable();
        public static DataTable DT_DetailInProcess = new DataTable();
        public static DataTable DT_KBADD = new DataTable();
        public static DataTable DT_KBCUT = new DataTable();



        public async Task completeRecalculateCKD()
        {
            try
            {
                await _kbContext.Database.ExecuteSqlRawAsync("UPDATE TB_MS_Parameter SET F_Value2 = '3' WHERE F_Code = 'CI_CKD'");

                string[] arryVariable = new string[] { "0", KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd"), KBNOR310.chrProcessShift_CKD };

                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [CKD_Inhouse].sp_ManipulateForecast {0},{1},{2}", arryVariable);
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [CKD_Inhouse].sp_UpdateFlagWeekend {0},{1},{2}", arryVariable);
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [CKD_Inhouse].sp_ManipulateMRP_NG {0},{1},{2}", arryVariable);
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [CKD_Inhouse].sp_UpdateUrgentOrder {0},{1},{2}", arryVariable);
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [CKD_Inhouse].sp_UpdatePattern {0},{1},{2}", arryVariable);
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [CKD_Inhouse].sp_UpdateKBSTOP {0},{1},{2}", arryVariable);
                await completeGetNecessaryDataTable(arryVariable);
                await completeManipulateRemainLastTrip();
                await completeManipulateKBAdd();
                await completeManipulateKBCut();
                await completeUpdateTempTable();
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [CKD_Inhouse].sp_ManipulateLotSizing {0},{1},{2}", arryVariable);
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [CKD_Inhouse].sp_UpdateDeliveryDate {0},{1},{2}", arryVariable);
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [CKD_Inhouse].sp_UpdateActualOrder {0},{1},{2}", arryVariable);
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [CKD_Inhouse].sp_UpdateReceivePlan {0},{1},{2}", arryVariable);

                using (Process p = new Process())
                {
                    p.StartInfo.FileName = "C:\\Windows\\System32\\schtasks.exe";
                    p.StartInfo.Arguments = "/run /tn \"KB3_CKD_BL\"";  // Name of the scheduled task
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardOutput = true;  // Capture output
                    p.StartInfo.RedirectStandardError = true;  // Capture errors too

                    p.Start();

                    // Read output and error streams asynchronously
                    string output = await p.StandardOutput.ReadToEndAsync();
                    string error = await p.StandardError.ReadToEndAsync();

                    // Wait for the process to exit asynchronously
                    await p.WaitForExitAsync();

                    // Log both the output and any error that occurred
                    _log.WriteLogMsg("Start Process EXE: " + p.StartInfo.FileName + " Output: " + output + " Error: " + error);
                }


            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        //public async Task

        private async Task completeGetNecessaryDataTable(string[] arryVariable)
        {
            try
            {
                await getDTHeaderInProcess(arryVariable);
                await getDTDetailInProcess(arryVariable);
                await getKBADD(arryVariable);
                await getKBCUT(arryVariable);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task getDTHeaderInProcess(string[] arryVariable)
        {
            try
            {
                string sqlQuery = $@"IF OBJECT_ID('tempdb..#TEMP') IS NOT NULL DROP TABLE #TEMP;
                    CREATE TABLE #TEMP ( 
                    F_Prev_Date nvarchar(8), 
                    F_Prev_Shift nvarchar(1), 
                    F_Current_Date nvarchar(8), 
                    F_Current_Shift nvarchar(1), 
                    F_Next_Date nvarchar(8), 
                    F_Next_Shift nvarchar(1)); 
                    INSERT INTO #TEMP 
                    SELECT C.F_Date AS F_Prev_Date , C.F_Shift AS F_Prev_Shift 
                                ,  A.F_Date AS F_Current_Date , A.F_Shift AS F_Current_Shift
                                ,  B.F_Date AS F_Next_Date, B.F_Shift AS F_Next_Shift
                    FROM (	SELECT	ROW_NUMBER() OVER (ORDER BY F_Store_cd) AS Row 
                    				, F_Date, F_Shift, F_Work, F_YM, F_Day
                    		FROM CKD_Inhouse.V_Calendar_UNPIVOT
                    		WHERE F_Work = 1
                    		AND F_Store_cd = '3D'
                    	) AS A
                    LEFT JOIN
                        (	SELECT	ROW_NUMBER() OVER (ORDER BY F_Store_cd) AS Row
                    				, F_Date, F_Shift, F_Work, F_YM, F_Day
                    		FROM CKD_Inhouse.V_Calendar_UNPIVOT
                		WHERE F_Work = 1
                		AND F_Store_cd = '3D'
                	) AS B
                ON A.Row = B.Row - 1
                LEFT JOIN
                	(	SELECT	ROW_NUMBER() OVER (ORDER BY F_Store_cd) AS Row
                				, F_Date, F_Shift, F_Work, F_YM, F_Day
                		FROM CKD_Inhouse.V_Calendar_UNPIVOT
                		WHERE F_Work = 1
                		AND F_Store_cd = '3D'
                	) AS C
                ON A.Row = C.Row + 1
                WHERE A.F_Date+A.F_Shift >= '{arryVariable[1]}{arryVariable[2]}';
                SELECT ROW_NUMBER() OVER (ORDER BY F_Supplier_Code, F_Supplier_Plant, F_Part_No, F_Ruibetsu, F_Store_Code, F_Kanban_No, F_Process_Date, F_Process_Shift) AS Row
                , *
                FROM TB_Calculate_H_CKD H INNER JOIN #TEMP TMP
                ON H.F_Process_Date = TMP.F_Prev_Date
                AND H.F_Process_Shift = TMP.F_Prev_Shift";

                if (arryVariable.Length > 3)
                {
                    sqlQuery += Environment.NewLine + $"AND F_Supplier_Code = '{arryVariable[3]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Supplier_Plant = '{arryVariable[4]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Part_No = '{arryVariable[5]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Ruibetsu = '{arryVariable[6]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Store_Code = '{arryVariable[7]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Kanban_No = '{arryVariable[8]}'";
                }

                DT_HeaderInProcess = _FillDT.ExecuteSQL(sqlQuery);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task getDTDetailInProcess(string[] arryVariable)
        {
            try
            {
                string sqlQuery = $@"SELECT ROW_NUMBER() OVER (ORDER BY F_Supplier_Code, F_Supplier_Plant, F_Part_No, F_Ruibetsu, F_Store_Code, F_Kanban_No, F_Process_Date, F_Process_Shift) AS Row
                    , * FROM TB_Calculate_D_CKD 
                    WHERE F_Process_Date+F_Process_Shift >= '{arryVariable[1]}{arryVariable[2]}' 
                    AND F_NON_STOP <> 0";

                if (arryVariable.Length > 3)
                {
                    sqlQuery += Environment.NewLine + $"AND F_Supplier_Code = '{arryVariable[3]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Supplier_Plant = '{arryVariable[4]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Part_No = '{arryVariable[5]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Ruibetsu = '{arryVariable[6]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Store_Code = '{arryVariable[7]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Kanban_No = '{arryVariable[8]}'";
                }

                DT_DetailInProcess = _FillDT.ExecuteSQL(sqlQuery);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task getKBADD(string[] arryVariable)
        {
            try
            {
                string sqlQuery = $@"SELECT * FROM CKD_Inhouse.FN_GetKBADD ('{arryVariable[1]}','{arryVariable[2]}') ";

                if (arryVariable.Length > 3)
                {
                    sqlQuery += Environment.NewLine + $"AND F_Supplier_Code = '{arryVariable[3]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Supplier_Plant = '{arryVariable[4]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Part_No = '{arryVariable[5]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Ruibetsu = '{arryVariable[6]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Store_Code = '{arryVariable[7]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Kanban_No = '{arryVariable[8]}'";
                }

                DT_KBADD = _FillDT.ExecuteSQL(sqlQuery);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task getKBCUT(string[] arryVariable)
        {
            try
            {
                string sqlQuery = $@"SELECT * FROM CKD_Inhouse.FN_GetKBCUT ('{arryVariable[1]}','{arryVariable[2]}') ";

                if (arryVariable.Length > 3)
                {
                    sqlQuery += Environment.NewLine + $"AND F_Supplier_Code = '{arryVariable[3]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Supplier_Plant = '{arryVariable[4]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Part_No = '{arryVariable[5]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Ruibetsu = '{arryVariable[6]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Store_Code = '{arryVariable[7]}'";
                    sqlQuery += Environment.NewLine + $"AND F_Kanban_No = '{arryVariable[8]}'";
                }

                DT_KBCUT = _FillDT.ExecuteSQL(sqlQuery);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task completeManipulateRemainLastTrip()
        {
            string strChangeSupplier = "";
            string strChangeKanban = "";
            int intRemainLastTrip = 0, intOrderBase = 0, intMinusOrderBase = 0;
            bool blnFlag_MinusOrderBase = false;

            for (int i = 0; i < DT_HeaderInProcess.Rows.Count; i++)
            {
                if (strChangeSupplier == (DT_HeaderInProcess.Rows[i]["F_Supplier_Code"].ToString()
                    + DT_HeaderInProcess.Rows[i]["F_Supplier_Plant"].ToString())
                    && strChangeKanban == DT_HeaderInProcess.Rows[i]["F_Kanban_No"].ToString())
                {
                    DT_HeaderInProcess.Rows[i]["F_Remain_LastTrip"] = intRemainLastTrip;

                    if (intMinusOrderBase < 0)
                    {
                        DT_HeaderInProcess.Rows[i]["F_Remain_LastClear"] = Math.Abs(intMinusOrderBase);
                    }

                    intOrderBase = int.Parse(DT_HeaderInProcess.Rows[i]["F_Total"].ToString()) -
                        (intRemainLastTrip + int.Parse(DT_HeaderInProcess.Rows[i]["F_Remain_LastClear_Constant"].ToString())
                        + int.Parse(DT_HeaderInProcess.Rows[i]["F_Remain_LastClear"].ToString()));

                    if (intOrderBase < 0)
                    {
                        DT_HeaderInProcess.Rows[i]["F_Order_Base"] = 0;
                        intMinusOrderBase = intOrderBase;
                        blnFlag_MinusOrderBase = true;
                    }
                    else
                    {
                        DT_HeaderInProcess.Rows[i]["F_Order_Base"] = intOrderBase;
                        intMinusOrderBase = 0;
                        blnFlag_MinusOrderBase = false;
                    }

                    DT_HeaderInProcess.Rows[i]["F_Lot_SizeOrder"] = Math.Ceiling(decimal.Parse(DT_HeaderInProcess.Rows[i]["F_Order_Base"].ToString())
                        / decimal.Parse(DT_HeaderInProcess.Rows[i]["F_Qty_Box"].ToString()));

                }
                else
                {
                    intMinusOrderBase = 0;
                    blnFlag_MinusOrderBase = false;
                }

                intRemainLastTrip = (int.Parse(DT_HeaderInProcess.Rows[i]["F_Lot_SizeOrder"].ToString()) * int.Parse(DT_HeaderInProcess.Rows[i]["F_Qty_Box"].ToString()))
                    - int.Parse(DT_HeaderInProcess.Rows[i]["F_Order_Base"].ToString());
                strChangeSupplier = DT_HeaderInProcess.Rows[i]["F_Supplier_Code"].ToString() + DT_HeaderInProcess.Rows[i]["F_Supplier_Plant"].ToString();
                strChangeKanban = DT_HeaderInProcess.Rows[i]["F_Kanban_No"].ToString();
            }


        }
        private async Task completeManipulateKBAdd()
        {
            try
            {
                DataView DV_Detail = new DataView();
                DataTable DT = new DataTable();

                DV_Detail = DT_DetailInProcess.DefaultView;

                for (int iAdd = 0; iAdd < DT_KBADD.Rows.Count; iAdd++)
                {
                    string strDeliveryDate = DT_KBADD.Rows[iAdd]["F_Delivery_Date"].ToString().Trim();
                    string deliveryTrip = "0" + DT_KBADD.Rows[iAdd]["F_Delivery_Trip"].ToString().Trim();
                    string strDeliveryRound = deliveryTrip.Substring(deliveryTrip.Length - 2, 2);
                    int intKBAdd_Remain = int.Parse(DT_KBADD.Rows[iAdd]["F_KB_Remain"].ToString());

                    if (strDeliveryRound != "0")
                    {
                        int intKBADD_Rn = int.Parse(DT_KBADD.Rows[iAdd]["F_KB_Add_RN"].ToString());
                        DV_Detail.RowFilter = $@"F_Work = 1
                            AND F_Supplier_Code = '{DT_KBADD.Rows[iAdd]["F_Supplier_Code"].ToString().Trim()}'
                            AND F_Supplier_Plant = '{DT_KBADD.Rows[iAdd]["F_Supplier_Plant"].ToString().Trim()}'
                            AND F_Part_No = '{DT_KBADD.Rows[iAdd]["F_Part_No"].ToString().Trim()}'
                            AND F_Ruibetsu = '{DT_KBADD.Rows[iAdd]["F_Ruibetsu"].ToString().Trim()}'
                            AND F_Store_Code = '{DT_KBADD.Rows[iAdd]["F_Store_Code"].ToString().Trim()}'
                            AND F_Kanban_No = '{DT_KBADD.Rows[iAdd]["F_Kanban_No"].ToString().Trim()}'
                            AND F_Delivery_Date+SUBSTRING('0'+CONVERT(F_Delivery_Round, System.String),
                            LEN('0'+CONVERT(F_Delivery_Round, System.String))-1,2) >= '{strDeliveryDate}{strDeliveryRound}'
                        ";
                        DV_Detail.Sort = "Row";
                        DT = DV_Detail.ToTable();

                        if (DV_Detail.Count > 0)
                        {
                            int intLoop = (int)Math.Ceiling((double)intKBAdd_Remain / (double)intKBADD_Rn);
                            for (int i = 0; i < intLoop; i++)
                            {
                                if (i == intLoop - 1)
                                {
                                    DV_Detail[i]["F_KB_ADD"] = intKBAdd_Remain;
                                    break;
                                }
                                else
                                {
                                    DV_Detail[i]["F_KB_ADD"] = intKBADD_Rn;
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        DV_Detail.RowFilter = $@"F_Work = 1
                            AND F_Supplier_Code = '{DT_KBADD.Rows[iAdd]["F_Supplier_Code"].ToString().Trim()}'
                            AND F_Supplier_Plant = '{DT_KBADD.Rows[iAdd]["F_Supplier_Plant"].ToString().Trim()}'
                            AND F_Part_No = '{DT_KBADD.Rows[iAdd]["F_Part_No"].ToString().Trim()}'
                            AND F_Ruibetsu = '{DT_KBADD.Rows[iAdd]["F_Ruibetsu"].ToString().Trim()}'
                            AND F_Store_Code = '{DT_KBADD.Rows[iAdd]["F_Store_Code"].ToString().Trim()}'
                            AND F_Kanban_No = '{DT_KBADD.Rows[iAdd]["F_Kanban_No"].ToString().Trim()}'
                            AND F_Delivery_Date = '{strDeliveryDate}'
                            AND F_Delivery_Round = '{DT_KBADD.Rows[iAdd]["F_Round"].ToString().Trim()}'
                            ";

                        if (DV_Detail.Count > 0)
                        {
                            DV_Detail[0]["F_KB_ADD"] = DT_KBADD.Rows[iAdd]["F_KB_ADD"];
                        }
                    }

                    string sqlQuery = $@"UPDATE TB_Kanban_Add
                        SET F_KB_Remain = 0, F_Status = '2'
                        WHERE F_Supplier_Code = '{DT_KBADD.Rows[iAdd]["F_Supplier_Code"].ToString().Trim()}'
                        WHERE F_Supplier_Plant = '{DT_KBADD.Rows[iAdd]["F_Supplier_Plant"].ToString().Trim()}'
                        WHERE F_Store_Code = '{DT_KBADD.Rows[iAdd]["F_Store_Code"].ToString().Trim()}'
                        WHERE F_Kanban_No = '{DT_KBADD.Rows[iAdd]["F_Kanban_No"].ToString().Trim()}'
                        WHERE F_Part_No = '{DT_KBADD.Rows[iAdd]["F_Part_No"].ToString().Trim()}'
                        WHERE F_Ruibetsu = '{DT_KBADD.Rows[iAdd]["F_Ruibetsu"].ToString().Trim()}'
                        ";

                    await _kbContext.Database.ExecuteSqlRawAsync(sqlQuery);

                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task completeManipulateKBCut()
        {
            try
            {
                DataView DV_Detail = new DataView();
                DataTable DT = new DataTable();

                DV_Detail = DT_DetailInProcess.DefaultView;

                for (int iCut = 0; iCut < DT_KBCUT.Rows.Count; iCut++)
                {
                    string strDeliveryDate = DT_KBCUT.Rows[iCut]["F_Delivery_Date"].ToString().Trim();
                    string deliveryTrip = "0" + DT_KBCUT.Rows[iCut]["F_Delivery_Trip"].ToString().Trim();
                    string strDeliveryRound = deliveryTrip.Substring(deliveryTrip.Length - 2, 2);
                    int intKBCut = int.Parse(DT_KBCUT.Rows[iCut]["F_KB_Cut"].ToString());
                    int intKBCut_Rn = int.Parse(DT_KBCUT.Rows[iCut]["F_KB_Cut_RN"].ToString());

                    DV_Detail.RowFilter = $@"F_Work = 1
                        AND F_Lot_SizeOrder = 0
                        AND F_Supplier_Code = '{DT_KBCUT.Rows[iCut]["F_Supplier_Code"].ToString().Trim()}'
                        AND F_Supplier_Plant = '{DT_KBCUT.Rows[iCut]["F_Supplier_Plant"].ToString().Trim()}'
                        AND F_Part_No = '{DT_KBCUT.Rows[iCut]["F_Part_No"].ToString().Trim()}'
                        AND F_Ruibetsu = '{DT_KBCUT.Rows[iCut]["F_Ruibetsu"].ToString().Trim()}'
                        AND F_Store_Code = '{DT_KBCUT.Rows[iCut]["F_Store_Code"].ToString().Trim()}'
                        AND F_Kanban_No = '{DT_KBCUT.Rows[iCut]["F_Kanban_No"].ToString().Trim()}'
                        AND F_Delivery_Date+SUBSTRING('0'+CONVERT(F_Delivery_Round, System.String),
                        LEN('0'+CONVERT(F_Delivery_Round, System.String))-1,2) >= '{strDeliveryDate}{strDeliveryRound}'
                    ";
                    DV_Detail.Sort = "Row";
                    DT = DV_Detail.ToTable();

                    if (DV_Detail.Count > 0)
                    {
                        //int intLoop = (int)Math.Ceiling((double)intKBAdd_Remain / (double)intKBADD_Rn);
                        for (int i = 0; i < DV_Detail.Count; i++)
                        {
                            if (intKBCut > 0)
                            {
                                int F_KB_CUT = intKBCut_Rn <= int.Parse(DV_Detail[i]["F_Lot_SizeOrder"].ToString()) ? intKBCut_Rn * -1 : int.Parse(DV_Detail[i]["F_Lot_SizeOrder"].ToString()) * -1;
                                intKBCut = intKBCut - (F_KB_CUT * -1);
                                DV_Detail[i]["F_KB_CUT"] = F_KB_CUT;
                            }
                            else
                            {
                                DV_Detail[i]["F_KB_CUT"] = 0;
                            }
                        }
                    }
                    else
                    {

                    }
                    string sqlQuery = $@"UPDATE TB_Kanban_Add
                        SET F_KB_Remain = 0, F_Status = '2'
                        WHERE F_Supplier_Code = '{DT_KBCUT.Rows[iCut]["F_Supplier_Code"].ToString().Trim()}'
                        WHERE F_Supplier_Plant = '{DT_KBCUT.Rows[iCut]["F_Supplier_Plant"].ToString().Trim()}'
                        WHERE F_Store_Code = '{DT_KBCUT.Rows[iCut]["F_Store_Code"].ToString().Trim()}'
                        WHERE F_Kanban_No = '{DT_KBCUT.Rows[iCut]["F_Kanban_No"].ToString().Trim()}'
                        WHERE F_Part_No = '{DT_KBCUT.Rows[iCut]["F_Part_No"].ToString().Trim()}'
                        WHERE F_Ruibetsu = '{DT_KBCUT.Rows[iCut]["F_Ruibetsu"].ToString().Trim()}'
                        ";

                    await _kbContext.Database.ExecuteSqlRawAsync(sqlQuery);

                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task completeUpdateTempTable()
        {
            try
            {
                int intResult = 0;
                SqlConnection sqlCon = new SqlConnection(_kbContext.Database.GetConnectionString().ToString());
                sqlCon.Open();
                SqlTransaction trans = sqlCon.BeginTransaction();

                string sqlQuery = $@"CREATE TABLE [dbo].[#Temp_TB_Calculate_H_CKD]
                    (
                        [F_Supplier_Code] [char](4) NOT NULL,
                        [F_Supplier_Plant] [char](1) NOT NULL,
                        [F_Part_No] [char](10) NOT NULL,
                        [F_Ruibetsu] [char](2) NOT NULL,
                        [F_Store_Code] [char](2) NOT NULL,
                        [F_Kanban_No] [char](4) NOT NULL,
                        [F_Process_Date] [char](8) NOT NULL,
                        [F_Process_Shift] [char](1) NOT NULL,
                        [F_Remain_LastTrip] [int] NULL,
                        [F_Remain_LastClear] [int] NULL,
                        [F_Order_Base] [int] NULL,
                        [F_Lot_SizeOrder] [int] NULL,
                        [Flag_MinusOrderBase] [bit] NULL
                    );

                    CREATE TABLE [dbo].[#Temp_TB_Calculate_D_CKD]
                    (
                        [F_Supplier_Code] [char](4) NOT NULL,
                        [F_Supplier_Plant] [char](1) NOT NULL,
                        [F_Part_No] [char](10) NOT NULL,
                        [F_Ruibetsu] [char](2) NOT NULL,
                        [F_Store_Code] [char](2) NOT NULL,
                        [F_Kanban_No] [char](4) NOT NULL,
                        [F_Process_Date] [char](8) NOT NULL,
                        [F_Process_Shift] [char](1) NOT NULL,
                        [F_Process_Round] [int] NOT NULL,
                        [F_KB_CUT] [int] NULL,
                        [F_KB_ADD] [int] NULL
                    );";

                //await _kbContext.Database.ExecuteSqlRawAsync(sqlQuery);

                using var commandEvent = new SqlCommand(sqlQuery, sqlCon, trans);
                commandEvent.ExecuteNonQuery();

                using var bulkCopy = new SqlBulkCopy(sqlCon, SqlBulkCopyOptions.Default, trans);
                bulkCopy.DestinationTableName = "#Temp_TB_Calculate_H_CKD";
                bulkCopy.ColumnMappings.Add("F_Supplier_Code", "F_Supplier_Code");
                bulkCopy.ColumnMappings.Add("F_Supplier_Plant", "F_Supplier_Plant");
                bulkCopy.ColumnMappings.Add("F_Part_No", "F_Part_No");
                bulkCopy.ColumnMappings.Add("F_Ruibetsu", "F_Ruibetsu");
                bulkCopy.ColumnMappings.Add("F_Store_Code", "F_Store_Code");
                bulkCopy.ColumnMappings.Add("F_Kanban_No", "F_Kanban_No");
                bulkCopy.ColumnMappings.Add("F_Process_Date", "F_Process_Date");
                bulkCopy.ColumnMappings.Add("F_Process_Shift", "F_Process_Shift");
                bulkCopy.ColumnMappings.Add("F_Remain_LastTrip", "F_Remain_LastTrip");
                bulkCopy.ColumnMappings.Add("F_Remain_LastClear", "F_Remain_LastClear");
                bulkCopy.ColumnMappings.Add("F_Order_Base", "F_Order_Base");
                bulkCopy.ColumnMappings.Add("F_Lot_SizeOrder", "F_Lot_SizeOrder");
                bulkCopy.ColumnMappings.Add("Flag_MinusOrderBase", "Flag_MinusOrderBase");
                await bulkCopy.WriteToServerAsync(DT_HeaderInProcess);

                bulkCopy.DestinationTableName = "#Temp_TB_Calculate_D_CKD";
                bulkCopy.ColumnMappings.Add("F_Supplier_Code", "F_Supplier_Code");
                bulkCopy.ColumnMappings.Add("F_Supplier_Plant", "F_Supplier_Plant");
                bulkCopy.ColumnMappings.Add("F_Part_No", "F_Part_No");
                bulkCopy.ColumnMappings.Add("F_Ruibetsu", "F_Ruibetsu");
                bulkCopy.ColumnMappings.Add("F_Store_Code", "F_Store_Code");
                bulkCopy.ColumnMappings.Add("F_Kanban_No", "F_Kanban_No");
                bulkCopy.ColumnMappings.Add("F_Process_Date", "F_Process_Date");
                bulkCopy.ColumnMappings.Add("F_Process_Shift", "F_Process_Shift");
                bulkCopy.ColumnMappings.Add("F_Process_Round", "F_Process_Round");
                bulkCopy.ColumnMappings.Add("F_KB_CUT", "F_KB_CUT");
                bulkCopy.ColumnMappings.Add("F_KB_ADD", "F_KB_ADD");
                await bulkCopy.WriteToServerAsync(DT_DetailInProcess);

                sqlQuery = $@"UPDATE H 
                    SET F_Remain_LastTrip = TEMP.F_Remain_LastTrip 
                    , F_Remain_LastClear = TEMP.F_Remain_LastClear 
                    , F_Order_Base = TEMP.F_Order_Base 
                    , F_Lot_SizeOrder = TEMP.F_Lot_SizeOrder 
                    FROM	( 
                    			SELECT *
                    			FROM #Temp_TB_Calculate_H_CKD
                    		) TEMP
                    INNER JOIN TB_Calculate_H_CKD H
                    ON H.F_Supplier_Code = TEMP.F_Supplier_Code
                    AND H.F_Supplier_Plant = TEMP.F_Supplier_Plant
                    AND H.F_Part_No = TEMP.F_Part_No
                    AND H.F_Ruibetsu = TEMP.F_Ruibetsu
                    AND H.F_Store_Code = TEMP.F_Store_Code
                    AND H.F_Kanban_No = TEMP.F_Kanban_No
                    AND H.F_Process_Date = TEMP.F_Process_Date
                    AND H.F_Process_Shift = TEMP.F_Process_Shift;

                    UPDATE H
                    SET F_KB_CUT = TEMP.F_KB_CUT
                    , F_KB_ADD = TEMP.F_KB_ADD
                    FROM	(
                    			SELECT	F_Supplier_Code, F_Supplier_Plant, F_Part_No, F_Ruibetsu, F_Store_Code, F_Kanban_No
                    					, F_Process_Date, F_Process_Shift
                    					, SUM(F_KB_CUT) AS F_KB_CUT, SUM(F_KB_ADD) AS F_KB_ADD
                    			FROM #Temp_TB_Calculate_D_CKD
                    			WHERE F_KB_CUT <> 0 OR F_KB_ADD <> 0
                    			GROUP BY	F_Supplier_Code, F_Supplier_Plant, F_Part_No, F_Ruibetsu, F_Store_Code, F_Kanban_No
                    						, F_Process_Date, F_Process_Shift
                    		) TEMP
                    INNER JOIN TB_Calculate_H_CKD H
                    ON H.F_Supplier_Code = TEMP.F_Supplier_Code
                    AND H.F_Supplier_Plant = TEMP.F_Supplier_Plant
                    AND H.F_Part_No = TEMP.F_Part_No
                    AND H.F_Ruibetsu = TEMP.F_Ruibetsu
                    AND H.F_Store_Code = TEMP.F_Store_Code
                    AND H.F_Kanban_No = TEMP.F_Kanban_No
                    AND H.F_Process_Date = TEMP.F_Process_Date
                    AND H.F_Process_Shift = TEMP.F_Process_Shift;

                    UPDATE D
                    SET    F_KB_CUT = TEMP.F_KB_CUT
                            , F_KB_ADD = TEMP.F_KB_ADD
                    FROM TB_Calculate_D_CKD D INNER JOIN #Temp_TB_Calculate_D_CKD TEMP
                    ON D.F_Supplier_Code = TEMP.F_Supplier_Code
                    AND D.F_Supplier_Plant = TEMP.F_Supplier_Plant
                    AND D.F_Part_No = TEMP.F_Part_No
                    AND D.F_Ruibetsu = TEMP.F_Ruibetsu
                    AND D.F_Store_Code = TEMP.F_Store_Code
                    AND D.F_Kanban_No = TEMP.F_Kanban_No
                    AND D.F_Process_Date = TEMP.F_Process_Date
                    AND D.F_Process_Shift = TEMP.F_Process_Shift
                    AND D.F_Process_Round = TEMP.F_Process_Round;";

                commandEvent.CommandText = sqlQuery;
                intResult = commandEvent.ExecuteNonQuery();

                if (intResult >= 0)
                {
                    trans.Commit();
                }
                else
                {
                    if (commandEvent != null)
                    {
                        commandEvent.Dispose();
                    }
                    if (trans != null)
                    {
                        trans.Rollback();
                    }
                    throw new CustomHttpException(400, "Error Sub : ExcuteNonQuery_Trans Return Row(s) affected = 0 ");
                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
