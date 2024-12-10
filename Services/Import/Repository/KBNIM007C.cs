using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialData.ViewModel;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Import.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;

namespace KANBAN.Services.Import.Repository
{
    public class KBNIM007C : IKBNIM007C
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNIM007C
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

        public string GetPDS(string? DeliDateFrom ,string? DeliDateTo)
        {
            try
            {
                string sql = $@"Select Distinct F_PDS_No From TB_TRANSACTION_TMP 
                    Where (F_Type='Special' or F_TYPE ='Trial' ) ";

                if (!string.IsNullOrEmpty(DeliDateFrom) && !string.IsNullOrEmpty(DeliDateTo))
                {
                    sql += $" and F_Delivery_Date between '{DeliDateFrom}' and '{DeliDateTo}' ";
                }

                sql += " Order by F_PDS_No";

                DataTable dt = _FillDT.ExecuteSQL(sql);

                if (dt.Rows.Count == 0)
                {
                    throw new CustomHttpException(404, "Data Not Found");
                }

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public string GetUser(string? DeliDateFrom, string? DeliDateTo)
        {
            try
            {
                string sql = $@"Select Distinct rtrim(T.F_Update_By) +':' 
                    + rtrim(U.F_User_Name) as F_Update_By From TB_TRANSACTION_TMP
                    T INNER JOIN TB_USER U ON T.F_Update_By = U.F_User_ID  
                     Where (T.F_Type='Special' or T.F_TYPE ='Trial' ) ";

                if (!string.IsNullOrEmpty(DeliDateFrom) && !string.IsNullOrEmpty(DeliDateTo))
                {
                    sql += $" and T.F_Delivery_Date between '{DeliDateFrom}' and '{DeliDateTo}' ";
                }

                sql += " Order by F_Update_By";

                DataTable dt = _FillDT.ExecuteSQL(sql);

                if (dt.Rows.Count == 0)
                {
                    throw new CustomHttpException(404, "Data Not Found");
                }

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public string GetListData(string? DeliDateFrom, string? DeliDateTo,string? PDSNo, string? User)
        {
            try
            {
                string sql = $@"Select Distinct F_PDS_No,case when Len(F_PDS_ISSUED_DATE) = 6 then substring(F_PDS_ISSUED_DATE,5,2)+'/'+substring(F_PDS_ISSUED_DATE,1,4) else substring(F_PDS_ISSUED_DATE,7,2)+'/'+substring(F_PDS_ISSUED_DATE,5,2)+'/'+substring(F_PDS_ISSUED_DATE,1,4) end F_PDS_ISSUED_DATE, 
                    substring(F_Delivery_Date,7,2)+'/'+substring(F_Delivery_Date,5,2)+'/'+substring(F_Delivery_Date,1,4) as F_Delivery_Date 
                    From TB_TRANSACTION_TMP Where (F_Type='Trial' or F_Type='Special') ";

                if (!string.IsNullOrEmpty(DeliDateFrom) && !string.IsNullOrEmpty(DeliDateTo))
                {
                    sql += $" and F_Delivery_Date between '{DeliDateFrom}' and '{DeliDateTo}' ";
                }

                if (!string.IsNullOrEmpty(PDSNo))
                {
                    sql += $" and F_PDS_No = '{PDSNo}' ";
                }

                if (!string.IsNullOrEmpty(User))
                {
                    sql += $" and F_Update_By = '{User}' ";
                }

                sql += " Order by F_PDS_NO,substring(F_Delivery_Date,7,2)+'/'+substring(F_Delivery_Date,5,2)+'/'+substring(F_Delivery_Date,1,4)";

                DataTable dt = _FillDT.ExecuteSQL(sql);

                if(dt.Rows.Count == 0)
                {
                    throw new CustomHttpException(404, "Data Not Found");
                }

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }   

        public async Task Update_Cycle(string ProcessDate)
        {
            try
            {
                string sql = $@"Update TB_Transaction_TMP set F_Cycle_Time = S.F_Cycle 
                    From TB_Transaction_TMP T INNER JOIN TB_MS_DeliveryTime S ON T.F_Supplier_Cd collate Thai_CI_AS = S.F_Supplier_Code  
                    and T.F_Supplier_Plant collate Thai_CI_AS = S.F_Supplier_Plant 
                    Where (T.F_Type='Special' or T.F_Type='Trial') and T.F_Plant = '{_BearerClass.Plant}'
                    and S.F_Start_Date <='{ProcessDate}' and S.F_End_Date >='{ProcessDate}'";

                await _kbContext.Database.ExecuteSqlRawAsync(sql);
                _log.WriteLogMsg("Update Cycle from Delivery Time Master Again | SQL : " + sql);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task Confirm (List<VM_CONF_KBNIM007C> listObj,string? ProcessDate,string? ProcessShift)
        {
            try
            {
                string sql = $@"Update TB_TRANSACTION_TMP set F_Inventory_Flg ='0' 
                    Where (F_Type='Special' or F_Type='Trial') ";

                await _kbContext.Database.ExecuteSqlRawAsync(sql);
                _log.WriteLogMsg("Update Inventory Flag to 0 | SQL : " + sql);

                int rowAffected = await _kbContext.Database.ExecuteSqlRawAsync(
                    "Select distinct F_Inventory_Flg  From TB_TRANSACTION_TMP Where (F_Type='Special" +
                    "' or F_Type='Trial') and F_Inventory_Flg = '9'");

                if (rowAffected > 0)
                {
                    throw new CustomHttpException(400, "Please Check Cycle Time <> Delivery Time Master!!");
                }

                listObj = listObj.DistinctBy(x=>x.F_PDS_No).ToList();

                foreach (var obj in listObj) {


                    string IssuedDate = DateTime.ParseExact(obj.F_PDS_ISSUED_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");
                    string DeliveryDate = DateTime.ParseExact(obj.F_Delivery_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");

                    sql = $@"SELECT distinct C.F_workCd_D1, C.F_workCd_N1, C.F_workCd_D2, C.F_workCd_N2, C.F_workCd_D3, C.F_workCd_N3, C.F_workCd_D4, C.F_workCd_N4, 
                    C.F_workCd_D5, C.F_workCd_N5, C.F_workCd_D6, C.F_workCd_N6, C.F_workCd_D7, C.F_workCd_N7, C.F_workCd_D8, C.F_workCd_N8, 
                    C.F_workCd_D9, C.F_workCd_N9, C.F_workCd_D10, C.F_workCd_N10, C.F_workCd_D11, C.F_workCd_N11, C.F_workCd_D12, C.F_workCd_N12, 
                    C.F_workCd_D13, C.F_workCd_N13, C.F_workCd_D14, C.F_workCd_N14, C.F_workCd_D15, C.F_workCd_N15, C.F_workCd_D16, C.F_workCd_N16, 
                    C.F_workCd_D17, C.F_workCd_N17, C.F_workCd_D18, C.F_workCd_N18, C.F_workCd_D19, C.F_workCd_N19, C.F_workCd_D20, C.F_workCd_N20, 
                    C.F_workCd_D21, C.F_workCd_N21, C.F_workCd_D22, C.F_workCd_N22, C.F_workCd_D23, C.F_workCd_N23, C.F_workCd_D24, C.F_workCd_N24, 
                    C.F_workCd_D25, C.F_workCd_N25, C.F_workCd_D26, C.F_workCd_N26, C.F_workCd_D27, C.F_workCd_N27, C.F_workCd_D28, C.F_workCd_N28, 
                    c.F_workCd_D29, c.F_workCd_N29, c.F_workCd_D30, c.F_workCd_N30, c.F_workCd_D31, c.F_workCd_N31,C.F_Store_cd , C.F_YM 
                    FROM  TB_Transaction_TMP AS T INNER JOIN TB_Calendar AS C ON C.F_YM ='{ProcessDate.Substring(0, 6)}' 
                    AND T.F_Store_CD = C.F_Store_cd collate Thai_CI_AS 
                    WHERE (T.F_Type='Special' or T.F_Type = 'Trial') and F_PDS_NO='{obj.F_PDS_No}' 
                    and F_PDS_Issued_Date = '{IssuedDate}' and T.F_ORDERTYPE='N' ";


                    DataTable dt = _FillDT.ExecuteSQL(sql);

                    if (dt.Rows.Count > 0)
                    {
                        for (int y = 0; y < dt.Rows.Count; y++)
                        {
                            if (ProcessShift.StartsWith("D"))
                            {
                                if (dt.Rows[y].ItemArray[((int.Parse(ProcessDate.Substring(6, 2)) - 1) * 2)].ToString() == "0")
                                {
                                    _log.WriteLogMsg("Please Check Delivery Date Again is Holiday!! | SQL : " + sql);
                                    throw new CustomHttpException(400, "Please Check Delivery Date Again is Holiday!!");
                                }
                            }
                            else
                            {
                                if (dt.Rows[y].ItemArray[((int.Parse(ProcessDate.Substring(6, 2)) * 2) - 1)].ToString() == "0")
                                {
                                    _log.WriteLogMsg("Please Check Delivery Date Again is Holiday!! | SQL : " + sql);
                                    throw new CustomHttpException(400, "Please Check Delivery Date Again is Holiday!!");
                                }
                            }
                        }
                    }


                    sql = $@"INSERT INTO TB_Transaction(F_Type, F_Type_Spc,F_Plant, F_PDS_No, F_PDS_Issued_Date, F_Store_CD, F_Part_No, F_Ruibetsu, F_Kanban_No, F_Part_Name, F_Qty_Pack, F_Part_Order, 
                        F_Ruibetsu_Order, F_Store_Order, F_Name_Order, F_Qty,F_Qty_LEvel1, F_Seq_No, F_Seq_Type, F_Delivery_Date, F_Adv_Deli_Date, F_OrderType, F_Country, 
                        F_Reg_Flg, F_Inventory_Flg, F_Supplier_CD, F_Supplier_Plant, F_Cycle_Time, F_Safty_Stock, F_Part_Refer, F_Ruibetsu_Refer, 
                        F_Update_By, F_Update_Date, F_Process_By, F_Process_Date, F_Remark, F_HMMT_PDS, F_Survey_Doc,F_Parent_Level2, F_Qty_Level2, F_Parent_Level3, F_Qty_Level3, F_Parent_Level4, F_Qty_Level4,F_round)
                        SELECT     F_Type,F_Type_Spc, F_Plant, F_PDS_No, F_PDS_Issued_Date, F_Store_CD, F_Part_No, F_Ruibetsu, F_Kanban_No, F_Part_Name, F_Qty_Pack, F_Part_Order, 
                        F_Ruibetsu_Order, F_Store_Order, F_Name_Order, F_Qty,F_Qty_LEvel1, F_Seq_No, F_Seq_Type, F_Delivery_Date, F_Adv_Deli_Date, F_OrderType, F_Country, 
                        '1' as F_Reg_Flg, F_Inventory_Flg, F_Supplier_CD, F_Supplier_Plant, F_Cycle_Time, F_Safty_Stock, F_Part_Refer, F_Ruibetsu_Refer, 
                        '{_BearerClass.UserCode}' as F_Update_By, getdate() as F_Update_Date,'' as F_Process_By,'' as F_Process_Date, F_Remark,'' as F_HMMT_PDS,F_Survey_Doc,F_Parent_Level2, F_Qty_Level2, F_Parent_Level3, F_Qty_Level3, F_Parent_Level4, F_Qty_Level4,case when F_round = 0 then 1 else F_round end F_Round 
                        FROM TB_Transaction_TMP 
                        Where (F_Type='Special' or F_TYPE ='Trial' ) and F_PDS_NO='{obj.F_PDS_No}' 
                        and F_PDS_Issued_Date = '{IssuedDate}' 
                        and F_Delivery_Date='{DeliveryDate}'";

                    await _kbContext.Database.ExecuteSqlRawAsync(sql);

                    _log.WriteLogMsg("Insert Data to TB_Transaction | SQL : " + sql);

                    sql = $@"Delete From TB_Transaction_TMP 
                        Where (F_Type='Special' or F_TYPE ='Trial' ) and F_PDS_NO='{obj.F_PDS_No}' 
                        and F_PDS_Issued_Date = '{IssuedDate}' 
                        and F_Delivery_Date='{DeliveryDate}' ";

                    await _kbContext.Database.ExecuteSqlRawAsync(sql);

                    _log.WriteLogMsg("Delete Data from TB_Transaction_TMP | SQL : " + sql);

                    await _kbContext.TB_Transaction.Where(x=>x.F_Type == "Special" || x.F_Type == "Trial"
                        && x.F_PDS_Issued_Date == IssuedDate && x.F_PDS_No == obj.F_PDS_No
                        && x.F_Delivery_Date == DeliveryDate)
                        .ExecuteUpdateAsync(x=>x.SetProperty(x=>x.F_Process_Date, ProcessDate)
                        .SetProperty(x => x.F_Process_Shift, ProcessShift[0]));

                    _log.WriteLogMsg("Update Process Date and Shift to TB_Transaction Process Date = " + ProcessDate + " Process Shift = " + ProcessShift);

                }

                await _kbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
