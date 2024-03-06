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

using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;

namespace HINOSystem.Libs
{
    public class TextFileClass
    {
        private readonly IConfiguration _config;
        private readonly ERPConnection _erpConnection;
        private readonly KanbanConnection _KBCN;

        protected IWorkbook workbook;
        protected IFormulaEvaluator formulaEvaluator;
        protected DataFormatter dataFormatter;

        private readonly string StoragePath = @"wwwroot\Storage\DownloadTemp";

        private int FormatRow = 0;

        public JObject Data = null;

        public TextFileClass(
            IConfiguration configuration,
            KanbanConnection kanbanConnection,
            ERPConnection erpConnection
            )
        {
            _config = configuration;
            _erpConnection = erpConnection;
            _KBCN = kanbanConnection;

        }

        public async Task<bool> Write(string filePath = @"\file.txt", string text = @"")
        {

            try
            {
                string fullPath = Path.Combine(this.StoragePath, filePath);

                if (!System.IO.File.Exists(fullPath))
                {
                    // Create the file if it does not exist
                    using (FileStream fs = System.IO.File.Create(fullPath))
                    {
                        // Leave the file open so StreamWriter can write to it
                    }
                }

                using (StreamWriter writer = new StreamWriter(fullPath, true))
                {
                    await writer.WriteAsync(text);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> WriteLine(string filePath = @"\file.txt", string text = @"")
        {

            try
            {
                string fullPath = Path.Combine(this.StoragePath, filePath);

                if (!System.IO.File.Exists(fullPath))
                {
                    // Create the file if it does not exist
                    using (FileStream fs = System.IO.File.Create(fullPath))
                    {
                        // Leave the file open so StreamWriter can write to it
                    }
                }

                using (StreamWriter writer = new StreamWriter(fullPath, true))
                {
                    await writer.WriteLineAsync(text);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


    }
}