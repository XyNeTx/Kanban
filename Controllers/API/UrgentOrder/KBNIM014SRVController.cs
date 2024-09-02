using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.KB3.UrgentOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Controllers.API.UrgentOrder
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    
    public class KBNIM014SRVController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDataTable;
        private readonly SerilogLibs _log;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNIM014SRVController(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            PPMInvenContext pPMInvenContext,
            PPM3Context pPM3Context,
            KB3Context kB3Context,
            FillDataTable fillDataTable,
            SerilogLibs log
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
            _FillDataTable = fillDataTable;
            _log = log;
        }


        [HttpPost]
        public async Task<IActionResult> InsertDataFromImport(List<TB_Import_Service_Excel> listObj)
        {
            try
            {
                bool IsExcel = false;
                _BearerClass.Authentication(Request);

                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });
                
                string USERID = HttpContext.Session.GetString("USER_CODE");
                await _KB3Context.Database.ExecuteSqlRawAsync($"DELETE FROM TB_IMPORT_SERVICE WHERE F_UPDATE_BY = @p0",USERID);
                foreach (var each in listObj)
                {
                    if(each.F_Trip_No == null)
                    {
                        IsExcel = true;
                    }

                    if(each.F_PO_No.Substring(0,3) == "T99" || each.F_PO_No.Substring(0,3) == "T89" || each.F_PO_No.Substring(0,3) == "TC2")
                    {
                        if(each.F_Part_No.Count() == 10)
                        {
                            each.F_Ruibetsu = "00";
                        }
                        else
                        {
                            each.F_Ruibetsu = each.F_Part_No.Substring(10, 2);
                            each.F_Part_No = each.F_Part_No.Substring(0, 10);
                        }
                        each.F_Update_By = USERID;

                        if(IsExcel) _log.WriteLogMsg($"Add Data to TB_Import_Service_Excel : {JsonConvert.SerializeObject(each)}");
                        else _log.WriteLogMsg($"Add Data to TB_Import_Service : {JsonConvert.SerializeObject(each)}");

                    }
                    else
                    {
                        return BadRequest(new
                        {
                            status = "400",
                            response = "Bad Request",
                            title = "Import Data Error !",
                            message = "PO No. is invalid."
                        });
                    }
                }

                if (IsExcel)
                {

                    _KB3Context.TB_Import_Service_Excel.AddRange(listObj);

                    await _KB3Context.SaveChangesAsync();

                    await _KB3Context.Database.ExecuteSqlRawAsync($"DELETE FROM TB_Import_Error Where F_Type = 'SRV' AND F_Update_By = @p0 ", USERID);

                    await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC [exec].[spKBNIM014SRV_MRP_EXCEL] @p0", USERID);
                }
                else
                {
                    List<TB_Import_Service> listSrv = new List<TB_Import_Service>();
                    foreach (var each in listObj)
                    {
                        TB_Import_Service srv = new TB_Import_Service();
                        srv.F_Dept_Code = each.F_Dept_Code;
                        srv.F_Supplier_Code = each.F_Supplier_Code;
                        srv.F_Factory_Code = each.F_Factory_Code;
                        srv.F_Supplier_Name = each.F_Supplier_Name;
                        srv.F_Invoice_No = each.F_Invoice_No;
                        srv.F_Invoice_Date = each.F_Invoice_Date;
                        srv.F_Shipment_Date = each.F_Shipment_Date;
                        srv.F_Total_Amount = each.F_Total_Amount;
                        srv.F_Vat_Amount = each.F_Vat_Amount;
                        srv.F_Grand_Total = each.F_Grand_Total;
                        srv.F_Receive_Case_No = each.F_Receive_Case_No;
                        srv.F_PO_No = each.F_PO_No;
                        srv.F_Item_No = each.F_Item_No;
                        srv.F_Part_No = each.F_Part_No;
                        srv.F_Ruibetsu = each.F_Ruibetsu;
                        srv.F_Part_Name = each.F_Part_Name;
                        srv.F_Supplier_Part_No = each.F_Supplier_Part_No;
                        srv.F_Update_By = each.F_Update_By;
                        listSrv.Add(srv);

                    }

                    _KB3Context.TB_Import_Service.AddRange(listSrv);

                    await _KB3Context.SaveChangesAsync();

                    await _KB3Context.Database.ExecuteSqlRawAsync($"DELETE FROM TB_Import_Error Where F_Type = 'SRV' AND F_Update_By = @p0 ", USERID);

                    await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC [exec].[spKBNIM014SRV_MRP] @p0", USERID);

                }
                
                DataTable _dt = _FillDataTable.ExecuteSQL($"SELECT * From TB_Import_Error Where F_Type ='SRV' and F_Update_By = '{USERID}'");

                if(_dt.Rows.Count > 0)
                {                                  
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Import Data Error !",
                        message = "Data has been imported. but Have Some Error",
                        userid = USERID,
                        type = "SRV",
                    });
                }
                return Ok(new
                {
                    status = "200",
                    response = "Ok",
                    title = "Success !",
                    message = "Data has been imported."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Ok",
                    title = "Unexpected Error !",
                    message = "Insert data Error",
                    err = ex.Message
                });
            }
        }
    }
}
