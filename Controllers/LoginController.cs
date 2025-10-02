using HINOSystem.Context;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NuGet.ProjectModel;
using System.Data;
using System.Security.Claims;


namespace HINOSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly ERPConnection _erpConnect;
        private readonly BearerClass _BearerClass;
        private readonly KB3Context _KB3Context;
        private readonly HttpClient _httpClient;


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
            KB3Context kB3Context,
            HttpClient httpClient
            )
        {
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
            _erpConnect = erpConnect;
            _BearerClass = bearerClass;
            _KB3Context = kB3Context;
            _httpClient = httpClient;

            this._DB = _config.GetValue<string>(this.SystemCode + ":Database");
        }


        #region Login page for supplier
        [HttpGet]
        public IActionResult Index()
        {
            this.fncCheckEnvironment();

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

                string _url = "~/Home";

                var postData = new Dictionary<string, object>
                {
                    { "System_Name", "Hino Kanban System" },
                    { "Employee_Code", _txtUserName },
                    { "Computer_Name", _txtDeviceName },
                    { "Ip_Address", _txtIPAddress }
                };

                var _reqUrl = "http://hmmt-app07/sso_test/api/SingleSignOn/LoggedIn";

                var response = await _httpClient.PostAsJsonAsync(_reqUrl, postData);

                if (response.IsSuccessStatusCode)
                {
                    var rawJson = await response.Content.ReadAsStringAsync();

                    var parsedJson = JToken.Parse(rawJson);
                    var prettyJson = parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode} - {error}");
                }

                HttpContext.Session.SetString("SYSTEM", this.Systems);
                HttpContext.Session.SetString("HINO", _txtIsHINO);
                HttpContext.Session.SetString("TOKEN", Request.Headers.Authorization.ToString());
                HttpContext.Session.SetString("USER_ID", User.FindFirst(ClaimTypes.NameIdentifier).Value);
                HttpContext.Session.SetString("USER_CODE", User.FindFirst(ClaimTypes.UserData).Value);
                HttpContext.Session.SetString("USER_NAME", User.FindFirst(ClaimTypes.Name).Value);
                HttpContext.Session.SetString("USER_EMAIL", User.FindFirst(ClaimTypes.Email).Value);
                HttpContext.Session.SetString("USER_DOMAIN", _txtDomain);
                HttpContext.Session.SetString("USER_DEVICENAME", _txtDeviceName);
                HttpContext.Session.SetString("USER_FULLDEVICENAME", _txtFullDeviceName);
                HttpContext.Session.SetString("USER_IPADDRESS", _txtIPAddress);
                HttpContext.Session.SetString("USER_PROCESSDATE", _txtProcessDate);
                HttpContext.Session.SetString("USER_PLANT", _ddlFactory);
                HttpContext.Session.SetString("USER_SHIFT", _ddlShift);

                return Redirect(_url);
            }
            catch (Exception ex)
            {
                return Redirect("~/");
            }
        }
        #endregion




        #region Logout with clear session
        public async Task<IActionResult> Logout()
        {
            try
            {
                //this.fncActionLog("Logout", "OK");

                string _url = "~/";
                Response.Cookies.Delete("Avatar");

                HttpContext.Session.Clear();
                Request.Headers.Clear();
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Redirect(_url);
            }
            catch (Exception ex)
            {
                //this.fncActionLog("Logout", "FAILED");
                return Redirect("~/");
            }

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

        #endregion



    }
}
