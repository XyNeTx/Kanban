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
using Org.BouncyCastle.Asn1.Ocsp;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace HINOSystem.Libs
{
    public class ActionResultClass
    {
        private readonly IConfiguration _config;
        private readonly DefaultConnection _connection;

        protected IWorkbook workbook;
        protected IFormulaEvaluator formulaEvaluator;
        protected DataFormatter dataFormatter;

        private int FormatRow = 0;

        public ActionResultClass(
            IConfiguration configuration,
            DefaultConnection defaultConnection
            )
        {
            _config = configuration;
            _connection = defaultConnection;
        }


        public string Failed(string pMessage, dynamic pData = null)
        {
            if(pData == null )
            {
                return @"{
                    ""status"":""200"",
                    ""response"":""FAILED"",
                    ""message"": """ + pMessage + @"""
                }";
            }
            else
            {
                return @"{
                    ""status"":""200"",
                    ""response"":""FAILED"",
                    ""message"": """ + pMessage + @""",
                    ""data"": " + pData + @"
                }";
            }
        }






    }

}