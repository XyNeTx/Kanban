using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Services.SpecialOrdering
{
    public interface IKBNOR220
    {

        DataTable GetTransactionSPCNOSurvey(string Fac, string? PDSNo, string? PDSDate, string Mode);
        string LoadColorofTag();
        string LoadListView();
        List<string> InitialScreenCmb(string ProcessDT);

    }

    public class KBNOR220 : IKBNOR220
    {

        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNOR220
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

        public DataTable GetTransactionSPCNOSurvey(string Fac, string? PDSNo, string? PDSDate, string? Mode)
        {
            try
            {
                string sql = $@"Select TRN.F_PDS_No, '' As F_Issued_Date,F_Store_CD,F_Dept_Use, 
                        F_Acc_Dr, F_Acc_Cr,F_Work_Code,F_Remark,F_Remark2,F_Remark3 
                        ,Upper(F_Remark_KB) as F_Remark_KB,F_CustomerOrder_Type,F_CusOrderType_CD 
                        FROM TB_Transaction_Spc TRN Left outer join 
                        ( Select F_PDS_No,Count(*) As Cnt 
                        FROM TB_Transaction_Spc 
                        Where F_Survey_Flg = '0' and F_Survey_Doc = '' and F_PDS_NO <> '' 
                        and F_Delivery_Date_New = '' and F_Qty <> 0 
                        Group by F_PDS_No ) PDS on TRN.F_PDS_No = PDS.F_PDS_No 
                        Where F_Survey_Flg = '0' and F_Survey_Doc = '' and TRN.F_PDS_NO <> '' and F_Qty <> 0 ";

                if (!string.IsNullOrWhiteSpace(Fac))
                {
                    sql += $" and F_Process_Plant = '{Fac}' ";
                }
                if (!string.IsNullOrWhiteSpace(PDSNo))
                {
                    sql += $" and PDS.F_PDS_No = '{PDSNo}' ";
                }
                if (!string.IsNullOrWhiteSpace(PDSDate))
                {
                    sql += $" and F_PDS_Issued_Date = '{PDSDate}' ";
                }
                if (Mode.ToLower() == "check")
                {
                    sql += $"and (F_Dept_Use = '' or F_Acc_Dr = '' or F_Acc_Cr = '' ) ";
                }

                sql += "and Isnull(PDS.F_PDS_NO,'') = '' ";
                sql += "Group by PDS.F_PDS_No,TRN.F_PDS_No,F_Store_CD,F_Dept_Use, F_Acc_Dr, F_Acc_Cr,F_Work_Code,F_Remark,F_Remark2,F_Remark3,F_Remark_KB,F_CustomerOrder_Type,F_CusOrderType_CD ";

                var _dt = _FillDT.ExecuteSQL(sql);

                if (_dt.Rows.Count == 0)
                {
                    throw new Exception("Data not found");
                }

                return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable GetDeptMS(string? ProcessDT = "",string? DeptCode = "")
        {
            try
            {
                string sql = $@"Select F_Dept_Cd FROM  V_T_Dept_MS Where F_Flg = 'N' ";
                if(!string.IsNullOrWhiteSpace(ProcessDT))
                {
                    sql += $" and (ISNULL(F_TC_STR, '') <= '{ProcessDT}' ) AND (ISNULL(F_TC_END, '') >= '{ProcessDT}' ) ";
                }
                if (!string.IsNullOrWhiteSpace(DeptCode))
                {
                    sql += $" and F_Dept_Cd = '{DeptCode}' ";
                }

                sql += " Order by F_Dept_Cd ";

                var _dt = _FillDT.ExecuteSQL(sql);

                if (_dt.Rows.Count == 0)
                {
                    throw new Exception("Data not found");
                }

                return _dt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable GetACCOUNTMS(string ProcessDT)
        {
            string sql = $@"Select F_Acc_CD FROM  V_T_Account_MS 
                        Where  (ISNULL(F_Start_Date, '') <= '{ProcessDT}' ) 
                        AND (ISNULL(F_End_Date, '') >= '{ProcessDT}' )";

            var _dt = _FillDT.ExecuteSQL(sql);
            if (_dt.Rows.Count == 0)
            {
                throw new Exception("Data not found");
            }

            return _dt;
        }

        public string LoadColorofTag()
        {
            try
            {

                var data = _kbContext.TB_MS_TagColor.Select(x => new
                {
                    F_Color_Tag = x.F_Color_Tag == null ? "" : x.F_Color_Tag.ToUpper(),
                });

                if (data.Count() == 0)
                {
                    throw new Exception("Data not found");
                }

                return JsonConvert.SerializeObject(data);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string LoadListView()
        {
            try
            {
                return JsonConvert.SerializeObject(GetTransactionSPCNOSurvey(_BearerClass.Plant, null, null,null));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<string> InitialScreenCmb(string ProcessDT = "")
        {
            try
            {
                var dataList = new List<string>();

                var data = GetDeptMS(ProcessDT, null);
                dataList.Add(JsonConvert.SerializeObject(data));

                data = GetACCOUNTMS(ProcessDT);
                dataList.Add(JsonConvert.SerializeObject(data));

                return dataList;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
