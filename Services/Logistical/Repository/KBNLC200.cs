using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.LogisticCondition.ViewModel;
using KANBAN.Services.Logistical.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KANBAN.Services.Logistical.Repository
{
    public class KBNLC200 : IKBNLC200
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNLC200
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
        }

       

        public async Task<List<string>> Sup_DropDown()
        {
            var suppliers = await _PPM3Context.T_Supplier_MS
                                  .OrderBy(s => s.F_supplier_cd).ThenBy(s => s.F_Plant_cd)
                                  .Select(s => s.F_supplier_cd + "-" + s.F_Plant_cd)
                                  .Distinct()
                                  .ToListAsync();

            return suppliers;
        }

        public async Task Print(VM_REPORT_KBNLC200 model)
        {
            try
            {
                var userID = _BearerClass.UserCode;
                var plant_CTL = _BearerClass.Plant;

                await _kbContext.Database.ExecuteSqlRawAsync($"DELETE FROM [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].TB_Import_Delivery_History WHERE F_Update_By = '{userID}' ");

                string sql = $@"
                                INSERT INTO [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].TB_Import_Delivery_history 
                                        (F_Plant, F_YM, F_Supplier_Code, F_Supplier_Name, 
                                        F_Truck_Card, F_Update_By)
                                SELECT  F_Plant
                                       ,Left(F_YM,4)                         AS F_YM
                                       ,F_Supplier_Code+'-'+F_Supplier_Plant AS F_Supplier_Code
                                       ,F_Supplier_Name
                                       ,MAX(Left(F_Truck_Card,4))            AS F_Truck_Card
                                       ,'{userID}'                           AS F_Update_By
                                FROM TB_Import_Delivery I
                                WHERE F_Plant = '{plant_CTL}'
                                AND Left(F_YM, 4) = '{model.ProdYM}'
                                ";
                if (!string.IsNullOrWhiteSpace(model.Cmb_SupF) && !string.IsNullOrWhiteSpace(model.Cmb_SupT))
                    sql += $@"AND F_Supplier_Code+'-'+F_Supplier_Plant >= '{model.Cmb_SupF}'
                              AND F_Supplier_Code+'-'+F_Supplier_Plant <= '{model.Cmb_SupT}'
                             ";
                sql += $@"GROUP BY  F_Plant ,Left(F_YM,4),F_Supplier_Code+'-'+F_Supplier_Plant,F_Supplier_Name
                          ORDER BY  F_Plant ,Left(F_YM,4) ,F_Supplier_Code+'-'+F_Supplier_Plant";

                await _kbContext.Database.ExecuteSqlRawAsync(sql);
                _log.WriteLogMsg($@"Insert TB_Import_Delivery_History : {sql}" );

                //---Update cycle time per month (Jan - Dec)---
                for (int i = 1; i <= 12; i++){

                    sql = $@"
                            UPDATE [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].TB_Import_Delivery_History
                            SET M{i} = I.F_Cycle_Time
                            FROM TB_Import_Delivery I
                            INNER JOIN [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].TB_Import_Delivery_History H
                            ON I.F_Plant = H.F_Plant AND Left(I.F_YM, 4) = H.F_YM 
                            AND I.F_Supplier_Code+'-'+I.F_Supplier_Plant = H.F_Supplier_Code 
                            AND I.F_Supplier_Name = H.F_Supplier_Name
                            WHERE Right(I.F_YM, 2) = '{i.ToString("00")}'
                            AND convert(char, (rtrim(F_Arrival_HMMT)))+convert(char, F_rev ) IN ( SELECT Top 1 convert(char, (rtrim(F_Arrival_HMMT)))+convert(char, F_rev ) 
                                                                                                  FROM TB_Import_Delivery ID 
                                                                                                  Where (I.F_Plant = ID.F_Plant 
                                                                                                  AND i.F_Supplier_Code = ID.F_Supplier_Code 
                                                                                                  AND i.F_Supplier_Plant = ID.F_Supplier_Plant ) 
                                        AND I.F_Delivery_Trip = ID.F_Delivery_Trip 
                                        AND I.F_Cycle_Time = ID.F_Cycle_Time 
                                        AND I.F_YM = ID.F_YM 
                                        ORDER BY F_Rev Desc, F_Arrival_HMMT)
                            AND H.F_Update_By = '{userID}' ";

                    await _kbContext.Database.ExecuteSqlRawAsync(sql);
                    _log.WriteLogMsg($@"UPDATE TB_Import_Delivery_History Set M{i} : {sql}");
                }

                //var result = await _kbContext.TB_Import_Delivery_History.Where(x => x.F_Update_By == userID).ToListAsync();
                var result = await _kbContext.TB_Import_Delivery_History.FromSqlRaw($@"SELECT * FROM [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].TB_Import_Delivery_History WHERE F_Update_By = '{userID}'").ToListAsync();
                if (result.Count == 0) throw new Exception("Not Found Data!");


            }
            catch (Exception ex)
            {

                if (ex is CustomHttpException) throw;
                if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                throw new CustomHttpException(500, ex.Message);
            }
        }
    }
}
