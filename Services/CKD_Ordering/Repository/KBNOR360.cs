using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.CKD_Ordering;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Services.CKD_Ordering.Repository
{
    public class KBNOR360 : IKBNOR360
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly CKDWH_Context _CKDContext;
        private readonly CKDUSA_Context _CKDUSAContext;


        public KBNOR360
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMapService,
            CKDWH_Context CKDContext,
            CKDUSA_Context CKDUSAContext
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _automapService = autoMapService;
            _CKDContext = CKDContext;
            _CKDUSAContext = CKDUSAContext;
        }

        public static string PI_Date = "", PI_Time = "", PI_Shift = "", PI_By = "KANBAN",PI_Date_RemainShelf = "",PI_Time_RemainShelf ="";

        public async Task<List<string>> Check_CKDStatus()
        {
            try
            {
                var _dbParam = await _kbContext.TB_MS_Parameter.AsNoTracking()
                    .Where(x => x.F_Code == "LO_CKD" || x.F_Code == "ST_CKD")
                    .ToListAsync();

                var listString = new List<string>
                {
                    _dbParam.FirstOrDefault(x => x.F_Code == "LO_CKD")?.F_Value3,
                    _dbParam.FirstOrDefault(x => x.F_Code == "ST_CKD")?.F_Value2.ToString()
                };

                return listString;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<DataTable> List_Data()
        {
            try
            {
                var sqlParams = new List<SqlParameter>
                {
                    new SqlParameter("@dateProcessDate_CKD", KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")),
                    new SqlParameter("@chrProcessShift_CKD", KBNOR310.chrProcessShift_CKD)
                };

                var dt = await _FillDT.ExecuteStoreSQLAsync("[exec].[spKBNOR360]", sqlParams.ToArray());

                return dt;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task Register(List<VM_KBNOR360_Register> listObj)
        {
            var F_Delivery_Date = "";
            try
            {
                string _sql = "";
                foreach (var obj in listObj)
                {
                    F_Delivery_Date = obj.F_Delivery_Date.Substring(6, 4) + obj.F_Delivery_Date.Substring(3, 2) + obj.F_Delivery_Date.Substring(0, 2);
                    var _dbPDS_Head = await _kbContext.TB_PDS_Header.AsNoTracking()
                        .Where(x => x.F_Plant == _BearerClass.Plant
                        && x.F_OrderType == "N"
                        //&& x.F_OrderType == "C"
                        && x.F_Delivery_Date == F_Delivery_Date
                        && x.F_OrderNo == obj.F_OrderNo
                        && x.F_Supplier_Code == obj.F_Supplier_Code.Substring(0, 4)
                        && x.F_Supplier_Plant == obj.F_Supplier_Code.Substring(5, 1)
                        && x.F_Delivery_Trip == obj.F_Delivery_Trip).FirstOrDefaultAsync();

                    TB_REC_HEADER tB_REC_HEADER = JsonConvert.DeserializeObject<TB_REC_HEADER>(JsonConvert.SerializeObject(_dbPDS_Head));

                    if (tB_REC_HEADER != null)
                    {
                        tB_REC_HEADER.F_Approver = "";
                        tB_REC_HEADER.F_Cancel_By = "";
                        tB_REC_HEADER.F_Delay_Invoice_Date = "";
                        tB_REC_HEADER.F_Type_Version = "";
                        tB_REC_HEADER.F_Cancel_Date = DateTime.Now;
                        _kbContext.TB_REC_HEADER.Add(tB_REC_HEADER);
                        await _kbContext.SaveChangesAsync();
                        _log.WriteLogMsg($"Insert TB_REC_HEADER : {JsonConvert.SerializeObject(tB_REC_HEADER, Formatting.Indented)}");
                    }
                    else
                    {
                        continue;
                        throw new CustomHttpException(500, "Can't Convert TB_PDS_HEADER => TB_REC_HEADER ");
                    }

                    var _dbPDS_Detail = await _kbContext.TB_PDS_Detail.AsNoTracking()
                        .Where(x => x.F_OrderNo == obj.F_OrderNo).ToListAsync();

                    if (_dbPDS_Detail.Count > 0)
                    {
                        foreach (var item in _dbPDS_Detail)
                        {
                            TB_REC_DETAIL tB_REC_DETAIL = JsonConvert.DeserializeObject<TB_REC_DETAIL>(JsonConvert.SerializeObject(item));
                            if (tB_REC_DETAIL != null)
                            {
                                _kbContext.TB_REC_DETAIL.Add(tB_REC_DETAIL);
                                await _kbContext.SaveChangesAsync();
                                _log.WriteLogMsg($"Insert TB_REC_DETAIL : {JsonConvert.SerializeObject(tB_REC_DETAIL, Formatting.Indented)}");
                            }
                            else
                            {
                                throw new CustomHttpException(500, "Can't Convert TB_PDS_DETAIL => TB_REC_DETAIL ");
                            }
                        }
                    }

                    _sql = $@"Update TB_Calculate_V_CKD Set F_Lock= '2',F_Update_By='{_BearerClass.UserCode}' 
                        ,F_Update_Date = getdate() From TB_Calculate_V_CKD V Inner Join TB_Calculate_D_CKD D On 
                        V.F_Supplier_Code = D.F_Supplier_COde And V.F_Supplier_Plant = D.F_Supplier_plant And V.F_part_no = D.F_Part_no
                        and V.F_Ruibetsu = D.F_Ruibetsu and V.F_Store_Code = D.F_Store_Code and V.F_kanban_NO = D.F_Kanban_NO 
                        and V.F_Process_Date = D.F_Process_Date and V.F_PRocess_Round = D.F_Process_Round 
                        Where D.F_Supplier_Code = '{obj.F_Supplier_Code.Split("-")[0]}' 
                        and D.F_SUpplier_Plant='{obj.F_Supplier_Code.Split("-")[1]}' 
                        and V.F_Delivery_Date ='{F_Delivery_Date}'
                        and V.F_Delivery_Round ='{obj.F_Delivery_Trip}'
                        and V.F_Process_Date ='{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}'";

                    if (KBNOR310.chrProcessShift_CKD == "D")
                    {
                        _sql += $" and D.F_Process_SHift ='{KBNOR310.chrProcessShift_CKD}'";
                    }
                    else
                    {
                        _sql += $" and D.F_Process_SHift IN ('N','T')";
                    }

                    await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                    _log.WriteLogMsg($"Update TB_Calculate_V_CKD : {_sql}");

                    _sql = $@"Update TB_Calculate_H_CKD set F_Lock='2',F_Update_By='${_BearerClass.UserCode}' ,F_Update_Date = getdate() 
                        from (Select M.F_Supplier_Code,M.F_Supplier_Plant,M.F_Part_no,M.F_Ruibetsu,M.F_Store_Code,M.F_kanban_No,M.F_Process_Date,M.F_Delivery_Date
                        from (Select distinct V.F_Supplier_Code,V.F_Supplier_Plant,V.F_Part_no,V.F_Ruibetsu,V.F_Store_Code,V.F_kanban_No,V.F_Process_Date,V.F_Delivery_Date
                        From TB_Calculate_V_CKD V INNER Join TB_Calculate_H_CKD H On 
                        V.F_Supplier_Code = H.F_Supplier_Code And V.F_Supplier_Plant = H.F_Supplier_plant And V.F_part_no = H.F_Part_no
                        and V.F_Ruibetsu = H.F_Ruibetsu and V.F_Store_Code = H.F_Store_Code and V.F_kanban_NO = H.F_Kanban_NO 
                        and V.F_Process_Date = H.F_Process_Date 
                        Where H.F_Supplier_Code = '{obj.F_Supplier_Code.Split("-")[0]}' 
                        and H.F_SUpplier_Plant='{obj.F_Supplier_Code.Split("-")[1]}'
                        and V.F_Delivery_Date ='{F_Delivery_Date}' ) M
                        LEFT OUTER JOIN (Select distinct V.F_Supplier_Code,V.F_Supplier_Plant,V.F_Part_no,V.F_Ruibetsu,V.F_Store_Code,V.F_kanban_No,V.F_Process_Date,V.F_Delivery_Date
                        From TB_Calculate_V_CKD V INNER Join TB_Calculate_H_CKD H On 
                        V.F_Supplier_Code = H.F_Supplier_Code And V.F_Supplier_Plant = H.F_Supplier_plant And V.F_part_no = H.F_Part_no
                        and V.F_Ruibetsu = H.F_Ruibetsu and V.F_Store_Code = H.F_Store_Code and V.F_kanban_NO = H.F_Kanban_NO 
                        and V.F_Process_Date = H.F_Process_Date 
                        Where H.F_Supplier_Code = '{obj.F_Supplier_Code.Split("-")[0]}' and H.F_SUpplier_Plant='{obj.F_Supplier_Code.Split("-")[1]}'
                        and V.F_Delivery_Date ='{F_Delivery_Date}' and V.F_Lock ='0') M1 ON 
                        M.F_Supplier_Code = M1.F_Supplier_Code And M.F_Supplier_Plant = M1.F_Supplier_plant And M.F_part_no = M1.F_Part_no
                        and M.F_Ruibetsu = M1.F_Ruibetsu and M.F_Store_Code = M1.F_Store_Code and M.F_kanban_NO = M1.F_Kanban_NO 
                        and M.F_Process_Date = M1.F_Process_Date 
                        Where M1.F_Supplier_Code is null)MAIN INNER JOIN TB_Calculate_H_CKD H ON
                        MAIN.F_Supplier_Code = H.F_Supplier_Code And MAIN.F_Supplier_Plant = H.F_Supplier_plant And MAIN.F_part_no = H.F_Part_no
                        and MAIN.F_Ruibetsu = H.F_Ruibetsu and MAIN.F_Store_Code = H.F_Store_Code and MAIN.F_kanban_NO = H.F_Kanban_NO 
                        and MAIN.F_Process_Date = H.F_PRocess_Date ";

                    await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                    _log.WriteLogMsg($"Update TB_Calculate_H_CKD : {_sql}");
                }

                _sql = $@"Update TB_REC_HEADER SET F_TYPE_VERSION ='TRIAL',F_MRN_Flag ='2' 
                    FROM TB_REC_HEADER H INNER JOIN TB_PDS_HEADER H1 ON H.F_OrderNo = H1.F_OrderNo INNER JOIN TB_Supplier_Trial T ON 
                    H.F_Supplier_Code collate Thai_CI_AS = T.F_SUpplier_Code and H.F_Supplier_Plant collate Thai_CI_AS = T.F_SUpplier_Plant 
                    and H.F_Delivery_Date + case when len(H.F_Delivery_Trip) = 1 then '0'+ cast(H.F_Delivery_Trip as char(1)) else cast(H.F_Delivery_Trip as char(2)) end  < T.F_SVP_Date + case when len(F_SVP_TRIP) = 1 then '0' + cast(F_SVP_Trip as char(1)) else cast(F_SVP_Trip as char(2)) end   
                    and H.F_OrderType='N' and H.F_SUpplier_Code='9999'";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);

                _sql = $@"UPDATE TB_Transaction set F_Reg_Flg='3' WHERE  F_OrderType ='N'
                    AND F_Reg_Flg = '2' AND F_Plant='{_BearerClass.Plant}'
                    and F_Process_Date ='{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}'
                    and F_Process_Shift ='{KBNOR310.chrProcessShift_CKD}'
                    and F_SUpplier_Cd='9999' ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg($"Update TB_Transactio.F_Reg_Flg : {_sql}");

                _sql = $@"DELETE FROM  TB_ORDER_CKD
                    WHERE  F_OrderType ='N'
                    AND F_Plant='{_BearerClass.Plant}'
                    and F_For_Date ='{DateTime.Now.ToString("yyyyMMdd")}'
                    and F_FOr_SHift ='{_BearerClass.Shift}'";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg($"Delete TB_ORDER_CKD : {_sql}");

                _sql = $@"UPDATE TB_Kanban_CUT SET F_KB_Remain = F_KB_Remain + ORD.F_KB_CUT, F_Update_Date=Getdate(),F_Update_By ='{_BearerClass.UserCode}',
                    F_Status = case when F_KB_Remain + ORD.F_KB_CUT <= 0 then '3' else '2' end  ,F_Start_Date = case when F_Status ='1' then '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}' else F_Start_Date end  ,
                    F_Start_Shift = case when F_Status ='1' then 'D' else F_Start_SHift end,
                    F_Finish_Date = case when F_KB_Remain + ORD.F_KB_CUT = 0 then '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}' else '' end ,F_Finish_Trip = case when F_KB_Remain + ORD.F_KB_CUT = 0 then ORD.F_Delivery_Trip else '' end
                    from (select D.F_SUpplier_Code,D.F_SUpplier_Plant,D.F_Part_no,D.F_Ruibetsu,D.F_Store_Code,D.F_kanban_No,D.F_Process_Date,V.F_Delivery_Date,Max(V.F_Delivery_Round) as F_Delivery_Trip,SUM(D.F_KB_CUT/nullif(D.F_QTY_BOX, 0)) as F_KB_CUT
                    from TB_Calculate_D_CKD D Inner join TB_Calculate_V_CKD V ON V.F_Supplier_Code = D.F_Supplier_Code and V.F_Supplier_Plant = D.F_Supplier_Plant 
                    and V.F_part_No = D.F_part_No and V.F_Ruibetsu = D.F_Ruibetsu and V.F_Kanban_NO = D.F_Kanban_NO and V.F_STore_Code = D.F_STore_Code 
                    And V.F_Process_Date = D.F_Process_Date And V.F_Process_round = D.F_Process_round
                    Where D.F_Process_Date ='{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}' and D.F_Process_Shift='{KBNOR310.chrProcessShift_CKD}' 
                    and D.F_Supplier_Code='9999' 
                    Group by D.F_SUpplier_Code,D.F_SUpplier_Plant,D.F_Part_no,D.F_Ruibetsu,D.F_Store_Code,D.F_kanban_No,D.F_Process_Date,V.F_Delivery_Date
                    Having SUM(D.F_KB_CUT) < 0)Ord INNER JOIN TB_KANBAN_CUT C ON Ord.F_Supplier_Code = C.F_Supplier_Code and ORD.F_Supplier_Plant = C.F_Supplier_Plant 
                    and ORD.F_part_No = C.F_part_No and ORD.F_Ruibetsu = C.F_Ruibetsu and ORD.F_Kanban_NO = C.F_Kanban_NO and ORD.F_STore_Code = C.F_STore_Code ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg($"Update TB_Kanban_CUT : {_sql}");

                _sql = $@"UPDATE TB_Kanban_ADD SET F_KB_Remain = F_KB_Remain - ORD.F_KB_ADD, F_Update_Date=Getdate(),F_Update_By ='{_BearerClass.UserCode}',
                    F_Status = case when F_KB_Remain - ORD.F_KB_ADD <= 0 then '3' else '2' end  ,F_Start_Date = case when F_Status ='1' then '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}' else F_Start_Date end  ,
                    F_Start_Shift = case when F_Status ='1' then 'D' else F_Start_SHift end,
                    F_Finish_Date = case when F_KB_REMAIN - ORD.F_KB_ADD = 0 then '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}' 
                    else '' end ,F_Finish_Trip = case when F_KB_REMAIN - ORD.F_KB_ADD = 0 then ORD.F_Delivery_Trip else '' end
                    from (select D.F_SUpplier_Code,D.F_SUpplier_Plant,D.F_Part_no,D.F_Ruibetsu,D.F_Store_Code,D.F_kanban_No,D.F_Process_Date,V.F_Delivery_Date,Max(V.F_Delivery_Round) as F_Delivery_Trip,SUM(D.F_KB_ADD/nullif(D.F_QTY_BOX, 0)) as F_KB_ADD
                    from TB_Calculate_D_CKD D Inner join TB_Calculate_V_CKD V ON V.F_Supplier_Code = D.F_Supplier_Code and V.F_Supplier_Plant = D.F_Supplier_Plant 
                    and V.F_part_No = D.F_part_No and V.F_Ruibetsu = D.F_Ruibetsu and V.F_Kanban_NO = D.F_Kanban_NO and V.F_STore_Code = D.F_STore_Code 
                    And V.F_Process_Date = D.F_Process_Date And V.F_Process_round = D.F_Process_round
                    Where D.F_Process_Date ='{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}' and D.F_Process_Shift='{KBNOR310.chrProcessShift_CKD}'
                    and D.F_Supplier_Code='9999'
                    Group by D.F_SUpplier_Code,D.F_SUpplier_Plant,D.F_Part_no,D.F_Ruibetsu,D.F_Store_Code,D.F_kanban_No,D.F_Process_Date,V.F_Delivery_Date
                    Having SUM(D.F_KB_ADD) > 0)Ord INNER JOIN TB_KANBAN_ADD A ON Ord.F_Supplier_Code = A.F_Supplier_Code and ORD.F_Supplier_Plant = A.F_Supplier_Plant 
                    and ORD.F_part_No = A.F_part_No and ORD.F_Ruibetsu = A.F_Ruibetsu and ORD.F_Kanban_NO = A.F_Kanban_NO and ORD.F_STore_Code = A.F_STore_Code ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg($"Update TB_Kanban_ADD : {_sql}");

                _sql = $@"UPDATE TB_Kanban_Stop set F_Update_Date=Getdate(),F_Update_By ='{_BearerClass.UserCode}' 
                    ,F_Status = '2' 
                    ,F_Start_Date = case when F_Status ='1' then '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}' else F_Start_Date end
                    ,F_Start_Shift = case when F_Status ='1' then '{KBNOR310.chrProcessShift_CKD}' else F_Start_Shift end
                    from TB_Kanban_Stop C INNER JOIN TB_Calculate_V_CKD D
                    on C.F_Supplier_Code = D.F_Supplier_Code collate THAI_CI_AS
                    and C.F_Supplier_Plant = D.F_Supplier_Plant Collate THAI_CI_AS and C.F_Store_Code = D.F_Store_Code Collate THAI_CI_AS
                    and C.F_Kanban_No = D.F_Kanban_No collate THAI_CI_AS and C.F_Part_no = D.F_Part_no Collate THAI_CI_AS
                    and C.F_ruibetsu = D.F_ruibetsu collate THAI_CI_AS
                    and C.F_Delivery_Date <= D.F_Delivery_Date collate THAI_CI_AS
                    and C.F_Delivery_Trip <= D.F_Delivery_Round
                    Where D.F_Process_Date ='{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}' 
                    and (F_Status ='1' or F_Status ='2') and C.F_Supplier_Code='9999'";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg($"Update TB_Kanban_Stop : {_sql}");

                _sql = $@"UPDATE TB_Kanban_Chg_Qty set F_Update_Date=Getdate(),F_Update_By ='{_BearerClass.UserCode}'
                    ,F_Status = '2' 
                    ,F_Start_Date = case when F_Status ='1' then '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}' else F_Start_Date end
                    ,F_Start_Shift = case when F_Status ='1' then '{KBNOR310.chrProcessShift_CKD}' else F_Start_Shift end
                    from TB_Kanban_Chg_Qty C INNER JOIN TB_Calculate_V_CKD D
                    on C.F_Supplier_Code = D.F_Supplier_Code collate THAI_CI_AS
                    and C.F_Supplier_Plant = D.F_Supplier_Plant Collate THAI_CI_AS and C.F_Store_Code = D.F_Store_Code Collate THAI_CI_AS
                    and C.F_Kanban_No = D.F_Kanban_No collate THAI_CI_AS and C.F_Part_no = D.F_Part_no Collate THAI_CI_AS
                    and C.F_ruibetsu = D.F_ruibetsu collate THAI_CI_AS
                    and C.F_Delivery_Date <= D.F_Delivery_Date collate THAI_CI_AS
                    and C.F_Delivery_Trip <= D.F_Delivery_Round
                    Where D.F_Process_Date ='{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}' 
                    and (F_Status ='1' or F_Status ='2') and C.F_Supplier_Code='9999'   ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg($"Update TB_Kanban_Chg_Qty : {_sql}");

                _sql = $@"UPDATE TB_Calculate_V_CKD 
                    SET F_lock = '2', F_OrderNO = 'No Gen' 
                    FROM TB_Calculate_D_CKD D INNER JOIN TB_Calculate_Volume V 
                    ON D.F_Supplier_Code = V.F_Supplier_Code 
                    AND D.F_Supplier_Plant = V.F_Supplier_Plant 
                    AND D.F_Part_No = V.F_Part_No 
                    AND D.F_Ruibetsu = V.F_Ruibetsu 
                    AND D.F_Store_Code = V.F_Store_Code 
                    AND D.F_Kanban_No = V.F_Kanban_No 
                    AND D.F_Process_Date = V.F_Process_Date 
                    AND D.F_Process_Round = V.F_Process_Round 
                    Where V.F_Process_Date ='{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}' 
                    AND D.F_Process_Shift ='{KBNOR310.chrProcessShift_CKD}' 
                    AND V.F_OrderNo = '' and V.F_Lock='0' ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg($"UPDATE TB_Calculate_Volume (Incase Not Generate PDS) : {_sql}");

                foreach (var obj in listObj)
                {
                    _sql = $@"DELETE FROM  TB_PDS_HEADER 
                        WHERE F_Plant = '{_BearerClass.Plant}'
                        AND F_OrderType = 'N'
                        AND F_Supplier_Code = '{obj.F_Supplier_Code.Split("-")[0]}'
                        AND F_Supplier_Plant = '{obj.F_Supplier_Code.Split("-")[1]}'
                        AND F_Delivery_Date = '{F_Delivery_Date}'
                        AND F_Delivery_Trip = '{obj.F_Delivery_Trip}'
                        AND F_OrderNo = '{obj.F_OrderNo}'
                    ";

                    await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                    _log.WriteLogMsg($"Delete TB_PDS_HEADER : {_sql}");

                    _sql = $@"DELETE FROM  TB_PDS_Detail 
                        WHERE F_OrderNo = '{obj.F_OrderNo}'";

                    await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                    _log.WriteLogMsg($"Delete TB_PDS_Detail : {_sql}");
                }

                await _kbContext.Database.ExecuteSqlRawAsync("UPDATE TB_MS_Parameter SET F_Value2 = '5' WHERE F_Code = 'ST_CKD'");
            }
            catch (Exception ex)
            {
                string _sql = ""; 
                foreach(var obj in listObj)
                {
                    _sql = $@"DELETE FROM  TB_REC_HEADER 
                        WHERE F_Plant = '{_BearerClass.Plant}'
                        AND F_OrderType = 'N'
                        AND F_Supplier_Code = '{obj.F_Supplier_Code.Split("-")[0]}'
                        AND F_Supplier_Plant = '{obj.F_Supplier_Code.Split("-")[1]}'
                        AND F_Delivery_Date = '{F_Delivery_Date}'
                        AND F_Delivery_Trip = '{obj.F_Delivery_Trip}'
                        AND F_OrderNo = '{obj.F_OrderNo}'
                    ";

                    await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                    _log.WriteLogMsg($"ERROR DELETE TB_REC_HEADER : {_sql}");

                    _sql = $@"DELETE FROM  TB_REC_DETAIL 
                        WHERE F_OrderNo = '{obj.F_OrderNo}'";

                    await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                    _log.WriteLogMsg($"ERROR DELETE TB_REC_DETAIL: {_sql}");
                }

                _sql = $@"UPDATE TB_Transaction set F_Reg_Flg='2' WHERE  F_OrderType ='N'
                    AND F_Reg_Flg = '3' AND F_Plant= '{_BearerClass.Plant}'
                    and F_Process_Date ='{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}'   
                    and F_Process_Shift ='{KBNOR310.chrProcessShift_CKD}'
                    ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg($"ERROR UPDATE TB_Transaction.F_Reg_Flg : {_sql}");

                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<List<string>> GeneratePicking_Click()
        {
            try
            {
                string _sql = $@"SELECT F_OrderNo 
                    FROM TB_PDS_CKD_Picking
                    WHERE F_Supplier_Code = '9999'
                    AND F_Issued_Date = '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}'
                    AND F_Issued_Shift = '{KBNOR310.chrProcessShift_CKD}'
                    AND F_Flag_CompletePicking = 0
                    AND F_OrderType = 'N'";

                var parmListOfPDSNo = await _FillDT.ExecuteSQLAsync(_sql);

                if (parmListOfPDSNo.Rows.Count > 0)
                {
                    var oPIProcess = await _FillDT.ExecuteSQLAsyncCKDWH("SELECT * FROM TB_PI_Control");
                    if(oPIProcess.Rows.Count > 0)
                    {
                        throw new CustomHttpException(400, $@"ระบบ {oPIProcess.Rows[0].ItemArray[0].ToString()} กำลังคำนวณ Picking อยู่!!! กรุณารอสักครู่ แล้วลองใหม่อีกครั้ง");
                    }

                    await _CKDContext.Database.ExecuteSqlRawAsync("EXEC SP_CKDPI_001_LockPickingProcess 'KANBAN'");
                    await  CalculatePicking_01(parmListOfPDSNo);
                    await _kbContext.Database.ExecuteSqlRawAsync($@"EXEC [CKD_Inhouse].SP_UpdatePDS_CKD_Picking '{PI_Date}','{PI_Time}','{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}','{KBNOR310.chrProcessShift_CKD}'");
                    await CalculatePicking_02();
                    await _CKDContext.Database.ExecuteSqlRawAsync("EXEC SP_CKDPI_001_UnLockPickingProcess");
                }

                _sql = $@"SELECT F_OrderNo FROM TB_PDS_CKD_Picking
                        WHERE F_Supplier_Code = '9995'
                        AND F_Issued_Date = '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}'
                        AND F_Issued_Shift = '{KBNOR310.chrProcessShift_CKD}'
                        AND F_Flag_CompletePicking = 0 AND F_OrderType = 'N'";

                parmListOfPDSNo = await _FillDT.ExecuteSQLAsync(_sql);

                if (parmListOfPDSNo.Rows.Count > 0)
                {
                    var oPIProcess = await _FillDT.ExecuteSQLAsyncCKDWH("SELECT * FROM TB_PI_Control");
                    if (oPIProcess.Rows.Count > 0)
                    {
                        throw new CustomHttpException(400, $@"ระบบ {oPIProcess.Rows[0].ItemArray[0].ToString()} กำลังคำนวณ Picking อยู่!!! กรุณารอสักครู่ แล้วลองใหม่อีกครั้ง");
                    }
                    await _CKDContext.Database.ExecuteSqlRawAsync("EXEC SP_CKDPI_001_LockPickingProcess 'KANBAN'");

                    await CalculatePicking_01_USA(parmListOfPDSNo);

                    await _kbContext.Database.ExecuteSqlRawAsync($@"EXEC [CKD_Inhouse].SP_UpdatePDS_CKD_Picking_USA '{PI_Date}','{PI_Time}','{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}','{KBNOR310.chrProcessShift_CKD}'");

                    await CalculatePicking_02_USA();

                    await _CKDUSAContext.Database.ExecuteSqlRawAsync("EXEC SP_CKDPI_001_UnLockPickingProcess");

                }

                _sql = $@"UPDATE TB_MS_Parameter SET F_Value2 = '0', F_Value3 = '', F_Update_By = '{_BearerClass.UserCode}'
                    , F_Update_Date = getdate() WHERE F_Code = 'CI_CKD';
                    UPDATE TB_MS_Parameter SET F_Value3 = '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}{KBNOR310.chrProcessShift_CKD}', F_Update_By = '{_BearerClass.UserCode}'
                    , F_Update_Date = getdate() WHERE F_Code = 'LO_CKD';
                    UPDATE TB_MS_Parameter SET F_Value2 = '0' WHERE F_Code = 'ST_CKD';
                    ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);

                return new List<string>
                {
                    "Picking Process Completed Successfully!",
                    PI_Date_RemainShelf,
                    PI_Time_RemainShelf
                };

            }
            catch (Exception ex)
            {
                await _CKDContext.Database.ExecuteSqlRawAsync($"EXEC sp_CKDPI_001_New_ClearPIData '{PI_Date}','{PI_Time}'");
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task CalculatePicking_01(DataTable parmListOfPDSNo)
        {
            try
            {
                DataTable DT_TempShelf = new DataTable(), DT = new DataTable();
                DataTable oPliD = await _FillDT.ExecuteSQLAsyncCKDWH("SELECT * FROM VW_PI_GetPIID");

                PI_Date = oPliD.Rows[0]["F_PI_Date"].ToString();
                PI_Time = oPliD.Rows[0]["F_PI_Time"].ToString();
                PI_Shift = oPliD.Rows[0]["F_PI_Shift"].ToString();
                PI_By = "KANBAN";

                await calculatePickingForKanban(parmListOfPDSNo);

                string _sql = $@"SELECT * FROM TB_PI_New_Gen_Remain_Shelf
                    WHERE F_PI_Date = '{PI_Date}'
                    AND F_PI_Time = '{PI_Time}'";

                DT_TempShelf = await _FillDT.ExecuteSQLAsyncCKDWH(_sql);

                if(DT_TempShelf.Rows.Count > 0)
                {
                    PI_Date_RemainShelf = PI_Date;
                    PI_Time_RemainShelf = PI_Time;

                    await _CKDContext.Database.ExecuteSqlRawAsync($@"EXEC SP_CKDPI_001_New_CancelPicking 
                        '{PI_Date_RemainShelf}','{PI_Time_RemainShelf}','{_BearerClass.UserCode}'");

                    _sql = $@"SELECT * FROM TB_REC_HEADER H
                        WHERE CONVERT(CHAR(8),H.F_Issued_Date,112) = '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}'
                        AND H.F_Issued_Shift = '{KBNOR310.chrProcessShift_CKD}' 
                        AND H.F_OrderType = 'U'
                        AND H.F_OrderNo LIKE '%UT%'
                        AND H.F_Supplier_Code ='9999'";

                    DT = await _FillDT.ExecuteSQLAsync(_sql);
                    if (DT.Rows.Count > 0)
                    {
                        throw new CustomHttpException(400, "ระบบมี Urgent Temp (9999) ของ Process Date : " + KBNOR310.dateProcessDate_CKD.ToString("dd/MM/yyyy") + " " + KBNOR310.chrProcessShift_CKD);
                    }
                    else
                    {
                        await _kbContext.Database.ExecuteSqlRawAsync($"EXEC [CKD_Inhouse].[sp_RegisterPDS_UT] '{PI_Date}','{PI_Time}','{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}','{KBNOR310.chrProcessShift_CKD}','{_BearerClass.UserCode}'");
                    }

                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task calculatePickingForKanban(DataTable parmListOfPDSNo)
        {
            try
            {
                await InterfaceCKDPDS(parmListOfPDSNo);

                for(int iTemNo = 0; iTemNo < parmListOfPDSNo.Rows.Count; iTemNo++)
                {
                    var test = parmListOfPDSNo.Rows[iTemNo][0].ToString();
                    var rowAff = await _CKDContext.Database.ExecuteSqlRawAsync($@"EXEC SP_CKDPI_001_Insert_TB_PI_Gen_Summary '{PI_Date}','{PI_Time}','{PI_Shift}','{PI_By}','{parmListOfPDSNo.Rows[iTemNo][0].ToString()}'");
                }

                await _CKDContext.Database.ExecuteSqlRawAsync($@"EXEC SP_CKDPI_001_Insert_TB_PI_Gen_Temp '{PI_Date}' ,'{PI_Time}'
                    ,'{PI_By}' , '{PI_Shift}' ");

                var DTM = await _FillDT.ExecuteSQLAsyncCKDWH($"SELECT * FROM FN_CKDPI_001_Get_OrderInfo('{PI_Date}','{PI_Time}')");

                if(DTM.Rows.Count > 0)
                {
                    string sPartNo = "", sRuibetsu = "";
                    for(int i = 0; i < DTM.Rows.Count; i++)
                    {
                        sPartNo = DTM.Rows[i]["F_Part_No"].ToString().Trim();
                        sRuibetsu = DTM.Rows[i]["F_Ruibetsu"].ToString().Trim();
                        await _CKDContext.Database.ExecuteSqlRawAsync($@"SP_CKDPI_001_New_CalculatePI  '{PI_Date}','{PI_Time}','{PI_Shift}','{PI_By}','{sPartNo}','{sRuibetsu}'");
                    }
                }
                else
                {
                    await _CKDContext.Database.ExecuteSqlRawAsync($@"EXEC sp_CKDPI_001_New_ClearPIData '{PI_Date}','{PI_Time}'");
                    throw new CustomHttpException(400, "ไม่พบข้อมูลสำหรับ Generate Picking !!");
                }
                await _CKDContext.Database.ExecuteSqlRawAsync($@"EXEC SP_CKDPI_001_Update_TB_PI_Gen_Summary '{PI_Date}','{PI_Time}'");
                await _CKDContext.Database.ExecuteSqlRawAsync($@"EXEC SP_CKDPI_001_Insert_TB_PI_Gen_Remain_Shelf '{PI_Date}','{PI_Time}'");

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task InterfaceCKDPDS(DataTable parmListOfPDSNo)
        {
            try
            {
                var DT = new DataTable();
                string OrderNo = "",_sql = "";
                string KBN_Connect = _FillDT.kbnConnect();
                for (int i = 0; i < parmListOfPDSNo.Rows.Count; i++)
                {
                    OrderNo = parmListOfPDSNo.Rows[i]["F_OrderNo"].ToString();

                    _sql = $@"Delete from TB_KB_PDS_Header Where F_OrderNo = '{OrderNo}'";
                    _sql += $@"; Delete from TB_KB_PDS_Detail Where F_OrderNo = '{OrderNo}'";

                    await _CKDContext.Database.ExecuteSqlRawAsync(_sql);

                    _sql = $@"Insert into TB_KB_PDS_Header 
                        (F_OrderNo, F_PO_Customer, F_Plant, F_Supplier_Code, F_Supplier_Plant, F_Delivery_Date, F_Delivery_Trip, F_Delivery_Time, F_Delivery_Cycle, F_Delivery_Dock, 
                        F_OrderType, F_Issued_By, F_Issued_Date, F_Issued_Shift, F_Dept, F_Status, F_Vat, F_Dock_Code, F_MRN_Flag, F_Printed, F_Remark, F_Remark2, F_Remark3, 
                        F_Remark_KB, F_Transportor, F_Collect_Date, F_Collect_Time, F_OrderNo_Old, F_Barcode, F_PDS_CKD, F_Flag_Cancel, F_Cancel_By, F_Cancel_Date, F_Type_PDS, 
                        F_Supplier_Type)
                        Select F_OrderNo, F_PO_Customer, F_Plant, F_Supplier_Code, F_Supplier_Plant, F_Delivery_Date, F_Delivery_Trip, F_Delivery_Time, F_Delivery_Cycle, F_Delivery_Dock,
                        F_OrderType, F_Issued_By, F_Issued_Date, F_Issued_Shift, F_Dept, F_Status,F_Vat, F_Dock_Code, F_MRN_Flag, F_Printed, F_Remark, F_Remark2, F_Remark3, 
                        F_Remark_KB, F_Transportor, F_Collect_Date, F_Collect_Time, F_OrderNo_Old, F_Barcode,F_PDS_CKD, F_Flag_Cancel, F_Cancel_By, F_Cancel_Date, 
                        'Interface', '' 
                        From {KBN_Connect}.dbo.TB_Rec_Header With (Nolock) 
                        Where F_OrderNo = '{OrderNo}';

                        Insert into TB_KB_PDS_Detail(F_OrderNo, F_Part_No, F_Ruibetsu, F_Kanban_No, F_Box_Qty, 
                        F_Unit_price, F_No, F_Unit_Amount, F_Part_Name, F_Address, F_Inf_KB, F_Dock_CD) 
                        Select F_OrderNo, F_Part_No, F_Ruibetsu, F_Kanban_No, F_Box_Qty, 
                        F_Unit_price, F_No, F_Unit_Amount, F_Part_Name, F_Address, F_Inf_KB, F_Dock_CD 
                        From {KBN_Connect}.dbo.TB_Rec_Detail With (Nolock) 
                        Where F_OrderNo = '{OrderNo}';";

                    await _CKDContext.Database.ExecuteSqlRawAsync(_sql);
                    _log.WriteLogMsg($"Insert PDS Detail for CKD Warehouse : {_sql}");

                    _sql = $@"UPDATE TB_Rec_Header
                        SET F_CKD_Flag  = '1' WHERE F_OrderNo = '{OrderNo}'
                        AND F_CKD_Flag = '0' ;
                        
                        UPDATE TB_PDS_CKD_Picking   
                        SET F_Flag_CompleteTransfer = 1
                        WHERE F_OrderNo = '{OrderNo}'";

                    await _kbContext.Database.ExecuteSqlRawAsync(_sql);

                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task CalculatePicking_02()
        {
            try
            {
                string ckdDB = _BearerClass.Plant switch
                {
                    "3" => "[HMMTA-APP09]",
                    "2" => "[HMMT-CKD-WH]",
                    "1" => "[HMMT-CKD-WH]",
                    _ => throw new CustomHttpException(400, "ไม่พบข้อมูล Plant Code ในระบบ")
                };
                DataTable parmListOfPDSNo = new DataTable();
                DataTable oPIiD = new DataTable();
                var DT = await _FillDT.ExecuteSQLAsyncCKDWH("SELECT DISTINCT F_Group_ID FROM TB_PI_New_Gen_Group");

                foreach(DataRow row in DT.Rows)
                {
                    string _sql = $@"SELECT F_OrderNo, F_Supplier_Code, F_Supplier_Plant
                        FROM TB_PDS_CKD_Picking P
                        WHERE P.F_Supplier_Code = '9999'
                        AND P.F_Flag_CompletePicking = 0
                        AND P.F_Issued_Date = '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}'
                        AND P.F_Issued_Shift = '{KBNOR310.chrProcessShift_CKD}'
                        AND P.F_Supplier_Plant IN (	SELECT F_Supplier_Plant
                            FROM {ckdDB}.[CKD_WH_STOCK].[dbo].TB_PI_New_Gen_Group
                            WHERE F_Group_ID = '{row["F_Group_ID"].ToString()}')";

                    parmListOfPDSNo = await _FillDT.ExecuteSQLAsync(_sql);

                    if(parmListOfPDSNo.Rows.Count > 0)
                    {
                        oPIiD = await _FillDT.ExecuteSQLAsyncCKDWH($"SELECT * FROM VW_PI_GetPIID");

                        PI_Date = oPIiD.Rows[0]["F_PI_Date"].ToString();
                        PI_Time = oPIiD.Rows[0]["F_PI_Time"].ToString();
                        PI_Shift = oPIiD.Rows[0]["F_PI_Shift"].ToString();
                        PI_By = "KANBAN";

                        await InterfaceCKDPDS(parmListOfPDSNo);
                    }

                }

                await _kbContext.Database.ExecuteSqlRawAsync($"EXEC [CKD_Inhouse].SP_UpdatePDS_CKD_Picking '{PI_Date}','{PI_Time}','{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}','{KBNOR310.chrProcessShift_CKD}'");

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        private async Task CalculatePicking_01_USA(DataTable parmListOfPDSNo)
        {
            try
            {
                var DT_TempShelf = new DataTable();
                DataTable oPliD = await _FillDT.ExecuteSQLAsyncCKDUSA("SELECT * FROM VW_PI_GetPIID");
                PI_Date = oPliD.Rows[0]["F_PI_Date"].ToString();
                PI_Time = oPliD.Rows[0]["F_PI_Time"].ToString();
                PI_Shift = oPliD.Rows[0]["F_PI_Shift"].ToString();
                PI_By = "KANBAN";

                await calculatePickingForKanban_USA(parmListOfPDSNo);

                string _sql = $@"SELECT * FROM TB_PI_New_Gen_Remain_Shelf
                    WHERE F_PI_Date = '{PI_Date}'
                    AND F_PI_Time = '{PI_Time}'";

                DT_TempShelf = await _FillDT.ExecuteSQLAsyncCKDUSA(_sql);
                if(DT_TempShelf.Rows.Count > 0)
                {
                    await _CKDUSAContext.Database.ExecuteSqlRawAsync($@"EXEC SP_CKDPI_001_New_CancelPicking  
                        '{PI_Date}','{PI_Time}','{_BearerClass.UserCode}'");

                    _sql = $@"SELECT * FROM TB_REC_HEADER H
                        WHERE CONVERT(CHAR(8),H.F_Issued_Date,112) = '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}'
                        AND H.F_Issued_Shift = '{KBNOR310.chrProcessShift_CKD}' 
                        AND H.F_OrderType = 'U'
                        AND H.F_OrderNo LIKE '%UT%'
                        AND H.F_Supplier_Code ='9995'";

                    var DT = await _FillDT.ExecuteSQLAsyncCKDUSA(_sql);

                    if(DT.Rows.Count > 0)
                    {
                        throw new CustomHttpException(400, $@"ระบบมี Urgent Temp (9995) ของ Process Date : {KBNOR310.dateProcessDate_CKD.ToString("dd/MM/yyyy")} Process Shift : {KBNOR310.chrProcessShift_CKD} ");
                    }
                    else
                    {
                        await _kbContext.Database.ExecuteSqlRawAsync($"EXEC [CKD_Inhouse].[sp_RegisterPDS_UT_USA] '{PI_Date}','{PI_Time}','{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}','{KBNOR310.chrProcessShift_CKD}','{_BearerClass.UserCode}'");
                    }


                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        private async Task CalculatePicking_02_USA()
        {
            try
            {
                DataTable parmListOfPDSNo = new DataTable();
                DataTable oPIiD = new DataTable();
                var DT = await _FillDT.ExecuteSQLAsyncCKDUSA("SELECT DISTINCT F_Group_ID FROM TB_PI_New_Gen_Group");

                foreach(DataRow row in DT.Rows)
                {
                    string _sql = $@"SELECT F_OrderNo, F_Supplier_Code, F_Supplier_Plant
                        FROM TB_PDS_CKD_Picking P
                        WHERE P.F_Supplier_Code = '9995'
                        AND P.F_Flag_CompletePicking = 0
                        AND P.F_Issued_Date = '{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}'
                        AND P.F_Issued_Shift = '{KBNOR310.chrProcessShift_CKD}'
                        AND P.F_Supplier_Plant IN (	SELECT F_Supplier_Plant
                            FROM [HMMT-CKD-WH].[USA_CKD_WH_STOCK].[dbo].TB_PI_New_Gen_Group
                            WHERE F_Group_ID = '{row["F_Group_ID"].ToString()}')";

                    parmListOfPDSNo = await _FillDT.ExecuteSQLAsync(_sql);

                    if(parmListOfPDSNo.Rows.Count > 0)
                    {
                        oPIiD = await _FillDT.ExecuteSQLAsyncCKDUSA($"SELECT * FROM VW_PI_GetPIID");

                        PI_Date = oPIiD.Rows[0]["F_PI_Date"].ToString();
                        PI_Time = oPIiD.Rows[0]["F_PI_Time"].ToString();
                        PI_Shift = oPIiD.Rows[0]["F_PI_Shift"].ToString();
                        PI_By = "KANBAN";

                        await InterfaceCKDPDS_USA(parmListOfPDSNo);
                    }

                }

                await _kbContext.Database.ExecuteSqlRawAsync($"EXEC [CKD_Inhouse].SP_UpdatePDS_CKD_Picking_USA '{PI_Date}','{PI_Time}','{KBNOR310.dateProcessDate_CKD.ToString("yyyyMMdd")}','{KBNOR310.chrProcessShift_CKD}'");

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        private async Task calculatePickingForKanban_USA(DataTable parmListOfPDSNo)
        {
            try
            {
                await InterfaceCKDPDS_USA(parmListOfPDSNo);

                for (int iTemNo = 0; iTemNo < parmListOfPDSNo.Rows.Count; iTemNo++)
                {
                    await _CKDUSAContext.Database.ExecuteSqlRawAsync(@"EXEC SP_CKDPI_001_Insert_TB_PI_Gen_Summary '{0}','{1}','{2}','{3}','{4}'",
                        PI_Date, PI_Time, PI_Shift, PI_By, parmListOfPDSNo.Rows[iTemNo][0].ToString());

                }
                await _CKDUSAContext.Database.ExecuteSqlRawAsync($@"EXEC SP_CKDPI_001_Insert_TB_PI_Gen_Temp '{PI_Date}' ,'{PI_Time}'
                    ,'{PI_By}' , '{PI_Shift}' ");

                var DTM = await _FillDT.ExecuteSQLAsyncCKDUSA($"SELECT * FROM FN_CKDPI_001_Get_OrderInfo('{PI_Date}','{PI_Time}')");

                if (DTM.Rows.Count > 0)
                {
                    string sPartNo = "", sRuibetsu = "";
                    for (int i = 0; i < DTM.Rows.Count; i++)
                    {
                        sPartNo = DTM.Rows[i]["F_Part_No"].ToString().Trim();
                        sRuibetsu = DTM.Rows[i]["F_Ruibetsu"].ToString().Trim();
                        await _CKDUSAContext.Database.ExecuteSqlRawAsync($@"SP_CKDPI_001_New_CalculatePI  '{PI_Date}','{PI_Time}','{PI_Shift}','{PI_By}','{sPartNo}','{sRuibetsu}'");
                    }
                }
                else
                {
                    await _CKDUSAContext.Database.ExecuteSqlRawAsync($@"EXEC sp_CKDPI_001_New_ClearPIData '{PI_Date}','{PI_Time}'");
                    throw new CustomHttpException(400, "ไม่พบข้อมูลสำหรับ Generate Picking !!");
                }

                await _CKDUSAContext.Database.ExecuteSqlRawAsync($@"EXEC SP_CKDPI_001_Update_TB_PI_Gen_Summary '{PI_Date}','{PI_Time}'");
                await _CKDUSAContext.Database.ExecuteSqlRawAsync($@"EXEC SP_CKDPI_001_Insert_TB_PI_Gen_Remain_Shelf '{PI_Date}','{PI_Time}'");

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        private async Task InterfaceCKDPDS_USA(DataTable parmListOfPDSNo)
        {
            try
            {
                string OrderNo = "";
                string KBN_Connect = _FillDT.kbnConnect();

                for(int i = 0;i < parmListOfPDSNo.Rows.Count; i++)
                {
                    OrderNo = parmListOfPDSNo.Rows[i]["F_OrderNo"].ToString();

                    string _sql = $@"Delete from TB_KB_PDS_Header Where F_OrderNo = '{OrderNo}'";
                    _sql += $@"; Delete from TB_KB_PDS_Detail Where F_OrderNo = '{OrderNo}'";

                    await _CKDUSAContext.Database.ExecuteSqlRawAsync(_sql);

                    _sql = $@"Insert into TB_KB_PDS_Header 
                        (F_OrderNo, F_PO_Customer, F_Plant, F_Supplier_Code, F_Supplier_Plant, F_Delivery_Date, F_Delivery_Trip, F_Delivery_Time, F_Delivery_Cycle, F_Delivery_Dock, 
                        F_OrderType, F_Issued_By, F_Issued_Date, F_Issued_Shift, F_Dept, F_Status, F_Vat, F_Dock_Code, F_MRN_Flag, F_Printed, F_Remark, F_Remark2, F_Remark3, 
                        F_Remark_KB, F_Transportor, F_Collect_Date, F_Collect_Time, F_OrderNo_Old, F_Barcode,F_PDS_CKD,F_Flag_Cancel,F_Cancel_By,F_Cancel_Date,F_Type_PDS,
                        F_Supplier_Type)
                        Select F_OrderNo, F_PO_Customer, F_Plant, F_Supplier_Code, F_Supplier_Plant, F_Delivery_Date, F_Delivery_Trip, F_Delivery_Time, F_Delivery_Cycle, F_Delivery_Dock,
                                F_OrderType, F_Issued_By, F_Issued_Date, F_Issued_Shift, F_Dept, F_Status,F_Vat, F_Dock_Code, F_MRN_Flag, F_Printed, F_Remark, F_Remark2, F_Remark3, 
                                F_Remark_KB, F_Transportor, F_Collect_Date, F_Collect_Time, F_OrderNo_Old, F_Barcode,F_PDS_CKD, F_Flag_Cancel, F_Cancel_By, F_Cancel_Date,'Interface', '' 
                        From {KBN_Connect}.dbo.TB_Rec_Header With (Nolock) 
                        Where 	F_OrderNo = '{OrderNo}';

                        Insert into TB_KB_PDS_Detail(F_OrderNo, F_Part_No, F_Ruibetsu, 
                        F_Kanban_No, 	F_Box_Qty ,F_Unit_price ,F_No ,F_Unit_Amount ,F_Part_Name ,F_Address ,F_Inf_KB ,F_Dock_CD) 
                        Select 	F_OrderNo ,F_Part_No ,F_Ruibetsu ,
                                F_Kanban_No ,F_Box_Qty ,F_Unit_price ,F_No ,F_Unit_Amount ,
                                F_Part_Name ,F_Address ,F_Inf_KB ,F_Dock_CD
                                From {KBN_Connect}.dbo.TB_Rec_Detail With (Nolock)
                                Where F_OrderNo = '{OrderNo}';
                                ";

                    await _CKDUSAContext.Database.ExecuteSqlRawAsync(_sql);
                    _log.WriteLogMsg($"Insert PDS Detail for CKD Warehouse : {_sql}");

                    _sql = $@"UPDATE TB_Rec_Header
                        SET F_CKD_Flag  = '1' WHERE F_OrderNo = '{OrderNo}'
                        AND F_CKD_Flag = '0' ;
                        
                        UPDATE TB_PDS_CKD_Picking   
                        SET F_Flag_CompleteTransfer = 1
                        WHERE F_OrderNo = '{OrderNo}'";

                    await _kbContext.Database.ExecuteSqlRawAsync(_sql);
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
