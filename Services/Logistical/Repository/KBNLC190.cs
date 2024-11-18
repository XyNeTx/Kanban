using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.ReportOrder;
using KANBAN.Services.Logistical.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Services.Logistical.Repository
{
    public class KBNLC190 : IKBNLC190
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNLC190
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

        public async Task<TB_Import_Delivery> GetRev(string YM)
        {
            try
            {
                var data = _kbContext.TB_Import_Delivery
                    .Where(x => x.F_YM == YM
                    && x.F_Plant == _BearerClass.Plant
                    && x.F_Delivery_Trip != 0 && x.F_Flag == "2")
                    .OrderByDescending(x => x.F_Rev);

                var disticntData = data.ToList().DistinctBy(x => new
                {
                    x.F_Plant,
                    x.F_YM,
                    x.F_Rev
                }).FirstOrDefault();

                if (disticntData == null)
                {
                    throw new Exception("No Data Found");
                }

                return disticntData;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TB_Import_Delivery>> Search(string YM, int Rev)
        {
            try
            {

                var data = await _kbContext.TB_Import_Delivery
                    .Where(x => x.F_YM == YM && x.F_Rev == Rev
                    && x.F_Plant == _BearerClass.Plant
                    && x.F_Delivery_Trip != 0 && x.F_Flag == "2")
                    .OrderBy(x => x.F_short_Logistic)
                    .ToListAsync();

                if (data == null || data.Count == 0)
                {
                    throw new Exception("No Data Found");
                }

                var distinctData = data.ToList().DistinctBy(x => new
                {
                    x.F_Supplier_Code,
                    x.F_Supplier_Plant,
                    x.F_Cycle_Time,
                    x.F_short_Logistic,
                    F_Route = x.F_Truck_Card.Substring(0, 4)
                }).ToList();

                if (distinctData == null || distinctData.Count == 0)
                {
                    throw new Exception("No Data Found");
                }

                return distinctData;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Interface(string YM, string Rev, string StartDate)
        {
            try
            {

                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [exec].[spKBNLC190]  @Plant,@YM,@Rev,@StartDate,@User",
                        new SqlParameter("@Plant", _BearerClass.Plant),
                        new SqlParameter("@YM", YM),
                        new SqlParameter("@Rev", Rev),
                        new SqlParameter("@StartDate", StartDate),
                        new SqlParameter("@User", _BearerClass.UserCode)
                     );

                int CheckError = await _kbContext.Database.ExecuteSqlRawAsync
                    ($"Select * From TB_Import_Error Where F_Update_By " +
                    $"= {_BearerClass.UserCode} and F_TYpe ='KBNLC190'");

                if (CheckError > 0)
                {
                    throw new Exception("Error Found, Please Check Error Log");
                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
