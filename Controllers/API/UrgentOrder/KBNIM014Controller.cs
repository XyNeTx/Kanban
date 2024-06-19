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
using HINOSystem.Models.KB3;
using NPOI.HPSF;
using Humanizer;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.Eval;
using PdfSharp.Pdf.Filters;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;
using NPOI.POIFS.Properties;
using KANBAN.Models.KB3.UrgentOrder;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNIM014Controller : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;        
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;

        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNIM014Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;

        }
        [HttpPost]
        public async Task<IActionResult> ImportSave(TB_Import_EKanban_Pack obj)
        {
            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                _KB3Transaction.CreateSavepoint("Start_ImportSave");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                obj.F_Plant_CD = Plant;
                obj.F_Update_By = UserID;
                obj.F_Update_Date = DateTime.Now;

                if (ModelState.IsValid)
                {
                    _KB3Context.TB_Import_EKanban_Pack.Add(obj);
                }
                else
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Bad Request",
                        message = "Data Not Valid!"
                    });
                }

                await _KB3Context.SaveChangesAsync();
                _KB3Transaction.Commit();

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
                    message = "Import Data Success!"
                });
            }
            catch (Exception ex)
            {
                _KB3Transaction.Rollback();
                return StatusCode(500,new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = "Data Didn't Import!",
                    err = ex.Message.ToString()
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AfterImported()
        {
            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                _KB3Transaction.CreateSavepoint("Start_AfterImported");
                string UserID = HttpContext.Session.GetString("USER_CODE");
                string Plant = HttpContext.Session.GetString("USER_PLANT");

                await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC [exec].[spKBNIM014] '{Plant}','{UserID}'");

                _KB3Transaction.Commit();

                int _haveError = await _KB3Context.Database.ExecuteSqlRawAsync("SELECT * FROM TB_Import_Error Where F_Update_By = '20062084' and F_Type = 'KBNIM014'; ");

                if(_haveError > 0)
                {
                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        title = "OK",
                        message = "Import Direct Supply Complete!!",
                        err = "Data Imported but Had Some Error"
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "OK",
                    message = "Import Direct Supply Complete!!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Internal Server Error",
                    message = "Data Didn't Import!",
                    err = ex.Message.ToString()
                });
            }
        }
    }
}
