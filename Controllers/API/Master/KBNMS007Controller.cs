using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.Web;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.Net.Http.Headers;
using System.Collections.Specialized;
using System.Net;
using System.DirectoryServices.ActiveDirectory;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Org.BouncyCastle.Asn1.Ocsp;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading.Tasks;

using HINOSystem.Libs;
using HINOSystem.Context;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNMS007Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _IEmail;

        public KBNMS007Controller
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
        public async Task<IActionResult> Search(string? F_Kanban_No, string? F_Supplier_Code, string? F_Store_Code, string? F_Part_No)
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return Unauthorized(new
                    {
                        status = "401",
                        response = "Unauthorized",
                        title = "Error",
                        message = "Please Login then try again",
                    });
                }

                if (string.IsNullOrWhiteSpace(F_Kanban_No) && string.IsNullOrWhiteSpace(F_Supplier_Code) && string.IsNullOrWhiteSpace(F_Store_Code) && string.IsNullOrWhiteSpace(F_Part_No))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Please Select Data to Search Data"
                    });
                }

                if (!string.IsNullOrWhiteSpace(F_Supplier_Code) && string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Please Select Kanban No Before"
                    });
                }

                if (string.IsNullOrWhiteSpace(F_Supplier_Code) && !string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Please Select Supplier Code Before"
                    });
                }

                if (!string.IsNullOrWhiteSpace(F_Store_Code) && string.IsNullOrWhiteSpace(F_Part_No))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Please Select Part No Before"
                    });
                }

                if (string.IsNullOrWhiteSpace(F_Store_Code) && !string.IsNullOrWhiteSpace(F_Part_No))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Error",
                        message = "Please Select Store Code Before"
                    });
                }
                
                var dbObj = _KB3Context.TB_Kanban_Add.Where(x=>x.F_Plant == _BearerClass.Plant).AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    dbObj = dbObj.Where(x => x.F_Kanban_No == F_Kanban_No);
                }
                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    dbObj = dbObj.Where(x => x.F_Supplier_Code == F_Supplier_Code.Substring(0,4)
                    && x.F_Supplier_Plant == F_Supplier_Code.Substring(5, 1));
                }
                if (!string.IsNullOrWhiteSpace(F_Store_Code))
                {
                    dbObj = dbObj.Where(x => x.F_Store_Code == F_Store_Code);
                }
                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    dbObj = dbObj.Where(x => x.F_Part_No == F_Part_No.Substring(0,10)
                    && x.F_Ruibetsu == F_Part_No.Substring(11,2));
                }


                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = dbObj.FirstOrDefault()

                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error !",
                    error = ex.Message
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Save(TB_Kanban_Add obj)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401)
                {
                    return Unauthorized(new
                    {
                        status = "401",
                        response = "Unauthorized",
                        title = "Error",
                        message = "Please Login then try again",
                    });
                }

                var dbObj = _KB3Context.TB_Kanban_Add.FirstOrDefault(x => x.F_Kanban_No == obj.F_Kanban_No && x.F_Plant == _BearerClass.Plant
                && x.F_Supplier_Code == obj.F_Supplier_Code && x.F_Supplier_Plant == obj.F_Supplier_Plant && x.F_Store_Code == obj.F_Store_Code
                && x.F_Part_No == obj.F_Part_No && x.F_Ruibetsu == obj.F_Ruibetsu);

                if(dbObj == null)
                {
                    obj.F_Status = "0";
                    obj.F_Create_By = _BearerClass.UserCode;
                    obj.F_Create_Date = DateTime.Now;
                    obj.F_Update_By = _BearerClass.UserCode;
                    obj.F_Update_Date = DateTime.Now;
                    obj.F_Start_Date = "";
                    obj.F_Start_Shift = "";
                    obj.F_Finish_Date = null;
                    obj.F_Finish_Trip = null;

                    _KB3Context.TB_Kanban_Add.Add(obj);
                }
                else
                {
                    _KB3Context.TB_Kanban_Add.Remove(dbObj);

                    obj.F_Create_By = dbObj.F_Create_By;
                    obj.F_Create_Date = dbObj.F_Create_Date;
                    dbObj = obj;
                    dbObj.F_Status = "0";
                    dbObj.F_Update_By = _BearerClass.UserCode;
                    dbObj.F_Update_Date = DateTime.Now;
                    dbObj.F_Start_Date = "";
                    dbObj.F_Start_Shift = "";
                    dbObj.F_Finish_Date = null;
                    dbObj.F_Finish_Trip = null;

                    _KB3Context.TB_Kanban_Add.Add(dbObj);
                }

                await _KB3Context.SaveChangesAsync();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Saved",
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error !",
                    error = ex.Message
                });
            }
        }

    }
}
