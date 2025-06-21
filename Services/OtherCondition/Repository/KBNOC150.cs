using DocumentFormat.OpenXml.Spreadsheet;
using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.OtherCondition.ViewModel;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.OtherCondition.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NPOI.SS.Formula.Functions;
using System.Reflection.Metadata.Ecma335;

namespace KANBAN.Services.OtherCondition.Repository
{
    public class KBNOC150 : IKBNOC150
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNOC150
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

        public async Task<List<string>> Sup_DropDown()
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            var suppliers = await _PPM3Context.T_Supplier_MS
                                  .Where(s => s.F_TC_Str.CompareTo(date) <= 0
                                           && s.F_TC_End.CompareTo(date) >= 0)
                                  .OrderBy(s => s.F_supplier_cd)
                                  .Select(s => s.F_supplier_cd + "-" + s.F_Plant_cd + " : " + s.F_short_name)
                                  .Distinct()
                                  .ToListAsync();

            return suppliers;

        }
        public async Task Print(VM_REPORT_KBNOC150 model)
        {

            try
            {
                var userID = _BearerClass.UserCode;
                var plant_CTL = _BearerClass.Plant;

                await _kbContext.Database.ExecuteSqlRawAsync($"DELETE FROM [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].RPT_KBNOC_150 WHERE F_Update_By = '{userID}' ");


                string sql = $@"INSERT INTO [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].RPT_KBNOC_150 
                                        (F_Plant, F_Supplier_Code, F_Supplier_Plant, F_Part_No, F_Ruibetsu, F_Kanban_No, 
                                        F_Store_Cd, F_Delivery_Date, F_Delivery_Trip1, F_Delivery_Trip2, F_Delivery_Trip3, 
                                        F_Delivery_Trip4, F_Delivery_Trip5, F_Delivery_Trip6, F_Delivery_Trip7, F_Delivery_Trip8, 
                                        F_Delivery_Trip9, F_Delivery_Trip10, F_Delivery_Trip11, F_Delivery_Trip12, F_Delivery_Trip13,
                                        F_Delivery_Trip14, F_Delivery_Trip15, F_Delivery_Trip16, F_Delivery_Trip17, F_Delivery_Trip18,
                                        F_Delivery_Trip19, F_Delivery_Trip20, F_Delivery_Trip21, F_Delivery_Trip22, F_Delivery_Trip23,
                                        F_Delivery_Trip24, F_Delivery_Summary, F_Cycle_Time, F_Std, F_Min, F_Max, F_Update_By, F_Update_Date)
                                SELECT  H.F_Plant
                                       ,H.F_Supplier_Code
                                       ,H.F_Supplier_Plant
                                       ,D.F_Part_no
                                       ,D.F_ruibetsu
                                       ,D.F_kanban_No
                                       ,H.F_Delivery_Dock
                                       ,H.F_delivery_Date
                                       ,SUM(Case WHEN H.F_Delivery_trip = 1 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip1
                                       ,SUM(Case WHEN H.F_Delivery_trip = 2 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip2
                                       ,SUM(Case WHEN H.F_Delivery_trip = 3 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip3
                                       ,SUM(Case WHEN H.F_Delivery_trip = 4 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip4
                                       ,SUM(Case WHEN H.F_Delivery_trip = 5 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip5
                                       ,SUM(Case WHEN H.F_Delivery_trip = 6 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip6
                                       ,SUM(Case WHEN H.F_Delivery_trip = 7 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip7
                                       ,SUM(Case WHEN H.F_Delivery_trip = 8 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip8
                                       ,SUM(Case WHEN H.F_Delivery_trip = 9 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip9
                                       ,SUM(Case WHEN H.F_Delivery_trip = 10 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip10
                                       ,SUM(Case WHEN H.F_Delivery_trip = 11 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip11
                                       ,SUM(Case WHEN H.F_Delivery_trip = 12 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip12
                                       ,SUM(Case WHEN H.F_Delivery_trip = 13 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip13
                                       ,SUM(Case WHEN H.F_Delivery_trip = 14 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip14
                                       ,SUM(Case WHEN H.F_Delivery_trip = 15 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip15
                                       ,SUM(Case WHEN H.F_Delivery_trip = 16 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip16
                                       ,SUM(Case WHEN H.F_Delivery_trip = 17 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip17
                                       ,SUM(Case WHEN H.F_Delivery_trip = 18 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip18
                                       ,SUM(Case WHEN H.F_Delivery_trip = 19 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip19
                                       ,SUM(Case WHEN H.F_Delivery_trip = 20 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip20
                                       ,SUM(Case WHEN H.F_Delivery_trip = 21 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip21
                                       ,SUM(Case WHEN H.F_Delivery_trip = 22 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip22
                                       ,SUM(Case WHEN H.F_Delivery_trip = 23 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip23
                                       ,SUM(Case WHEN H.F_Delivery_trip = 24 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip24
                                       ,SUM(D.F_Unit_Amount)                                                  AS F_Delivery_Summary
                                       ,H.F_Delivery_Cycle ,0,0 ,0
                                       ,'{userID}' ,getdate()
                                FROM TB_REC_HEADER H
                                INNER JOIN TB_REC_DETAIL D
                                ON H.F_OrderNo = D.F_OrderNo
                                WHERE F_Plant = '{plant_CTL}' 
                                ";

                if (!string.IsNullOrWhiteSpace(model.Cmb_SupF) && !string.IsNullOrWhiteSpace(model.Cmb_SupT))
                    sql += $@"and rtrim(F_Supplier_Code collate THAI_CI_AS) +'-'+F_Supplier_Plant >='{model.Cmb_SupF}' 
                             and rtrim(F_Supplier_Code collate THAI_CI_AS) +'-'+F_Supplier_Plant <='{model.Cmb_SupT}' 
                            ";

                if (!string.IsNullOrWhiteSpace(model.Txt_DeliveryF) && !string.IsNullOrWhiteSpace(model.Txt_DeliveryT))
                    sql += $@"and F_Delivery_Date >='{model.Txt_DeliveryF}'
                              and F_Delivery_Date <='{model.Txt_DeliveryT}' 
                            ";

                sql += @" Group by H.F_Plant,H.F_Supplier_Code,H.F_Supplier_Plant,D.F_Part_no,D.F_ruibetsu,H.F_Delivery_Dock,D.F_kanban_No,H.F_delivery_Date,H.F_Delivery_Cycle";

                await _kbContext.Database.ExecuteSqlRawAsync(sql);

                var connection = _kbContext.Database.GetDbConnection();

                // ดึงค่า Server Name และ Database Name
                string server = connection.DataSource;
                string database = connection.Database;

                sql = $@"INSERT INTO [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].RPT_KBNOC_150 
                                (F_Plant, F_Supplier_Code, F_Supplier_Plant, F_Part_No, F_Ruibetsu, F_Store_Cd, F_Kanban_No, 
                                F_Delivery_Date, F_Delivery_Trip1, F_Delivery_Trip2, F_Delivery_Trip3, F_Delivery_Trip4, 
                                F_Delivery_Trip5, F_Delivery_Trip6, F_Delivery_Trip7, F_Delivery_Trip8, F_Delivery_Trip9, 
                                F_Delivery_Trip10, F_Delivery_Trip11, F_Delivery_Trip12, F_Delivery_Trip13, F_Delivery_Trip14, 
                                F_Delivery_Trip15, F_Delivery_Trip16, F_Delivery_Trip17, F_Delivery_Trip18, F_Delivery_Trip19, 
                                F_Delivery_Trip20, F_Delivery_Trip21, F_Delivery_Trip22, F_Delivery_Trip23, F_Delivery_Trip24, 
                                F_Delivery_Summary, F_Cycle_Time, F_Std, F_Min, F_Max, F_Update_By, F_Update_Date)
                        SELECT  H.F_Plant
                               ,H.F_Supplier_Code
                               ,H.F_Supplier_Plant
                               ,D.F_Part_no
                               ,D.F_ruibetsu
                               ,H.F_Delivery_Dock
                               ,D.F_kanban_No
                               ,H.F_delivery_Date
                               ,SUM(Case WHEN H.F_Delivery_trip = 1 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip1
                               ,SUM(Case WHEN H.F_Delivery_trip = 2 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip2
                               ,SUM(Case WHEN H.F_Delivery_trip = 3 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip3
                               ,SUM(Case WHEN H.F_Delivery_trip = 4 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip4
                               ,SUM(Case WHEN H.F_Delivery_trip = 5 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip5
                               ,SUM(Case WHEN H.F_Delivery_trip = 6 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip6
                               ,SUM(Case WHEN H.F_Delivery_trip = 7 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip7
                               ,SUM(Case WHEN H.F_Delivery_trip = 8 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip8
                               ,SUM(Case WHEN H.F_Delivery_trip = 9 THEN D.F_Unit_amount else 0 end)  AS F_Delivery_Trip9
                               ,SUM(Case WHEN H.F_Delivery_trip = 10 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip10
                               ,SUM(Case WHEN H.F_Delivery_trip = 11 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip11
                               ,SUM(Case WHEN H.F_Delivery_trip = 12 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip12
                               ,SUM(Case WHEN H.F_Delivery_trip = 13 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip13
                               ,SUM(Case WHEN H.F_Delivery_trip = 14 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip14
                               ,SUM(Case WHEN H.F_Delivery_trip = 15 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip15
                               ,SUM(Case WHEN H.F_Delivery_trip = 16 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip16
                               ,SUM(Case WHEN H.F_Delivery_trip = 17 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip17
                               ,SUM(Case WHEN H.F_Delivery_trip = 18 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip18
                               ,SUM(Case WHEN H.F_Delivery_trip = 19 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip19
                               ,SUM(Case WHEN H.F_Delivery_trip = 20 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip20
                               ,SUM(Case WHEN H.F_Delivery_trip = 21 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip21
                               ,SUM(Case WHEN H.F_Delivery_trip = 22 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip22
                               ,SUM(Case WHEN H.F_Delivery_trip = 23 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip23
                               ,SUM(Case WHEN H.F_Delivery_trip = 24 THEN D.F_Unit_amount else 0 end) AS F_Delivery_Trip24
                               ,SUM(D.F_Unit_Amount)                                                  AS F_Delivery_Summary
                               ,H.F_Delivery_Cycle
                               ,0,0,0
                               ,'{userID}' ,getdate()
                        FROM [{server}].[{database}].dbo.TB_REC_HEADER H
                        INNER JOIN [{server}].[{database}].dbo.TB_REC_DETAIL D
                        ON H.F_OrderNo = D.F_OrderNo
                        WHERE F_Plant = '{plant_CTL}' 
                        ";

                if (!string.IsNullOrWhiteSpace(model.Cmb_SupF) && !string.IsNullOrWhiteSpace(model.Cmb_SupT))
                    sql += $@"and rtrim(F_Supplier_Code collate THAI_CI_AS) +'-'+F_Supplier_Plant >='{model.Cmb_SupF}' 
                             and rtrim(F_Supplier_Code collate THAI_CI_AS) +'-'+F_Supplier_Plant <='{model.Cmb_SupT}' 
                            ";

                if (!string.IsNullOrWhiteSpace(model.Txt_DeliveryF) && !string.IsNullOrWhiteSpace(model.Txt_DeliveryT))
                    sql += $@"and F_Delivery_Date >='{model.Txt_DeliveryF}'
                              and F_Delivery_Date <='{model.Txt_DeliveryT}' 
                            ";

                sql += @" Group by H.F_Plant,H.F_Supplier_Code,H.F_Supplier_Plant,D.F_Part_no,D.F_ruibetsu,H.F_Delivery_Dock,D.F_kanban_No,H.F_delivery_Date,H.F_Delivery_Cycle";

                //await _kbContext.Database.ExecuteSqlRawAsync(sql);


                //--- Update Std Value---
                double? nStd = 0, nMin = 0, nMax = 0;

                //var Main = await _kbContext.RPT_KBNOC_150.Where(x => x.F_Update_By == userID)
                //                                         .OrderBy(x => x.F_Part_No)
                //                                         .ThenBy(x => x.F_Ruibetsu)
                //                                         .ThenBy(x => x.F_Store_Cd)
                //                                         .ThenBy(x => x.F_Delivery_Date)
                //                                         .ToListAsync();

                var Main = await _kbContext.RPT_KBNOC_150.FromSqlRaw($@"SELECT * FROM [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].RPT_KBNOC_150 
                                                                        WHERE F_Update_By = '{userID}'
                                                                        ORDER  BY F_Part_No,F_Ruibetsu,F_Store_Cd,F_Delivery_Date")
                                                         .ToListAsync();


                foreach (var main in Main)
                {
                    nStd = GetStd(main.F_Part_No, main.F_Ruibetsu, main.F_Store_Cd, main.F_Delivery_Date) * Get_SafetyStock(main.F_Part_No, main.F_Ruibetsu, main.F_Store_Cd, main.F_Supplier_Plant, main.F_Delivery_Date);
                    nMin = nStd * (60 / 100);
                    nMax = nStd + (GetStd(main.F_Part_No, main.F_Ruibetsu, main.F_Store_Cd, main.F_Delivery_Date) / double.Parse(main.F_Cycle_Time.Substring(2,2)));

                    sql = $@"   UPDATE RPT_KBNOC_150 
                                SET F_Std = {nStd} ,
	                                F_Min = {nMin} ,
	                                F_Max = {nMax}  
                                WHERE F_Part_No = '{main.F_Part_No}' 
                                AND F_Ruibetsu = '{main.F_Ruibetsu}'  
                                AND F_Store_Cd = '{main.F_Store_Cd}'  
                                AND F_Supplier_Code = '{main.F_Supplier_Code}'  
                                AND F_Supplier_Plant = '{main.F_Supplier_Plant}'  
                                AND F_Update_By ='{userID}' ";

                    await _kbContext.Database.ExecuteSqlRawAsync(sql);
                }

                //var result = await _kbContext.RPT_KBNOC_150
                //                     .Where(x => x.F_Update_By == userID)
                //                     .Where(x =>
                //                         (model.ConditionReport == "MINIMUM" && x.F_Delivery_Summary <= x.F_Min) ||
                //                         (model.ConditionReport == "STANDARD" && x.F_Delivery_Summary > x.F_Min && x.F_Delivery_Summary < x.F_Max) ||
                //                         (model.ConditionReport == "MAXIMUM" && x.F_Delivery_Summary >= x.F_Max)
                //                     ).ToListAsync();

                var result = await _kbContext.RPT_KBNOC_150
                                     .FromSqlRaw($@"SELECT * FROM  [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].RPT_KBNOC_150
                                                    WHERE RPT_KBNOC_150.F_Update_By= '{userID}'
                                                    AND 
                                                    (    '{model.ConditionReport}' = 'MINIMUM' AND F_Delivery_Summary <= F_Min)
                                                     OR ('{model.ConditionReport}' = 'STANDARD' AND F_Delivery_Summary > F_Min AND F_Delivery_Summary < F_Max)
                                                     OR ('{model.ConditionReport}' = 'MAXIMUM' AND F_Delivery_Summary >= F_Max)"
                                     ).ToListAsync();

                if (result.Count == 0) throw new Exception("Not Found Data!");

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                throw new CustomHttpException(500, ex.Message);
            }



        }

        public double? GetStd(string partNo, string ruibetsu, string storeCd, string deliveryDate)
        {

            DateTime dateDelivery = DateTime.ParseExact(deliveryDate.Trim(), "yyyyMMdd", null);
            int nDate = (int)dateDelivery.DayOfWeek + 1;
            int nDateNum = 7 - nDate;
            DateTime nStart = dateDelivery.AddDays(-(nDate - 1));

            double? get_Std = 0;
            int? maxValue = 0;

            for (int i = 1; i <= 7; i++)
            {
                string ndate = nStart.Day.ToString();
                string sql = $@"SELECT F_Amount_MD{nStart.Day.ToString()} AS F_Amount 
                             FROM [HMMT-APP03].[Proc_DB].dbo.T_POM_Detail P   
                             WHERE F_Part_No ='{partNo.Trim()}' 
                             AND F_ruibetsu = '{ruibetsu.Trim()}'  
                             AND F_Store_CD ='{storeCd}' 
                             AND F_Production_Date ='{dateDelivery.ToString("yyyyMM")}' 
                             AND F_revision_NO = (SELECT MAX(F_revision_NO) FROM [HMMT-APP03].[Proc_DB].dbo.T_POM_Detail 
							                             WHERE F_Part_No = P.F_part_No  AND F_Ruibetsu = P.F_Ruibetsu 
							                             AND F_Store_CD = P.F_Store_CD AND F_Production_Date ='{dateDelivery.ToString("yyyyMM")}')";

                var T = _kbContext.GetStd_KBNOC_150.FromSqlRaw(sql).FirstOrDefault();

                if (T != null)
                {
                    if (maxValue < T.F_Amount) maxValue = T.F_Amount;
                }

                nStart = nStart.AddDays(1);
            }

            return get_Std = maxValue;

        }

        public  double Get_SafetyStock(string partNo, string ruibetsu, string storeCd,string plant , string deliveryDate)
        {
            //double Get_SafetyStock = 0.0;
            double nSafetyStock = 0.0;

            //=== Found Safety Stock ===

            string sql = $@"
                            SELECT *
                            FROM T_Construction
                            WHERE F_Part_no = '{partNo}' 
                            AND F_Ruibetsu = '{ruibetsu}'
                            AND F_store_cd = '{storeCd}'
                            AND F_plant = '{plant}'
                            AND ((Substring(F_TC_End,2,3)='999' And left(convert(char(6),F_TC_Str,112),8)<= '{deliveryDate}') 
	                            OR  (left(convert(char(8),F_TC_Str,112),8)<= CONVERT(char(8),'{deliveryDate}',112)  
	                            AND left(convert(char(8),F_TC_End,112),8)>= CONVERT(char(8),'{deliveryDate}',112)))
                            ";

            var daChk =  _PPM3Context.T_Construction.FromSqlRaw(sql).FirstOrDefault();

            if (daChk != null) 
                nSafetyStock = (double)daChk.F_Safety_Stk;

            return nSafetyStock;
        }

    }
}
