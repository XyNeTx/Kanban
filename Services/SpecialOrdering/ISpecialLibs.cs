using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using System.Data;

namespace KANBAN.Services.SpecialOrdering
{
    public interface ISpecialLibs
    {
        DataTable GetSupCodeSPC(string PDSNo, string PDSDate, string StoreCD);
        DataTable GetIssueBy();
        int getMaxSurveyID(string PDSNo, string Fac = "");
        string FormatNumber(int Number, int Digits = 3);
        string GetDelayDate(string IssDate);
        DataTable GetTransactionSPCDetail(string PDSNo, string PDSDate, string SuppCD, string SuppPlant,                                          string Fac, string? DeliDT = "", string? StoreCD = "");
        DataTable GetSurveyHeader(string Fac, string? SurveyDoc, string? UploadFlg, string? Mode);
    }

    public class SpecialLibs : ISpecialLibs
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public SpecialLibs
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

        public DataTable GetSupCodeSPC(string PDSNo,string PDSDate,string StoreCD)
        {
            try
            {
                string sql = $@"Select F_Supplier_CD,F_Supplier_Plant, 
                        Left(F_Delivery_Date_New,6) As  F_Delivery_Date_New, 
                        F_CusOrderType_CD 
                        FROM TB_Transaction_Spc Where  F_PDS_No = '{PDSNo}' 
                        and F_Survey_Doc = '' and F_Survey_Flg = '0' ";

                if(!string.IsNullOrWhiteSpace(StoreCD))
                {
                    sql += $" and F_Store_CD = '{StoreCD}' ";
                }

                sql += "Group By Left(F_Delivery_Date_New,6),F_Supplier_CD,F_Supplier_Plant,F_CusOrderType_CD ";

                return _FillDT.ExecuteSQL(sql);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public DataTable GetIssueBy()
        {
            try
            {
                string sql = $@"Select F_User_Name,F_Telephone,F_Fax,F_Email 
                        FROM  TB_Ms_Operator 
                        Where F_User_ID = '{_BearerClass.UserCode}' ";

                return _FillDT.ExecuteSQL(sql);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public int getMaxSurveyID(string PDSNo,string Fac = "")
        {
            try
            {
                string sql = $@"Select Isnull(Count(F_Survey_Doc),0) As F_MaxID 
                            From TB_Survey_Header 
                            Where F_PO_Customer = '{PDSNo}' ";

                if (!string.IsNullOrWhiteSpace(Fac))
                {
                    sql += $" and F_Factory_Code  in ('{Fac}') ";
                }

                var dt = _FillDT.ExecuteSQL(sql);

                if (dt.Rows.Count > 0)
                {
                    return int.Parse(dt.Rows[0]["F_MaxID"].ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public string FormatNumber(int Number,int Digits = 3)
        {
            try
            {
                string M = "";
                if (Digits == 3)
                {
                    if (Number < 10) M = "00" + Number.ToString();
                    else if (Number < 100) M = "0" + Number.ToString();
                    else M = Number.ToString();
                }
                else if (Digits == 2)
                {
                    if (Number < 10) M = "0" + Number.ToString();
                    else M = Number.ToString();
                }
                else if (Digits == 5)
                {
                    if (Number < 10) M = "0000" + Number.ToString();
                    else if (Number < 100) M = "000" + Number.ToString();
                    else if (Number < 1000) M = "00" + Number.ToString();
                    else if (Number < 10000) M = "0" + Number.ToString();
                    else M = Number.ToString();
                }
                else if (Digits == 6)
                {
                    if (Number < 10) M = "00000" + Number.ToString();
                    else if (Number < 100) M = "0000" + Number.ToString();
                    else if (Number < 1000) M = "000" + Number.ToString();
                    else if (Number < 10000) M = "00" + Number.ToString();
                    else if (Number < 100000) M = "0" + Number.ToString();
                    else M = Number.ToString();
                }
                else if (Digits == 7)
                {
                    if (Number < 10) M = "000000" + Number.ToString();
                    else if (Number < 100) M = "00000" + Number.ToString();
                    else if (Number < 1000) M = "0000" + Number.ToString();
                    else if (Number < 10000) M = "000" + Number.ToString();
                    else if (Number < 100000) M = "00" + Number.ToString();
                    else if (Number < 1000000) M = "0" + Number.ToString();
                    else M = Number.ToString();
                }
                else M = Number.ToString();

                return M;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public string GetDelayDate(string IssDate)
        {
            try
            {
                string M = "", IssueDT = "", CurrDT = "", IssDay = "", CurrMonth = "", NextMonth = "";
                int i, j, ValDay, Val1 = new int();

                string sql = $@"Select '{IssDate}' As F_Issued_Date, Left('{IssDate}',6)  As F_CurrMth, 
                        Convert(varchar(6),DateADD(month,1,CAST('{IssDate}' As datetime)),112)  As F_NextMth, 
                        Convert(varchar(8),getdate(),112) As F_CurrDate ";

                var dt = _FillDT.ExecuteSQL(sql);

                if (dt.Rows.Count > 0)
                {
                    IssueDT = dt.Rows[0]["F_Issued_Date"].ToString();
                    CurrDT = dt.Rows[0]["F_CurrDate"].ToString();
                    IssDay = int.Parse(IssueDT.Substring(IssueDT.Length - 2, 2)).ToString();
                    CurrMonth = dt.Rows[0]["F_CurrMth"].ToString();
                    NextMonth = dt.Rows[0]["F_NextMth"].ToString();
                }
                else
                {
                    M = "";
                }

                sql = $@"SELECT F_Store_cd, F_YM, F_workCd_D1, F_workCd_D2, F_workCd_D3, F_workCd_D4, F_workCd_D5, F_workCd_D6, F_workCd_D7, F_workCd_D8, 
                        F_workCd_D9, F_workCd_D10, F_workCd_D11, F_workCd_D12, F_workCd_D13, F_workCd_D14, F_workCd_D15, F_workCd_D16, F_workCd_D17,
                        F_workCd_D18, F_workCd_D19, F_workCd_D20, F_workCd_D21, F_workCd_D22, F_workCd_D23, F_workCd_D24, F_workCd_D25, F_workCd_D26,
                        F_workCd_D27, F_workCd_D28, F_workCd_D29, F_workCd_D30, F_workCd_D31, F_Create_Date, F_Update_Date, F_Update_By 
                        FROM TB_Calendar
                        WHERE  (F_YM = '{CurrMonth}') AND (F_Store_cd = '1A') ";

                var DTM = _FillDT.ExecuteSQL(sql);

                if(DTM.Rows.Count > 0)
                {
                    for(i = int.Parse(IssDay); i <= 31; i++)
                    {
                        ValDay = int.Parse(DTM.Rows[0]["F_workCd_D" + i].ToString());
                        Val1 = Val1 + ValDay;
                        if (Val1 == 4)
                        {
                            M = CurrMonth + FormatNumber(i, 2);
                        }
                    }

                    if(Val1 < 4)
                    {
                        sql = $@"SELECT     F_Store_cd, F_YM, F_workCd_D1, F_workCd_D2, F_workCd_D3, F_workCd_D4, F_workCd_D5, F_workCd_D6, F_workCd_D7, F_workCd_D8, 
                                F_workCd_D9, F_workCd_D10, F_workCd_D11, F_workCd_D12, F_workCd_D13, F_workCd_D14, F_workCd_D15, F_workCd_D16, F_workCd_D17, 
                                F_workCd_D18, F_workCd_D19, F_workCd_D20, F_workCd_D21, F_workCd_D22, F_workCd_D23, F_workCd_D24, F_workCd_D25, F_workCd_D26, 
                                F_workCd_D27, F_workCd_D28, F_workCd_D29, F_workCd_D30, F_workCd_D31, F_Create_Date, F_Update_Date, F_Update_By 
                                FROM      TB_Calendar 
                                WHERE     (F_YM = '{NextMonth}') AND (F_Store_cd = '1A') ";

                        var DTN = _FillDT.ExecuteSQL(sql);

                        if(DTN.Rows.Count > 0)
                        {
                            for(j=1; j <= 31; j++)
                            {
                                ValDay = int.Parse(DTN.Rows[0]["F_workCd_D" + j].ToString());
                                Val1 = Val1 + ValDay;
                                if (Val1 == 4)
                                {
                                    M = NextMonth + FormatNumber(j, 2);
                                }
                            }
                        }
                    }
                }
                return M;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public DataTable GetTransactionSPCDetail(string PDSNo, string PDSDate ,string SuppCD, string SuppPlant,
                                                string Fac, string? DeliDT = "",string? StoreCD = "")
        {
            try
            {
                string sql = $@" Select RTrim(F_Part_No) As F_Part_No,Ltrim(F_Ruibetsu) As F_Ruibetsu,
                        F_Kanban_No, F_Store_CD,F_Qty_Pack,Sum(F_Qty) As F_Qty,F_Delivery_Date_New 
                        FROM TB_Transaction_Spc 
                        Where F_PDS_No = '{PDSNo}' and F_Supplier_CD = '{SuppCD}'
                        and F_Supplier_Plant = '{SuppPlant}'";
                if (!string.IsNullOrWhiteSpace(Fac))
                {
                    sql += $" and F_Process_Plant = '{Fac}' ";
                }
                if (!string.IsNullOrWhiteSpace(DeliDT))
                {
                    sql += $" and F_Delivery_Date_New Like '{DeliDT}%' ";
                }
                if (!string.IsNullOrWhiteSpace(StoreCD))
                {
                    sql += $" and F_Store_CD like '{StoreCD}%' ";
                }

                sql += "Group By F_Part_No,F_Ruibetsu,F_Kanban_No,F_Store_CD,F_Qty_Pack,F_Delivery_Date_New ";
                sql += "Order by F_Delivery_Date_New,F_Part_No,F_Ruibetsu";

                return _FillDT.ExecuteSQL(sql);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public DataTable GetSurveyHeader(string Fac, string? SurveyDoc, string? UploadFlg, string? Mode)
        {
            try
            {
                string _sql = $@"Select RTrim(F_Survey_Doc) As F_Survey_Doc,F_PO_Customer,Rtrim(F_Supplier_Cd)+'-'+F_Supplier_Plant As F_Supplier_Code
                    , (Select Top 1 F_short_name From V_Supplier_MS Where Rtrim(F_Supplier_Cd)+F_Plant_CD collate Thai_CI_AS = Rtrim(TB_Survey_Header.F_Supplier_cd)+TB_Survey_header.F_Supplier_Plant 
                    and (F_TC_Str collate Thai_CI_AS <= TB_Survey_Header.F_Issued_Date and F_TC_End collate Thai_CI_AS >= TB_Survey_Header.F_Issued_Date)) AS F_Short_name 
                    ,Convert(varchar(11),CAST(F_Issued_Date As datetime),103) As F_Issued_Date,F_Dept_Code,F_Acc_Dr,F_Acc_Cr,F_Wk_code,F_Remark,F_Remark2,F_Remark3,F_Remark_KB,F_CustomerOrder_Type 
                    FROM  TB_Survey_Header 
                    Where  F_Status <> 'D' 
                    and F_Survey_Doc in ( Select F_Survey_Doc From TB_Survey_Detail Where F_PDS_No= '' ) ";

                if (!string.IsNullOrWhiteSpace(Fac))
                {
                    _sql += $" and F_Factory_Code in ('{Fac}') ";
                }
                if (!string.IsNullOrWhiteSpace(SurveyDoc))
                {
                    _sql += $" and F_Survey_Doc = '{SurveyDoc}' ";
                }

                if(Mode?.ToLower() == "delete")
                {
                    _sql = $@"Select RTrim(TB_Survey_Header.F_Survey_Doc) As F_Survey_Doc,TB_Survey_Header.F_Revise_Rev,TB_Survey_Header.F_PO_Customer, 
                        Rtrim(F_Supplier_Cd)+'-'+F_Supplier_Plant As F_Supplier_Code  ,(Select Top 1 F_short_name From V_Supplier_MS 
                        Where Rtrim(F_Supplier_Cd)+F_Plant_CD collate Thai_CI_AS = Rtrim(TB_Survey_Header.F_Supplier_cd)+TB_Survey_header.F_Supplier_Plant 
                        and (F_TC_Str collate Thai_CI_AS <= TB_Survey_Header.F_Issued_Date and F_TC_End collate Thai_CI_AS >= TB_Survey_Header.F_Issued_Date)) AS F_Short_name 
                        ,Convert(varchar(11),CAST(F_Issued_Date As datetime),103) As F_Issued_Date,F_Dept_Code,F_Acc_Dr,F_Acc_Cr,F_Wk_code,F_Remark,F_Remark2,F_Remark3,F_Remark_KB 
                        FROM  TB_Survey_Header  inner join TB_Survey_Detail 
                        on TB_Survey_Header.F_Survey_Doc= TB_Survey_Detail.F_Survey_Doc 
                        and TB_Survey_Header.F_PO_Customer= TB_Survey_Detail.F_PO_Customer 
                        and TB_Survey_Header.F_Revise_Rev= TB_Survey_Detail.F_Revise_Rev 
                        Where  TB_Survey_Header.F_Status <> 'D'  and TB_Survey_Detail.F_PDS_Flg = 0 
                        and TB_Survey_Detail.F_PDS_No  = '' 
                        and F_Factory_Code  in ('1','B') 
                        Group by RTrim(TB_Survey_Header.F_Survey_Doc),TB_Survey_Header.F_Revise_Rev,Rtrim(F_Supplier_Cd),F_Supplier_Plant,TB_Survey_Header.F_PO_Customer, 
                        F_Issued_Date, F_Dept_Code, F_Acc_Dr, F_Acc_Cr, F_Wk_code, F_Remark, F_Remark2, F_Remark3, F_Remark_KB ";
                }

                return _FillDT.ExecuteSQL(_sql);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        } 
    }
}
