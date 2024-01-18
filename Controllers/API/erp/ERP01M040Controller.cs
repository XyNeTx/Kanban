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
using HINOSystem.Models.KB3;

namespace HINOSystem.Controllers.API.erp
{
    public class ERP01M040Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly DefaultConnection _KB3Connect;

        private readonly ERPContext _ERPContext;
        private readonly KB3Context _KB3Context;

        public ERP01M040Controller(
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


                _SQL = @" EXEC [exec].[spERP01M011_SEARCH] '3' ";
                string _erpGroup = _KB3Connect.executeSQLJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
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


                _SQL = @" EXEC [exec].[spERP01M010_SEARCH] '3' ";
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


                erpUser _erpUser = new erpUser();
                _erpUser.Code = Request.Form["Code"].ToString();
                _erpUser.Name = Request.Form["Name"].ToString();
                _erpUser.Surname = Request.Form["Surname"].ToString();
                _erpUser.NameTH = Request.Form["NameTH"].ToString();
                _erpUser.SurnameTH = Request.Form["SurnameTH"].ToString();
                _erpUser.NameJP = Request.Form["NameJP"].ToString();
                _erpUser.SurnameJP = Request.Form["SurnameJP"].ToString();
                _erpUser.CreateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                _erpUser.CreateBy = _JBearer.UserCode.ToString();
                _erpUser.UILanguage = "EN";
                _erpUser.UITheme = "DEFAULT";
                _erpUser.Status = "ACTIVE";
                _erpUser.isDelete = 0;
                _ERPContext.erpUser.Add(_erpUser);
                _ERPContext.SaveChanges();


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

                var _erpUser = _ERPContext.erpUser
                                    .FirstOrDefault(x => x._ID == int.Parse(Request.Form["_ID"].ToString()));
                if (_erpUser != null)
                {
                    _erpUser.Code = Request.Form["Code"].ToString();
                    _erpUser.Name = Request.Form["Name"].ToString();
                    _erpUser.Surname = Request.Form["Surname"].ToString();
                    _erpUser.NameTH = Request.Form["NameTH"].ToString();
                    _erpUser.SurnameTH = Request.Form["SurnameTH"].ToString();
                    _erpUser.NameJP = Request.Form["NameJP"].ToString();
                    _erpUser.SurnameJP = Request.Form["SurnameJP"].ToString();
                    _erpUser.Status = Request.Form["Status"].ToString();
                    _erpUser.CreateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpUser.CreateBy = _JBearer.UserCode.ToString();
                    _ERPContext.erpUser.Update(_erpUser);
                    _ERPContext.SaveChanges();
                }


                var _erpGroupUser = _ERPContext.erpGroupUser
                                    .FirstOrDefault(x => x.User_ID == int.Parse(Request.Form["_ID"].ToString()));
                if (_erpGroupUser == null)
                {
                    erpGroupUser _erpGroupUserAdd = new erpGroupUser();
                    _erpGroupUserAdd.System_ID = 4;
                    _erpGroupUserAdd.User_ID = int.Parse(Request.Form["_ID"].ToString());
                    _erpGroupUserAdd.Group_ID = int.Parse(Request.Form["GroupID"].ToString());
                    _erpGroupUserAdd.Remark = "";
                    _erpGroupUserAdd.CreateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpGroupUserAdd.CreateBy = _JBearer.UserCode.ToString();
                    _erpGroupUserAdd.Status = "ACTIVE";
                    _erpGroupUserAdd.isDelete = 0;
                    _ERPContext.erpGroupUser.Add(_erpGroupUserAdd);
                    _ERPContext.SaveChanges();
                }
                else { 
                    _erpGroupUser.System_ID = 4;
                    _erpGroupUser.User_ID = int.Parse(Request.Form["_ID"].ToString());
                    _erpGroupUser.Group_ID = int.Parse(Request.Form["GroupID"].ToString());
                    _erpGroupUser.Remark = "";
                    _erpGroupUser.CreateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpGroupUser.CreateBy = _JBearer.UserCode.ToString();
                    _erpGroupUser.Status = "ACTIVE";
                    _erpGroupUser.isDelete = 0;
                    _ERPContext.erpGroupUser.Update(_erpGroupUser);
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

                var _erpUser = _ERPContext.erpUser
                                    .FirstOrDefault(x => x._ID == int.Parse(Request.Form["_ID"].ToString()));
                if (_erpUser != null)
                {
                    _erpUser.isDelete = 1;
                    _erpUser.UpdateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpUser.UpdateBy = _JBearer.UserCode.ToString();
                    _ERPContext.erpUser.Update(_erpUser);
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
