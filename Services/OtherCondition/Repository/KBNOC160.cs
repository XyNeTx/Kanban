using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.OtherCondition.ViewModel;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.OtherCondition.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KANBAN.Services.OtherCondition.Repository
{
    public class KBNOC160 : IKBNOC160
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public KBNOC160
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

        public async Task Print(VM_REPORT_KBNOC160 model)
        {
            try
            {
                var userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value;
                var plant_CTL = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value;

                await _kbContext.Database.ExecuteSqlRawAsync($"DELETE FROM [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].RPT_KBNOC_160 WHERE F_Update_By = '{userID}' ");

                string sql = $@"
                               INSERT INTO [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].RPT_KBNOC_160
                                        (F_Plant, F_OrderNo, F_OrderType, F_Supplier_Cd, F_Supplier_Plant, F_Part_No, 
                                        F_Ruibetsu, F_Store_Cd, F_Kanban_NO, F_Part_Name, F_Delivery_Date, F_Delivery_Trip, 
                                        F_Qty, F_Update_By, F_Update_Date)
                                SELECT  F_Plant
                                       ,F_OrderNo
                                       ,F_OrderType
                                       ,F_Supplier_Code
                                       ,F_SUpplier_Plant
                                       ,F_Part_No
                                       ,F_Ruibetsu
                                       ,F_Delivery_Dock
                                       ,F_Kanban_No
                                       ,F_Part_Name
                                       ,F_Delivery_Date
                                       ,F_Delivery_Trip
                                       ,F_Unit_Amount
                                       ,'{userID}' AS F_Update_By
                                       ,getdate()  AS F_Update_Date
                                FROM VW_RPT_KBNOC_160
                                WHERE F_Plant = '{plant_CTL}'
                                AND F_OrderType = 'U' 
                                ";

                if (!string.IsNullOrWhiteSpace(model.Cmb_SupF) && !string.IsNullOrWhiteSpace(model.Cmb_SupT))
                    sql += $@"AND rtrim(F_Supplier_Code collate THAI_CI_AS) +'-'+F_Supplier_Plant >='{model.Cmb_SupF}' 
                              AND rtrim(F_Supplier_Code collate THAI_CI_AS) +'-'+F_Supplier_Plant <='{model.Cmb_SupT}' 
                                            ";

                if (!string.IsNullOrWhiteSpace(model.Txt_DeliveryF) && !string.IsNullOrWhiteSpace(model.Txt_DeliveryT))
                    sql += $@"AND F_Delivery_Date >='{model.Txt_DeliveryF}'
                              AND F_Delivery_Date <='{model.Txt_DeliveryT}' 
                            ";

                await _kbContext.Database.ExecuteSqlRawAsync(sql);

                //var result = await _kbContext.RPT_KBNOC_160.Where(x => x.F_Update_By == userID).ToListAsync();
                var result = await _kbContext.RPT_KBNOC_160.FromSqlRaw(@$"SELECT * FROM [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].RPT_KBNOC_160 WHERE F_Update_By = '{userID}'").ToListAsync();
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
