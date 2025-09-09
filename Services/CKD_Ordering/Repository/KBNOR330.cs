using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace KANBAN.Services.CKD_Ordering.Repository
{
    public class KBNOR330 : IKBNOR330
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public KBNOR330
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMapService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _automapService = autoMapService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Tuple<DataTable, string>> Generate()
        {
            try
            {
                string ckdDB = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value switch
                {
                    "3" => "[HMMTA-APP09]",
                    "2" => "[HMMT-CKD-WH]",
                    "1" => "[HMMT-CKD-WH]",
                    _ => throw new CustomHttpException(400, "ไม่พบข้อมูล Plant Code ในระบบ")
                };

                string _SqlQuery = $@"SELECT   V.F_Supplier_Code+'-'+V.F_Supplier_Plant AS F_Supplier_Code
                    , V.F_Part_No+'-'+V.F_Ruibetsu AS F_Part_No, V.F_Store_Code, V.F_Kanban_No
                    , RIGHT(V.F_Delivery_Date,2)+'/'+SUBSTRING(V.F_Delivery_Date,5,2)+'/'+LEFT(V.F_Delivery_Date,4) AS F_Delivery_Date
                    , V.F_Delivery_Shift, V.F_Delivery_Round, V.F_Qty
                    , ISNULL(VW_PI.F_Remain_Qty, 0) AS CKD_Remain_Qty
                    FROM TB_Calculate_V_CKD V
                    LEFT JOIN {ckdDB}.[CKD_WH_STOCK].[dbo].[VW_PI_GetCurrentStock] VW_PI
                    ON V.F_Part_No = VW_PI.F_Part_No
                    AND V.F_Ruibetsu = VW_PI.F_Ruibetsu
                    WHERE (V.F_Supplier_Code = '9999')
                    AND V.F_Process_Date = '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}'
                    AND V.F_Process_Shift = '{KBNOR310.chrProcessShift_CKD}'
                    AND V.F_Delivery_Date <> ''
                    AND V.F_Qty > ISNULL(VW_PI.F_Remain_Qty,0)
                    ORDER BY V.F_Supplier_Code, V.F_Supplier_Plant, V.F_Kanban_No";

                var dt = await _FillDT.ExecuteSQLAsync(_SqlQuery);

                if (dt.Rows.Count > 0)
                {
                    //throw new CustomHttpException(400,
                    //    @"ไม่สามารถ Generate PDS สำหรับ CKD Order ได้
                    //    CKD Remain Qty ไม่เพียงพอต่อ Kanban Order Qty
                    //    กรุณาแก้ไขข้อมูล จำนวน Kanban Order Qty ดังต่อไปนี้");

                    return Tuple.Create(dt, @"ไม่สามารถ Generate PDS สำหรับ CKD Order ได้
                        CKD Remain Qty ไม่เพียงพอต่อ Kanban Order Qty
                        กรุณาแก้ไขข้อมูล จำนวน Kanban Order Qty ดังต่อไปนี้");
                }

                _SqlQuery = $@"DELETE D
                    FROM TB_PDS_Detail D INNER JOIN 
                    ( 
                        SELECT F_OrderNo
                        FROM TB_PDS_HEADER
                        WHERE (F_Supplier_Code = '9999')
                        AND F_OrderType = 'N'
                        AND F_Update_By = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'
                        AND SUBSTRING(F_OrderNo,11,1) = 'C'
                    ) H
                    ON D.F_OrderNo = H.F_OrderNo";

                await _kbContext.Database.ExecuteSqlRawAsync(_SqlQuery);

                _SqlQuery = $@"DELETE FROM TB_PDS_HEADER
                    WHERE (F_Supplier_Code = '9999')
                    AND F_OrderType = 'N'
                    AND F_Update_By = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'
                    AND SUBSTRING(F_OrderNo,11,1) = 'C';";

                await _kbContext.Database.ExecuteSqlRawAsync(_SqlQuery);

                _SqlQuery = $@"DELETE FROM TB_PDS_CKD_Picking
                    WHERE (F_Supplier_Code = '9999') 
                    AND F_OrderType = 'N'
                    AND F_Update_By = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'
                    AND CONVERT(CHAR(8),F_Issued_Date,112) = '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}' 
                    AND F_Issued_Shift = '{KBNOR310.chrProcessShift_CKD}'
                    AND SUBSTRING(F_OrderNo,11,1) = 'C';";

                await _kbContext.Database.ExecuteSqlRawAsync(_SqlQuery);

                string ppmConnect = _FillDT.ppmConnect();

                _SqlQuery = $@"DELETE FROM TMP_Construction
                    WHERE F_Update_By = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'
                    INSERT INTO TMP_Construction 
                    (	
                        F_Part_no, F_Ruibetsu, F_Store_cd, F_subcontract, F_TC_Str, F_TC_End
                        , F_supplier_cd, F_plant, F_Sebango, F_Order_Met, F_Kanban_con
                        , F_incre_cut, F_Safety_Stk, F_ratio, F_Send_class, F_send_supplier, F_send_plant
                        , F_send_store, F_qty_box, F_Weight, F_box_cd, F_Part_nm, F_KD_Flag, F_STD_stock_ratio
                        , F_Cycle_A, F_cycle_B, F_cycle_C, F_Logistic_cd, F_commemt
                        , F_update, F_inputuser, F_Plant_CD, F_Update_By 
                    )
                    SELECT  F_Part_no, F_Ruibetsu, F_Store_cd, F_subcontract, F_Local_Str, F_Local_End
                    , F_supplier_cd, F_plant, F_Sebango, F_Order_Met, F_Kanban_con
                    , F_incre_cut, F_Safety_Stk, F_ratio, F_Send_class, F_send_supplier, F_send_plant
                    , F_send_store, F_qty_box, F_Weight, F_box_cd, F_Part_nm,  F_KD_Flag, F_STD_stock_ratio
                    , F_Cycle_A, F_cycle_B, F_cycle_C, F_Logistic_cd, F_commemt
                    , F_update, F_inputuser, F_Plant_CD, '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' AS F_Update_By
                    FROM {ppmConnect}.dbo.T_Construction
                    WHERE F_Local_END >= CONVERT(CHAR(8),getDate(),112)
                    AND (F_supplier_cd = '9999');";

                await _kbContext.Database.ExecuteSqlRawAsync(_SqlQuery);

                _SqlQuery = $@"SELECT   F_Supplier_Code, F_Supplier_Plant, F_Part_No, F_Ruibetsu, F_Store_Code, F_Kanban_No
                    , F_Process_Date, F_Process_Shift, F_Process_Round
                    , V.F_Delivery_Date, F_Delivery_Shift, RIGHT('00'+CAST(V.F_Delivery_Round AS VARCHAR),2) AS F_Delivery_Round
                    FROM TB_Calculate_V_CKD V
                    WHERE (V.F_Supplier_Code = '9999')
                    AND V.F_Process_Date = '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}'
                    AND V.F_Process_Shift = '{KBNOR310.chrProcessShift_CKD}'
                    AND V.F_Delivery_Date <> ''
                    AND F_Qty > 0
                    ORDER BY V.F_Delivery_Date + RIGHT('00'+CAST(V.F_Delivery_Round AS VARCHAR),2)";

                dt = await _FillDT.ExecuteSQLAsync(_SqlQuery);

                var arryVariable = new List<SqlParameter>();

                if (dt.Rows.Count > 0)
                {
                    arryVariable.Add(new SqlParameter("@ForDate_CKD", KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")));
                    arryVariable.Add(new SqlParameter("@ForShift_CKD", KBNOR310.chrProcessShift_CKD));
                    arryVariable.Add(new SqlParameter("@UserName", _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value));
                    arryVariable.Add(new SqlParameter("@Type_Import", "N"));

                    _SqlQuery = "EXEC [CKD_Inhouse].sp_GeneratePDS @ForDate_CKD, @ForShift_CKD, @UserName, @Type_Import";

                    using var KbTransaction = await _kbContext.Database.BeginTransactionAsync();

                    try
                    {
                        await _kbContext.Database.ExecuteSqlRawAsync(_SqlQuery,
                            new SqlParameter("@ForDate_CKD", KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")),
                            new SqlParameter("@ForShift_CKD", KBNOR310.chrProcessShift_CKD),
                            new SqlParameter("@UserName", _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value),
                            new SqlParameter("@Type_Import", "N"));

                        var headerData = await _kbContext.TB_PDS_Header.AsNoTracking()
                            .Where(x => x.F_OrderType == "N" && x.F_Supplier_Code == "9999")
                            .ToListAsync();
                       
                        if (headerData.Count > 0)
                        {
                            await KbTransaction.CommitAsync();

                            _SqlQuery = $@"SELECT DISTINCT F_Supplier_Code+'-'+F_Supplier_Plant COLLATE DATABASE_DEFAULT AS F_Supplier_Code
                            , F_Delivery_Cycle, F_Delivery_Date, F_Delivery_Trip, F_OrderNo
                            From TB_PDS_HEADER
                            Where F_OrderType='N' and (F_Supplier_Code='9999')";

                            dt = await _FillDT.ExecuteSQLAsync(_SqlQuery);

                            return Tuple.Create(dt, "Success");
                        }
                        else
                        {
                            throw new CustomHttpException(400, "ไม่พบข้อมูลในระบบ TB_PDS_HEADER");
                        }
                    }
                    catch (Exception ex)
                    {
                        await KbTransaction.RollbackAsync();
                        _log.WriteErrorLogMsg(ex.Message);
                        throw new CustomHttpException(400, ex.Message + " Error Sub : frmKBNOR_330 [CKD_Inhouse].sp_GeneratePDS ");
                    }

                }
                else
                {
                    throw new CustomHttpException(400, "ไม่พบข้อมูลในระบบ TB_Calculate_V_CKD");
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
