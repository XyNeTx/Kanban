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
        public async Task<IActionResult> SaveAddCycle(TB_Kanban_Add obj)
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
