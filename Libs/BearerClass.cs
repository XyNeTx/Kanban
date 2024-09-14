//using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using SkiaSharp;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace HINOSystem.Libs
{
    public class BearerClass
    {
        private readonly IConfiguration _config;
        private readonly ERPConnection _erpConnection;
        private readonly KanbanConnection _KBCN;
        private readonly IHttpContextAccessor _http;

        protected IWorkbook workbook;
        protected IFormulaEvaluator formulaEvaluator;
        protected DataFormatter dataFormatter;

        private int FormatRow = 0;

        public int Status = 401;
        public string Token = "";
        public string UserCode = "";
        public string Device = "";
        public string Plant = "";
        public string IPAddress = "";
        public string ProcessDate = "";
        public string Shift = "";
        public string ControllerName = "";
        public string ActionName = "";
        public string Response = "";
        public string Message = "";
        public dynamic Records = null;
        public string LOV = "";
        public JObject Data = null;

        public JObject Result = null;

        public BearerClass(
            IConfiguration configuration,
            KanbanConnection kanbanConnection,
            ERPConnection erpConnection,
            IHttpContextAccessor http
            )
        {
            _config = configuration;
            _erpConnection = erpConnection;
            _KBCN = kanbanConnection;
            _http = http;

        }

        public dynamic Authentication(HttpRequest pRequest)
        {
            if (pRequest == null)
            {
                this.Status = 403;
                this.Response = "FORBIDDEN";
                this.Message = "Unauthorized with the header request not found.";

                return JObject.Parse(@"{
                                    ""status"":""403"",
                                    ""response"":""FORBIDDEN"",
                                    ""message"": ""Unauthorized with the header request not found.""
                                }"
                );
            }
            //_KBCN.Plant = (_KBCN.Plant.ToString() == "" ? pRequest.Headers["Plant"].ToString() : _KBCN.Plant);
            DataTable _dt = _KBCN.ExecuteSQL("SELECT * FROM [erp].[User] WHERE Token = '" + pRequest.Headers["Authorization"].ToString().Replace("Bearer ", "") + "' ", skipLog: true);
            if (_dt.Rows.Count <= 0)
            {
                this.Status = 401;
                this.Response = "UNAUTHORIZED";
                this.Message = "Unauthorized";

                this.Result = JObject.Parse(@"{
                                        ""status"":""401"",
                                        ""response"":""UNAUTHORIZED"",
                                        ""message"": ""Unauthorized with the authorization token is not found.""
                                    }"
                );

                return this.Result;
            }

            DataRow _dr = _dt.Rows[0];
            JObject _BearerClass = JObject.Parse(JsonConvert.SerializeObject(_dr));

            this.Status = 200;
            this.Token = pRequest.Headers.Authorization.ToString().Replace("Bearer ", "");
            this.UserCode = pRequest.Headers["UserCode"].ToString();
            this.Device = pRequest.Headers["Device"].ToString();
            this.IPAddress = pRequest.Headers["IPAddress"].ToString();
            this.Plant = pRequest.Headers["Plant"].ToString();
            this.ProcessDate = pRequest.Headers["ProcessDate"].ToString();
            this.Shift = pRequest.Headers["Shift"].ToString();
            this.ControllerName = pRequest.Headers["Controller"].ToString();
            this.ActionName = pRequest.Headers["Action"].ToString();
            this.Records = JsonConvert.DeserializeObject((pRequest.Headers["Records"].ToString() == "" ? "{}" : pRequest.Headers["Records"].ToString()));
            this.LOV = pRequest.Headers["LOV"].ToString();
            this.Data = JObject.Parse(JsonConvert.SerializeObject(_dr));

            Int64 UserID = _dr.Table.Rows[0].Field<Int64>("_ID");
            string sql = "SELECT * FROM [erp].[UserAuthorize] WHERE User_ID = " + UserID + " AND Remark LIKE '%" + this.ActionName + "%' ";
            _dt = _KBCN.ExecuteSQL(sql, skipLog: true);

            if(_dt == null || _dt.Rows.Count <= 0)
            {
                this.Status = 403;
                this.Response = "FORBIDDEN";
                this.Message = "You are not allowed to access this page.";

                this.Result = JObject.Parse(@"{
                                        ""status"":""403"",
                                        ""response"":""FORBIDDEN"",
                                        ""message"": ""You are not allowed to access this page.""
                                    }"
                );

                return this.Result;
            }
            return this;
        }

        public int CheckAuthen()
        {
            this.Authentication(_http.HttpContext.Request);
            return this.Status;
        }

        public string StoreAccess()
        {
            return this.Plant switch
            {
                "1" => "1A",
                "2" => "2B",
                "3" => "3C",
                _ => "3C",
            };
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
            JObject _BearerClass = JObject.Parse(JsonConvert.SerializeObject(_dr));

            return JObject.Parse(@"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": """",
                                    ""user"":" + _BearerClass.GetValue("Table")[0] + @"
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
            JObject _BearerClass = JObject.Parse(JsonConvert.SerializeObject(_dr));

            this.Status = 200;
            //this.Bearer = _BearerClass.GetValue("Table")[0]["Bearer"].ToString();
            this.UserCode = _BearerClass.GetValue("Table")[0]["Code"].ToString();
            //this.Plant = _BearerClass.GetValue("Table")[0]["Code"].ToString();
            //this.ProcessDate = _BearerClass.GetValue("Table")[0]["Code"].ToString();
            //this.Shift = _BearerClass.GetValue("Table")[0]["Code"].ToString();
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


        public string versions()
        {
            string _version = @"KANBAN.dll";
            if (System.IO.File.Exists(_version))
            {
                DateTime _lastModified = System.IO.File.GetLastWriteTime(_version);
                _version = _lastModified.ToString("yy.MM.ddhhmm");
            }
            else
            {
                _version = DateTime.Now.ToString("yy.MM.ddhhmm");
            }

            return _version;
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