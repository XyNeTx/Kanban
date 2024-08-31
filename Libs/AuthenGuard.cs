using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;


namespace HINOSystem.Libs
{
    public class AuthenGuard : Controller
    {
        private readonly IConfiguration _config;
        private readonly ERPConnection _erpConnect;
        private readonly BearerClass _BearerClass;
        //private readonly MyExtensions _myExtensions;
        private readonly CookieClass _cookie;

        private string _DB = "";

        public string ComponentItem = "";

        public Boolean ComponentToolbar = true;
        public Boolean ComponentToolbarSearch = true;
        public Boolean ComponentToolbarNew = true;
        public Boolean ComponentToolbarSave = true;
        public Boolean ComponentToolbarDelete = true;
        public Boolean ComponentToolbarPrint = true;
        public Boolean ComponentToolbarExecute = true;
        public Boolean ComponentToolbarExport = true;
        public string ComponentToolbarSearchText = "";
        public string ComponentToolbarNewText = "";
        public string ComponentToolbarSaveText = "";
        public string ComponentToolbarDeleteText = "";
        public string ComponentToolbarPrintText = "";
        public string ComponentToolbarExecuteText = "";
        public string ComponentToolbarExportText = "";

        public string _MENU_ = @"";
        public string _hostname_prod = @"";
        public string _MENUFOCUS_ = @"";

        public AuthenGuard(
            IConfiguration configuration, 
            ERPConnection erpConnect,
            BearerClass bearerClass,
            CookieClass cookieClass
            )
        {
            _config = configuration;
            _erpConnect = erpConnect;
            _BearerClass = bearerClass;
            _cookie = cookieClass; 

            this._DB = _config.GetValue<string>("Application:Database");
        }

        public void initial()
        {
            this.ComponentToolbarSearch = false;
            this.ComponentToolbarNew = false;
            this.ComponentToolbarSave = false;
            this.ComponentToolbarDelete = false;
            this.ComponentToolbarPrint = false;
            this.ComponentToolbarExecute = false;
            this.ComponentToolbarExport = false;
        }

