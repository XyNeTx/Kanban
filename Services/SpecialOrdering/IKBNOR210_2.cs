using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Services.SpecialOrdering
{
    public interface IKBNOR210_2
    {
        Task<List<TB_Transaction_Spc>> GetCustomerPO(string? DeliDT, string? OrderNo);
        Task<bool> Merge(List<VM_Merge_KBNOR210_2> listObj);

    }

    public class KBNOR210_2 : IKBNOR210_2
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNOR210_2
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

        public async Task<List<TB_Transaction_Spc>> GetCustomerPO(string? DeliDT, string? OrderNo)
        {
            try
            {
                var data = _kbContext.TB_Transaction_Spc.Where(x => x.F_PDS_No == ""
                    && x.F_Reg_Flg == "1").AsQueryable();

                if (!string.IsNullOrWhiteSpace(DeliDT))
                {
                    data = data.Where(x => x.F_Delivery_Date.StartsWith(DeliDT));
                }
                if (!string.IsNullOrWhiteSpace(OrderNo))
                {
                    data = data.Where(x => x.F_PDS_No_New.StartsWith(OrderNo));
                }

                if (data.Count() == 0)
                {
                    throw new Exception("Data Not Found");
                }

                var result = await data.ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Merge (List<VM_Merge_KBNOR210_2> listObj)
        {
            using var transaction = _kbContext.Database.BeginTransaction();
            try
            {
                transaction.CreateSavepoint("BeforeMerge");

                var checkPDS = _kbContext.TB_Transaction_Spc.Any(x => x.F_PDS_No == listObj[0].F_PDS_No_New);
                if (checkPDS)
                {
                    throw new Exception("Duplicate Customer Order No");
                }

                foreach (var obj in listObj)
                {
                    obj.F_Delivery_Date = DateTime.ParseExact(obj.F_Delivery_Date, "dd/MM/yyyy", null).ToString("yyyyMMdd");
                    
                    var exist = await _kbContext.TB_Transaction_Spc
                        .FirstOrDefaultAsync(x => x.F_PDS_No_New == obj.F_PDS_No
                        && x.F_Delivery_Date == obj.F_Delivery_Date);

                    if (exist == null)
                    {
                        throw new Exception($"Data Not Found | {JsonConvert.SerializeObject(obj)}");
                    }

                    exist.F_Update_By = _BearerClass.UserCode;
                    exist.F_Update_Date = DateTime.Now;
                    exist.F_PDS_No = obj.F_PDS_No_New;

                    _kbContext.TB_Transaction_Spc.Update(exist);

                    _log.WriteLogMsg($"Merge Data = {JsonConvert.SerializeObject(exist)}");

                }

                await _kbContext.SaveChangesAsync();
                _log.WriteLogMsg($"Merge Success");
                transaction.Commit();

                return true;

            }
            catch (Exception ex)
            {
                transaction.RollbackToSavepoint("BeforeMerge");
                _log.WriteLogMsg($"Merge Failed | {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

    }
}