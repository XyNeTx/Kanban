using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;

namespace KANBAN.Services.SpecialOrdering.Repository
{
    public class KBNOR270 : IKBNOR270
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ISpecialLibs _specialLibs;
        private readonly IAutoMapService _automapService;


        public KBNOR270
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            ISpecialLibs specialLibs,
            IAutoMapService autoMapService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _specialLibs = specialLibs;
            _automapService = autoMapService;
        }

        public async Task Preview(List<VM_Post_KBNOR261> listObj)
        {
            try
            {
                await _kbContext.Database.ExecuteSqlRawAsync($"DELETE FROM [dbo].[KBNOR_450] WHERE F_Update_By='{_BearerClass.UserCode}'");
                foreach (var obj in listObj)
                {
                    await _kbContext.Database.ExecuteSqlRawAsync("EXEC [exec].[spKBNOR700_PDS] @pUserCode, @pPlant, @pDeliveryDate," +
                        "@F_orderType,@F_OrderNo,@F_OrderNoTo,@F_Supplier_Code,@F_Supplier_CodeTo,@F_Delivery_Date,@F_Delivery_DateTo,@ErrorMessage",
                        new SqlParameter("@pUserCode", _BearerClass.UserCode),
                        new SqlParameter("@pPlant", _BearerClass.Plant),
                        new SqlParameter("@pDeliveryDate", ""),
                        new SqlParameter("@F_orderType", "S"),
                        new SqlParameter("@F_OrderNo", obj.F_OrderNO),
                        new SqlParameter("@F_OrderNoTo", obj.F_OrderNO),
                        new SqlParameter("@F_Supplier_Code", ""),
                        new SqlParameter("@F_Supplier_CodeTo", ""),
                        new SqlParameter("@F_Delivery_Date", ""),
                        new SqlParameter("@F_Delivery_DateTo", ""),
                        new SqlParameter("@ErrorMessage", "")
                        );

                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task PreviewKB()
        {
            try
            {
                await _kbContext.Database.ExecuteSqlRawAsync($"DELETE FROM [dbo].[KBNOR_140_KB] WHERE F_Update_By='{_BearerClass.UserCode}'");
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [exec].[spKBNOR700_KANBAN] " +
                    "@pUserCode,@pPlant,@pDeliveryDate,@F_orderType",
                    new SqlParameter("@pUserCode", _BearerClass.UserCode),
                    new SqlParameter("@pPlant", _BearerClass.Plant),
                    new SqlParameter("@pDeliveryDate", DateTime.Now.AddMonths(-3).ToString("yyyyMMdd")),
                    new SqlParameter("@F_orderType", "S")
                    );
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

    }
}
