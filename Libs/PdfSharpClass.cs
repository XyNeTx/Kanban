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
    public class PdfSharpClass
    {

        private readonly IConfiguration _config;


        protected IWorkbook workbook;
        protected DataFormatter dataFormatter;
        protected IFormulaEvaluator formulaEvaluator;

        private int FormatRow = 0;

        public PdfSharpClass(IConfiguration configuration)
        {
            _config = configuration;
        }


        public string[] ExtractTextFromPage(dynamic pContent)
        {
            string lineText = "";
            string[] _PDFLine = { };

            foreach (var contentElement in pContent)
            {

                if (contentElement is COperator)
                {
                    var op = (COperator)contentElement;
                    if (op.OpCode.Name == "Tj" || op.OpCode.Name == "'") // show text operators
                    {
                        var opArg = op.Operands[0];
                        if (opArg is CString)
                        {
                            string text = ((CString)opArg).Value;


                            Array.Resize(ref _PDFLine, _PDFLine.Length + 1);
                            _PDFLine[_PDFLine.Length - 1] = text;




                            //if (text.Contains("\n")) // end of line
                            //{
                            //    string[] lines = text.Split('\n');
                            //    lineText += lines[0];
                            //    Console.WriteLine(lineText); // output current line
                            //    for (int i = 1; i < lines.Length - 1; i++)
                            //    {
                            //        Console.WriteLine(lines[i]); // output intermediate lines
                            //    }
                            //    lineText = lines[lines.Length - 1];
                            //}
                            //else
                            //{
                            //    lineText += text;
                            //}
                        }
                    }
                    else if (op.OpCode.Name == "TJ") // show text with adjustments operator
                    {
                        var arr = (CArray)op.Operands[0];
                        foreach (var opArg in arr)
                        {
                            if (opArg is PdfString)
                            {
                                string text = "";
                                //text =((PdfString)opArg).Value;
                                if (text.Contains("\n")) // end of line
                                {
                                    string[] lines = text.Split('\n');
                                    lineText += lines[0];
                                    Console.WriteLine(lineText); // output current line
                                    for (int i = 1; i < lines.Length - 1; i++)
                                    {
                                        Console.WriteLine(lines[i]); // output intermediate lines
                                    }
                                    lineText = lines[lines.Length - 1];
                                }
                                else
                                {
                                    lineText += text;
                                }
                            }
                        }
                    }

                }
            }


            return _PDFLine;
        }




    }

}