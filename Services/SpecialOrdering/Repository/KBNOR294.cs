using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Services.SpecialOrdering.Repository
{
    public class KBNOR294 : IKBNOR294
    {

        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNOR294
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

        public string LoadContactList()
        {
            try
            {
                string sql = "Select  F_User_ID AS UserID," +
                    " F_User_Name AS UserName, F_Telephone AS Telelphone," +
                    " F_Fax AS Fax, F_Email AS Email from TB_MS_Operator ";

                var dt = _FillDT.ExecuteSQL(sql);

                if (dt.Rows.Count == 0)
                {
                    throw new Exception("No data found");
                }


                return JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Confirm(List<TB_MS_Operator> listObj)
        {
            try
            {

                await _kbContext.Database.BeginTransactionAsync();

                await _kbContext.Database.ExecuteSqlRawAsync("DELETE FROM TB_MS_Operator");

                foreach (var obj in listObj)
                {
                    obj.F_RecUser = _BearerClass.UserCode;
                    obj.F_RecDate = DateTime.Now;
                }

                await _kbContext.TB_MS_Operator.AddRangeAsync(listObj);

                await _kbContext.SaveChangesAsync();

                await _kbContext.Database.CommitTransactionAsync();

            }
            catch (Exception ex)
            {
                await _kbContext.Database.RollbackTransactionAsync();
                throw new Exception(ex.Message);
            }

        }
    }
}
