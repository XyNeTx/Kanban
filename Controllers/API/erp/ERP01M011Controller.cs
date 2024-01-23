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
    public class ERP01M011Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly ERPContext _ERPContext;
        private readonly KB3Context _KB3Context;

        public ERP01M011Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            ERPContext erpContext,
            KB3Context kb3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
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
                BearerClass _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");


                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_MS_Factory"" : " + _jsTB_MS_Factory + @"
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
                BearerClass _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                //_json = JsonConvert.DeserializeObject(pData);


                _SQL = @" EXEC [exec].[spERP01M011_SEARCH] '3' ";
                string _erpGroup = _KBCN.ExecuteJSON(_SQL, pUser: _JBearer, pAction: "READ", pControllerName: ControllerContext.ActionDescriptor.ControllerName.ToString(), pActionName: MethodBase.GetCurrentMethod().Name.ToString());
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
                BearerClass _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");


                erpGroup _erpGroup = new erpGroup();
                _erpGroup.Code = Request.Form["Code"].ToString();
                _erpGroup.Name = Request.Form["Name"].ToString();
                _erpGroup.NameTH = Request.Form["NameTH"].ToString();
                _erpGroup.CreateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                _erpGroup.CreateBy = _JBearer.UserCode.ToString();
                _erpGroup.Status = "ACTIVE";
                _erpGroup.isDelete = 0;
                _ERPContext.erpGroup.Add(_erpGroup);
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
                if (_JBearer.Status== 401) return Content(JsonConvert.SerializeObject(_JBearer.Data), "application/json");


                var _erpGroup = _ERPContext.erpGroup
                                    .FirstOrDefault(x => x._ID == int.Parse(Request.Form["_ID"].ToString()));
                if (_erpGroup != null)
                {
                    _erpGroup.Code = Request.Form["Code"].ToString();
                    _erpGroup.Name = Request.Form["Name"].ToString();
                    _erpGroup.NameTH = Request.Form["NameTH"].ToString();
                    _erpGroup.UpdateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpGroup.UpdateBy = _JBearer.UserCode.ToString();
                    _ERPContext.erpGroup.Update(_erpGroup);
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


                var _erpGroup = _ERPContext.erpGroup
                                    .FirstOrDefault(x => x._ID == int.Parse(Request.Form["_ID"].ToString()));
                if (_erpGroup != null)
                {
                    _erpGroup.isDelete = 1;
                    _erpGroup.UpdateAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    _erpGroup.UpdateBy = _JBearer.UserCode.ToString();
                    _ERPContext.erpGroup.Update(_erpGroup);
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



        [HttpPost]
        public IActionResult Detail([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                BearerClass _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                _json = JsonConvert.DeserializeObject(pData);

                _SQL = @" EXEC [exec].[spERP01M010_SEARCH] '3', '', '" + _json.GroupID + "' ";
                string _erpUser = _KBCN.ExecuteJSON(_SQL, pUser: _JBearer, pAction: "READ", pControllerName: ControllerContext.ActionDescriptor.ControllerName.ToString(), pActionName: MethodBase.GetCurrentMethod().Name.ToString());

                _SQL = @" EXEC [exec].[spERP01M020_SEARCH] '3', '" + _json.GroupID + "' ";
                string _erpMenu = _KBCN.ExecuteJSON(_SQL, pUser: _JBearer, pAction: "READ", pControllerName: ControllerContext.ActionDescriptor.ControllerName.ToString(), pActionName: MethodBase.GetCurrentMethod().Name.ToString());

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""User"" : " + _erpUser + @",
                                ""Menu"" : " + _erpMenu + @"
                            }
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
