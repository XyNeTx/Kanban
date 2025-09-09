using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace KANBAN.Services.SpecialOrdering.Repository
{
    public class KBNOR261 : IKBNOR261
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ISpecialLibs _specialLibs;
        private readonly IAutoMapService _automapService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public KBNOR261
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            ISpecialLibs specialLibs,
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
            _specialLibs = specialLibs;
            _automapService = autoMapService;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetPDSWaitApprove()
        {
            try
            {
                string sql = $"Select * from dbo.fnPDSWaitApprove('{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}') Order by F_OrderNO ";

                var dt = _FillDT.ExecuteSQL(sql);

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Approve(List<VM_Post_KBNOR261> listObj)
        {
            try
            {
                foreach(var obj in listObj)
                {
                    await _kbContext.TB_REC_HEADER
                        .Where(x => x.F_OrderNo == obj.F_OrderNO
                        && x.F_Status == 'W'
                        && x.F_Flg_Epro == '9'
                        && x.F_Supplier_Code.Trim() + "-" + x.F_Supplier_Plant == obj.F_Supplier_CD)
                        .ExecuteUpdateAsync(x => x.SetProperty(y => y.F_Status, 'N')
                        .SetProperty(y => y.F_Flg_Epro, '1'));

                    string sql = $@"Update TB_PDS_Approve 
                        Set F_Approve_Date = getDate() Where F_OrderNo = '{obj.F_OrderNO}' 
                        and Convert(varchar(4),F_Approve_Date,112)  like '1900%' ";

                    await _kbContext.Database.ExecuteSqlRawAsync(sql);
                }

                await _kbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }
        public async Task Preview(VM_Post_KBNOR261 obj)
        {
            try
            {
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [exec].[spKBNOR700_PDS] @pUserCode, @pPlant, @pDeliveryDate," +
                    "@F_orderType,@F_OrderNo,@F_OrderNoTo,@F_Supplier_Code,@F_Supplier_CodeTo,@F_Delivery_Date,@F_Delivery_DateTo,@ErrorMessage",
                    new SqlParameter("@pUserCode", _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value),
                    new SqlParameter("@pPlant", _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value),
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


                await _kbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }
    }
}
