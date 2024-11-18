using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Services.SpecialOrdering.Repository
{
    public class KBNOR240 : IKBNOR240
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ISpecialLibs _specialLibs;


        public KBNOR240
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            ISpecialLibs specialLibs
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _specialLibs = specialLibs;
        }

        public string procDBConnect;

        public async Task DownloadClicked(string SurveyDoc)
        {
            try
            {
                procDBConnect = _FillDT.procDBConnect();
                string _sql = $@"Update TB_Survey_Header 
                    Set F_Confirm_Date = L.F_Confirm_Date, 
                    F_Status = L.F_Status, 
                    F_Remark_Delivery = L.F_Remark_Delivery, 
                    F_Approve_Name = L.F_Approve_Name, 
                    F_Approve_Mobile = L.F_Approve_Mobile, 
                    F_Approve_Position = L.F_Approve_Position, 
                    F_Approve_Dept = L.F_Approve_Dept, 
                    F_Update_Date = getDate() , F_Update_By = '{_BearerClass.UserCode}' 
                    From (Select F_Survey_Doc,F_Revise_Rev,F_Confirm_Date,F_Status,F_Remark_Delivery,F_Approve_Name,F_Approve_Mobile,F_Approve_Position,F_Approve_Dept 
                    From {procDBConnect}.dbo.TB_Survey_Header Where F_Download_Flg = '1') L 
                    inner join TB_Survey_Header on L.F_Survey_Doc collate Thai_CI_AS = TB_Survey_Header.F_Survey_Doc 
                    and L.F_Revise_Rev  = TB_Survey_Header.F_Revise_Rev 
                    Where L.F_Survey_Doc = '{SurveyDoc}' ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg("Update Date TB_Survey_Header " + _sql);

                _sql = $@"Update TB_Survey_Detail 
                    Set F_Status = M.F_Status 
                    From (Select F_Survey_Doc,F_Delivery_Date,F_Part_No,F_Ruibetsu,F_PO_Customer,F_Status 
                    From {procDBConnect}.dbo.TB_Survey_Detail) M 
                    inner join TB_Survey_Detail D 
                    on M.F_Survey_Doc collate Thai_CI_AS = D.F_Survey_Doc 
                    and M.F_PO_Customer collate Thai_CI_AS = D.F_PO_Customer 
                    and M.F_Part_No collate Thai_CI_AS = D.F_Part_No 
                    and M.F_Ruibetsu collate Thai_CI_AS = D.F_Ruibetsu 
                    and M.F_Delivery_Date collate Thai_CI_AS = D.F_Delivery_Date 
                    Where M.F_Survey_Doc collate Thai_CI_AS = '{SurveyDoc}' ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg("Update Date TB_Survey_Detail " + _sql);

                _sql = $@"Update {procDBConnect}.dbo.TB_Survey_Header 
                    Set F_Download_Flg = '2', F_Download_By = '{_BearerClass.UserCode}'
                    , F_Download_Date = getDate() 
                    Where F_Survey_Doc = '{SurveyDoc}' and F_Download_Flg = '1' ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg("Update Download Flag (PROC_DB) To 2 " + _sql);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

    }
}