        public IActionResult guard(ControllerContext _context, 
            string redirectView = "",
            string pViewPath = "",
            string pViewPage = "")
        {
            this.initial();

            var _isLogin = _context.HttpContext.Session.GetString("TOKEN");
            var _systems = _context.HttpContext.Session.GetString("SYSTEM");

            if (redirectView != "") return View(redirectView);


            if (string.IsNullOrEmpty(_isLogin))
            {
                return Redirect("~/Login");
            }

            _BearerClass.UserCode = _context.HttpContext.Session.GetString("USER_CODE");
            _BearerClass.Token = _context.HttpContext.Session.GetString("TOKEN");
            _BearerClass.Device = _context.HttpContext.Session.GetString("USER_DEVICENAME");
            _BearerClass.IPAddress = _context.HttpContext.Session.GetString("USER_IPADDRESS");

            string _assets = (_context.HttpContext.Request.GetDisplayUrl().ToString().IndexOf("localhost:") > 0 ?
                _config.GetValue<string>("Systems:"+ _systems + ":Assets") :
                _config.GetValue<string>("Systems:"+ _systems + ":Assets_Production"));
            string _storage = (_context.HttpContext.Request.GetDisplayUrl().ToString().IndexOf("localhost:") > 0 ?
                _config.GetValue<string>("Systems:" + _systems + ":Storage") :
                _config.GetValue<string>("Systems:" + _systems + ":Storage_Production"));
            _hostname_prod = (_context.HttpContext.Request.GetDisplayUrl().ToString().IndexOf("localhost:") > 0 ?
                _config.GetValue<string>("Systems:" + _systems + ":Path") :
                _config.GetValue<string>("Systems:" + _systems + ":Path_Production"));

            string _system = _config.GetValue<string>("Systems:"+ _systems + ":System");
            string _name = _config.GetValue<string>("Systems:"+ _systems + ":Name");
            string _title = _config.GetValue<string>("Systems:"+ _systems + ":Title");
            string _logo = _assets + _config.GetValue<string>("Systems:"+ _systems + ":Logo:Icon");
            string _image = _assets + _config.GetValue<string>("Systems:"+ _systems + ":Logo:Image");
            string _controller = _context.RouteData.Values["controller"].ToString();
            string _acton = _context.RouteData.Values["action"].ToString();
            string _appname = "Home";
            string _parent_id = "";

            string _SQL = "EXEC [erp].[AuthenGuardLoadPage] '" + _system + @"', '" + _controller + @"',  '" + _acton + @"' ";
            DataTable _dataTable = _erpConnect.ExecuteSQL(_SQL, _context.HttpContext, pAction: "LOAD", pUser: _BearerClass, pControllerName: _controller, pActionName: _acton, pSystem: _system);

            if (_dataTable.Rows.Count > 0)
            {
                _appname = _dataTable.Rows[0]["Code"].ToString() + " : " + _dataTable.Rows[0]["Name" + _context.HttpContext.Session.GetString("USER_UILANGUAGE").ToString()].ToString();
                _parent_id = _dataTable.Rows[0]["mpID"].ToString();
                _MENUFOCUS_ = _dataTable.Rows[0]["pAction"].ToString();
                //## get permission
                _SQL = "EXEC [erp].[AuthenGuardLoadPagePermission] '" + _system + @"', '" + _context.HttpContext.Session.GetString("USER_GROUP_ID").ToString() + @"',  '" + _dataTable.Rows[0]["mID"].ToString() + @"' ";
                DataTable _dtPermission = _erpConnect.ExecuteSQL(_SQL, _context.HttpContext, skipLog: true);
                if (_dtPermission.Rows.Count > 0)
                {
                    if(this.ComponentToolbar) this.ComponentToolbar = (_dtPermission.Rows[0]["Toolbar"].ToString() == "1" ? true : false);
                    this.ComponentToolbarSearch = (_dtPermission.Rows[0]["ToolbarSearch"].ToString() == "1" ? true : false);
                    this.ComponentToolbarNew = (_dtPermission.Rows[0]["ToolbarNew"].ToString() == "1" ? true : false);
                    this.ComponentToolbarSave = (_dtPermission.Rows[0]["ToolbarSave"].ToString() == "1" ? true : false);
                    this.ComponentToolbarDelete = (_dtPermission.Rows[0]["ToolbarDelete"].ToString() == "1" ? true : false);
                    this.ComponentToolbarPrint = (_dtPermission.Rows[0]["ToolbarPrint"].ToString() == "1" ? true : false);
                    this.ComponentToolbarExecute = (_dtPermission.Rows[0]["ToolbarExecute"].ToString() == "1" ? true : false);
                    this.ComponentToolbarExport = (_dtPermission.Rows[0]["ToolbarExport"].ToString() == "1" ? true : false);
                    this.ComponentToolbarSearchText = _dtPermission.Rows[0]["ToolbarSearchText"].ToString();
                    this.ComponentToolbarNewText = _dtPermission.Rows[0]["ToolbarNewText"].ToString();
                    this.ComponentToolbarSaveText = _dtPermission.Rows[0]["ToolbarSaveText"].ToString();
                    this.ComponentToolbarDeleteText = _dtPermission.Rows[0]["ToolbarDeleteText"].ToString();
                    this.ComponentToolbarPrintText = _dtPermission.Rows[0]["ToolbarPrintText"].ToString();
                    this.ComponentToolbarExecuteText = _dtPermission.Rows[0]["ToolbarExecuteText"].ToString();
                    this.ComponentToolbarExportText = _dtPermission.Rows[0]["ToolbarExportText"].ToString();
                }

            }


            //string _version = @"KANBAN.dll";
            //if (System.IO.File.Exists(_version))
            //{
            //    DateTime _lastModified = System.IO.File.GetLastWriteTime(_version);
            //    _version = _lastModified.ToString("yy.MM.ddmmss");
            //}
            //else
            //{
            //    _version = DateTime.Now.ToString("yy.MM.ddmmss");
            //}
            string _version = _BearerClass.versions();

            ViewData["Version"] = _version;
            ViewData["VersionJS"] = DateTime.Now.ToString("yyyyMMddhhmmss");
            ViewData["System"] = _system;
            ViewData["Controller"] = _controller;
            ViewData["Action"] = _acton;
            ViewData["Name"] = _name;
            ViewData["Title"] = _title.ToString().ToUpper();
            ViewData["Assets"] = _assets;
            ViewData["Storage"] = _storage;
            ViewData["Logo"] = (_controller == "Home" && _controller == "Index" ? _logo : _logo);
            ViewData["Image"] = (_controller == "Home" && _controller == "Index" ? _image : _image);
            ViewData["AppName"] = _appname;

            ViewData["Bearer"] = _context.HttpContext.Session.GetString("TOKEN").ToString();
            ViewData["UserDisplay"] = _context.HttpContext.Session.GetString("USER_DISPLAY").ToString();
            ViewData["UserID"] = _context.HttpContext.Session.GetString("USER_ID").ToString();
            ViewData["UserCode"] = _context.HttpContext.Session.GetString("USER_CODE").ToString();
            ViewData["UserName"] = _context.HttpContext.Session.GetString("USER_NAME").ToString();
            ViewData["UserSurname"] = _context.HttpContext.Session.GetString("USER_SURNAME").ToString();
            ViewData["UserNameTH"] = _context.HttpContext.Session.GetString("USER_NAMETH").ToString();
            ViewData["UserSurnameTH"] = _context.HttpContext.Session.GetString("USER_SURNAMETH").ToString();
            ViewData["UserEmail"] = _context.HttpContext.Session.GetString("USER_EMAIL").ToString();
            ViewData["UserLanguage"] = (_context.HttpContext.Session.GetString("USER_UILANGUAGE").ToString() != "" ? _context.HttpContext.Session.GetString("USER_UILANGUAGE").ToString() : "EN");

            ViewData["UserThemeExpandIcon"] = _context.HttpContext.Session.GetString("USER_UIEXPANDICON").ToString();
            ViewData["UserThemeHeader"] = _context.HttpContext.Session.GetString("USER_UIHEADER").ToString();
            ViewData["UserThemeBrand"] = _context.HttpContext.Session.GetString("USER_UIHEADERBRAND").ToString();
            ViewData["UserThemeIconColor"] = _context.HttpContext.Session.GetString("USER_UIICONCOLOR").ToString();
            ViewData["UserTheme"] = _context.HttpContext.Session.GetString("USER_UITHEME").ToString();
            ViewData["UserThemeLinkColor"] = _context.HttpContext.Session.GetString("USER_UILINKCOLOR").ToString();
            ViewData["UserThemeMenuColor"] = _context.HttpContext.Session.GetString("USER_UIMENUCOLOR").ToString();
            ViewData["UserThemeMenuIcon"] = _context.HttpContext.Session.GetString("USER_UIMENUICON").ToString();
            ViewData["UserThemeSideBar"] = _context.HttpContext.Session.GetString("USER_UISIDEBAR").ToString();

            string _sessionAvatar = (_context.HttpContext.Session.GetString("USER_AVATAR").ToString() != "" ? _context.HttpContext.Session.GetString("USER_AVATAR").ToString() : "hino/" + _context.HttpContext.Session.GetString("USER_CODE").ToString() + ".jpg");
            string _cookieAvatar = (_context.HttpContext.Request.Cookies["Avatar"] != null ? _context.HttpContext.Request.Cookies["Avatar"].ToString() : "");
            ViewData["UserAvatar"] = (_cookieAvatar == "" ? _sessionAvatar : (_cookieAvatar != _sessionAvatar ? _cookieAvatar : _sessionAvatar));


            ViewData["UserGroupID"] = _context.HttpContext.Session.GetString("USER_GROUP_ID").ToString();
            ViewData["UserGroupName"] = _context.HttpContext.Session.GetString("USER_GROUP_NAME").ToString();
            ViewData["UserGroupNameTH"] = _context.HttpContext.Session.GetString("USER_GROUP_NAMETH").ToString();

            //### Display component on layout page ###
            //# Item on NAV Bar
            ViewData["ComponentItem"] = (this.ComponentItem != "" ? this.ComponentItem : "");

            //# Item on ToolBar
            ViewData["ComponentToolbar"] = (this.ComponentToolbar ? "true" : "false");
            ViewData["ComponentToolbarSearch"] = (this.ComponentToolbarSearch ? "true" : "false");
            ViewData["ComponentToolbarNew"] = (this.ComponentToolbarNew ? "true" : "false");
            ViewData["ComponentToolbarSave"] = (this.ComponentToolbarSave ? "true" : "false");
            ViewData["ComponentToolbarDelete"] = (this.ComponentToolbarDelete ? "true" : "false");
            ViewData["ComponentToolbarPrint"] = (this.ComponentToolbarPrint ? "true" : "false");
            ViewData["ComponentToolbarExecute"] = (this.ComponentToolbarExecute ? "true" : "false");
            ViewData["ComponentToolbarExport"] = (this.ComponentToolbarExport ? "true" : "false");
            ViewData["ComponentToolbarSearchText"] = this.ComponentToolbarSearchText;
            ViewData["ComponentToolbarNewText"] = this.ComponentToolbarNewText;
            ViewData["ComponentToolbarSaveText"] = this.ComponentToolbarSaveText;
            ViewData["ComponentToolbarDeleteText"] = this.ComponentToolbarDeleteText;
            ViewData["ComponentToolbarPrintText"] = this.ComponentToolbarPrintText;
            ViewData["ComponentToolbarExecuteText"] = this.ComponentToolbarExecuteText;
            ViewData["ComponentToolbarExportText"] = this.ComponentToolbarExportText;


            //### Information Section ###
            //# Log-On
            ViewData["_LogOnID"] = _context.HttpContext.Session.GetString("USER_CODE").ToString();
            ViewData["_ProcessDate"] = _context.HttpContext.Session.GetString("USER_PROCESSDATE").ToString();
            ViewData["_Plant"] = _context.HttpContext.Session.GetString("USER_PLANT").ToString();
            ViewData["_ProcessShift"] = _context.HttpContext.Session.GetString("USER_SHIFT").ToString();
            ViewData["_ShiftTitle"] = (_context.HttpContext.Session.GetString("USER_SHIFT").ToString() =="1" ? "1 - Day Shift" : "2 - Night Shift");
            //# Session
            ViewData["_Domain"] = _context.HttpContext.Session.GetString("USER_DOMAIN").ToString();
            ViewData["_Account"] = (_context.HttpContext.Session.GetString("USER_DOMAIN").ToString() != "" ? _context.HttpContext.Session.GetString("USER_DOMAIN").ToString() + @"\" : "") + _context.HttpContext.Session.GetString("USER_CODE").ToString();
            ViewData["_Device"] = _context.HttpContext.Session.GetString("USER_DEVICENAME").ToString();
            ViewData["_FullDeviceName"] = _context.HttpContext.Session.GetString("USER_FULLDEVICENAME").ToString();
            ViewData["_IPAddress"] = _context.HttpContext.Session.GetString("USER_IPADDRESS").ToString();
            ViewData["_IsHINO"] = _context.HttpContext.Session.GetString("HINO").ToString();
            //# Profile 
            ViewData["_UserID"] = _context.HttpContext.Session.GetString("USER_ID").ToString();
            ViewData["_UserCode"] = _context.HttpContext.Session.GetString("USER_CODE").ToString();
            ViewData["_DisplayName"] = _context.HttpContext.Session.GetString("USER_DISPLAY").ToString();
            ViewData["_Email"] = _context.HttpContext.Session.GetString("USER_EMAIL").ToString();
            ViewData["_UILanguage"] = (_context.HttpContext.Session.GetString("USER_UILANGUAGE").ToString()!="" ? _context.HttpContext.Session.GetString("USER_UILANGUAGE").ToString() : "EN");
            ViewData["_System"] = _context.HttpContext.Session.GetString("SYSTEM").ToString();
            ViewData["_Token"] = _context.HttpContext.Session.GetString("TOKEN").ToString();

            ViewData["ComputerNameSQL"] = _erpConnect.ExecuteJSON("select Top(1)  HOST_NAME() from [New_Kanban_F3].[erp].[User]", skipLog: true);



            ////_context.HttpContext.Request.Cookies["Avatar"] = ViewData["UserCode"];
            //_cookie.SetCookie(_context, "_CONTROLLER_", _controller);
            //_cookie.SetCookie(_context, "_PAGE_", _acton);
            //_cookie.SetCookie(_context, "_APPNAME_", _appname);
            //_cookie.SetCookie(_context, "_SYSTEM_", _system);
            //_cookie.SetCookie(_context, "_LANGUAGE_", (_context.HttpContext.Session.GetString("USER_UILANGUAGE").ToString() != "" ? _context.HttpContext.Session.GetString("USER_UILANGUAGE").ToString() : "EN"));
            //_cookie.SetCookie(_context, "_COMPONENT_TOOLBAR_SEARCH_", (this.ComponentToolbarSearch ? "true" : "false"));
            //_cookie.SetCookie(_context, "_COMPONENT_TOOLBAR_NEW_", (this.ComponentToolbarNew ? "true" : "false"));
            //_cookie.SetCookie(_context, "_COMPONENT_TOOLBAR_SAVE_", (this.ComponentToolbarSave ? "true" : "false"));
            //_cookie.SetCookie(_context, "_COMPONENT_TOOLBAR_DELETE_", (this.ComponentToolbarDelete ? "true" : "false"));
            //_cookie.SetCookie(_context, "_COMPONENT_TOOLBAR_PRINT_", (this.ComponentToolbarPrint ? "true" : "false"));
            //_cookie.SetCookie(_context, "_COMPONENT_TOOLBAR_EXECUTE_", (this.ComponentToolbarExecute ? "true" : "false"));
            //_cookie.SetCookie(_context, "_UID_", _context.HttpContext.Session.GetString("USER_CODE").ToString());
            //_cookie.SetCookie(_context, "_DISPLAY_", _context.HttpContext.Session.GetString("USER_DISPLAY").ToString());
            //_cookie.SetCookie(_context, "_ASSET_", _assets);
            //_cookie.SetCookie(_context, "_STORAGE_", _storage);
            //_cookie.SetCookie(_context, "_BearerClass_", _context.HttpContext.Session.GetString("TOKEN").ToString());



            string clientIpAddress = _context.HttpContext.Connection.LocalIpAddress.ToString();
            string hostName = Dns.GetHostEntry(IPAddress.Parse(clientIpAddress)).HostName;

            ViewData["ComputerName"] = hostName;
            

            if (!string.IsNullOrEmpty(Environment.UserName)) ViewData["Environment"] = Environment.UserName.ToString();
            if (!string.IsNullOrEmpty(System.Security.Principal.WindowsIdentity.GetCurrent().Name)) ViewData["WindowsIdentity"] = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();


            //_SQL = "EXEC [erp].[AuthenGuardLoadMenu] '" + _system + "' ";
            //if (ViewData["UserGroupID"].ToString() != "1") _SQL = @"EXEC [erp].[AuthenGuardLoadMenuByID] '" + _system + "', '"+ ViewData["UserCode"].ToString() + "'";

            //DataTable _dtMenuDisplay = _erpConnect.ExecuteSQL(_SQL, skipLog: true);

            //            string _sql = @"
            //    SELECT m._ID
            //        , m.Code
            //        , m.Name
            //        , m.NameTH
            //        , m.NameJP
            //        , m.Title
            //        , m.TitleTH
            //        , m.TitleJP
            //        , m.Icon
            //        , m.i18n
            //        , m.Status
            //        , m.UpdateAt
            //        , m.UpdateBy
            //        , m.CreateAt
            //        , m.CreateBy
            //        , m.isDelete 
            //	    , mp._ID AS MPID
            //	    , mp.Parent_ID
            //	    , mp.Controller
            //	    , mp.Action
            //	    , mp.ViewType
            //    FROM erp.MenuParent mp 
            //	    INNER JOIN erp.Menu m ON m._ID=Menu_ID
            //	    INNER JOIN erp.GroupMenu gm ON gm.Menu_ID=m._ID
            //    WHERE 1=1
            //    AND (mp.Parent_ID IS NULL OR mp.Parent_ID=0)
            //    AND mp.isDelete=0 " + (int.Parse(ViewData["UserGroupID"].ToString()) > 2 ? "AND mp.Status='ACTIVE' " : "") + @"
            //    AND m.isDelete=0 " + (int.Parse(ViewData["UserGroupID"].ToString()) > 2 ? "AND m.Status='ACTIVE' " : "") + @"
            //    AND gm.isDelete=0 " + (int.Parse(ViewData["UserGroupID"].ToString()) > 2 ? "AND gm.Status='ACTIVE' " : "") + @"
            //    " + (int.Parse(ViewData["UserGroupID"].ToString()) > 2 ? "AND gm.Group_ID=" + ViewData["UserGroupID"].ToString() : "") + @"
            //	GROUP BY m._ID
            //        , m.Code
            //        , m.Name
            //        , m.NameTH
            //        , m.NameJP
            //        , m.Title
            //        , m.TitleTH
            //        , m.TitleJP
            //        , m.Icon
            //        , m.i18n
            //        , m.Status
            //        , m.UpdateAt
            //        , m.UpdateBy
            //        , m.CreateAt
            //        , m.CreateBy
            //        , m.isDelete
            //	    , mp._ID 
            //	    , mp.Parent_ID
            //	    , mp.Controller
            //	    , mp.Action
            //	    , mp.ViewType
            //		, mp.Seq
            //    ORDER BY Seq
            //";

            //            string _sql = @"
            //    SELECT m._ID
            //        , m.Code
            //        , m.Name
            //        , m.NameTH
            //        , m.NameJP
            //        , m.Title
            //        , m.TitleTH
            //        , m.TitleJP
            //        , m.Icon
            //        , m.i18n
            //        , m.Status
            //        , m.UpdateAt
            //        , m.UpdateBy
            //        , m.CreateAt
            //        , m.CreateBy
            //        , m.isDelete 
            //	    , mp._ID AS MPID
            //	    , mp.Parent_ID
            //	    , mp.Controller
            //	    , mp.Action
            //	    , mp.ViewType
            //    FROM [erp].[UserAuthorize] ua
            //	    INNER JOIN [erp].[User] u ON u._ID=ua.User_ID
            //        INNER JOIN [erp].[Menu] m ON m._ID = ua.Menu_ID
            //        INNER JOIN [erp].[MenuParent] mp ON mp.Menu_ID = m._ID

            //    WHERE 1=1
            //    AND (mp.Parent_ID IS NULL OR mp.Parent_ID=0)
            //    AND mp.isDelete=0 " + (int.Parse(ViewData["UserGroupID"].ToString()) > 2 ? "AND mp.Status='ACTIVE' " : "") + @"
            //    AND m.isDelete=0 " + (int.Parse(ViewData["UserGroupID"].ToString()) > 2 ? "AND m.Status='ACTIVE' " : "") + @"
            //    " + (int.Parse(ViewData["UserGroupID"].ToString()) > 2 ? "AND u.Code='" + _context.HttpContext.Session.GetString("USER_CODE").ToString() + "' " : "") + @"
            //	GROUP BY m._ID
            //        , m.Code
            //        , m.Name
            //        , m.NameTH
            //        , m.NameJP
            //        , m.Title
            //        , m.TitleTH
            //        , m.TitleJP
            //        , m.Icon
            //        , m.i18n
            //        , m.Status
            //        , m.UpdateAt
            //        , m.UpdateBy
            //        , m.CreateAt
            //        , m.CreateBy
            //        , m.isDelete
            //	    , mp._ID 
            //	    , mp.Parent_ID
            //	    , mp.Controller
            //	    , mp.Action
            //	    , mp.ViewType
            //		, mp.Seq
            //    ORDER BY mp.Seq
            //";

            string _sql = @"
    SELECT m._ID
        , m.Code
        , m.Name
        , m.NameTH
        , m.NameJP
        , m.Title
        , m.TitleTH
        , m.TitleJP
        , m.Icon
        , m.i18n
        , m.Status
        , m.UpdateAt
        , m.UpdateBy
        , m.CreateAt
        , m.CreateBy
        , m.isDelete 
	    , mp._ID AS MPID
	    , mp.Parent_ID
	    , mp.Controller
	    , mp.Action
	    , mp.ViewType
    FROM [erp].[Menu] m
        INNER JOIN [erp].[MenuParent] mp ON mp.Menu_ID = m._ID
	    INNER " + @" JOIN (
				    SELECT
					    mp.*
				    FROM
					    [erp].[UserAuthorize] ua
					    INNER JOIN [erp].[User] u ON u._ID = ua.User_ID
					    INNER JOIN [erp].[Menu] m ON m._ID = ua.Menu_ID
					    INNER JOIN [erp].[MenuParent] mp ON mp.Menu_ID = m._ID
				    WHERE
					    1 = 1
					    AND (
						    mp.Parent_ID IS NOT NULL
						    OR mp.Parent_ID <> 0
					    )
					    AND u.Code='" + _context.HttpContext.Session.GetString("USER_CODE").ToString() + "' " + @"

				    ) ua ON Ua.Parent_ID=m._ID

    WHERE 1=1
    AND (mp.Parent_ID IS NULL OR mp.Parent_ID=0)
    AND mp.isDelete=0 AND mp.Status='ACTIVE' " + @"
    AND m.isDelete=0 AND m.Status='ACTIVE' " + @"
	GROUP BY m._ID
        , m.Code
        , m.Name
        , m.NameTH
        , m.NameJP
        , m.Title
        , m.TitleTH
        , m.TitleJP
        , m.Icon
        , m.i18n
        , m.Status
        , m.UpdateAt
        , m.UpdateBy
        , m.CreateAt
        , m.CreateBy
        , m.isDelete
	    , mp._ID 
	    , mp.Parent_ID
	    , mp.Controller
	    , mp.Action
	    , mp.ViewType
		, mp.Seq
    ORDER BY mp.Seq
";

            DataTable _dtMenu = _erpConnect.ExecuteSQL(_sql, skipLog: true);
            string _s = @"", _menu="";
            for (int i = 0; i < _dtMenu.Rows.Count; i++)
            {
                string _id = _dtMenu.Rows[i]["_id"].ToString();


                string _ctrl = _dtMenu.Rows[i]["Controller"].ToString();
                string _childM = _getChildren(_id, _controller, _acton, _parent_id, _context.HttpContext.Session.GetString("USER_CODE").ToString());
                string _icon = (_dtMenu.Rows[i]["Icon"].ToString() != "" ? @"<i class=""" +  _dtMenu.Rows[i]["Icon"].ToString() + @"""></i>" : @"<i class=""feather icon-command""></i>");


                _menu += @"
                                    <li class=""pcoded-hasmenu " + (_controller == _ctrl ? "active pcoded-trigger" : "" ) + @""" dropdown-icon=""style2"" subitem-icon=""style2"">
                                        <a href=""javascript:void(0)"">
                                            <span class=""pcoded-micon"">" + _icon + @"</span>
                                            <span class=""pcoded-mtext"">" + _dtMenu.Rows[i]["Name"].ToString() + @"</span>
                                        </a>
                                        "+ _childM + @"
                                    </li>";


//                //Generate menu with json (Not use)
//                string _child = _getChildrenJSON(_id);
//                _s += (_s == "" ? "" : ",") + @"
//{
//    ""id"": " + _dtMenu.Rows[i]["_ID"].ToString() + @",
//    ""text"": """ + _dtMenu.Rows[i]["Name"].ToString() + @""",
//    ""population"": null,
//    ""flagUrl"": null,
//    ""checked"": false,
//    ""hasChildren"": false,
//    ""parent"": true,
//    ""children"": [" + _child + @"]
//}";

            }



            _s = @"[" + _s + @"]";
            string filePath = @"wwwroot\assets\template\Menu\" + ViewData["UserCode"] + ".json";
            System.IO.File.WriteAllText(filePath, _s);              // Write the JSON data to the file


            ViewData["_MENUFOCUS_"] = _MENUFOCUS_;
            ViewData["_MENU_"] = _menu;

            if (_controller.ToUpper() == "REPORTS") return View("/Views/" + _system + "/" + _controller + "/" + _acton + ".cshtml");
            if (pViewPath != "") return View(pViewPath + "/" + _acton);
            if (pViewPage != "") return View("/Views/" + pViewPage);

            return View();
            //if (_controller.ToUpper() == "REPORTS") return View("/Views/" + _system + "/" + _controller + "/" + _acton + ".cshtml", _dtMenuDisplay);
            //if (pViewPath != "") return View(pViewPath + "/" + _acton, _dtMenuDisplay);
            //if (pViewPage != "") return View("/Views/" + pViewPage, _dtMenuDisplay);

            //return View(_dtMenuDisplay);
        }


        public string _getChildren(string _id, string _controllor, string _action, string _mpID, string _usercode)
        {
            string _s = @"";
            //            string _SQL = @"
            //    SELECT m._ID
            //        , m.Code
            //        , m.Name
            //        , m.NameTH
            //        , m.NameJP
            //        , m.Title
            //        , m.TitleTH
            //        , m.TitleJP
            //        , m.Icon
            //        , m.i18n
            //        , m.Status
            //        , m.UpdateAt
            //        , m.UpdateBy
            //        , m.CreateAt
            //        , m.CreateBy
            //        , m.isDelete 
            //	    , mp._ID AS MPID
            //	    , mp.Parent_ID
            //	    , mp.Controller
            //	    , mp.Action
            //	    , mp.ViewType
            //    FROM erp.MenuParent mp 
            //	    INNER JOIN erp.Menu m ON m._ID=Menu_ID
            //	    INNER JOIN erp.GroupMenu gm ON gm.Menu_ID=m._ID
            //    WHERE 1=1
            //    AND mp.Parent_ID=" + _id + @"
            //    AND mp.isDelete=0 " + (int.Parse(ViewData["UserGroupID"].ToString()) > 2 ? "AND mp.Status='ACTIVE' " : "") + @"
            //    AND m.isDelete=0 " + (int.Parse(ViewData["UserGroupID"].ToString()) > 2 ? "AND m.Status='ACTIVE' " : "") + @"
            //    AND gm.isDelete=0 " + (int.Parse(ViewData["UserGroupID"].ToString()) > 2 ? "AND gm.Status='ACTIVE' " : "") + @"
            //    " + (int.Parse(ViewData["UserGroupID"].ToString()) > 2 ? "AND gm.Group_ID=" + ViewData["UserGroupID"].ToString() : "") + @"
            //    GROUP BY m._ID
            //        , m.Code
            //        , m.Name
            //        , m.NameTH
            //        , m.NameJP
            //        , m.Title
            //        , m.TitleTH
            //        , m.TitleJP
            //        , m.Icon
            //        , m.i18n
            //        , m.Status
            //        , m.UpdateAt
            //        , m.UpdateBy
            //        , m.CreateAt
            //        , m.CreateBy
            //        , m.isDelete
            //	    , mp._ID 
            //	    , mp.Parent_ID
            //	    , mp.Controller
            //	    , mp.Action
            //	    , mp.ViewType
            //		, mp.Seq
            //    ORDER BY Seq
            //";

            string _SQL = @"
    SELECT m._ID
        , m.Code
        , m.Name
        , m.NameTH
        , m.NameJP
        , m.Title
        , m.TitleTH
        , m.TitleJP
        , m.Icon
        , m.i18n
        , m.Status
        , m.UpdateAt
        , m.UpdateBy
        , m.CreateAt
        , m.CreateBy
        , m.isDelete 
	    , mp._ID AS MPID
	    , mp.Parent_ID
	    , mp.Controller
	    , mp.Action
	    , mp.ViewType
    FROM [erp].[UserAuthorize] ua
	    INNER JOIN [erp].[User] u ON u._ID=ua.User_ID
        INNER JOIN [erp].[Menu] m ON m._ID = ua.Menu_ID
        INNER JOIN [erp].[MenuParent] mp ON mp.Menu_ID = m._ID
    WHERE 1=1
    AND mp.Parent_ID=" + _id + @"
    AND mp.isDelete=0 AND mp.Status='ACTIVE' " + @"
    AND m.isDelete=0 AND m.Status='ACTIVE' " + @"
    AND u.Code='" + _usercode + "' " + @"
    GROUP BY m._ID
        , m.Code
        , m.Name
        , m.NameTH
        , m.NameJP
        , m.Title
        , m.TitleTH
        , m.TitleJP
        , m.Icon
        , m.i18n
        , m.Status
        , m.UpdateAt
        , m.UpdateBy
        , m.CreateAt
        , m.CreateBy
        , m.isDelete
	    , mp._ID 
	    , mp.Parent_ID
	    , mp.Controller
	    , mp.Action
	    , mp.ViewType
		, mp.Seq
    ORDER BY mp.Seq
";
            DataTable _dtMenu = _erpConnect.ExecuteSQL(_SQL, skipLog: true);
            _s += @"
                                        <ul class=""pcoded-submenu"">";
            if (_dtMenu.Rows.Count > 0)
            {
                for (int i = 0; i < _dtMenu.Rows.Count; i++)
                {
                    string _parent_id = _dtMenu.Rows[i]["Parent_ID"].ToString();
                    string _ctrl = _dtMenu.Rows[i]["Controller"].ToString();
                    string _ac = _dtMenu.Rows[i]["Action"].ToString();
                    string _icon = _dtMenu.Rows[i]["Icon"].ToString();

                    if (_dtMenu.Rows[i]["Code"].ToString() == "")
                    {
                        string _childM = _getChildren(_dtMenu.Rows[i]["_ID"].ToString(), _controllor, _action, _parent_id, _usercode);
                        _s += @"
                                            <li class=""pcoded-hasmenu"" dropdown-icon=""style2"" subitem-icon=""style2"">
                                                <a href=""javascript:void(0)"">
                                                    " + (_icon != ""
                                                            ? @"<i class=""" + _icon + @"""></i>"
                                                            : @"<span class=""pcoded-micon""><i class=""feather icon-command""></i></span>"
                                                        ) + @"
                                                    " + (_icon != ""
                                                            ? _dtMenu.Rows[i]["Name"].ToString()
                                                            : @"<span class=""pcoded-mtext"">" + _dtMenu.Rows[i]["Name"].ToString() + @"</span>"
                                                        ) + @"
                                                </a>
                                                " + _childM + @"
                                            </li>";

                    }
                    else
                    {
                        //if (_ACTION_ != "")
                        //{
                        //    _ac = _ACTION_;
                        //}
                        _s += @"
                                            <li " + (_controllor == _ctrl && _action == _ac ? @"data-focus=""active""" : "") + @" class" + (_controllor == _ctrl && _action == _ac ? @"=""active""" : "") + @" id=""" + _ctrl + _ac + @""" >
                                                <a href="""+ _hostname_prod + "/" + _dtMenu.Rows[i]["Controller"].ToString() + "/" + _dtMenu.Rows[i]["Code"].ToString() + @""">
                                                    " + (_icon != ""
                                                            ? @"<i class=""" + _icon + @"""></i>"
                                                            : @"<span class=""pcoded-micon""><i class=""feather icon-command""></i></span>"
                                                        ) + @"
                                                    " + (_icon != ""
                                                            ? @"<span style=""font-size:0.8rem;"">" + _dtMenu.Rows[i]["Name"].ToString() + @"</span>"
                                                            : @"<span class=""pcoded-mtext"" style=""font-size:0.8rem;"">" + _dtMenu.Rows[i]["Name"].ToString() + @"</span>"
                                                        ) + @"
                                                </a>
                                            </li>";
                    }
                }
            }
            _s += @"
                                        </ul>";
            return _s;
        }



//        public string _getChildrenJSON(string _id)
//        {
//            string _s = @"";
//            string _SQL = @"
//SELECT m.* 
//	, mp._ID AS MPID
//	, mp.Parent_ID
//	, mp.Controller
//	, mp.Action
//	, mp.ViewType
//FROM erp.MenuParent mp 
//	LEFT JOIN erp.Menu m ON m._ID=Menu_ID
//WHERE mp.Parent_ID=" + _id + @"
//AND mp.isDelete = 0
//ORDER BY Seq
//";
//            DataTable _dtMenu = _erpConnect.ExecuteSQL(_SQL, skipLog: true);
//            if (_dtMenu.Rows.Count > 0)
//            {
//                for (int i = 0; i < _dtMenu.Rows.Count; i++)
//                {
//                    string _child = _getChildrenJSON(_dtMenu.Rows[i]["_ID"].ToString());
//                    if (_dtMenu.Rows[i]["Code"].ToString() == "")
//                    {
//                        _s += (_s == "" ? "" : ",") + @"
//{
//    ""id"": " + _dtMenu.Rows[i]["_ID"].ToString() + @",
//    ""text"": """ + _dtMenu.Rows[i]["Name"].ToString() + @""",
//    ""population"": null,
//    ""flagUrl"": null,
//    ""checked"": false,
//    ""hasChildren"": false,
//    ""parent"": true,
//    ""children"": [" + _child + @"]
//}";
//                    }
//                    else
//                    {

//                        _s += (_s == "" ? "" : ",") + @"
//{
//    ""id"": " + _dtMenu.Rows[i]["_ID"].ToString() + @",
//    ""text"": """ + _dtMenu.Rows[i]["Name"].ToString() + @""",
//    ""population"": null,
//    ""flagUrl"": null,
//    ""checked"": false,
//    ""hasChildren"": false,
//    ""parent"": false,
//    ""url"": ""~/kanban/" + _dtMenu.Rows[i]["Controller"].ToString() + "/" + _dtMenu.Rows[i]["Code"].ToString() + @""",
//    ""children"": [" + _child + @"]
//}";
//                    }
//                }

//                return _s;
//            }
//            return "";
//        }


    }
}
