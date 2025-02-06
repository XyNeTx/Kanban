using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.KB3.ReportOrder;
using KANBAN.Models.KB3.ServiceData.ViewModel;
using KANBAN.Models.KB3.UrgentOrder;
using KANBAN.Services;
using KANBAN.Services.Automapper.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.ServiceData
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNIM001CController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly KB3Context _KB3Context;
        private readonly SerilogLibs _Log;
        private readonly FillDataTable _FillDT;
        private readonly IAutoMapService _automapService;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNIM001CController(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context,
            PPM3Context pPM3Context,
            SerilogLibs serilogLibs,
            FillDataTable fillDT,
            IAutoMapService automapService
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _PPM3Context = pPM3Context;
            _Log = serilogLibs;
            _FillDT = fillDT;
            _automapService = automapService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderNo(bool isChkDate, string? DateFrom, string? DateTo)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _KB3Context.TB_Transaction_TMP
                    .AsNoTracking()
                    .Where(x => x.F_Type == "SRV")
                    .ToListAsync();

                if (isChkDate)
                {
                    data = data.Where(x => x.F_Delivery_Date.CompareTo(DateFrom) >= 0
                        && x.F_Delivery_Date.CompareTo(DateTo) <= 0)
                        .ToList();
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been retrieved",
                    data = data.Select(x => new
                    {
                        F_PDS_No = x.F_PDS_No
                    }).DistinctBy(x => x.F_PDS_No).OrderBy(x => x.F_PDS_No).ToList()
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    throw new CustomHttpException((ex as CustomHttpException).StatusCode, ex.Message);
                }
                else
                {
                    throw new CustomHttpException(500, ex.Message);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string OrderNo, bool isChkDate, string? DateFrom, string? DateTo)
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                string nText = "";

                string sqlQ = $@"select T.F_PDS_NO From TB_Transaction_TMP T INNER JOIN
                    TB_Transaction T1 ON T.F_TYPE = T1.F_TYpe and T.F_PDS_NO = T1.F_PDS_NO 
                    Where T.F_TYPE='SRV' Group by T.F_PDS_NO ";

                var checkData = _FillDT.ExecuteSQL(sqlQ);

                if (checkData.Rows.Count > 0)
                {
                    for (int i = 0; i < checkData.Rows.Count; i++)
                    {
                        nText = nText + checkData.Rows[i]["F_PDS_NO"].ToString() + ",";
                    }
                    nText = nText.Substring(0, nText.Length - 1);

                    throw new CustomHttpException(400, "กรุณาตรวจสอบ PDS No. " + nText + " ข้อมูลซ้ำไม่สามารถ Confirm ข้อมูลได้. กรุณาลบข้อมูลก่อนการ Confirm!!");

                }

                var data = await _KB3Context.TB_Transaction_TMP
                    .AsNoTracking().Where(x => x.F_Type == "SRV").ToListAsync();

                if (isChkDate)
                {
                    data = data.Where(x => x.F_Delivery_Date.CompareTo(DateFrom) >= 0
                    && x.F_Delivery_Date.CompareTo(DateTo) >= 0).ToList();
                }
                if (!string.IsNullOrWhiteSpace(OrderNo))
                {
                    data = data.Where(x => x.F_PDS_No == OrderNo).ToList();
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been retrieved",
                    data = data.Select(x => new
                    {
                        x.F_PDS_No,
                        F_PDS_Issued_Date_old = x.F_PDS_Issued_Date,
                        F_Delivery_Date_old = x.F_Delivery_Date,
                        F_PDS_Issued_Date = DateTime.ParseExact(x.F_PDS_Issued_Date, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                        F_Delivery_Date = DateTime.ParseExact(x.F_Delivery_Date, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy")
                    })
                    .DistinctBy(x => new
                    {
                        x.F_PDS_No,
                        x.F_PDS_Issued_Date,
                        x.F_Delivery_Date
                    }).OrderBy(x => x.F_PDS_No).ThenBy(x => x.F_PDS_Issued_Date_old)
                    .ThenBy(x => x.F_Delivery_Date_old).ToList()
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    throw new CustomHttpException((ex as CustomHttpException).StatusCode, ex.Message);
                }
                else
                {
                    throw new CustomHttpException(500, ex.Message);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCycle()
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var sqlQ = $@"Update TB_Transaction_TMP set F_Cycle_Time = S.F_Cycle 
                    From TB_Transaction_TMP T INNER JOIN TB_MS_DeliveryTime S ON T.F_Supplier_Cd collate Thai_CI_AS = S.F_Supplier_Code 
                    and T.F_Supplier_Plant collate Thai_CI_AS = S.F_Supplier_Plant 
                    and T.F_Delivery_Date >= S.F_Start_Date
                    and T.F_Delivery_Date <= S.F_End_Date 
                    Where T.F_Type='SRV' and T.F_Plant ='{_BearerClass.Plant}' ";

                await _KB3Context.Database.ExecuteSqlRawAsync(sqlQ);

                var data = _KB3Context.TB_REC_DETAIL
                    .AsNoTracking()
                    .AsEnumerable();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Update Cycle Time has been completed",
                    data = data
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    throw new CustomHttpException((ex as CustomHttpException).StatusCode, ex.Message);
                }
                else
                {
                    throw new CustomHttpException(500, ex.Message);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(List<VM_KBNIM001C_Confirm> listObj)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                await _KB3Context.Database.ExecuteSqlRawAsync("Update TB_TRANSACTION_TMP set F_Inventory_Flg ='0' Where F_Type='SRV'");

                List<TB_Transaction> logList = new();
                List<TB_Transaction_TMP> logDelList = new();

                foreach (var each in listObj)
                {
                    string sDeliDate = DateTime.ParseExact(each.F_Delivery_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");
                    string sIssuedDate = DateTime.ParseExact(each.F_PDS_Issued_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");

                    var invenFlg_Tmp = await _KB3Context.TB_Transaction_TMP
                        .AsNoTracking()
                        .Where(x => x.F_Type == "SRV" && x.F_OrderType == 'N'
                        && x.F_Store_CD != "06" && x.F_Inventory_Flg == '9'
                        && x.F_Delivery_Date == sDeliDate && x.F_PDS_No == each.F_PDS_No)
                        .ToListAsync();

                    if (invenFlg_Tmp.Count > 0)
                    {
                        throw new CustomHttpException(400, "Please Check Cycle Time <> Delivery Time Master!!");
                    }

                    var singleTMP = await _KB3Context.TB_Transaction_TMP
                        .AsNoTracking().FirstOrDefaultAsync(x => x.F_Type == "SRV" && x.F_PDS_No == each.F_PDS_No
                        && x.F_PDS_Issued_Date == sIssuedDate && x.F_Delivery_Date == sDeliDate);

                    if (singleTMP == null)
                    {
                        throw new CustomHttpException(404, "Transaction Tmp not found");
                    }

                    var mappingService = _automapService.GetAutoMapRepo<TB_Transaction_TMP, TB_Transaction>();
                    var addTransaction = mappingService.MapTo(singleTMP);

                    addTransaction.F_Process_By = "";
                    addTransaction.F_Process_Date = DateTime.Now.ToString("yyyyMMdd");
                    addTransaction.F_Round = 1;
                    addTransaction.F_Reg_Flg = '1';
                    addTransaction.F_HMMT_PDS = "";
                    addTransaction.F_Survey_Doc = "";
                    addTransaction.F_Update_Date = DateTime.Now;
                    addTransaction.F_Update_By = _BearerClass.UserCode;

                    await _KB3Context.TB_Transaction.AddAsync(addTransaction);
                    _KB3Context.TB_Transaction_TMP.Remove(singleTMP);

                    logList.Add(addTransaction);
                    logDelList.Add(singleTMP);

                }
                await _KB3Context.SaveChangesAsync();
                _Log.WriteLogMsg("INSERT INTO TB_TRANSACTION SRV => " + JsonConvert.SerializeObject(logList));
                _Log.WriteLogMsg("DELETE FROM TB_TRANSACTION_TMP SRV => " + JsonConvert.SerializeObject(logDelList));

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been confirmed"
                    //data = listObj
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    throw new CustomHttpException((ex as CustomHttpException).StatusCode, ex.Message);
                }
                else
                {
                    throw new CustomHttpException(500, ex.Message);
                }
            }
        }

    }
}
