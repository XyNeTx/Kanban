using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public class KBNOR295 : IKBNOR295
    {

        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNOR295
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
                string sql = "Select F_User_ID, F_Name, F_Surname," +
                    " F_Email, F_Path_File  FROM  TB_MS_SpcApprover ";

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

        public async Task Confirm(List<VM_Post_KBNOR295> listObj)
        {
            using var transaction = await _kbContext.Database.BeginTransactionAsync();
            try
            {
                transaction.CreateSavepoint("Start Confirm");
                await _kbContext.Database.ExecuteSqlRawAsync("Delete from TB_MS_SpcApprover");

                foreach (var obj in listObj)
                {
                    obj.F_RecUser = _BearerClass.UserCode;
                    obj.F_RecDate = DateTime.Now;

                    string sql = $"Select * From TB_MS_SpcApprover Where F_User_ID = '{obj.F_User_ID}'";
                    int count = _kbContext.Database.ExecuteSqlRaw(sql);
                    if (count <= 0)
                    {
                        if (obj.F_Path_File == "")
                        {
                            sql = $"Insert into TB_MS_SpcApprover( F_User_ID, F_Name, F_Surname, F_Email, F_RecUser, F_RecDate) " +
                                $"Select '{obj.F_User_ID}', '{obj.F_Name}', '{obj.F_Surname}', '{obj.F_Email}', '{obj.F_RecUser}', '{obj.F_RecDate}'";

                            await _kbContext.Database.ExecuteSqlRawAsync(sql);
                        }
                        else
                        {

                            sql = @$"Insert into TB_MS_SpcApprover( F_User_ID, F_Name, F_Surname, F_Email, F_Path_File, F_RecUser, F_RecDate, F_Sign) 
                                    Select '{obj.F_User_ID}', '{obj.F_Name}', '{obj.F_Surname}', '{obj.F_Email}', '{obj.F_Path_File}', '{obj.F_RecUser}', '{obj.F_RecDate}', 
                                    * From OPENROWSET(BULK N'{obj.F_Path_File}', SINGLE_BLOB) as PicTure ";

                            await _kbContext.Database.ExecuteSqlRawAsync(sql);
                        }
                    }
                }

                await _kbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }

        }

        public async Task<string> UploadIMG(IFormFile formFile)
        {
            try
            {

                var path = Path.Combine("\\\\hmmta-tpcap", "kanban", "wwwroot", "Storage", "Approver", formFile.FileName);

                if (!Directory.Exists(Path.Combine("\\\\hmmta-tpcap", "kanban", "wwwroot", "Storage", "Approver")))
                {
                    Directory.CreateDirectory(Path.Combine("\\\\hmmta-tpcap", "kanban", "wwwroot", "Storage", "Approver"));
                }
                else if (Directory.Exists(Path.Combine("\\\\hmmta-tpcap", "kanban", "wwwroot", "Storage", "Approver")))
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                    stream.Close();
                }

                return path;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
