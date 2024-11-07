using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Services.SpecialOrdering
{
    public interface IKBNOR230
    {
        Task<string> LoadSurvey();
        Task Upload(List<VM_Upload_KBNOR230> listObj);
    }
    public class KBNOR230 : IKBNOR230
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ISpecialLibs _specialLibs;
        private readonly ProcDBContext _procDBContext;


        public KBNOR230
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            ISpecialLibs specialLibs,
            ProcDBContext procDBContext
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _specialLibs = specialLibs;
            _procDBContext = procDBContext;
        }

        private async Task ClearSurveyZero()
        {
            try
            {
                var delList = await _kbContext.TB_Survey_Detail
                    .Where(x => x.F_Qty == 0 || x.F_Qty.ToString() == "").ToListAsync();

                _kbContext.TB_Survey_Detail.RemoveRange(delList);
                await _kbContext.SaveChangesAsync();

                string _sql = @"Delete From TB_Survey_Header 
                    Where F_Survey_Doc in ( Select H.F_Survey_Doc
                    from TB_Survey_Header H left outer join 
                    ( Select F_Survey_Doc From TB_Survey_Detail 
                    Group by F_Survey_Doc ) D 
                    on H.F_Survey_Doc = D.F_Survey_Doc 
                    Where Isnull(D.F_Survey_Doc,'') = '' ) ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<string> LoadSurvey()
        {
            try
            {
                await ClearSurveyZero();

                await _kbContext.Database.ExecuteSqlRawAsync("EXEC SP_UPDATEPRICESURVEY");

                var dt = _specialLibs.GetSurveyHeaderUpload("N", _BearerClass.Plant, "", "0");

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task Upload(List<VM_Upload_KBNOR230> listObj)
        {
            try
            {
                string procDBConnect = _FillDT.procDBConnect();
                
                foreach (var obj in listObj)
                {
                    string SurveyDoc = obj.F_Survey_Doc;
                    DataTable DTM = _specialLibs.GetStatusSurveyHeader(SurveyDoc);

                    if (DTM.Rows.Count > 0)
                    {
                        string sql = "";
                        string Status = DTM.Rows[0]["F_Status"].ToString().Trim();
                        int ReviseRev = int.Parse(DTM.Rows[0]["F_Revise_Rev"].ToString().Trim());
                        ReviseRev = Status == "R" ? ReviseRev + 1 : ReviseRev;

                        await _procDBContext.Database.ExecuteSqlRawAsync(
                            $@"DELETE FROM TB_Survey_Header
                            WHERE F_Survey_Doc = '{SurveyDoc}'");

                        await _procDBContext.Database.ExecuteSqlRawAsync(
                            $@"DELETE FROM TB_Survey_Detail
                            WHERE F_Survey_Doc = '{SurveyDoc}'");

                        sql = $@"Update TB_Survey_Detail Set F_Revise_Rev = {ReviseRev} 
                                    Where F_Survey_Doc = '{SurveyDoc}' ";

                        await _kbContext.Database.ExecuteSqlRawAsync(sql);
                        _log.WriteLogMsg("Update TB_Survey_Detail " + sql);

                        sql = $@"Update TB_Survey_Header Set F_Revise_Rev = {ReviseRev} 
                                    ,F_Upload_Flg = '1' , F_Status = 'N', F_Resend = {ReviseRev} 
                                    Where F_Survey_Doc = '{SurveyDoc}' and (F_Upload_Flg = '0' Or F_Upload_Flg = '1') ";

                        await _kbContext.Database.ExecuteSqlRawAsync(sql);
                        _log.WriteLogMsg("Update TB_Survey_Header " + sql);

                        sql = $@"Insert into {procDBConnect}.dbo.[TB_Survey_Header] 
                            (F_Survey_Doc, F_PO_Customer, F_Issued_Date, F_Supplier_CD, F_Supplier_Plant, F_Delivery_Date, F_Delivery_Trip, F_Cycle_Time,F_Acc_Dr, F_Acc_Cr, 
                            F_Dept_Code, F_WK_Code, F_Factory_Code, F_Confirm_Date, F_Delay_Date, F_Status,F_Remark, F_Remark2,F_Remark3,F_Remark_KB,F_Upload_Flg, F_Upload_By, F_Upload_Date,  
                            F_Download_Flg,F_Download_By,F_Revise_Rev,F_Issue_By,F_Issue_Tel,F_Issue_Fax,F_Issue_Mail) 
                            Select F_Survey_Doc, F_PO_Customer, F_Issued_Date, '0'+Ltrim(F_Supplier_CD), F_Supplier_Plant, F_Delivery_Date, F_Delivery_Trip, F_Cycle_Time,F_Acc_Dr, F_Acc_Cr, 
                            F_Dept_Code, F_WK_Code, F_Factory_Code, F_Confirm_Date, F_Delay_Date, F_Status, F_Remark,F_Remark2,F_Remark3, F_Remark_KB,'1' As F_Upload_Flg, '{_BearerClass.UserCode}' As F_Upload_By, getDate(), 
                            '0' As F_Download_Flg,'' AS F_Download_By, {ReviseRev} AS F_Resend,F_Issue_By,F_Issue_Tel,F_Issue_Fax,F_Issue_Mail 
                            From TB_Survey_Header Where F_Survey_Doc = '{SurveyDoc}' and F_Status <> 'D' ";

                        await _kbContext.Database.ExecuteSqlRawAsync(sql);
                        _log.WriteLogMsg("Insert into TB_Survey_Header " +sql);

                        sql = $@"Insert into {procDBConnect}.dbo.[TB_Survey_Detail] 
                            ( F_Survey_Doc, F_Revise_Rev,F_PO_Customer,F_No, F_Part_No, F_Part_Name,F_Ruibetsu, F_Kanban_No, F_Store_Code, F_Package, F_Qty,F_Adjust_Qty,F_Delivery_Date) 
                            Select  F_Survey_Doc, {ReviseRev} AS F_Revise_Rev, F_PO_Customer, F_No, F_Part_No, F_Part_Name,F_Ruibetsu, F_Kanban_No, F_Store_Code, F_Package, F_Qty,F_Adjust_Qty,F_Delivery_Date 
                            From TB_Survey_Detail 
                            Where F_Survey_Doc = '{SurveyDoc}' ";

                        await _kbContext.Database.ExecuteSqlRawAsync(sql);
                        _log.WriteLogMsg("Insert into TB_Survey_Detail " + sql);

                    }
                }

                var DT = _specialLibs.GetSurveyHeaderUpload("D", _BearerClass.Plant, "", "0");

                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow dr in DT.Rows)
                    {
                        string _sql = $@"Select F_Upload_Flg AS VALUE From {procDBConnect}.dbo.[TB_Survey_Header] 
                            Where F_Survey_Doc = '{dr["F_Survey_Doc"].ToString()}' ";

                        string UploadStatus = _kbContext.Database.SqlQueryRaw<string>(_sql).FirstOrDefault();

                        _sql = $@"Update {procDBConnect}.dbo.[TB_Survey_Header] Set F_Status = 'D', ";
                        if (UploadStatus == "0")
                        {
                            _sql += "F_Upload_Flg = '1'";
                        }
                        else
                        {
                            _sql += "F_Upload_Flg = '0'";
                        }
                        _sql += "Where F_Survey_Doc = '" + dr["F_Survey_Doc"].ToString() + "'";
                        _sql += " and F_Status = 'N' ";

                        await _kbContext.Database.ExecuteSqlRawAsync(_sql);

                        _sql = $@"Update TB_Survey_Header Set F_Upload_Flg = '1' 
                            Where F_Survey_Doc = '{dr["F_Survey_Doc"].ToString()}' 
                            and  F_Status = 'D' ";

                        await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                        _log.WriteLogMsg("Update TB_Survey_Header for Delete " + _sql);
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
