using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace KANBAN.Services.SpecialOrdering
{
    public interface IKBNOR250
    {
        DataTable GetSurveyNoPDS(string fac, string? SurveyDoc = "", string? DeliveryDT = "", string? DeliYM = "");
        DataTable CheckPriceAndPackageFlag(string SurveyDoc);
        Task Refresh();
        Task Unlock(List<VM_KBNOR250> listObj);
        Task Generate(List<VM_KBNOR250> listObj, [FromQuery] string DeliYM);
    }
    public class KBNOR250 : IKBNOR250
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ISpecialLibs _specialLibs;


        public KBNOR250
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

        public DataTable GetSurveyNoPDS(string fac, string? SurveyDoc = "", string? DeliveryDT = "", string? DeliYM = "")
        {
            try
            {
                string _sql = $@"SELECT   Rtrim(F_Supplier_Cd)+'-'+F_Supplier_Plant As F_Supplier_code
                    , F_Supplier_Cd,F_Supplier_Plant, H.F_Survey_Doc 
                    , CONVERT(varchar(30),CAST(D.F_Delivery_Date As datetime),6) AS F_Delivery_Date 
                    , D.F_Delivery_Date  As F_Delivery_DT ,H.F_Factory_Code 
                    , '' As F_Cycle_Time, F_Delivery_Trip, Substring(D.F_Delivery_Date,3,6) As F_Delivery_YM 
                    , CASE WHEN D.F_PDS_No = '' THEN
                    D.F_PO_Customer ELSE D.F_PDS_No END AS F_PO_Customer 
                    FROM TB_Survey_Header H INNER JOIN TB_Survey_Detail D 
                    ON H.F_Survey_Doc = D.F_Survey_Doc 
                    AND H.F_Revise_Rev = D.F_Revise_Rev 
                    WHERE D.F_PDS_Flg = '' AND D.F_PDS_Flg <> '2' 
                    AND Isnull(H.F_Confirm_Date,'') <> '' 
                    AND (H.F_Status = 'C' or H.F_Status = 'E')
                    AND H.F_Status_D  <> 'D'";

                if (!string.IsNullOrWhiteSpace(fac))
                {
                    _sql += $" and F_Factory_Code in ('{fac}') ";
                }
                if (!string.IsNullOrWhiteSpace(SurveyDoc))
                {
                    _sql += $" and H.F_Survey_Doc = '{SurveyDoc}' ";
                }
                if (!string.IsNullOrWhiteSpace(DeliveryDT))
                {
                    _sql += $" and D.F_Delivery_Date = '{DeliveryDT}' ";
                }
                if (!string.IsNullOrWhiteSpace(DeliYM))
                {
                    _sql += $" and D.F_Delivery_Date like '{DeliYM}%' ";
                }
                _sql += "GROUP BY    F_Supplier_Cd, F_Supplier_Plant, D.F_PDS_No, D.F_PO_Customer ";
                _sql += ", H.F_survey_Doc, H.F_Factory_Code, D.F_Delivery_Date, F_Delivery_Trip ";
                _sql += "ORDER BY F_Supplier_CD, F_Supplier_Plant, D.F_Delivery_Date ";

                var dt = _FillDT.ExecuteSQL(_sql);

                return dt;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public DataTable CheckPriceAndPackageFlag(string SurveyDoc)
        {
            try
            {
                string _sql = $@"SELECT DISTINCT F_Status 
                    FROM (   SELECT	CASE WHEN F_Price_Flg = '0' AND ISNULL(F_Package,'9999') = '9999' THEN 
                    'Price Zero & Package not found' WHEN F_Price_Flg = '0' THEN 
                    'Price Zero' WHEN ISNULL(F_Package,'9999') = '9999' THEN 
                    'Package not found' ELSE '' END F_Status 
                    --SELECT F_Price_Flg, F_Unit_Price, F_Package 
                    FROM TB_Survey_Detail WHERE F_Survey_Doc = '{SurveyDoc}' 
                    AND ( F_Price_Flg = '0' OR ISNULL(F_Package,'9999') = '9999' ) 
                    ) TB_Survey_Detail WHERE F_Status <> '' 
                    ORDER BY F_Status DESC ";

                var dt = _FillDT.ExecuteSQL(_sql);

                return dt;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        private string getStoreCode(string SurveyDoc)
        {
            try
            {
                 var listData = _kbContext.TB_Survey_Detail
                    .Where(x=>x.F_Survey_Doc == SurveyDoc)
                    .Select(x => x.F_Store_Code)
                    .ToList();

                listData = listData.Distinct().ToList();

                if(listData.Count > 0)
                {
                    return listData[0];
                }
                else
                {
                    return "1F";
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        private async Task CheckDataMaster(string SurveyDoc)
        {
            try
            {
                await _kbContext.Database.ExecuteSqlRawAsync("Delete From TB_PDS_Error Where F_User_Name = '" + _BearerClass.UserCode + "'");

                string _sql = $@"Insert into TB_PDS_Error Select H.F_Survey_Doc,H.F_Supplier_CD,H.F_Supplier_Plant, 
                    D.F_Delivery_Date,H.F_Delivery_Trip, Case When Len(H.F_Cycle_Time) = 6 Then Replace(left(H.F_Cycle_Time,2),'0','') +' : '+Replace(Substring(H.F_Cycle_Time ,3,2),'0','') +' : '+Replace(Substring(H.F_Cycle_Time ,5,2),'0','') Else H.F_Cycle_Time  End As CycleTime , 
                    'Not Found Delivery Time in Delivery Time Master' As F_Remark, 
                    '{_BearerClass.UserCode}' As F_User_Name, getdate() As F_Create_Date 
                    From TB_Survey_Header H inner join TB_Survey_Detail D 
                    on H.F_Survey_Doc = D.F_Survey_Doc 
                    and H.F_Revise_Rev = D.F_Revise_Rev 
                    left outer join TB_MS_DeliveryTime T 
                    on Substring(H.F_Supplier_CD,1,4) = T.F_Supplier_Code 
                    and H.F_Supplier_Plant = T.F_Supplier_Plant 
                    and (T.F_Start_Date <= D.F_Delivery_Date and T.F_End_Date >= D.F_Delivery_Date)  
                    and H.F_Delivery_Trip = T.F_Delivery_Trip 
                    Where D.F_PDS_Flg = '0' and D.F_PDS_No = '' and D.F_survey_Doc = '{SurveyDoc}' 
                    and  H.F_Factory_Code in ('{_BearerClass.Plant}') and Isnull(T.F_Supplier_Code,'') = ''";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        private string GenBarcode(string value)
        {
            try
            {
                string barcode_char = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%";
                int SumChar = 0;
                int CheckDigit = 0;

                for (int i = 0; i < value.Length; i++)
                {
                    SumChar += barcode_char.IndexOf(value[i]);
                }

                CheckDigit = SumChar % 43;

                return value + barcode_char[CheckDigit];
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        private string Get_Date(string sDelivery)
        {
            try
            {
                string Get_Date = "";
                string _sql = $"Select * from TB_Calendar Where F_YM = '{sDelivery.Substring(2, 6)}' and F_Store_CD='1A'";
                var dt = _FillDT.ExecuteSQL(_sql);

                if (dt.Rows.Count > 0)
                {
                    if(int.Parse(sDelivery.Substring(6, 2)) > 1)
                    {
                        for (int i = int.Parse(sDelivery.Substring(6, 2)); i > 1; i--)
                        {
                            if (int.Parse(dt.Rows[0].ItemArray[i*2].ToString()) + int.Parse(dt.Rows[0].ItemArray[i * 2 + 1].ToString()) == 2)
                            {
                                Get_Date = sDelivery.Substring(0, 6) + i.ToString("00");
                            }
                        }
                    }
                }
                
                if(string.IsNullOrWhiteSpace(Get_Date))
                {
                    DateTime CsDelivery = DateTime.ParseExact(sDelivery, "yyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    string sYMLast = CsDelivery.AddMonths(-1).ToString("yyyyMMdd");
                    _sql = $"Select * from TB_Calendar Where F_YM = '{sYMLast.Substring(0,6)}' and F_Store_CD='1A'";

                    var dtLast = _FillDT.ExecuteSQL(_sql);

                    if (dtLast.Rows.Count > 0)
                    {
                        for (int i = 31; i > 1; i--)
                        {
                            if (int.Parse(dtLast.Rows[0].ItemArray[i * 2].ToString()) + int.Parse(dtLast.Rows[0].ItemArray[i * 2 + 1].ToString()) == 2)
                            {
                                Get_Date = sYMLast.Substring(0, 6) + i.ToString("00");
                            }
                        }
                    }
                }

                return Get_Date;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        private async Task UpdateCollectDT()
        {
            try
            {
                string _sql = $@"UPDATE TB_PDS_HEADER set  F_Collect_Time = T.F_Arrival_Sup 
                    from TB_PDS_HEADER P INNER JOIN TB_Import_Delivery T ON 
                    P.F_Supplier_Code collate Thai_CI_AS = T.F_Supplier_Code and P.F_Supplier_plant collate Thai_CI_AS = T.F_Supplier_Plant 
                    and substring(P.F_Delivery_Date,1,6) = T.F_YM and P.F_Delivery_Trip = T.F_Delivery_Trip 
                    Where F_OrderType='S' and T.F_Rev in (Select Max(F_Rev) From TB_Import_Delivery 
                    Where F_Ym = substring(P.F_Delivery_Date,1,6) and F_Supplier_Code = P.F_SUpplier_Code collate Thai_CI_AS 
                    and F_Supplier_Plant = P.F_Supplier_Plant collate Thai_CI_AS)";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);

                _sql = $@"UPDATE TB_PDS_HEADER set F_Collect_Date = F_Delivery_Date 
                    Where F_OrderType='S' and F_Collect_Time='00:00' ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);

                _sql = $@"UPDATE TB_PDS_HEADER set F_Collect_Date = F_Delivery_Date 
                    Where F_OrderType='S' and F_Collect_Time>='07:30' 
                    and F_Collect_Date = ''";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);

                _sql = $@"UPDATE TB_PDS_HEADER set F_Collect_Date = F_Delivery_Date 
                    Where F_OrderType='S' and F_Collect_Time<'07:30' 
                    and F_Collect_Date = '' ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);

                _sql = $@"Select distinct F_Delivery_Date From TB_PDS_HEADER 
                    Where F_OrderType='S' and F_Collect_Date = '' and F_Collect_Time <> ''";

                var dt = _FillDT.ExecuteSQL(_sql);

                foreach (DataRow dr in dt.Rows)
                {

                    _sql = $@"UPDATE TB_PDS_HEADER set F_Collect_Date = F_Delivery_Date 
                        Where F_OrderType='S' and F_Collect_Date = '' 
                        and F_Delivery_Date = '{dr["F_Delivery_Date"].ToString()}'";

                    await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Refresh()
        {
            try
            {
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC SP_UPDATEPRICESURVEY");
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC SP_UPDATE_PACKAGE_SURVEY ");
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Unlock(List<VM_KBNOR250> listObj)
        {
            try
            {
                string sumSurvey = "";
                string surveyDoc = "";
                foreach (var obj in listObj)
                {
                    surveyDoc = obj.F_Survey_Doc;

                    if (sumSurvey.IndexOf(",") == -1)  // No comma found
                    {
                        sumSurvey = ",'" + surveyDoc + "'";
                    }
                    else
                    {
                        sumSurvey += ",'" + surveyDoc + "'";
                    }

                    await _emailService.SendEmailSurvey("Generate PDS",sumSurvey);

                    await _kbContext.Database.ExecuteSqlRawAsync($"Update TB_Survey_Detail Set F_Price_Flg = 1 " +
                        $"where F_Survey_Doc = '{sumSurvey.Substring(1,sumSurvey.Length)}'");

                    _log.WriteLogMsg("Unlock Price Re-Flag Unlock Price");
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Generate(List<VM_KBNOR250> listObj, [FromQuery] string DeliYM)
        {
            try
            {
                string userName = await _FillDT.GetUserName(_BearerClass.UserCode);
                userName = userName.Trim().Substring(0, 10);
                string PDSNo = "";
                string FormatPDS = "";
                string FacCD = _BearerClass.Plant == "1" ? "9Z" : _BearerClass.Plant == "2" ? "8Z" : "7Z";
                string _sql = $@"Update TB_Survey_Detail Set F_PDS_No = '', F_PDS_Flg = '0' 
                    From ( Select F_OrderNo,F_PO_Customer,F_Plant From TB_PDS_Header) L 
                    inner join TB_Survey_Detail H on L.F_OrderNo = H.F_PDS_NO 
                    Where F_Plant  in ('{_BearerClass.Plant}')   and H.F_PDS_Flg = '1' ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg("Update to TB_Survey_Header | _sql" + _sql);

                _sql = $@"Delete From TB_PDS_Detail Where F_Orderno like '{FacCD}%' 
                    and  F_OrderNo in (Select F_OrderNo From TB_PDS_Header Where F_Status = 'N' and F_Orderno like '{FacCD}%' ) ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg("Delete from TB_PDS_Detail | _sql" + _sql);

                _sql = $@"Delete From TB_PDS_Header Where F_Orderno like '{FacCD}%'
                    and  F_Status = 'N' ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg("Delete from TB_PDS_Header | _sql" + _sql);

                _sql = $@"Delete From TB_PDS_TMP Where F_Process_By = '{_BearerClass.UserCode}'";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);

                foreach(var obj in listObj)
                {
                    await CheckDataMaster(obj.F_Survey_Doc);
                }

                int checkError = _kbContext.Database.ExecuteSqlRaw("Select * From TB_PDS_Error Where F_User_Name = '" + _BearerClass.UserCode + "'");

                if (checkError > 0)
                {
                    throw new CustomHttpException(400, "Generate has been error. Pls check report");
                }
                
                int index = 0;
                foreach (var obj in listObj)
                {
                    string DeliveryDT = obj.F_Delivery_DT;
                    DataTable DT = GetSurveyNoPDS(_BearerClass.Plant, obj.F_Survey_Doc, DeliveryDT);
                    string StoreCD = getStoreCode(obj.F_Survey_Doc);
                    string PlantType = obj.F_Factory_Code;

                    if (PlantType == "B" || StoreCD == "1M" || StoreCD == "1N")
                    {
                        int maxRegRunning = _specialLibs.GetMaxRegRunning(PlantType
                                , DT.Rows[0]["F_Supplier_CD"].ToString().Trim()
                                , DT.Rows[0]["F_Supplier_Plant"].ToString().Trim()
                                , _BearerClass.Plant
                                , DeliYM
                                , DT.Rows[0]["F_Delivery_Trip"].ToString().Trim()
                                , StoreCD, DeliYM) + 1;

                        int maxPDSRunning = _specialLibs.GetMaxPDSRunning(PlantType
                            , DT.Rows[0]["F_Supplier_CD"].ToString().Trim()
                            , DT.Rows[0]["F_Supplier_Plant"].ToString().Trim()
                            , _BearerClass.Plant
                            , DeliYM
                            , DT.Rows[0]["F_Delivery_Trip"].ToString().Trim()
                            , StoreCD, DeliYM) + 1;

                        if (_specialLibs.ChkPDSData(PlantType, _BearerClass.Plant,
                            DT.Rows[0]["F_Delivery_YM"].ToString().Trim(),
                            DT.Rows[0]["F_Delivery_Trip"].ToString().Trim(), StoreCD) == false)
                        {
                            PDSNo = _specialLibs.GetPDSNo(PlantType, _BearerClass.Plant,
                                DT.Rows[0]["F_Delivery_YM"].ToString().Trim().Substring(0, 4)
                                , maxRegRunning.ToString(),
                                DT.Rows[0]["F_Delivery_Trip"].ToString().Trim()
                               , StoreCD, DeliYM);
                        }
                        else
                        {
                            PDSNo = _specialLibs.GetPDSNo(PlantType, _BearerClass.Plant,
                                DT.Rows[0]["F_Delivery_YM"].ToString().Trim().Substring(0, 4)
                                , maxPDSRunning.ToString()
                                , DT.Rows[0]["F_Delivery_Trip"].ToString().Trim()
                                , StoreCD, DeliYM);
                        }
                    }
                    else
                    {
                        if(DeliYM.CompareTo("201501") >= 0)
                        {
                            FormatPDS = "1";
                            int maxRegRunning = _specialLibs.GetMaxRegRunning(PlantType
                                , DT.Rows[0]["F_Supplier_CD"].ToString().Trim()
                                , DT.Rows[0]["F_Supplier_Plant"].ToString().Trim()
                                , _BearerClass.Plant
                                , DeliYM
                                , DT.Rows[0]["F_Delivery_Trip"].ToString().Trim()
                                ,"", FormatPDS) + 1;

                            int maxPDSRunning = _specialLibs.GetMaxPDSRunning(PlantType
                                , DT.Rows[0]["F_Supplier_CD"].ToString().Trim()
                                , DT.Rows[0]["F_Supplier_Plant"].ToString().Trim()
                                , _BearerClass.Plant
                                , DeliYM
                                , DT.Rows[0]["F_Delivery_Trip"].ToString().Trim()
                                ,"", FormatPDS) + 1;

                            if (_specialLibs.ChkPDSData(PlantType,_BearerClass.Plant,
                                DeliYM, DT.Rows[0]["F_Delivery_Trip"].ToString().Trim(), StoreCD,FormatPDS) == false)
                            {
                                PDSNo = _specialLibs.GetPDSNo(PlantType, _BearerClass.Plant,
                                    DeliYM, maxRegRunning.ToString(), DT.Rows[0]["F_Delivery_Trip"].ToString().Trim()
                                    , StoreCD, FormatPDS);
                            }
                            else
                            {
                                PDSNo = _specialLibs.GetPDSNo(PlantType, _BearerClass.Plant,
                                    DeliYM, maxPDSRunning.ToString(), DT.Rows[0]["F_Delivery_Trip"].ToString().Trim()
                                    , StoreCD, FormatPDS);
                            }
                        }
                        else
                        {
                            FormatPDS = "2";

                            int maxRegRunning = _specialLibs.GetMaxRegRunning(PlantType
                                , DT.Rows[0]["F_Supplier_CD"].ToString().Trim()
                                , DT.Rows[0]["F_Supplier_Plant"].ToString().Trim()
                                , _BearerClass.Plant
                                , DeliYM
                                , DT.Rows[0]["F_Delivery_Trip"].ToString().Trim()
                                , "", FormatPDS) + 1;

                            int maxPDSRunning = _specialLibs.GetMaxPDSRunning(PlantType
                                , DT.Rows[0]["F_Supplier_CD"].ToString().Trim()
                                , DT.Rows[0]["F_Supplier_Plant"].ToString().Trim()
                                , _BearerClass.Plant
                                , DeliYM
                                , DT.Rows[0]["F_Delivery_Trip"].ToString().Trim()
                                , "", FormatPDS) + 1;

                            if (_specialLibs.ChkPDSData(PlantType, _BearerClass.Plant, DT.Rows[0]["F_Delivery_YM"].ToString().Trim(),
                                    DT.Rows[0]["F_Delivery_Trip"].ToString().Trim(), StoreCD, FormatPDS) == false)
                            {
                                PDSNo = _specialLibs.GetPDSNo(PlantType, _BearerClass.Plant,
                                    DT.Rows[0]["F_Delivery_YM"].ToString().Trim(), maxRegRunning.ToString(),
                                    DT.Rows[0]["F_Delivery_Trip"].ToString().Trim()
                                    , StoreCD, FormatPDS);
                            }
                            else
                            {
                                PDSNo = _specialLibs.GetPDSNo(PlantType, _BearerClass.Plant,
                                    DT.Rows[0]["F_Delivery_YM"].ToString().Trim(), maxPDSRunning.ToString()
                                    , DT.Rows[0]["F_Delivery_Trip"].ToString().Trim()
                                    , StoreCD, FormatPDS);
                            }

                        }

                        string barcode = GenBarcode(PDSNo.Trim());
                        string Supp_CD = DT.Rows[0]["F_Supplier_CD"].ToString().Trim();
                        string Supp_Plant = DT.Rows[0]["F_Supplier_Plant"].ToString().Trim();
                        string DeliveryDate = DT.Rows[0]["F_Delivery_DT"].ToString().Trim().Substring(0, 6);
                        string DeliveryDateFull = DT.Rows[0]["F_Delivery_DT"].ToString().Trim();
                        string DeliveryTrip = DT.Rows[0]["F_Delivery_Trip"].ToString().Trim();
                        string Cycle = _specialLibs.getCycleTime(_BearerClass.Plant, Supp_CD, Supp_Plant, DeliveryDateFull, DeliveryTrip);
                        string DeliDock = getStoreCode(obj.F_Survey_Doc);
                        string DockCode = _specialLibs.GetDockCode(Supp_CD, Supp_Plant, DeliveryDate, DeliveryTrip);
                        string DeliTime = _specialLibs.GetDeliveryTime(Supp_CD, Supp_Plant,Cycle, DeliveryDate, DeliveryTrip);
                        string Vat = _specialLibs.getVat(_BearerClass.Plant);

                        _sql = $@"Insert into TB_PDS_Header 
                            (F_OrderNo, F_PO_Customer, F_Plant, F_Supplier_Code , F_Supplier_Plant, 
                            F_Delivery_Date, F_Delivery_Trip, F_Delivery_Time, F_Delivery_Cycle, 
                            F_Delivery_Dock, F_OrderType, F_Issued_By, F_Issued_Date, 
                            F_Dept, F_CR, F_DR, F_WK_Code,F_Status, F_Vat, F_Dock_Code, F_MRN_Flag, 
                            F_Printed, F_Remark,F_Remark2,F_Remark3,F_Remark_KB,F_Transportor,F_Barcode) 
                            SELECT Top 1 '{PDSNo}' As PDSNo, D.F_PO_Customer,F_Factory_Code,F_Supplier_CD, F_Supplier_Plant, 
                            '{DeliveryDateFull}' ,F_Delivery_Trip,'{DeliTime}' As Delitime, '{Cycle}'
                            ,'{DeliDock}' As F_Deli_Doc,'S' As OrderType,'{userName}' As F_Issue_By, getdate(), 
                            F_Dept_Code,F_ACC_Cr,F_Acc_Dr,F_WK_Code,'N' As F_Status,'{Vat}' As VAT,'{DockCode}' As F_Dock_Code,'', 
                            1, F_Remark,F_Remark2,F_Remark3,F_Remark_KB, 
                            (Select Top 1 F_Tran_Type From TB_Import_Delivery Where F_Supplier_Code collate Thai_CI_AS  = H.F_Supplier_CD  
                            and F_Supplier_Plant collate Thai_CI_AS  = H.F_Supplier_Plant 
                            and F_YM = '{DeliveryDate}' Order by F_YM,F_Rev DESC ) AS Transportor,'{barcode}' 
                            FROM TB_Survey_Header H inner join TB_Survey_Detail D 
                            on H.F_Survey_Doc = D.F_Survey_Doc 
                            and H.F_Revise_Rev = D.F_Revise_Rev 
                            Where H.F_Survey_Doc = '{obj.F_Survey_Doc}' 
                            and H.F_Supplier_CD = '{Supp_CD}' 
                            and H.F_Supplier_Plant = '{Supp_Plant}' 
                            and D.F_Delivery_Date = '{DeliveryDateFull}' ";

                        await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                        _log.WriteLogMsg("Insert to TB_PDS_Header | _sql : " + _sql);

                        _sql = $@"UPDATE TB_PDS_HEADER set F_Dock_Code = T.F_Dock_Code 
                            from TB_PDS_HEADER P INNER JOIN (select F_Supplier_Code,F_Supplier_plant,F_Store_Code,Max(F_Supply_Code) as F_Dock_Code from TB_MS_Kanban 
                            Group by F_Supplier_Code,F_Supplier_plant,F_Store_Code) T ON 
                            P.F_Supplier_Code collate Thai_CI_AS = T.F_Supplier_Code and P.F_Supplier_plant collate Thai_CI_AS = T.F_Supplier_Plant 
                            and P.F_Delivery_Dock = T.F_Store_Code 
                            Where F_OrderType='S' ";

                        await _kbContext.Database.ExecuteSqlRawAsync(_sql);

                        _sql = $@"Insert into TB_PDS_Detail 
                            (F_OrderNo, F_Part_No, F_Ruibetsu, F_Kanban_No, F_Box_Qty, F_Unit_price, F_No, F_Unit_Amount, F_Receive_amount, F_Receive_Date, F_Part_Name, F_Address,F_Dock_CD) 
                            Select '{PDSNo}' ,D.F_Part_No,D.F_Ruibetsu,D.F_Kanban_No,D.F_Package,D.F_Unit_price, ROW_NUMBER() OVER(ORDER BY D.F_NO ASC) As No,D.F_Qty,0,'' 
                            ,D.F_Part_Name 
                            ,Isnull((Select F_Address FROM  TB_MS_Kanban WHERE (F_Kanban_No = D.F_Kanban_No) AND (F_Store_Code = '{DeliDock}' ) AND (RTRIM(F_Part_No) + F_Ruibetsu = Rtrim(D.F_Part_No)+D.F_Ruibetsu ) 
                            AND (F_Start_Date <= D.F_Delivery_Date AND  F_End_Date >= D.F_Delivery_Date) 
                            AND F_Supplier_Code = H.F_Supplier_CD and F_Supplier_Plant = H.F_Supplier_Plant),'')  AS F_Address, 
                            Isnull((Select F_Supply_Code FROM  TB_MS_Kanban  WHERE (F_Kanban_No = D.F_Kanban_No) 
                            and (F_Store_Code = '{DeliDock}' ) AND (RTRIM(F_Part_No)+ F_Ruibetsu = Rtrim(D.F_Part_No)+D.F_Ruibetsu ) 
                            and F_Supplier_Code = H.F_Supplier_CD 
                            and F_Supplier_Plant = H.F_Supplier_Plant),'')  AS F_Dock_CD 
                            From  TB_Survey_Header H inner join TB_Survey_Detail D 
                            on H.F_Survey_Doc = D.F_Survey_Doc 
                            and H.F_Revise_Rev = D.F_Revise_Rev 
                            Where H.F_Survey_Doc = '{obj.F_Survey_Doc}' 
                            and D.F_Delivery_Date = '{DeliveryDateFull}' 
                            and H.F_Supplier_Cd = '{Supp_CD}' 
                            and H.F_Supplier_Plant = '{Supp_Plant}' ";

                        await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                        _log.WriteLogMsg("Insert to TB_PDS_Detail | _sql : " + _sql);

                        _sql = $@"Insert into TB_PDS_Tmp(F_ID, F_Supplier_Code, F_Survey_Doc, F_Delivery_Date, F_PDS_NO, F_Process_By, F_Process_Date) Values 
                            ({index + 1}, '{Supp_CD}', '{obj.F_Survey_Doc}', '{DeliveryDateFull}', '{PDSNo}', '{_BearerClass.UserCode}', getdate())";

                        await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                        _log.WriteLogMsg("Insert to TB_PDS_Tmp | _sql : " + _sql);

                        _sql = $@"UPDATE TB_PDS_Header Set F_Transportor ='TTKA' 
                            WHERE F_Delivery_Dock ='1L' and F_OrderType='S'  ";

                        await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                        _log.WriteLogMsg("Update Transportor to TB_PDS_Header  | _sql : " + _sql);

                        _sql = $@"UPDATE TB_Survey_Detail 
                            Set F_PDS_NO = '{PDSNo}', F_PDS_Flg = '1' 
                            Where F_Survey_Doc = '{obj.F_Survey_Doc}' 
                            and F_Delivery_Date = '{DeliveryDateFull}' ";

                        await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                        _log.WriteLogMsg("Update PDSNo, PDSFlag to TB_Survey_Detail  | _sql : " + _sql);

                        index++;
                    }
                    await UpdateCollectDT();

                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }
    }
}
