using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.UrgentOrder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;

namespace KANBAN.Controllers.API.OrderingProcess
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR470Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _IEmail;

        public KBNOR470Controller
        (
            BearerClass bearerClass,
            PPM3Context ppm3Context,
            PPMInvenContext ppmInvenContext,
            KB3Context kb3Context,
            FillDataTable fillDataTable,
            SerilogLibs log,
            IEmailService IEmail
        )
        {
            _BearerClass = bearerClass;
            _PPM3Context = ppm3Context;
            _PPMInvenContext = ppmInvenContext;
            _KB3Context = kb3Context;
            _FillDT = fillDataTable;
            _log = log;
            _IEmail = IEmail;
        }

        [HttpGet]
        public async Task<IActionResult> List_Data()
        {
            try
            {

                string _sql = "Exec [exec].[spKBNOR460_GetPriceZeroUrgent] NULL,NULL";

                var _dt = _FillDT.ExecuteSQL(_sql);

                if(_dt.Rows.Count == 0)
                {
                    return StatusCode(404, new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data Not Found"
                    });
                }

                foreach (DataRow dr in _dt.Rows)
                {
                    dr["F_SUpplier_Code"] = dr["F_SUpplier_Code"].ToString() + "-" + dr["F_SUpplier_Plant"].ToString();
                    dr["F_Part_No"] = dr["F_Part_No"].ToString() + "-" + dr["F_Ruibetsu"].ToString();
                    dr["F_Delivery_Date"] = dr["F_Delivery_Date"].ToString().Substring(6,2) + "/" + dr["F_Delivery_Date"].ToString().Substring(4,2) + "/" + dr["F_Delivery_Date"].ToString().Substring(0,4);
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Get List Data",
                    data = JsonConvert.SerializeObject(_dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Unlock(List<VM_KBNOR470_Unlock> listObj)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if(_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        title = "Error",
                        message = "Unauthorized"
                    });
                }

                string CookieProcDate = Request.Cookies["processDate"].ToString();
                string ProcessDate = DateTime.TryParseExact(CookieProcDate.Substring(0,10), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt) ? dt.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
                string ProcessShift = CookieProcDate.Substring(10, 1) == "D" ? "1:Day Shift" : "2:Night Shift";


                foreach (var obj in listObj)
                {
                    var _UpdatingObj = _KB3Context.TB_REC_HEADER.Where(x => x.F_OrderNo == obj.F_OrderNo).FirstOrDefault();

                    if (_UpdatingObj == null) return StatusCode(404, new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Error",
                        message = "Data Not Found"
                    });

                    _UpdatingObj.F_Status = 'N';
                    _UpdatingObj.F_Flg_Epro = '1';
                    _KB3Context.Update(_UpdatingObj);
                }

                await _KB3Context.SaveChangesAsync();

                var _distinctList = listObj.DistinctBy(x => new
                {
                    x.F_SUpplier_Code,
                    x.F_SUpplier_Plant,
                    x.F_OrderNo,
                    x.F_Delivery_Date
                });

                string nDetail = "";
                string nSupplier = "";
                foreach (var obj in _distinctList)
                {
                    nDetail += "Supplier : " + obj.F_SUpplier_Code +
                        " : PDS No : " + obj.F_OrderNo +
                        " : Delivery Date : " + obj.F_Delivery_Date + "<br/><br/>";

                    nSupplier += obj.F_SUpplier_Code.Substring(0, 4) + "','";
                }

                if (!string.IsNullOrWhiteSpace(nDetail))
                {
                    nSupplier = nSupplier.Substring(0, nSupplier.Length - 3);
                }

                await _IEmail.SendEmailUnlock("Urgent", "KBNOR470 : Un-Lock PDS not Price (For Interface Data to E-Procurement System)"
                    , nDetail, ProcessDate, ProcessShift, nSupplier);
                

                _log.WriteLogMsg("Un-Lock PDS Urgent");



                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Unlock Data Complete"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error",
                    error = ex.Message
                });
            }
        }

    }
}
