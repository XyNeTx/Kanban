using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;

namespace KANBAN.Services.SpecialOrdering.Repository
{
    public class KBNOR220 : IKBNOR220
    {

        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _email;
        private readonly ISpecialLibs _spcLib;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public KBNOR220
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            ISpecialLibs spcLib,
            IEmailService email,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _spcLib = spcLib;
            _email = email;
            _httpContextAccessor = httpContextAccessor;
        }

        public DataTable GetTransactionSPCNOSurvey(string Fac, string? PDSNo, string? PDSDate, string? Mode = null)
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
                if (Mode?.ToLower() == "check")
                {
                    sql += $"and (F_Dept_Use = '' or F_Acc_Dr = '' or F_Acc_Cr = '' ) ";
                }

                sql += "and Isnull(PDS.F_PDS_NO,'') = '' ";
                sql += "Group by PDS.F_PDS_No,TRN.F_PDS_No,F_Store_CD,F_Dept_Use, F_Acc_Dr, F_Acc_Cr,F_Work_Code,F_Remark,F_Remark2,F_Remark3,F_Remark_KB,F_CustomerOrder_Type,F_CusOrderType_CD ";

                var _dt = _FillDT.ExecuteSQL(sql);

                return _dt;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public DataTable GetDeptMS(string? ProcessDT = "", string? DeptCode = "")
        {
            try
            {
                string sql = $@"Select F_Dept_Cd FROM  V_T_Dept_MS Where F_Flg = 'N' ";
                if (!string.IsNullOrWhiteSpace(ProcessDT))
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
                    throw new CustomHttpException(404, "Data not found");
                }

                return _dt;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
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
                    throw new CustomHttpException(404, "Data not found");
                }

