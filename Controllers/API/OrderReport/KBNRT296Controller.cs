using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.Proc_DB;
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
                string prodMonth = _json["prodMonth"];
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
        public async Task<bool> LoadInvoiceData(string DeliveryYearMonth, string? SupFrom, string? SupTo)
        {
            try
            {
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                string sqlAppend = "";
                string sDeliveryDT = "";
                string sDeliveryYM = "";
                string sOrderNo = "";
                await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC RPT_Invoice.SP_Get_OrderNo_Data '{UserName}','{DeliveryYearMonth}'");

                if (SupFrom != "" && SupTo != "")
                {
                    sqlAppend = $" and (Right(Rtrim(F_Supplier_code),4) +'-'+ Rtrim(F_Supplier_Plant) >= '{SupFrom}'  and  Right(Rtrim(F_Supplier_code),4)+'-'+ Rtrim(F_Supplier_Plant) <= '{SupTo}' )";
                }

                DataTable DT = _FillDT.ExecuteSQL(" Select F_OrderNO, DeliveryYM,DeliveryDate  From  [RPT_Invoice].[TB_Rec_Header_EPRO] with (nolock) " +
                    $" Where F_Interface_By= '{UserName}' and F_Delay_Invoice_Date = '' {sqlAppend}");
                if (DT.Rows.Count > 0)
                {
                    for (int i = 0; i < DT.Rows.Count - 1; i++)
                    {
                        sDeliveryDT = DT.Rows[i]["DeliveryDate"].ToString();
                        sDeliveryYM = DT.Rows[i]["DeliveryYM"].ToString();
                        sOrderNo = DT.Rows[i]["F_OrderNO"].ToString();

                        await _KB3Context.Database.ExecuteSqlRawAsync("");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
