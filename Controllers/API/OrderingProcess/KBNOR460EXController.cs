using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.SqlServer.Server;
using Microsoft.VisualBasic;

using System.Data;
using System;
using System.Web;
using System.IO;
using System.Text;
using System.Dynamic;
using System.Globalization;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Reflection.PortableExecutable;
using System.Net;
using System.Net.Http;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Collections.Specialized;
using System.DirectoryServices.ActiveDirectory;


using System.Security.Claims;
using Org.BouncyCastle.Asn1.Ocsp;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using HINOSystem.Libs;
using HINOSystem.Context;
using HINOSystem.Models.KB3;
using NPOI.SS.Formula.Functions;
using NPOI.HPSF;
using Humanizer;
using NPOI.SS.Formula.Eval;
using PdfSharp.Pdf.Filters;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.CodeAnalysis.Differencing;
using static System.Net.Mime.MediaTypeNames;
using NPOI.POIFS.Properties;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNOR460EXController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        private readonly string StoragePath = @"wwwroot\Storage\DownloadTemp";
        private readonly TextFileClass _textFileClass;


        public KBNOR460EXController(
            IConfiguration configuration,
            BearerClass bearerClass,
            TextFileClass textFileClass,
            KanbanConnection kanbanConnection,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _textFileClass = textFileClass;
            _KBCN = kanbanConnection;
            _KB3Context = kB3Context;

        }



        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";

            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            try
            {
                _KBCN.Plant = _BearerClass.Plant;

                _SQL = @" EXEC [exec].[spKBNOR460EX_INI_PDS] 
                    'N',
                    '" + _BearerClass.Plant + @"',
                    '" + _BearerClass.UserCode + @"',
                    '' ";
                string _jsPDSNo = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                _SQL = @" EXEC [exec].[spKBNOR460EX_INI_SUPPLIER] 
                    'N',
                    '" + _BearerClass.Plant + @"',
                    '" + _BearerClass.UserCode + @"',
                    '' ";
                string _jsSupplier = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""PDSNo"" : " + _jsPDSNo + @",
                                ""Supplier"" : " + _jsSupplier + @"
                            }
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public async Task<IActionResult> write([FromBody] string pPostData = null)
        {
            dynamic _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";

            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            try
            {
                _KBCN.Plant = _BearerClass.Plant;

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                await _textFileClass.WriteLine(
                    filePath: _data.File.ToString(), 
                    text: _data.Text.ToString()
                    );

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Export data complete"",
                    ""data"": """ + _data.File.ToString() + @"""
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }


    }
}
