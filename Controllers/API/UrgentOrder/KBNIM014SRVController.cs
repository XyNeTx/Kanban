using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Models.KB3.UrgentOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            FillDataTable fillDataTable
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


        [HttpPost]
        public IActionResult InsertDataFromImport(List<TB_Import_Service> listObj)
        {
            try
            {
                setConString();
                string USERID = HttpContext.Session.GetString("USER_CODE");
                _KB3Context.Database.ExecuteSqlRawAsync($"DELETE FROM TB_IMPORT_SERVICE WHERE F_UPDATE_BY = '{USERID}'");
                foreach (var each in listObj)
                {
                    if(each.F_PO_No.Substring(0,3) == "T99" || each.F_PO_No.Substring(0,3) == "T89")
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

                // รอ พี่เกด ทำ store procedure แล้วเอามาใส่
                _KB3Context.TB_Import_Service.AddRange(listObj);
                _KB3Context.SaveChanges();

                _KB3Context.Database.ExecuteSqlRawAsync($"EXEC [exec].[spKBNIM014SRV_MRP] '{USERID}'");
                DataTable _dt = _FillDataTable.ExecuteSQL($"SELECT * From TB_Import_Error Where F_Type ='SRV' and F_Update_By = '{USERID}'");

                if(_dt.Rows.Count > 0)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Import Data Error !",
                        message = "Data has been imported. but some data was error"
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