                return JsonConvert.SerializeObject(data);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public string LoadListView()
        {
            try
            {
                var dt = GetTransactionSPCNOSurvey(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value, null, null, null);
                if (dt.Rows.Count == 0)
                {
                    throw new CustomHttpException(404, "Data not found");
                }

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Generate(List<VM_Post_KBNOR220_Gen> dataList)
        {
            int i, j, MaxID, NextID, k, M = 0;
            string SupCD, SupPlant, SurveyDoc, DelayDate, DeliDT, sumSurvey = "", strCusOrder, strCusOrderDetails;

            try
            {
                foreach (var data in dataList)
                {
                    var _dt = GetTransactionSPCNOSurvey(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value, data.F_PDS_No, data.F_Issued_Date, "Check");
                    if (_dt.Rows.Count > 0)
                    {
                        throw new CustomHttpException(400, "Please input data for generate survey!");
                    }
                    if (!ChkUser())
                    {
                        throw new CustomHttpException(400, "Please set Operater in Master before generate survey.");
                    }

                }

                foreach (var data in dataList)
                {
                    if (string.IsNullOrWhiteSpace(data.F_Dept_Use))
                    {
                        throw new CustomHttpException(400, "Please input data for User Section!");
                    }
                    if (string.IsNullOrWhiteSpace(data.F_Acc_Dr))
                    {
                        throw new CustomHttpException(400, "Please input data for Debit Account Code!");
                    }
                    if (string.IsNullOrWhiteSpace(data.F_Acc_Cr))
                    {
                        throw new CustomHttpException(400, "Please input data for Credit Account Code!");
                    }
                    if (string.IsNullOrWhiteSpace(data.F_Remark))
                    {
                        throw new CustomHttpException(400, "Please input data for Remark!");
                    }

                    string now = DateTime.Now.ToString("yyyyMMdd");
                    string PDSNo = data.F_PDS_No;
                    string PDSDate = DateTime.TryParse(data.F_Issued_Date, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt) ? dt.ToString("yyyyMMdd") : "";
                    string StoreCD = data.F_Store_CD;
                    string CustomerOrderType = data.F_CustomerOrder_Type;
                    DataTable DTSUP = _spcLib.GetSupCodeSPC(PDSNo, PDSDate, StoreCD);
                    DataTable DTIssue = _spcLib.GetIssueBy();

                    if (DTSUP.Rows.Count > 0)
                    {
                        for (j = 0; j < DTSUP.Rows.Count; j++)
                        {
                            SupCD = DTSUP.Rows[j]["F_Supplier_CD"].ToString().Trim();
                            SupPlant = DTSUP.Rows[j]["F_Supplier_Plant"].ToString().Trim();
                            DeliDT = DTSUP.Rows[j]["F_Delivery_Date_new"].ToString().Trim().Substring(0, 6);
                            MaxID = _spcLib.getMaxSurveyID(PDSNo, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value);
                            NextID = MaxID + 1;
                            SurveyDoc = PDSNo.Trim() + "/" + _spcLib.FormatNumber(NextID);
                            DelayDate = _spcLib.GetDelayDate(now);
                            strCusOrder = DTSUP.Rows[j]["F_CusOrderType_CD"].ToString().Trim();

                            if (SurveyDoc != "")
                            {
                                strCusOrderDetails = strCusOrder switch
                                {
                                    "3" => "/ ORDER TYPE: 3 (INCLUDE FORECAST)",
                                    "C" => "/ ORDER TYPE: C (EXCLUDE FORECAST)",
                                    "Y" => "/ ORDER TYPE: Y (NEW MODEL)",
                                    _ => ""
                                };

                                string sql = $@"Insert TB_Survey_Header
                                    (   F_Survey_Doc, F_PO_Customer, F_Issued_Date
                                    , F_Supplier_CD, F_Supplier_Plant , F_Delivery_Date, F_Delivery_Trip, F_Cycle_Time
                                    , F_Issue_By, F_Issue_Tel, F_Issue_Fax, F_Issue_Mail
                                    , F_Acc_Dr, F_Acc_Cr, F_Dept_Code, F_WK_Code, F_Factory_Code
                                    , F_Delay_Date, F_Remark, F_Remark2, F_Remark3, F_Remark_KB
                                    , F_CustomerOrder_Type
                                    , F_Status, F_Upload_Flg, F_Create_By, F_Create_Date)
                                    Select  '{SurveyDoc}' , F_PDS_No, '{now}'
                                    , Case When Len(F_Supplier_CD) < 5 Then
                                     LTrim(F_Supplier_CD) Else LTrim(F_Supplier_CD)
                                    End F_Supplier_CD, F_Supplier_Plant
                                    , '' as F_Delivery_Date, F_Round, F_Cycle_Time
                                    , '{DTIssue.Rows[0]["F_User_Name"].ToString().Trim()}'
                                    , '{DTIssue.Rows[0]["F_Telephone"].ToString().Trim()}'
                                    , '{DTIssue.Rows[0]["F_Fax"].ToString().Trim()}'
                                    , '{DTIssue.Rows[0]["F_Email"].ToString().Trim()}'
                                    , F_Acc_Dr, F_Acc_Cr, F_Dept_Use, F_Work_Code, F_Plant
                                    , '{DelayDate}' , RTrim(F_Remark)+'{strCusOrderDetails}'
                                    , RTrim(F_Remark2), Rtrim(F_Remark3), RTrim(F_Remark_KB)
                                    , '{CustomerOrderType}' AS F_CustomerOrder_Type
                                    , 'N','0','{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}', getdate()
                                    From TB_Transaction_Spc
                                    Where F_PDS_No = '{PDSNo}' and F_Supplier_CD = '{SupCD}'
                                    and F_Supplier_Plant = '{SupPlant}' and F_Process_Plant  = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}'
                                    and F_Delivery_Date_New like '{DeliDT}%'
                                    and F_Survey_Doc   = '' and F_Survey_Flg = '0' and F_Qty > 0
                                    Group by F_PDS_No,F_Supplier_CD, F_Supplier_Plant, F_Cycle_Time,
                                    F_Acc_Dr, F_Acc_Cr, F_Dept_Use, F_Work_Code, F_Plant, F_Remark,F_Remark2,F_Remark3,F_Remark_KB,F_Round
                                    ";

                                await _kbContext.Database.ExecuteSqlRawAsync(sql);
                                _log.WriteLogMsg($"Insert TB_Survey_Header : {SurveyDoc} | Query : {sql} ");

                                var DTD = _spcLib.GetTransactionSPCDetail(PDSNo, PDSDate, SupCD, SupPlant, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value, DeliDT, StoreCD);

                                if (DTD.Rows.Count > 0)
                                {
                                    M = 0;
                                    for (k = 0; k < DTD.Rows.Count; k++)
                                    {
                                        M = M + 1;
                                        string PartNo = DTD.Rows[k]["F_Part_No"].ToString().Trim();
                                        string Ruibetsu = DTD.Rows[k]["F_Ruibetsu"].ToString().Trim();
                                        string DeliveryDate = DTD.Rows[k]["F_Delivery_Date_New"].ToString().Trim();

                                        sql = $@"Insert TB_Survey_Detail
                                            ( F_Survey_Doc, F_PO_Customer,F_No, F_Part_No, F_Part_Name,F_Ruibetsu, F_Kanban_No, F_Store_Code, F_Package, F_Qty,F_Adjust_Qty,F_Delivery_Date)
                                            Select  '{SurveyDoc}' , F_PDS_No, '{M}' ,F_Part_No,F_Part_Name,F_Ruibetsu,F_Kanban_No,F_Store_CD,F_Qty_Pack, Sum(F_Qty),Sum(F_Qty), F_Delivery_Date_NEW
                                            From TB_Transaction_Spc
                                            Where F_PDS_No = '{PDSNo}' and F_Supplier_CD = '{SupCD}'
                                            and F_Supplier_Plant  = '{SupPlant}' and F_Process_Plant  = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}'
                                            and F_Delivery_Date_New like '{DeliveryDate}%'
                                            and F_Survey_Doc   = '' and F_Survey_Flg = '0' and F_Qty > 0
                                            and F_Store_cd = '{StoreCD}'
                                            and F_Part_No   = '{PartNo}' and F_Ruibetsu = '{Ruibetsu}'
                                            Group by F_PDS_No,F_Part_No,F_Part_Name,F_Ruibetsu,F_Kanban_No,F_Store_CD,F_Qty_Pack,F_Delivery_Date_NEW ";

                                        await _kbContext.Database.ExecuteSqlRawAsync(sql);
                                        _log.WriteLogMsg($"Insert TB_Survey_Detail : {SurveyDoc} | Query : {sql} ");

                                    }
                                }

                                sql = $@"Select F_Survey_Doc,F_Part_no,F_Ruibetsu,F_Status,F_Delivery_Date
                                    ,ROW_NUMBER() OVER(PARTITION BY F_Survey_Doc Order by F_Delivery_Date,F_Part_No,F_Ruibetsu) As ROWID
                                    From TB_Survey_Detail
                                    Where F_PO_Customer = '{PDSNo.Trim()}'
                                    and F_Survey_Doc = '{SurveyDoc.Trim()}'
                                    and F_Qty > 0
                                    Group by F_Survey_Doc,F_Delivery_Date,F_Part_no,F_Ruibetsu,F_Status
                                    Order by F_Delivery_Date,F_Part_no,F_Ruibetsu ";

                                var DTS = _FillDT.ExecuteSQL(sql);

                                if (DTS.Rows.Count > 0)
                                {
                                    for (k = 0; k < DTS.Rows.Count; k++)
                                    {
                                        sql = $@"Update TB_Survey_Detail Set F_No = '{DTS.Rows[k]["ROWID"].ToString().Trim()}'
                                            , F_Unit_Price = {CheckPriceOrder(DTS.Rows[k]["F_Part_no"].ToString(), DTS.Rows[k]["F_Ruibetsu"].ToString(), DTS.Rows[k]["F_Delivery_Date"].ToString(), StoreCD, SupCD)}
                                            Where F_PO_Customer = '{PDSNo.Trim()}'
                                            and F_Survey_Doc = '{DTS.Rows[k]["F_Survey_Doc"].ToString().Trim()}'
                                            and F_Part_no = '{DTS.Rows[k]["F_Part_no"].ToString().Trim()}'
                                            and F_Ruibetsu = '{DTS.Rows[k]["F_Ruibetsu"].ToString().Trim()}'
                                            and F_Delivery_Date = '{DTS.Rows[k]["F_Delivery_Date"].ToString().Trim()}' ";

                                        await _kbContext.Database.ExecuteSqlRawAsync(sql);
                                    }
                                }

                                sql = $@"Update TB_Transaction_Spc  Set F_Survey_Doc = '{SurveyDoc}',
                                    F_Survey_ID = '{NextID}', F_Survey_Flg = '1',
                                    F_Update_By = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}',
                                    F_Update_Date = getdate()
                                    Where F_PDS_No = '{PDSNo}'
                                    and F_Supplier_CD = '{SupCD}'
                                    and F_Supplier_Plant = '{SupPlant}'
                                    and F_Process_Plant = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}'
                                    and F_Delivery_Date_New like '{DeliDT}%'
                                    and F_Survey_Doc = ''
                                    and F_Survey_Flg = '0'
                                    and F_Store_CD   = '{StoreCD}'
                                    and F_OrderType = 'S'
                                    ";

                                await _kbContext.Database.ExecuteSqlRawAsync(sql);
                                _log.WriteLogMsg($"Update TB_Transaction_Spc : {SurveyDoc} | Query : {sql} ");

                                sql = $@"Select F_SURVEY_Doc FROM fnSURVEYNOTPRICE_SPC()
                                    Where F_SURVEY_Doc = '{SurveyDoc}'
                                    GROUP BY  F_SURVEY_Doc ORDER BY F_Survey_Doc ";

                                var DT = _FillDT.ExecuteSQL(sql);

                                if (DT.Rows.Count > 0)
                                {
                                    for (int l = 0; l < DT.Rows.Count; l++)
                                    {
                                        SurveyDoc = DT.Rows[l]["F_SURVEY_Doc"].ToString();
                                        if (!sumSurvey.Contains(","))
                                        {
                                            sumSurvey = "'" + SurveyDoc;
                                        }
                                        else
                                        {
                                            sumSurvey = sumSurvey + "','" + SurveyDoc;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                if (sumSurvey != "")
                {
                    string sql = "Update TB_Survey_Detail Set F_Price_Flg = 0 " +
                        $"Where F_Unit_price = 0  and F_Survey_Doc in ({sumSurvey.Substring(2, sumSurvey.Length - 2)})";

                    await _kbContext.Database.ExecuteSqlRawAsync(sql);
                }

                await _email.SendEmailSurvey("Generate Survey Document", sumSurvey);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Save(VM_Post_KBNOR220_Gen obj)
        {
            try
            {
                if (ChkDebitCode(obj.F_Acc_Dr, DateTime.Now.ToString("yyyyMMdd")))
                {
                    throw new CustomHttpException(400, "Not match debit account code in master!");
                }

                if (ChkDeptCode(obj.F_Dept_Use, DateTime.Now.ToString("yyyyMMdd")))
                {
                    throw new CustomHttpException(400, "Not match dept code in master!");
                }

                string sWKCode = "";
                if (obj.F_Work_Code.Trim().ToUpper() == "-  -")
                {
                    sWKCode = "";
                }
                else
                {
                    sWKCode = obj.F_Work_Code.Trim().ToUpper();
                }

                string _sql = $@"UPDATE TB_Transaction_Spc Set F_Acc_Dr = '{obj.F_Acc_Dr}',
                    F_Acc_Cr = '{obj.F_Acc_Cr}', F_Dept_Use = '{obj.F_Dept_Use}',
                    F_Work_Code = '{sWKCode}', F_Remark = '{obj.F_Remark}',
                    F_Remark2 = '{obj.F_Remark2}', F_Remark3 = '{obj.F_Remark3}',
                    F_Remark_KB = '{obj.F_Remark_KB}', F_CustomerOrder_Type = '{obj.F_CustomerOrder_Type}',
                    F_CusOrderType_CD = '{obj.F_CusOrderType_CD}',
                    F_Update_By = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}',
                    F_Update_Date = getdate()
                    Where F_PDS_No = '{obj.F_PDS_No}'
                    and F_Survey_Doc = '' and F_Survey_Flg = '0'
                    and F_Store_CD = '{obj.F_Store_CD}'
                    and F_Process_Plant = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}' ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg($"Update TB_Transaction_Spc : {obj.F_PDS_No} | Query : {_sql} ");

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public bool ChkDebitCode(string DBCode, string ProcessDate)
        {
            try
            {
                string _sql = "Select F_Acc_CD FROM  V_T_Account_MS " +
                    $"Where F_Acc_CD = '{DBCode}' " +
                    $"and  (ISNULL(F_Start_Date, '') <= '{ProcessDate}' ) " +
                    $"AND (ISNULL(F_End_Date, '') >= '{ProcessDate}' ) ";

                var _dt = _FillDT.ExecuteSQL(_sql);

                if (_dt.Rows.Count > 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public bool ChkDeptCode(string DeptCode, string ProcessDate)
        {
            try
            {
                string _sql = "Select F_Dept_CD FROM V_T_Dept_MS " +
                    $"Where F_Flg = 'N' " +
                    $"AND F_Dept_CD = '{DeptCode}' " +
                    $"and  (ISNULL(F_TC_STR, '') <= '{ProcessDate}' ) " +
                    $"AND (ISNULL(F_TC_END, '') >= '{ProcessDate}' ) ";

                var _dt = _FillDT.ExecuteSQL(_sql);

                if (_dt.Rows.Count > 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public bool ChkUser()
        {
            try
            {
                return _kbContext.TB_MS_Operator.Any(x => x.F_User_ID == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        private double CheckPriceOrder(string PartNo, string Ruibetsu, string DeliveryDate, string StoreCD, string SupCD)
        {
            try
            {
                string sql = $@"Select F_SPRICE
                    From [HMMT-PPM].[PPMDB_TOTAL].dbo.[T_Unit_Price]
                    Where F_Part_no = '{PartNo}' and F_Ruibetsu = '{Ruibetsu}'
                    and (F_TC_Str <= '{DeliveryDate}' and F_TC_End >= '{DeliveryDate}')
                    and F_Supplier_cd = '{SupCD}' ";

                var _dt = _FillDT.ExecuteSQL(sql);

                if (_dt.Rows.Count == 0)
                {
                    return 0.0;
                }

                return Convert.ToDouble(_dt.Rows[0]["F_SPRICE"].ToString());
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }
    }
}
