using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;

namespace KANBAN.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNMS005SController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly KB3Context _KB3Context;
        private readonly SerilogLibs _Log;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNMS005SController(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context,
            PPM3Context pPM3Context,
            SerilogLibs serilogLibs
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

        //_BearerClass.Authentication(Request);
        //if (_BearerClass.Status == 401) return Unauthorized(new
        //{
        //    status = "401",
        //    response = "Unauthorized",
        //    title = "Unauthorized",
        //    message = "Please Login First"
        //});

        [HttpGet]
        public async Task<IActionResult> GetSupplierCode(bool IsCmdNew)
        {
            try
            {
                setConString();
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });

                string now = DateTime.Now.ToString("yyyyMMdd");

                if (IsCmdNew)
                {
                    var data = _KB3Context.TB_MS_PartOrder
                        .Where(x => x.F_Start_Date.CompareTo(now) <= 0 && x.F_End_Date.CompareTo(now) >= 0 && x.F_Store_Code.StartsWith(_BearerClass.Plant))
                        .Select(x => new
                        {
                            F_Supplier_Code = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant
                        })
                        .AsEnumerable()
                        .DistinctBy(x => x.F_Supplier_Code)
                        .OrderBy(x => x.F_Supplier_Code)
                        .ToList();

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        title = "Success",
                        message = "Data Found",
                        data = data
                    });
                }
                else
                {
                    var data = _KB3Context.TB_BL_SET
                        .Where(x => x.F_Store_Cd.StartsWith(_BearerClass.Plant))
                        .Select(x => new
                        {
                            F_Supplier_Code = x.F_Sup_Cd + "-" + x.F_Sup_Plant
                        })
                        .AsEnumerable()
                        .DistinctBy(x => x.F_Supplier_Code)
                        .OrderBy(x => x.F_Supplier_Code)
                        .ToList();

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        title = "Success",
                        message = "Data Found",
                        data = data
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = "Unexpected Error Occured",
                    error = ex.Message.ToString()
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreCode(bool IsCmdNew,string? F_Supplier_Code)
        {
            try
            {
                setConString();
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });

                string now = DateTime.Now.ToString("yyyyMMdd");

                if (IsCmdNew)
                {
                    var data = _KB3Context.TB_MS_PartOrder
                        .Where(x => x.F_Start_Date.CompareTo(now) <= 0 && x.F_End_Date.CompareTo(now) >= 0 && x.F_Store_Code.StartsWith(_BearerClass.Plant))
                        .Select(x => new
                        {
                            F_Store_Code = x.F_Store_Code.Trim(),
                            F_Supplier_Code = x.F_Supplier_Cd + "-" + x.F_Supplier_Plant
                        })
                        .AsEnumerable()
                        .DistinctBy(x => x.F_Store_Code)
                        .OrderBy(x => x.F_Store_Code)
                        .ToList();

                    if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                    {
                        data = data.Where(x => x.F_Supplier_Code == F_Supplier_Code).ToList();
                    }

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        title = "Success",
                        message = "Data Found",
                        data = data
                    });
                }
                else
                {
                    var data = _KB3Context.TB_BL_SET
                        .Where(x => x.F_Store_Cd.StartsWith(_BearerClass.Plant))
                        .Select(x => new
                        {
                            F_Store_Code = x.F_Store_Cd.Trim(),
                            F_Supplier_Code = x.F_Sup_Cd + "-" + x.F_Sup_Plant
                        })
                        .AsEnumerable()
                        .DistinctBy(x => x.F_Store_Code)
                        .OrderBy(x => x.F_Store_Code)
                        .ToList();

                    if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                    {
                        data = data.Where(x => x.F_Supplier_Code == F_Supplier_Code).ToList();
                    }

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        title = "Success",
                        message = "Data Found",
                        data = data
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = "Unexpected Error Occured",
                    error = ex.Message.ToString()
                });
            }
        }
    }
}
