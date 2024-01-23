using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.Net.Http.Headers;
using System.Collections.Specialized;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Security.Policy;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using System.Globalization;
//using Microsoft.Office.Interop.Excel;
using NPOI.OpenXmlFormats.Spreadsheet;

using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using System.Reflection.Metadata;

namespace HINOSystem.Libs
{
    public class CookieClass
    {

        private readonly IConfiguration _config;


        protected IWorkbook workbook;
        protected DataFormatter dataFormatter;
        protected IFormulaEvaluator formulaEvaluator;

        private int FormatRow = 0;

        public CookieClass(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void SetCookie(ControllerContext _context, string pCookieName, string CookieValue)
        {
            var cookieOptions = new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.Now.AddDays(1)
            };
            _context.HttpContext.Response.Cookies.Append(pCookieName, CookieValue, cookieOptions);

            return;
        }


    }

}