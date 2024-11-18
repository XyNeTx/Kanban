using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public class KBNOR293 : IKBNOR293
    {

        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNOR293
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

        public string LoadColorTag()
        {
            try
            {
                string sql = "Select   F_Color_Tag AS COLOR , F_Type As TypePart  from TB_MS_TagColor ";
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

        public async Task Confirm(List<VM_Post_Tag_Color> listObj)
        {
            try
            {

                await _kbContext.Database.BeginTransactionAsync();

                await _kbContext.Database.ExecuteSqlRawAsync("DELETE FROM TB_MS_TagColor");

                foreach (var item in listObj)
                {
                    var IsExisted = _kbContext.Database.ExecuteSqlRaw("SELECT COUNT(*) FROM TB_MS_TagColor WHERE F_Color_Tag = {0} AND F_Type = {1}", item.F_Color_Tag, item.F_Type);

                    if (IsExisted <= 0)
                    {
                        await _kbContext.Database.ExecuteSqlRawAsync("INSERT INTO TB_MS_TagColor (F_Color_Tag, F_Type,F_RecUser,F_RecDate) VALUES ({0},{1},{2},getDate())", item.F_Color_Tag, item.F_Type, _BearerClass.UserCode);
                    }
                    else
                    {
                        await _kbContext.Database.ExecuteSqlRawAsync("UPDATE TB_MS_TagColor SET F_Type = {1} ,F_RecUser = {2}, F_RecDate = getdate() WHERE F_color_Tag = {0}", item.F_Color_Tag, item.F_Type, _BearerClass.UserCode);
                    }

                }

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
