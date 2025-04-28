using HINOSystem.Context;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Web;


namespace HINOSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly ERPConnection _erpConnect;
        private readonly BearerClass _BearerClass;
        private readonly KB3Context _KB3Context;

        private string Systems = "KB3";
        private string SystemCode = "Systems:KB3";
        private readonly string StoragePath = @"wwwroot\assets\img\avatars";

        private string _DB = "";
        private string _SQL = "";

        private string _CurrentUserName = "";

        public LoginController(
            IConfiguration configuration,
            IHttpContextAccessor? httpContextAccessor,
            ERPConnection erpConnect,
            BearerClass bearerClass,
            KB3Context kB3Context
            )
        {
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
            _erpConnect = erpConnect;
            _BearerClass = bearerClass;
            _KB3Context = kB3Context;

            this._DB = _config.GetValue<string>(this.SystemCode + ":Database");
        }


        //### Last Modify
        //# 2023-06-21  Prachaya Chotchoung
        #region Login page for supplier
        [HttpGet]
        public IActionResult Index()
        {
            this.fncCheckEnvironment();

            //ViewData["txtUserName"] = "";
            //ViewData["txtProcessDate"] = DateTime.Now.ToString("dd/MM/yyyy");
            //ViewData["ddlShift"] = (int.Parse(DateTime.Now.ToString("Hmm")) <1930 ? "D" : "N");
            //ViewData["ddlFactory"] = 3;

            //var xhr = new XMLHttpRequest();
            //xhr.open("get", "http://hmmta-tpcap/ad");
            //xhr.withCredentials = true;

            if (HttpContext.Request.Query["CN"].ToString() != "")
            {
                string _CN = _BearerClass.Decrypt(HttpContext.Request.Query["CN"].ToString());
                ViewData["CN"] = _CN;
                string[] _arr = _CN.Split("&");
                if (_arr.Length > 0) ViewData["txtUserName"] = _arr[1];
                return View();
            }

            //### Check user for recovery
            _SQL = @"
                SELECT Code
                FROM [erp].[User]
                WHERE [ResetToken] = '" + Request.Query["ref"].ToString() + @"' 
            ";
            DataTable _dataTable = _erpConnect.ExecuteSQL(_SQL, skipLog: true);

            if (_dataTable == null) return View();

            if (_dataTable.Rows.Count > 0)
            {
                ViewBag.refReset = _dataTable.Rows[0]["Code"].ToString();
                ViewData["txtUserName"] = _dataTable.Rows[0]["Code"].ToString();
            }


            string _version = _BearerClass.versions();
            ViewData["Version"] = _version;

            return View();
        }



        [HttpPost]
        public async Task<ActionResult> Index(IFormCollection collection)
        {
            try
            {

                string _txtUserName = Request.Form["txtUserName"].ToString().Trim();
                string _txtPassword = Request.Form["txtPassword"].ToString().Trim();
                string _txtDomain = Request.Form["txtDomain"].ToString().Trim();
                string _txtDeviceName = Request.Form["txtDeviceName"].ToString().Trim();
                string _txtFullDeviceName = Request.Form["txtFullDeviceName"].ToString().Trim();
                string _txtIPAddress = Request.Form["txtIPAddress"].ToString().Trim();
                //string _txtIsHINO = Request.Form["txtIsHINO"].ToString().Trim();
                string _txtIsHINO = "1";
                string _txtProcessDate = Request.Form["txtProcessDate"].ToString().Trim();
                string _ddlFactory = Request.Form["ddlFactory"].ToString().Trim();
                string _ddlShift = Request.Form["ddlShift"].ToString().Trim();


                var getUser = _KB3Context.User.Where(x => x.Code == _txtUserName).FirstOrDefault();
                if (getUser == null)
                {
                    TempData["ErrorText"] = "You Didn't have Permission to access this system.";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    if (getUser.LastLogin < DateTime.Now.AddMonths(-2))
                    {
                        if (getUser.Status.ToLower() == "active")
                        {
                            TempData["ErrorText"] = "Your didn't login for 60 days, Please contact IT Dept.";
                            getUser.Status = "INACTIVE";
                            _KB3Context.User.Update(getUser);
                            _KB3Context.SaveChanges();
                            return RedirectToAction("Index", "Login");
                        }
                        else if (getUser.Status.ToLower() == "inactive")
                        {
                            TempData["ErrorText"] = "Your didn't login for 60 days, Please contact IT Dept.";
                            getUser.Status = "ACTIVE";
                            getUser.LastLogin = DateTime.Now;
                            _KB3Context.User.Update(getUser);
                            _KB3Context.SaveChanges();
                            return RedirectToAction("Index", "Login");
                        }

                        TempData["ErrorText"] = "Your didn't login for 60 days, Please contact IT Dept.";
                        return RedirectToAction("Index", "Login");
                    }
                }


                this.fncCheckEnvironment(_txtUserName);

                _SQL = @" EXEC [erp].[AuthenGuardLogin] '" + _txtUserName + @"', '" + _txtPassword + @"', '" + _txtIsHINO + @"'; ";
                DataTable _dataTable = _erpConnect.ExecuteSQL(_SQL, skipLog: true);

                if (_dataTable == null)
                {
                    TempData["ErrorText"] = "Connection failed: Named Pipes Provider (Please try again later).";
                    this.fncActionLog("LOGIN", "FAILED", _SQL, ViewData["ErrorText"].ToString(), User: _txtUserName, Token: "");
                    return RedirectToAction("Index", "Login");
                }
                if (_dataTable.Rows.Count <= 0)
                {
                    TempData["ErrorText"] = "Invalid account or Password incorrect.";
                    this.fncActionLog("LOGIN", "FAILED", _SQL, ViewData["ErrorText"].ToString(), User: _txtUserName, Token: "", DeviceName: _txtDeviceName, IPAddress: _txtIPAddress);
                    return RedirectToAction("Index", "Login");
                }


                //### Login with HINO employee ###
                string d_code = "";
                string d_name = "";
                string d_surname = "";
                string d_displayname = "";
                string d_email = "";
                string[] _group = { "1", "2", "3", "5" };
                string[] _reserve = { "1", "2", "3", "4", "5", "6", "12" };
                string _hino = "NO";
                if ((Array.IndexOf(_reserve, _dataTable.Rows[0]["_ID"].ToString()) == -1) && (Array.IndexOf(_group, _dataTable.Rows[0]["Group_ID"].ToString()) != -1))
                {
                    PrincipalContext _pc = new PrincipalContext(ContextType.Domain, "THI_DM_1");
                    UserPrincipal user = UserPrincipal.FindByIdentity(_pc, _txtUserName);

                    d_code = user.SamAccountName;
                    d_name = user.GivenName;
                    d_surname = user.Surname;
                    d_displayname = user.DisplayName;
                    d_email = user.EmailAddress;
                    _hino = "YES";
                }



                //### Login with another user ###
                //if (_dataTable.Rows[0]["decypt"].ToString() != _txtPassword)
                //{
                //    ViewData["ErrorText"] = "Password incorrect.";
                //    this.fncActionLog("LOGIN", "FAILED", _SQL, ViewData["ErrorText"].ToString(), User: _txtUserName, Token: _dataTable.Rows[0]["Token"].ToString());
                //    return View();
                //}


                string _language = _dataTable.Rows[0]["UILanguage"].ToString() == "EN" ? "" : _dataTable.Rows[0]["UILanguage"].ToString();


                HttpContext.Session.SetString("SYSTEM", this.Systems);
                HttpContext.Session.SetString("HINO", _txtIsHINO);
                HttpContext.Session.SetString("TOKEN", _dataTable.Rows[0]["Token"].ToString());
                HttpContext.Session.SetString("USER_ID", _dataTable.Rows[0]["_ID"].ToString());
                HttpContext.Session.SetString("USER_CODE", (d_code != "" ? d_code : _dataTable.Rows[0]["Code"].ToString()));
                HttpContext.Session.SetString("USER_NAME", (d_name != "" ? d_name : _dataTable.Rows[0]["Name"].ToString()));
                HttpContext.Session.SetString("USER_SURNAME", (d_surname != "" ? d_surname : _dataTable.Rows[0]["Surname"].ToString()));
                HttpContext.Session.SetString("USER_NAMETH", _dataTable.Rows[0]["NameTH"].ToString());
                HttpContext.Session.SetString("USER_SURNAMETH", _dataTable.Rows[0]["SurnameTH"].ToString());
                HttpContext.Session.SetString("USER_DISPLAY", _dataTable.Rows[0]["Name" + _language].ToString() + " " + _dataTable.Rows[0]["Surname" + _language].ToString());
                if ((_dataTable.Rows[0]["Name" + _language].ToString() == null) && (_dataTable.Rows[0]["Surname" + _language].ToString() == null))
                {
                    HttpContext.Session.SetString("USER_DISPLAY", (d_displayname != "" ? d_displayname : _dataTable.Rows[0]["Name"].ToString() + " " + _dataTable.Rows[0]["Surname"].ToString()));
                }
                HttpContext.Session.SetString("USER_EMAIL", (d_email != "" ? d_email : _dataTable.Rows[0]["Email"].ToString()));
                HttpContext.Session.SetString("USER_UILANGUAGE", _language);
                HttpContext.Session.SetString("USER_DOMAIN", _txtDomain);
                HttpContext.Session.SetString("USER_DEVICENAME", _txtDeviceName);
                HttpContext.Session.SetString("USER_FULLDEVICENAME", _txtFullDeviceName);
                HttpContext.Session.SetString("USER_IPADDRESS", _txtIPAddress);
                HttpContext.Session.SetString("USER_PROCESSDATE", _txtProcessDate);
                HttpContext.Session.SetString("USER_PLANT", _ddlFactory);
                HttpContext.Session.SetString("USER_SHIFT", _ddlShift);

                string _avatar = (_dataTable.Rows[0]["Avatar"].ToString() == "" ? "hino/" + _dataTable.Rows[0]["Code"].ToString() + ".jpg" : "private/" + _dataTable.Rows[0]["Avatar"].ToString());
                if (!System.IO.File.Exists(this.StoragePath + @"\" + _avatar)) _avatar = "avatar.jpg";
                HttpContext.Session.SetString("USER_AVATAR", _avatar);
                HttpContext.Session.SetString("USER_UITHEME", _dataTable.Rows[0]["UITheme"].ToString());
                HttpContext.Session.SetString("USER_UIHEADERBRAND", _dataTable.Rows[0]["UIHeaderBrand"].ToString());
                HttpContext.Session.SetString("USER_UIHEADER", _dataTable.Rows[0]["UIHeader"].ToString());
                HttpContext.Session.SetString("USER_UILINKCOLOR", _dataTable.Rows[0]["UILinkColor"].ToString());
                HttpContext.Session.SetString("USER_UIMENUCOLOR", _dataTable.Rows[0]["UIMenuColor"].ToString());
                HttpContext.Session.SetString("USER_UIICONCOLOR", _dataTable.Rows[0]["UIIconColor"].ToString());
                HttpContext.Session.SetString("USER_UIEXPANDICON", _dataTable.Rows[0]["UIExpandIcon"].ToString());
                HttpContext.Session.SetString("USER_UIMENUICON", _dataTable.Rows[0]["UIMenuIcon"].ToString());
                HttpContext.Session.SetString("USER_UISIDEBAR", _dataTable.Rows[0]["UISideBar"].ToString());

                HttpContext.Session.SetString("USER_GROUP_ID", _dataTable.Rows[0]["Group_ID"].ToString());
                HttpContext.Session.SetString("USER_GROUP_NAME", _dataTable.Rows[0]["Group_Name"].ToString());
                HttpContext.Session.SetString("USER_GROUP_NAMETH", _dataTable.Rows[0]["Group_NameTH"].ToString());

                this.fncActionLog("LOGIN", "OK", _SQL);

                string _url = "~/Home";

                //string _url = "~/" + (this.Systems != "" ? this.Systems + "/" : "") + "Home";
                //if (_dataTable.Rows[0]["Group_ID"].ToString() == "3") _url = "~/UploadReceipt/UploadReceipt";
                //if (_dataTable.Rows[0]["Group_ID"].ToString() == "4") _url = "~/Supplier/ClaimInformation";

                var handler = new HttpClientHandler
                {
                    UseDefaultCredentials = true,
                };
                var _httpClient = new HttpClient(handler);

                var query = HttpUtility.ParseQueryString(string.Empty);
                query["system_name"] = "Hino Kanban System";
                query["isLogin"] = "True";

                // Build the full URI
                var uriBuilder = new UriBuilder("http://hmmt-app07/sso_test/api/SingleSignOn/GetLogin")
                {
                    Query = query.ToString() // Automatically encodes the query string
                };
                var response = await _httpClient.GetAsync(uriBuilder.Uri);

                if (response.IsSuccessStatusCode)
                {
                    var rawJson = await response.Content.ReadAsStringAsync();

                    var parsedJson = JToken.Parse(rawJson);
                    var prettyJson = parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);
                }

                return Redirect(_url);
            }
            catch (Exception ex)
            {
                this.fncActionLog("LOGIN", "FAILED");

                return Redirect("~/");
            }
        }
        #endregion




        #region Logout with clear session
        public IActionResult Logout()
        {
            try
            {
                this.fncActionLog("Logout", "OK");

                string _url = "~/";
                if (HttpContext.Session.GetString("HINO").ToString() == "YES") _url = "~/" + this.Systems + "/Hino";


                Response.Cookies.Delete("Avatar");

                HttpContext.Session.Clear();
                return Redirect(_url);
            }
            catch (Exception ex)
            {
                this.fncActionLog("Logout", "FAILED");
                return Redirect("~/");
            }

        }
        #endregion



        #region For recovery password
        public IActionResult Recovery()
        {
            string _assets = (ControllerContext.HttpContext.Request.GetDisplayUrl().ToString().IndexOf("localhost:") > 0 ?
                _config.GetValue<string>("Application:Assets") :
                _config.GetValue<string>("Application:Assets_Production"));
            string _storage = (ControllerContext.HttpContext.Request.GetDisplayUrl().ToString().IndexOf("localhost:") > 0 ?
                _config.GetValue<string>("Application:Storage") :
                _config.GetValue<string>("Application:Storage_Production"));

            ViewData["AppName"] = "Recovery Password";
            ViewData["Assets"] = _assets;
            ViewData["Storage"] = _storage;
            ViewData["Title"] = _config.GetValue<string>("Application:Title");
            ViewData["Logo"] = _assets + _config.GetValue<string>("Application:Logo:Icon");
            ViewData["LogoImage"] = _assets + _config.GetValue<string>("Application:Logo:Image");
            ViewData["ErrorText"] = "";


            _SQL = @"
                SELECT *
                FROM [erp].[User]
                WHERE [ResetToken] = '" + Request.Query["ref"].ToString() + @"' 
            ";
            DataTable _dataTable = _erpConnect.ExecuteSQL(_SQL, skipLog: true);

            if (_dataTable.Rows.Count > 0)
            {
                ViewData["ref"] = Request.Query["ref"].ToString();
                ViewData["txtUserName"] = _dataTable.Rows[0]["Code"].ToString();
            }
            else
            {
                ViewData["ErrorText"] = "Recovery code is expired.";
            }


            return View();
        }
        #endregion











        #region Private Function
        private void fncCheckEnvironment(string pUserName = "")
        {
            string _assets = (Request.GetDisplayUrl().ToString().IndexOf("localhost:") > 0 ?
                _config.GetValue<string>(this.SystemCode + ":Assets") :
                _config.GetValue<string>(this.SystemCode + ":Assets_Production"));
            string _storage = (Request.GetDisplayUrl().ToString().IndexOf("localhost:") > 0 ?
                _config.GetValue<string>(this.SystemCode + ":Storage") :
                _config.GetValue<string>(this.SystemCode + ":Storage_Production"));

            ViewData["_DEV_"] = (Request.GetDisplayUrl().ToString().IndexOf("localhost:") > 0 ? true : false);

            ViewData["Title"] = _config.GetValue<string>(this.SystemCode + ":Title").ToUpper();
            ViewData["Logo"] = _assets + _config.GetValue<string>(this.SystemCode + ":Logo:Icon");
            ViewData["LogoImage"] = _assets + _config.GetValue<string>(this.SystemCode + ":Logo:Image");
            ViewData["txtUserName"] = pUserName;
            ViewData["AppName"] = "Login";
            ViewData["Version"] = DateTime.Now.ToString("yyyyMMddhhmmss");
        }

        private void fncActionLog(string Action = "NONE", string Result = "NONE", string _SQL = null, string Message = "", string User = null, string Token = null, string DeviceName = null, string IPAddress = null)
        {
            string _user = null;
            string _token = null;
            string _SQL_Log = null;

            try
            {

                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("TOKEN")))
                {
                    _user = HttpContext.Session.GetString("USER_CODE").ToString();
                    _token = HttpContext.Session.GetString("TOKEN").ToString();
                }
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("USER_DEVICENAME")))
                {
                    _SQL_Log = @"INSERT INTO [log].[Action] ([UserCode]
                                  ,[Token]
                                  ,[DeviceName]
                                  ,[IPAddress]
                                  ,[ActionType]
                                  ,[ActionAt]
                                  ,[SystemName]
                                  ,[ControllerName]
                                  ,[ActionName]
                                  ,[Result]
                                  ,[Message]
                                  ,[SQL]
                                )VALUES('" + _user + @"'
                                  , '" + _token + @"'
                                  , '" + HttpContext.Session.GetString("USER_DEVICENAME").ToString() + @"'
                                  , '" + HttpContext.Session.GetString("USER_IPADDRESS").ToString() + @"'
                                  , '" + Action.ToUpper() + @"'
                                  , GETDATE()
                                  , '" + this.Systems + @"'
                                  , 'Login'
                                  , '" + (Action == "LOGIN" ? "Index" : Action) + @"'
                                  , '" + Result + @"'
                                  , '" + Message + @"'
                                  , '" + (_SQL == null ? "" : _SQL.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").ToString().Trim()) + @"'
                                  
                                )";
                    _erpConnect.Execute(_SQL_Log, skipLog: true);

                }


                if (Action == "LOGIN")
                {
                    _SQL_Log = @"INSERT INTO [dbo].[T_SQL_License] (
                                     [F_System_Name]
                                    ,[F_UserID]
                                    ,[F_Host_Client]
                                    ,[F_IPAddress]
                                    ,[F_Update]
                                )VALUES(
                                    'HINO KANBAN Factory 3'
                                  , '" + HttpContext.Session.GetString("USER_CODE").ToString() + @"'
                                  , '" + HttpContext.Session.GetString("USER_DEVICENAME").ToString() + @"'
                                  , '" + HttpContext.Session.GetString("USER_IPADDRESS").ToString() + @"'
                                  , GETDATE()
                                )";
                    _erpConnect.Execute(_SQL_Log, skipLog: true);
                }
            }
            catch (Exception ex)
            {
                _SQL_Log = @"INSERT INTO [log].[Action] ([UserCode]
                                  ,[Token]
                                  ,[ActionType]
                                  ,[ActionAt]
                                  ,[SystemName]
                                  ,[ControllerName]
                                  ,[ActionName]
                                  ,[Result]
                                  ,[Message]
                                  ,[SQL]
                                )VALUES('" + User + @"'
                                  , '" + Token + @"'
                                  , '" + Action + @"'
                                  , GETDATE()
                                  , '" + this.Systems + @"'
                                  , '" + DeviceName + @"'
                                  , '" + IPAddress + @"'
                                  , '" + Result + @"'
                                  , '" + Message + @"'
                                  , '" + (_SQL == null ? "" : _SQL.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").ToString().Trim()) + @"'
                                )";
                _erpConnect.Execute(_SQL_Log, skipLog: true);


                PrincipalContext _pc = new PrincipalContext(ContextType.Domain, "THI_DM_1");
                UserPrincipal user = UserPrincipal.FindByIdentity(_pc, User);

                string d_code = user.SamAccountName;
                string d_name = user.GivenName;
                string d_surname = user.Surname;
                string d_displayname = user.DisplayName;
                string d_email = user.EmailAddress;

                _SQL_Log = @"SELECT Code FROM [erp].[User] WHERE 1=1 AND Code = '" + User + "' ";

                DataTable _dtNewUser = _erpConnect.ExecuteSQL(_SQL_Log, skipLog: true);

                if (_dtNewUser.Rows.Count <= 0)
                {
                    _SQL_Log = @"INSERT INTO [erp].[User] (
                                    [Code]
                                    ,[Name]
                                    ,[Surname]
                                    ,[NameTH]
                                    ,[SurnameTH]
                                    ,[NameJP]
                                    ,[SurnameJP]
                                    ,[Email]
                                    ,[UILanguage]
                                    ,[UITheme]
                                    ,[LastLogin]
                                    ,[Status]
                                    ,[CreateAt]
                                    ,[CreateBy]
                                )VALUES('" + User + @"'
                                  , '" + user.GivenName + @"'
                                  , '" + user.Surname + @"'
                                  , '" + user.GivenName + @"'
                                  , '" + user.Surname + @"'
                                  , '" + user.GivenName + @"'
                                  , '" + user.Surname + @"'
                                  , '" + user.EmailAddress + @"'
                                  , 'EN'
                                  , 'DEFAULT'
                                  , GETDATE()
                                  , 'REGISTER'
                                  , GETDATE()
                                  , 'system'
                                )";
                    _erpConnect.Execute(_SQL_Log, skipLog: true);
                }


                ViewData["txtUserName"] = "";
                ViewData["txtProcessDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                ViewData["ddlShift"] = (int.Parse(DateTime.Now.ToString("Hmm")) < 1930 ? "D" : "N");
                ViewData["ddlFactory"] = 3;
            }
            finally
            {
            }

        }

        #endregion



    }
}
