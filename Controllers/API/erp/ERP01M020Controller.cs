using Microsoft.AspNetCore.Http;
using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using HINOSystem.Libs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;
using HINOSystem.Context;
using HINOSystem.Controllers.API.wrt;
using HINOSystem.Models.ERP;
using HINOSystem.Models.KB3.Master;

namespace HINOSystem.Controllers.API.erp
{
    public class ERP01M020Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly DefaultConnection _KB3Connect;

        private readonly ERPContext _ERPContext;
        private readonly KB3Context _KB3Context;

        public ERP01M020Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            DefaultConnection defaultConnection,
            ERPContext erpContext,
            KB3Context kb3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KB3Connect = defaultConnection;
            _ERPContext = erpContext;
            _KB3Context = kb3Context;
        }


        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                JObject _JBearer = _BearerClass.Authorization(Request.Headers.Authorization);
                if (_JBearer.GetValue("status").ToString() == "401") return Content(JsonConvert.SerializeObject(_JBearer), "application/json");


                _SQL = @" EXEC [exec].[spERP01M020_SEARCH] '3', '' ";
                string _erpMenu = _KB3Connect.executeSQLJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                _SQL = @" EXEC [exec].[spERP01M011_SEARCH] '3' ";
                string _erpGroup = _KB3Connect.executeSQLJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""erpMenu"" : " + _erpMenu + @",
                                ""erpGroup"" : " + _erpGroup + @"
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
        public IActionResult search([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                JObject _JBearer = _BearerClass.Authorization(Request.Headers.Authorization);
                if (_JBearer.GetValue("status").ToString() == "401") return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                //_json = JsonConvert.DeserializeObject(pData);


                _SQL = @" EXEC [exec].[spERP01M020_SEARCH] '3', '' ";
                string _erpGroup = _KB3Connect.executeSQLJSON(_SQL, pUser: _JBearer, pAction: "READ", pControllerName: ControllerContext.ActionDescriptor.ControllerName.ToString(), pActionName: MethodBase.GetCurrentMethod().Name.ToString());
                //var _erpGroup = _ERPContext.erpGroup.Where(t => t.isDelete != 1);


                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"": " + _erpGroup + @"
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public IActionResult save()
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                var _JBearer = _BearerClass.AuthorizationJSON(Request.Headers.Authorization);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer.Data), "application/json");


                _SQL = @" SELECT MAX(_ID)+1 AS MaxID FROM [erp].[Menu] WHERE 1=1 ";
                DataTable _dtMenu = _KB3Connect.executeSQL(_SQL);

                if (_dtMenu != null)
                {
                    erpMenu _erpMenu = new erpMenu();
                    _erpMenu.Code = Request.Form["Code"].ToString();
                    _erpMenu.Name = Request.Form["Name"].ToString();
                    _erpMenu.NameTH = Request.Form["NameTH"].ToString();
                    _erpMenu.NameJP = Request.Form["NameJP"].ToString();
                    _erpMenu.Title = Request.Form["Title"].ToString();
                    _erpMenu.TitleTH = Request.Form["TitleTH"].ToString();
                    _erpMenu.TitleJP = Request.Form["TitleJP"].ToString();
                    _erpMenu.Icon = Request.Form["Icon"].ToString();
                    _erpMenu.CreateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpMenu.CreateBy = _JBearer.UserCode.ToString();
                    _erpMenu.Status = "ACTIVE";
                    _erpMenu.isDelete = 0;
                    _ERPContext.erpMenu.Add(_erpMenu);
                    _ERPContext.SaveChanges();


                    erpMenuParent _erpMenuParent = new erpMenuParent();
                    _erpMenuParent.System_ID = 4;
                    _erpMenuParent.Menu_ID = int.Parse(_dtMenu.Rows[0]["MaxID"].ToString());
                    _erpMenuParent.Parent_ID = int.Parse(Request.Form["Parent_ID"].ToString());
                    _erpMenuParent.Controller = Request.Form["Controller"].ToString();
                    _erpMenuParent.Action = Request.Form["Action"].ToString();
                    _erpMenuParent.Seq = int.Parse(Request.Form["Seq"].ToString());
                    _erpMenuParent.Remark = Request.Form["Remark"].ToString();
                    _ERPContext.erpMenuParent.Update(_erpMenuParent);
                    _ERPContext.SaveChanges();


                    erpGroupMenu _erpGroupMenuAdd = new erpGroupMenu();
                    _erpGroupMenuAdd.System_ID = 4;
                    _erpGroupMenuAdd.Menu_ID = int.Parse(Request.Form["Menu_ID"].ToString());
                    _erpGroupMenuAdd.Group_ID = int.Parse(Request.Form["Group_ID"].ToString());
                    _erpGroupMenuAdd.Toolbar = int.Parse(Request.Form["Toolbar"].ToString());
                    _erpGroupMenuAdd.ToolbarSearch = int.Parse(Request.Form["ToolbarSearch"].ToString());
                    _erpGroupMenuAdd.ToolbarNew = int.Parse(Request.Form["ToolbarNew"].ToString());
                    _erpGroupMenuAdd.ToolbarSave = int.Parse(Request.Form["ToolbarSave"].ToString());
                    _erpGroupMenuAdd.ToolbarDelete = int.Parse(Request.Form["ToolbarDelete"].ToString());
                    _erpGroupMenuAdd.ToolbarPrint = int.Parse(Request.Form["ToolbarPrint"].ToString());
                    _erpGroupMenuAdd.ToolbarExecute = int.Parse(Request.Form["ToolbarExecute"].ToString());
                    _erpGroupMenuAdd.ToolbarExport = int.Parse(Request.Form["ToolbarExport"].ToString());
                    _erpGroupMenuAdd.ToolbarSearchText = Request.Form["ToolbarSearchText"].ToString();
                    _erpGroupMenuAdd.ToolbarNewText = Request.Form["ToolbarNewText"].ToString();
                    _erpGroupMenuAdd.ToolbarSaveText = Request.Form["ToolbarSaveText"].ToString();
                    _erpGroupMenuAdd.ToolbarDeleteText = Request.Form["ToolbarDeleteText"].ToString();
                    _erpGroupMenuAdd.ToolbarPrintText = Request.Form["ToolbarPrintText"].ToString();
                    _erpGroupMenuAdd.ToolbarExecuteText = Request.Form["ToolbarExecuteText"].ToString();
                    _erpGroupMenuAdd.ToolbarExportText = Request.Form["ToolbarExportText"].ToString();
                    _erpGroupMenuAdd.Remark = Request.Form["Remark"].ToString();
                    _erpGroupMenuAdd.CreateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpGroupMenuAdd.CreateBy = _JBearer.UserCode.ToString();
                    _erpGroupMenuAdd.Status = "ACTIVE";
                    _erpGroupMenuAdd.isDelete = 0;
                    _ERPContext.erpGroupMenu.Add(_erpGroupMenuAdd);
                    _ERPContext.SaveChanges();
                }


                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data has been save""
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPatch] //Update
        public IActionResult Save(int id = 0)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                var _JBearer = _BearerClass.AuthorizationJSON(Request.Headers.Authorization);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer.Data), "application/json");

                var _erpMenu = _ERPContext.erpMenu
                                    .FirstOrDefault(x => x._ID == int.Parse(Request.Form["_ID"].ToString()));
                 if (_erpMenu != null)
                {
                    _erpMenu.Code = Request.Form["Code"].ToString();
                    _erpMenu.Name = Request.Form["Name"].ToString();
                    _erpMenu.NameTH = Request.Form["NameTH"].ToString();
                    _erpMenu.NameJP = Request.Form["NameJP"].ToString();
                    _erpMenu.Title = Request.Form["Title"].ToString();
                    _erpMenu.TitleTH = Request.Form["TitleTH"].ToString();
                    _erpMenu.TitleJP = Request.Form["TitleJP"].ToString();
                    _erpMenu.Icon = Request.Form["Icon"].ToString();
                    _erpMenu.Status = Request.Form["Status"].ToString();
                    _erpMenu.CreateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpMenu.CreateBy = _JBearer.UserCode.ToString();
                    _ERPContext.erpMenu.Update(_erpMenu);
                    _ERPContext.SaveChanges();
                }


                var _erpMenuParent = _ERPContext.erpMenuParent
                                    .FirstOrDefault(x => x.Menu_ID == int.Parse(Request.Form["MenuParent_ID"].ToString()));
                if (_erpMenuParent == null)
                {
                    erpMenuParent _erpMenuParentAdd = new erpMenuParent();
                    _erpMenuParentAdd.System_ID = 4;
                    _erpMenuParentAdd.Menu_ID = int.Parse(Request.Form["_ID"].ToString());
                    _erpMenuParentAdd.Parent_ID = int.Parse(Request.Form["Parent_ID"].ToString());
                    _erpMenuParentAdd.Controller = Request.Form["Controller"].ToString();
                    _erpMenuParentAdd.Action = Request.Form["Action"].ToString();
                    _erpMenuParentAdd.Remark = Request.Form["Parent_ID"].ToString();
                    _erpMenuParentAdd.CreateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpMenuParentAdd.CreateBy = _JBearer.UserCode.ToString();
                    _erpMenuParentAdd.Status = "ACTIVE";
                    _erpMenuParentAdd.isDelete = 0;
                    _ERPContext.erpMenuParent.Add(_erpMenuParentAdd);
                    _ERPContext.SaveChanges();
                }
                else
                {
                    _erpMenuParent.System_ID = 4;
                    _erpMenuParent.Menu_ID = int.Parse(Request.Form["_ID"].ToString());
                    _erpMenuParent.Parent_ID = (Request.Form["Parent_ID"].ToString()!="" ? int.Parse(Request.Form["Parent_ID"].ToString()) : 0);
                    _erpMenuParent.Controller = Request.Form["Controller"].ToString();
                    _erpMenuParent.Action = Request.Form["Action"].ToString();
                    _erpMenuParent.Remark = Request.Form["Remark"].ToString();
                    _erpMenuParent.UpdateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpMenuParent.UpdateBy = _JBearer.UserCode.ToString();
                    _erpMenuParent.Status = "ACTIVE";
                    _erpMenuParent.isDelete = 0;
                    _ERPContext.erpMenuParent.Update(_erpMenuParent);
                    _ERPContext.SaveChanges();
                }




                var _erpGroupMenu = _ERPContext.erpGroupMenu
                                    .FirstOrDefault(x => x._ID == int.Parse(Request.Form["GroupMenu_ID"].ToString()));
                //var _erpGroupMenu = _ERPContext.erpGroupMenu
                //    .FirstOrDefault(x => 
                //        x.Group_ID == int.Parse(Request.Form["GroupMenu_ID"].ToString()) && 
                //        x.Menu_ID == int.Parse(Request.Form["Menu_ID"].ToString())
                //    );
                if (_erpGroupMenu == null)
                {
                    erpGroupMenu _erpGroupMenuAdd = new erpGroupMenu();
                    _erpGroupMenuAdd.System_ID = 4;
                    _erpGroupMenuAdd.Menu_ID = int.Parse(Request.Form["Menu_ID"].ToString());
                    _erpGroupMenuAdd.Group_ID = int.Parse(Request.Form["Group_ID"].ToString());
                    _erpGroupMenuAdd.Toolbar = int.Parse(Request.Form["Toolbar"].ToString());
                    _erpGroupMenuAdd.ToolbarSearch = int.Parse(Request.Form["ToolbarSearch"].ToString());
                    _erpGroupMenuAdd.ToolbarNew = int.Parse(Request.Form["ToolbarNew"].ToString());
                    _erpGroupMenuAdd.ToolbarSave = int.Parse(Request.Form["ToolbarSave"].ToString());
                    _erpGroupMenuAdd.ToolbarDelete = int.Parse(Request.Form["ToolbarDelete"].ToString());
                    _erpGroupMenuAdd.ToolbarPrint = int.Parse(Request.Form["ToolbarPrint"].ToString());
                    _erpGroupMenuAdd.ToolbarExecute = int.Parse(Request.Form["ToolbarExecute"].ToString());
                    _erpGroupMenuAdd.ToolbarExport = int.Parse(Request.Form["ToolbarExport"].ToString());
                    _erpGroupMenuAdd.ToolbarSearchText = Request.Form["ToolbarSearchText"].ToString();
                    _erpGroupMenuAdd.ToolbarNewText = Request.Form["ToolbarNewText"].ToString();
                    _erpGroupMenuAdd.ToolbarSaveText = Request.Form["ToolbarSaveText"].ToString();
                    _erpGroupMenuAdd.ToolbarDeleteText = Request.Form["ToolbarDeleteText"].ToString();
                    _erpGroupMenuAdd.ToolbarPrintText = Request.Form["ToolbarPrintText"].ToString();
                    _erpGroupMenuAdd.ToolbarExecuteText = Request.Form["ToolbarExecuteText"].ToString();
                    _erpGroupMenuAdd.ToolbarExportText = Request.Form["ToolbarExportText"].ToString();
                    _erpGroupMenuAdd.Remark = Request.Form["Remark"].ToString();
                    _erpGroupMenuAdd.CreateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpGroupMenuAdd.CreateBy = _JBearer.UserCode.ToString();
                    _erpGroupMenuAdd.Status = "ACTIVE";
                    _erpGroupMenuAdd.isDelete = 0;
                    _ERPContext.erpGroupMenu.Add(_erpGroupMenuAdd);
                    _ERPContext.SaveChanges();
                }
                else
                {
                    _erpGroupMenu.System_ID = 4;
                    _erpGroupMenu.Menu_ID = int.Parse(Request.Form["_ID"].ToString());
                    _erpGroupMenu.Group_ID = int.Parse(Request.Form["Group_ID"].ToString());
                    _erpGroupMenu.Toolbar = int.Parse(Request.Form["Toolbar"].ToString() == "" ? "0" : "1");
                    _erpGroupMenu.ToolbarSearch = int.Parse(Request.Form["ToolbarSearch"].ToString() == "" ? "0" : "1");
                    _erpGroupMenu.ToolbarNew = int.Parse(Request.Form["ToolbarNew"].ToString() == "" ? "0" : "1");
                    _erpGroupMenu.ToolbarSave = int.Parse(Request.Form["ToolbarSave"].ToString() == "" ? "0" : "1");
                    _erpGroupMenu.ToolbarDelete = int.Parse(Request.Form["ToolbarDelete"].ToString() == "" ? "0" : "1");
                    _erpGroupMenu.ToolbarPrint = int.Parse(Request.Form["ToolbarPrint"].ToString() == "" ? "0" : "1");
                    _erpGroupMenu.ToolbarExecute = int.Parse(Request.Form["ToolbarExecute"].ToString() == "" ? "0" : "1");
                    _erpGroupMenu.ToolbarExport = int.Parse(Request.Form["ToolbarExport"].ToString() == "" ? "0" : "1");
                    _erpGroupMenu.ToolbarSearchText = Request.Form["ToolbarSearchText"].ToString();
                    _erpGroupMenu.ToolbarNewText = Request.Form["ToolbarNewText"].ToString();
                    _erpGroupMenu.ToolbarSaveText = Request.Form["ToolbarSaveText"].ToString();
                    _erpGroupMenu.ToolbarDeleteText = Request.Form["ToolbarDeleteText"].ToString();
                    _erpGroupMenu.ToolbarPrintText = Request.Form["ToolbarPrintText"].ToString();
                    _erpGroupMenu.ToolbarExecuteText = Request.Form["ToolbarExecuteText"].ToString();
                    _erpGroupMenu.ToolbarExportText = Request.Form["ToolbarExportText"].ToString();
                    _erpGroupMenu.Remark = Request.Form["Remark"].ToString();
                    _erpGroupMenu.UpdateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpGroupMenu.UpdateBy = _JBearer.UserCode.ToString();
                    _erpGroupMenu.Status = "ACTIVE";
                    _erpGroupMenu.isDelete = 0;
                    _ERPContext.erpGroupMenu.Update(_erpGroupMenu);
                    _ERPContext.SaveChanges();
                }

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data has been save""
                }";
                return Content(_result, "application/json");

            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }




        [HttpDelete]  //Delete
        public IActionResult Delete(int _id)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                var _JBearer = _BearerClass.AuthorizationJSON(Request.Headers.Authorization);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer.Data), "application/json");

                var _erpMenu = _ERPContext.erpMenu
                                    .FirstOrDefault(x => x._ID == int.Parse(Request.Form["_ID"].ToString()));
                if (_erpMenu != null)
                {
                    _erpMenu.isDelete = 1;
                    _erpMenu.UpdateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpMenu.UpdateBy = _JBearer.UserCode.ToString();
                    _ERPContext.erpMenu.Update(_erpMenu);
                    _ERPContext.SaveChanges();
                }

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data has been save""
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
