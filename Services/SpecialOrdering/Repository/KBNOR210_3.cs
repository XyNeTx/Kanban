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
    public class KBNOR210_3 : IKBNOR210_3
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNOR210_3
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

        public async Task<List<TB_Transaction_Spc>> LoadOrderNo()
        {
            try
            {

                var data = await _kbContext.TB_Transaction_Spc.Where(x => x.F_PDS_No != ""
                   && (x.F_Survey_Flg == "0" || x.F_Survey_Flg == "")).OrderBy(x => x.F_PDS_No)
                   .ToListAsync();

                if (data.Count == 0)
                {
                    throw new Exception("Data not found");
                }

                return data;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TB_Transaction_Spc>> LoadCustomPo(string NewCusPO)
        {
            try
            {

                var data = await _kbContext.TB_Transaction_Spc
                        .Where(x => x.F_PDS_No == NewCusPO)
                        .OrderBy(x => x.F_PDS_No).ToListAsync();


                if (data.Count == 0)
                {
                    throw new Exception("Data not found");
                }

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Unmerge(List<VM_Merge_KBNOR210_2> listObj)
        {
            using var transaction = _kbContext.Database.BeginTransaction();

            try
            {
                transaction.CreateSavepoint("Start Unmerge");
                //Check Survey Document
                var checkSurvey = _FillDT.ExecuteSQL(
                    $"Select F_PO_Customer From TB_Survey_Header Where F_PO_Customer = '{listObj[0].F_PDS_No_New}'");
                if (checkSurvey.Rows.Count > 0)
                {
                    throw new Exception("new customer orderno not exist in survey doc.");
                }

                //Check Survey Status
                var checkSurveyStatus = _FillDT.ExecuteSQL(
                    $"Select F_PO_Customer From TB_Survey_Header Where F_Upload_Flg = '0' and F_PO_Customer = '{listObj[0].F_PDS_No_New}'");

                if (checkSurveyStatus.Rows.Count > 0)
                {
                    throw new Exception("Cannot unmerge new customer orderno.");
                }


                foreach (var item in listObj)
                {
                    var listData = await _kbContext.TB_Transaction_Spc
                        .Where(x => x.F_PDS_No_New == item.F_PDS_No)
                        .OrderBy(x => x.F_PDS_No).ToListAsync();

                    foreach (var existData in listData)
                    {

                        if (existData == null)
                        {
                            throw new Exception("Data not found");
                        }

                        existData.F_PDS_No = "";
                        existData.F_Reg_Flg = "1";
                        existData.F_Delivery_Date_New = "";
                        existData.F_Survey_Doc = "";
                        existData.F_Survey_ID = 0;
                        existData.F_Survey_Flg = "0";
                        existData.F_Acc_Cr = "";
                        existData.F_Acc_Dr = "";
                        existData.F_Work_Code = "";
                        existData.F_Dept_Use = "";
                        existData.F_Update_Date = DateTime.Now;
                        existData.F_Update_By = _BearerClass.UserCode;

                        _kbContext.Update(existData);

                        _log.WriteLogMsg($"Unmerge Data : {JsonConvert.SerializeObject(existData)}");

                    }


                    await _kbContext.Database.ExecuteSqlRawAsync
                        ("Delete From TB_Survey_Detail Where F_Survey_Doc in ( " +
                        $"Select F_Survey_Doc From TB_Survey_Header Where F_PO_Customer = '{item.F_PDS_No}') ");

                    _log.WriteLogMsg($"Delete Survey Detail : {item.F_PDS_No}");

                    await _kbContext.Database.ExecuteSqlRawAsync
                        ($"Delete From TB_Survey_Header Where F_PO_Customer = '{item.F_PDS_No}'");

                    _log.WriteLogMsg($"Delete Survey Header : {item.F_PDS_No}");

                }

                await _kbContext.SaveChangesAsync();

                transaction.Commit();
            }
            catch (Exception ex)
            {

                transaction.RollbackToSavepoint("Start Unmerge");
                _log.WriteLogMsg($"Unmerge Error : {ex.Message}");
                throw new Exception(ex.Message);

            }
        }

    }
}
