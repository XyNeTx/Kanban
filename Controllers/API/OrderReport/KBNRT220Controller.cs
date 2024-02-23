using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Crypto.Tls;
using System.Data;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT220Controller : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNRT220Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            PPMInvenContext pPMInvenContext,
            PPM3Context pPM3Context,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _PPM3Context = pPM3Context;
            _PPMInvenContext = pPMInvenContext;
        }

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

        public async Task<IActionResult> DeleteTemp()
        {
            try
            {
                setConString();
                string _result = "";
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Stock_planing_rpt_TMP WHERE F_Update_By = {0} AND F_Host_name = {1}", UserName, HostName);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": null
                                    }";

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public IActionResult CustomNotFound()
        {
            string _result = @"{
                                    ""status"":""404"",
                                    ""response"":""OK"",
                                    ""Title"":""Data Not Found"",
                                    ""message"": ""Please Select Other Option then Try Again""
                             }";

            return Ok(_result);
        }

        public async Task<IActionResult> ReportClick([FromBody] string data)
        {
            try
            {
                setConString();
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject<dynamic>(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                string Location = "";
                string use_line = "";
                string BL = "";
                string tran_cd = "";
                string year_month = "";
                string PEFF = "";
                string f_cd = "";
                string f_location = "";
                string MSP = "";
                string V2V = "";
                string Service = "";
                int sum_PEFF = 0;
                int sum_msp = 0;
                int sum_v2v = 0;
                int sum_srv = 0;


                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }

                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_Stock_planing_rpt_TMP WHERE F_Update_By = {0} AND F_Host_name = {1}", UserName, HostName);

                var viewList = await _KB3Context.V_KBNRT_220_rpt.Where(x => x.F_Delivery_Date.CompareTo(dateFrom) >= 0 && x.F_Delivery_Date.CompareTo(dateTo) <= 0)
                    .Where(x => x.F_Supplier_code.CompareTo(supFrom) >= 0 && x.F_Supplier_code.CompareTo(supTo) <= 0).ToListAsync();

                if (viewList.Count == 0)
                {
                    return CustomNotFound();
                }

                foreach (var item in viewList)
                {
                    string DeliDate = item.F_Delivery_Date.Trim();
                    string PartNo = item.F_Part_No.Trim();
                    string Ruibetsu = item.F_Ruibetsu.Trim();
                    string Str_cd = item.F_Delivery_Dock.Trim().Replace("3", "1");
                    string Str_cdReal = item.F_Delivery_Dock.Trim();
                    string Order = item.F_Unit_Amount.ToString().Trim();
                    string chk_Part = PartNo + Ruibetsu;
                    string SupCode = item.F_Supplier_code.Trim().Substring(0, 4);
                    char SupPlant = item.F_Supplier_code.Trim().Substring(5, 1)[0];
                    string SupplierName = item.F_short_name.Trim() + "-" + SupPlant;
                    string KBno = item.F_Kanban_No.Trim().Substring(1, 3);
                    string KbnRpt = item.F_Kanban_No.Trim();
                    string partNoRpt = PartNo + "-" + Ruibetsu;
                    string partName = item.F_Part_Name.Trim();

                    var TConList = await _PPM3Context.T_Construction
                       .Where(x => x.F_Ruibetsu.Trim() == Ruibetsu && x.F_Part_no.Trim() == PartNo
                       && x.F_Store_cd.Trim() == Str_cd && x.F_supplier_cd.Trim() == SupCode
                       && x.F_plant == SupPlant && x.F_Sebango.Trim() == KBno
                       ).ToListAsync();

                    if (TConList.Count > 0)
                    {
                        foreach (var con in TConList)
                        {
                            if (item.F_commemt != null) use_line = item.F_commemt.Trim();
                        }
                    }

                    // FIND BF STOCK
                    // ลบ วันออกไป 1 วัน เพื่อเอาวันล่าสุดที่เกิดขึ้น
                    string nDeli_Last = (int.Parse(DeliDate) - 1).ToString();

                    var BLList = await _KB3Context.TB_BL.Where(x => x.F_Delivery_Date == nDeli_Last && x.F_Part_No == chk_Part
                        && x.F_Store_Cd == Str_cd).OrderByDescending(x => x.F_Create_Date).ToListAsync();

                    if (BLList.Count > 0)
                    {
                        foreach (var bl in BLList)
                        {
                            BL = bl.F_BL.ToString().Trim();
                        }
                    }

                    // FIND PEFF OUT
                    tran_cd = "21";
                    year_month = DeliDate.Substring(0, 6).Trim();
                    sum_PEFF = 0;

                    var Trans_D_List = await _PPMInvenContext.T_Transaction_D_.FromSqlRaw("SELECT * FROM [HMMT-PPM].[INVENTORY].dbo.T_Transaction_D_" + year_month +
                        " WHERE F_Date = {0} AND F_PartNo = {1} AND F_Ruibetsu = {2} AND F_Tran_CD = {3}", DeliDate, PartNo, Ruibetsu, tran_cd).ToListAsync();

                    if (Trans_D_List.Count > 0)
                    {
                        foreach (var trans in Trans_D_List)
                        {
                            PEFF = trans.F_Qty.ToString().Trim();
                            sum_PEFF = int.Parse(PEFF) + sum_PEFF;
                        }
                    }

                    //FIND OUT MSP
                    tran_cd = "43";
                    f_cd = "-1";
                    f_location = "MSP";
                    sum_msp = 0;

                    var Trans_MSP_List = await _PPMInvenContext.T_Transaction_D_.FromSqlRaw("SELECT * FROM [HMMT-PPM].[INVENTORY].dbo.T_Transaction_D_" + year_month +
                        " WHERE F_Date = {0} AND F_PartNo = {1} AND F_Ruibetsu = {2} AND F_Tran_CD = {3} AND F_Code = {4} AND F_Location = {5} ", DeliDate, PartNo, Ruibetsu, tran_cd, f_cd, f_location).ToListAsync();

                    if (Trans_MSP_List.Count > 0)
                    {
                        foreach (var tranMSP in Trans_MSP_List)
                        {
                            MSP = tranMSP.F_Qty.ToString().Trim();
                            sum_msp = int.Parse(MSP) + sum_msp;
                        }
                    }

                    //FIND OUT V-V
                    tran_cd = "43";
                    f_cd = "-1";
                    f_location = "V2V";
                    sum_v2v = 0;

                    var Trans_V2V_List = await _PPMInvenContext.T_Transaction_D_.FromSqlRaw("SELECT * FROM dbo.T_Transaction_D_" + year_month +
                    " WHERE F_Date = {0} AND F_PartNo = {1} AND F_Ruibetsu = {2} AND F_Tran_CD = {3} AND F_Code = {4} AND F_Location = {5}", DeliDate, PartNo, Ruibetsu, tran_cd, f_cd, f_location).ToListAsync();

                    if (Trans_V2V_List.Count > 0)
                    {
                        foreach (var tranV2V in Trans_V2V_List)
                        {
                            V2V = tranV2V.F_Qty.ToString().Trim();
                            sum_v2v = int.Parse(V2V) + sum_v2v;
                        }
                    }

                    //FIND OUT SERVICE
                    tran_cd = "43";
                    f_cd = "-1";
                    f_location = "SRV";
                    sum_srv = 0;

                    var Trans_SRV_List = await _PPMInvenContext.T_Transaction_D_.FromSqlRaw("SELECT * FROM dbo.T_Transaction_D_" + year_month +
                    " WHERE F_Date = {0} AND F_PartNo = {1} AND F_Ruibetsu = {2} AND F_Tran_CD = {3} AND F_Code = {4} AND F_Location = {5}", DeliDate, PartNo, Ruibetsu, tran_cd, f_cd, f_location).ToListAsync();

                    foreach (var transSRVV in Trans_SRV_List)
                    {
                        Service = transSRVV.F_Qty.ToString().Trim();
                        sum_srv = int.Parse(Service) + sum_srv;
                    }

                    int difference = int.Parse(Order) - (sum_PEFF + sum_msp + sum_v2v + sum_srv);
                    int BL_stock = 0;
                    if (BL != "" && Order != "")
                    {
                        BL_stock = int.Parse(BL) + (int.Parse(Order) - sum_PEFF - sum_msp - sum_v2v - sum_srv);
                    }
                    else if (BL != "" && Order == "")
                    {
                        BL_stock = int.Parse(BL) + (sum_PEFF - sum_msp - sum_v2v - sum_srv);
                    }
                    else if (BL == "" && Order != "")
                    {
                        BL_stock = (int.Parse(Order) - sum_PEFF - sum_msp - sum_v2v - sum_srv);
                    }
                    else if (BL == "" && Order == "")
                    {
                        BL_stock = sum_PEFF - sum_msp - sum_v2v - sum_srv;
                    }

                    if (Str_cd == "1E")
                    {
                        Location = "MSP";
                    }
                    else if (Str_cd == "1F")
                    {
                        Location = "SRV";
                    }
                    else if (Str_cd == "1H")
                    {
                        Location = "V2V";
                    }
                    else if (Str_cd == "1K")
                    {
                        Location = "MARU-G";
                    }
                    else Location = "MAT1";

                    await _KB3Context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SP_TB_Stock_planing_rpt_TMP] {0},{1},{2},{3},{4},{5}," +
                        "{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}", Str_cdReal, KbnRpt, SupCode, SupplierName, partNoRpt, partName
                        , Location, use_line, BL, Order, sum_PEFF, sum_msp, sum_v2v, sum_srv, difference, BL_stock, UserName, HostName);

                }

                DataTable _dt = _KBCN.ExecuteSQL($"SELECT * FROM TB_Stock_planing_rpt_TMP WHERE F_Update_By = '{UserName}' AND F_Host_name = '{HostName}'", skipLog: true);
                if (_dt.Rows.Count == 0)
                {
                    _result = @"{
                                    ""status"":""404"",
                                    ""response"":""OK"",
                                    ""title"":""Report Data Not Found"",
                                    ""message"": ""Please Try Other Option!""
                                    }";

                    return Ok(_result);
                }

                string _jsonData = JsonConvert.SerializeObject(UserName);
                string _jsonData2 = JsonConvert.SerializeObject(HostName);

                _result = @"{
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
