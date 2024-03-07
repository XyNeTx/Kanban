using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.Proc_DB;
using MathNet.Numerics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT296Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _Serilog;
        private readonly ProcDBContext _ProcDB;

        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNRT296Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            PPMInvenContext pPMInvenContext,
            PPM3Context pPM3Context,
            KB3Context kB3Context,
            FillDataTable fillDataTable,
            SerilogLibs serilog,
            ProcDBContext procDB
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPM3Context = pPM3Context;
            _PPMInvenContext = pPMInvenContext;
            _Serilog = serilog;
            _FillDT = fillDataTable;
            _ProcDB = procDB;
        }
        public string OrderNormal = "";
        public string OrderSpecial = "";
        public string Type = "SPECIAL";

        public void setConString()
        {
            try
            {
                if (_KBCN.Plant.ToString() == "3")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB3Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPM3Connection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
                else if (_KBCN.Plant.ToString() == "2")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB2Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPMConnection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
                else if (_KBCN.Plant.ToString() == "1")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB1Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPMConnection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SetVariable()
        {
            try
            {
                if (_KBCN.Plant.ToString() == "1")
                {
                    OrderNormal = "1";
                    OrderSpecial = "9";
                }
                else if (_KBCN.Plant.ToString() == "2")
                {
                    OrderNormal = "2";
                    OrderSpecial = "8";
                }
                else if (_KBCN.Plant.ToString() == "3")
                {
                    OrderNormal = "3";
                    OrderSpecial = "7";
                }
                else
                {
                    throw new Exception("Incorrect Plant");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        [HttpPost]
        public IActionResult Onload([FromBody] string data)
        {
            try
            {
                SetVariable();
                dynamic _json = JsonConvert.DeserializeObject(data);
                string OrderType = _json["OrderType"];
                string yearMonth = _json["yearMonth"];
                string _result = "";
                List<string> _list = new List<string>();
                if (OrderType.ToUpper() == "NORMAL")
                {
                    DataTable DT = _FillDT.ExecuteSQLProcDB("SELECT F_Supplier FROM (  SELECT Right(Rtrim(SupplierCode),4)+'-'+Rtrim(SupplierPlant) AS F_Supplier FROM " +
                    $" T_PDS692_Header Where SupplierCode not in ('09999','00000') and Left(OrderNO,1) = '{OrderNormal}'  UNION ALL " +
                    "  SELECT Right(Rtrim(SupplierCode),4)+'-'+Rtrim(SupplierPlant) AS F_Supplier FROM  T_PDS692_Header_Spc " +
                    $" Where SupplierCode not in ('09999','00000') and Left(OrderNO,2) = '{OrderSpecial}Y' ) P  Group by  F_Supplier Order by F_Supplier ");

                    if (DT.Rows.Count > 0)
                    {
                        _list.Clear();
                        for (int i = 0; i < DT.Rows.Count - 1; i++)
                        {
                            _list.Add(DT.Rows[i].ItemArray[0].ToString().Trim());
                        }
                    }
                    else
                    {
                        throw new Exception("DataTable count is less than 0");
                    }
                }
                else if (OrderType.ToUpper() == "SPECIAL")
                {
                    DataTable DT = _FillDT.ExecuteSQLProcDB("SELECT F_Supplier FROM ( SELECT Right(Rtrim(SupplierCode),4) +'-'+Rtrim(SupplierPlant) AS F_Supplier FROM T_PDS692_Header_Spc " +
                    $" Where SupplierCode not in ('09999','00000') and Left(OrderNO,2) = '{OrderSpecial}Z' ) P  Group by  F_Supplier Order by F_Supplier ");

                    if (DT.Rows.Count > 0)
                    {
                        _list.Clear();
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            _list.Add(DT.Rows[i].ItemArray[0].ToString().Trim());
                        }
                    }
                    else
                    {
                        throw new Exception("DataTable count is less than 0");
                    }
                }
                string _jsonData = JsonConvert.SerializeObject(_list);
                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @"
                                    }";
                return Ok(_result);
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }
        }


        public async Task<IActionResult> LoadInvoiceData([FromBody] string data)
        {
            try
            {
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}");
                }
                dynamic _json = JsonConvert.DeserializeObject(data);
                string DeliveryYearMonth = _json["yearMonth"];
                string SupFrom = _json["supFrom"];
                string SupTo = _json["supTo"];
                string sqlAppend = "";
                string _result = "";
                await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC RPT_Invoice.SP_Get_OrderNo_Data '{UserName}','{DeliveryYearMonth}'");

                if (SupFrom != "" && SupTo != "")
                {
                    sqlAppend = $" and (Right(Rtrim(F_Supplier_code),4) +'-'+ Rtrim(F_Supplier_Plant) >= '{SupFrom}'  and  Right(Rtrim(F_Supplier_code),4)+'-'+ Rtrim(F_Supplier_Plant) <= '{SupTo}' )";
                }

                DataTable DT = _FillDT.ExecuteSQL(" Select Rtrim(F_OrderNO) AS F_OrderNO, DeliveryYM,DeliveryDate  From  [RPT_Invoice].[TB_Rec_Header_EPRO] with (nolock) " +
                    $" Where F_Interface_By= '{UserName}' and F_Delay_Invoice_Date = '' {sqlAppend}");

                if (DT.Rows.Count > 0)
                {
                    string _jsondata = JsonConvert.SerializeObject(DT);
                    _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""rows"": " + _jsondata + @"
                                    }";

                    return Ok(_result);
                }
                else
                {
                    _result = @"{
                                    ""status"":""404"",
                                    ""response"":""OK"",
                                    ""title"" : ""Data Not Found"",
                                    ""message"": ""Please Try Other Option""
                                    }";

                    return Ok(_result);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


        public async Task<IActionResult> UpdateInvoiceData([FromBody] string data)
        {
            try
            {
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}");
                }
                dynamic _json = JsonConvert.DeserializeObject(data);
                string sDeliveryDT = _json.DT["DeliveryDate"];
                string sDeliveryYM = _json.DT["DeliveryYM"];
                string sOrderNo = _json.DT["F_OrderNO"];
                string sqlAppend = "";
                string _result = "";

                await _KB3Context.Database.ExecuteSqlRawAsync($" Update RPT_Invoice.TB_Rec_Header_EPRO SET F_Delay_Invoice_Date = (Select RPT_Invoice.FN_CalculateDelayDate ('{sDeliveryDT}','{sDeliveryYM}')) " +
                    $" Where F_OrderNo = '{sOrderNo}' and  F_Interface_By= '{UserName}' ");

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found""
                                    }";

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


        public async Task<IActionResult> GetDataRunagain([FromBody] string data)
        {
            try
            {
                SetVariable();
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}");
                }
                dynamic _json = JsonConvert.DeserializeObject(data);
                string DeliveryYearMonth = _json["yearMonth"];
                string SupFrom = _json["supFrom"];
                string SupTo = _json["supTo"];
                string sqlAppend = "";
                string _result = "";

                await _KB3Context.Database.ExecuteSqlRawAsync($" Delete from [RPT_Invoice].[TB_Inv_Header_EPRO]  Where  F_interface_By  = '{UserName}' ");

                if (SupFrom != "" && SupTo != "")
                {
                    sqlAppend = $" and (Right(Rtrim(F_sup_cd),4) +'-'+ Rtrim(F_Sup_Plant) >= '{SupFrom}'  and  Right(Rtrim(F_sup_cd),4)+'-'+ Rtrim(F_Sup_Plant) <= '{SupTo}' )";
                }

                List<string> _list = new List<string>();
                DataTable DT = _FillDT.ExecuteSQLProc_Web(" SELECT Rtrim(F_inv_cd) AS F_inv_cd, Rtrim(F_no) AS F_no, Rtrim(F_tac_inv) AS F_tac_inv, Rtrim(F_pds_cd) AS F_pds_cd, F_sup_cd, F_sup_plant, F_inv_date, F_vat " +
                    $"  , F_user_create, F_user_update  , Convert(varchar(8),F_date_create,112) As F_date_Create  , F_date_update, F_Status, Rtrim(F_pro_date) AS F_pro_date, " +
                    $" F_amount_vat, F_tax_ch, F_Status_P, F_Status_Other FROM T_Inv_Header  with (Nolock)  Where 1=1 and F_pds_cd in ( " +
                    $" Select orderNO from T_PDS692_Header Where Left(OrderNO,1) = ('{OrderNormal}') and Convert(varchar(6),DeliveryDate,112) = '{DeliveryYearMonth}' " +
                    $" UNION ALL Select orderNO from T_PDS692_Header_spc Where Left(OrderNO,1) = ('{OrderSpecial}') and Convert(varchar(6),DeliveryDate,112) = '{DeliveryYearMonth}' ) {sqlAppend}");

                if (DT.Rows.Count > 0)
                {
                    string _jsonData = JsonConvert.SerializeObject(DT);
                    _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""rows"": " + _jsonData + @"
                                    }";
                    return Ok(_result);
                }
                else
                {
                    throw new Exception("DataTable count is 0");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<IActionResult> InsertDataRunagain([FromBody] string data)
        {
            try
            {
                SetVariable();
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}");
                }
                dynamic _json = JsonConvert.DeserializeObject(data);
                string F_pds_cd = _json.DT["F_pds_cd"];
                string F_Plant = F_pds_cd.Substring(0, 1);
                string F_inv_cd = _json.DT["F_inv_cd"];
                string F_tac_inv = _json.DT["F_tac_inv"];
                string F_sup_cd = _json.DT["F_sup_cd"];
                string F_sup_plant = _json.DT["F_sup_plant"];
                DateTime F_inv_date = _json.DT["F_inv_date"];
                int F_vat = _json.DT["F_vat"];
                string F_user_create = _json.DT["F_user_create"];
                string F_user_update = _json.DT["F_user_update"];
                string F_date_Create = _json.DT["F_date_Create"];
                DateTime F_date_update = _json.DT["F_date_update"];
                bool F_Status = _json.DT["F_Status"];
                string F_pro_date = _json.DT["F_pro_date"];
                float F_amount_vat = _json.DT["F_amount_vat"];
                bool F_tax_ch = _json.DT["F_tax_ch"];
                char F_Status_P = _json.DT["F_Status_P"];
                char F_Status_Other = _json.DT["F_Status_Other"];

                await _KB3Context.Database.ExecuteSqlRawAsync("Insert into  [RPT_Invoice].[TB_Inv_Header_EPRO] " +
                    " (F_Plant,F_inv_cd,  F_tac_inv, F_pds_cd, F_sup_cd, F_sup_plant, F_inv_date, F_vat " +
                    "  , F_user_create, F_user_update,F_Confirm_Invdate  , F_date_update, F_Status, F_pro_date," +
                    "  F_amount_vat, F_tax_ch, F_Status_P, F_Status_Other,F_interface_by)  Values ( " +
                    $" '{F_Plant}','{F_inv_cd}','{F_tac_inv}','{F_pds_cd}','{F_sup_cd}','{F_sup_plant}','{F_inv_date}'," +
                    $" '{F_vat}','{F_user_create}','{F_user_update}','{F_date_Create}','{F_date_update}','{F_Status}','{F_pro_date}', " +
                    $" '{F_amount_vat}','{F_tax_ch}','{F_Status_P}','{F_Status_Other}','{UserName}') ");

                string _jsonData = JsonConvert.SerializeObject(UserName);
                string _jsonData2 = JsonConvert.SerializeObject(_KBCN.Plant);
                string _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @",
                                    ""data2"": " + _jsonData2 + @"
                                    }";

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}
