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
    public class BearerClass
    {
        private readonly IConfiguration _config;
        private readonly ERPConnection _erpConnection;
        private readonly KanbanConnection _KBCN;

        protected IWorkbook workbook;
        protected IFormulaEvaluator formulaEvaluator;
        protected DataFormatter dataFormatter;

        private int FormatRow = 0;

        public int Status = 200;
        public string Token = "";
        public string UserCode = "";
        public string Plant = "";
        public string ProcessDate = "";
        public string Shift = "";
        public JObject Data = null;

        public BearerClass(
            IConfiguration configuration,
            KanbanConnection kanbanConnection,
            ERPConnection erpConnection
            )
        {
            _config = configuration;
            _erpConnection = erpConnection;
            _KBCN = kanbanConnection;

        }

        public dynamic Header(HttpRequest pRequest)
        {
            if (pRequest == null)
            {
                return JObject.Parse(@"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Header not found.""
                                }"
                );
            }
            _KBCN.Plant = pRequest.Headers["Plant"].ToString();
            DataTable _dt = _KBCN.ExecuteSQL("SELECT * FROM [erp].[User] WHERE Token = '" + pRequest.Headers["Authorization"].ToString().Replace("Bearer ", "") + "' ", skipLog: true);
            if (_dt.Rows.Count <= 0)
            {
                return JObject.Parse(@"{
                                        ""status"":""401"",
                                        ""response"":""Unauthorized"",
                                        ""message"": ""Unauthorized"",
                                        ""data"": null
                                    }"
                );
            }

            DataRow _dr = _dt.Rows[0];
            JObject _JBearer = JObject.Parse(JsonConvert.SerializeObject(_dr));

            this.Status = 200;
            this.Token = pRequest.Headers.Authorization.ToString().Replace("Bearer ", "");
            this.UserCode = pRequest.Headers["UserCode"].ToString();
            this.Plant = pRequest.Headers["Plant"].ToString();
            this.ProcessDate = pRequest.Headers["ProcessDate"].ToString();
            this.Shift = pRequest.Headers["Shift"].ToString();
            this.Data = JObject.Parse(JsonConvert.SerializeObject(_dr));

            return this;

        }



        public JObject Authorization(string pHeader)
        {
            if (pHeader == null)
            {
                return JObject.Parse(@"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Header not found.""
                                }"
                );
            }
            _KBCN.Plant = "3";
            DataTable _dt = _KBCN.ExecuteSQL("SELECT * FROM [erp].[User] WHERE Token = '" + pHeader.Replace("Bearer ", "") + "' ", skipLog: true);
            if (_dt.Rows.Count <= 0)
            {
                return JObject.Parse(@"{
                                        ""status"":""401"",
                                        ""response"":""Unauthorized"",
                                        ""message"": ""Unauthorized"",
                                        ""data"": null
                                    }"
                );
            }

            DataRow _dr = _dt.Rows[0];
            JObject _JBearer = JObject.Parse(JsonConvert.SerializeObject(_dr));

            return JObject.Parse(@"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": """",
                                    ""user"":" + _JBearer.GetValue("Table")[0] + @"
                                }"
            );
        }


        public dynamic AuthorizationJSON(string pHeader)
        {
            if (pHeader == null)
            {
                return JObject.Parse(@"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Header not found.""
                                }"
                );
            }
            _KBCN.Plant = "3";
            DataTable _dt = _KBCN.ExecuteSQL("SELECT * FROM [erp].[User] WHERE Token = '" + pHeader.Replace("Bearer ", "") + "' ", skipLog: true);
            if (_dt.Rows.Count <= 0)
            {
                return JObject.Parse(@"{
                                        ""status"":""401"",
                                        ""response"":""Unauthorized"",
                                        ""message"": ""Unauthorized"",
                                        ""data"": null
                                    }"
                );
            }

            DataRow _dr = _dt.Rows[0];
            JObject _JBearer = JObject.Parse(JsonConvert.SerializeObject(_dr));

            this.Status = 200;
            //this.Bearer = _JBearer.GetValue("Table")[0]["Bearer"].ToString();
            this.UserCode = _JBearer.GetValue("Table")[0]["Code"].ToString();
            //this.Plant = _JBearer.GetValue("Table")[0]["Code"].ToString();
            //this.ProcessDate = _JBearer.GetValue("Table")[0]["Code"].ToString();
            //this.Shift = _JBearer.GetValue("Table")[0]["Code"].ToString();
            this.Data = JObject.Parse(JsonConvert.SerializeObject(_dr));

            return this;

        }

            public string Encrypt(string clearText)
        {
            string EncryptionKey = _config.GetValue<string>("Application:EncryptionKey").ToString();
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }



        public string Decrypt(string cipherText)
        {
            string EncryptionKey = _config.GetValue<string>("Application:EncryptionKey").ToString();
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }




    }

    public class BearerTokenClass
    {
        public string User = "";


        public BearerTokenClass(
            )
        {

        }
    }

}