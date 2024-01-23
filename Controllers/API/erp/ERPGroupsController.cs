using Microsoft.AspNetCore.Http;
using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using HINOSystem.Libs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace HINOSystem.Controllers.API.wrt
{
    public class ERPGroupsController : Controller
    {
        private readonly ERPConnection _erpConnect;
        private readonly BearerClass _BearerClass;

        public ERPGroupsController(
            ERPConnection erpConnect, 
            BearerClass bearerClass
            )
        {
            _erpConnect = erpConnect;
            _BearerClass = bearerClass;
        }


        [HttpGet]  //Read
        public IActionResult Read(string _id = null)
        {
            BearerClass _JBearer = _BearerClass.Header(Request);
            if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

            string SQL = @"
                SELECT '' AS RunningNo,*
                FROM [erp].[Group] 
                WHERE 1=1
                AND isDelete=0 
                ORDER BY _ID;
            ";
            if (_id != null) SQL = "SELECT * FROM [erp].[Group] WHERE _ID = '" + _id + "' ";

            string _jsonData = _erpConnect.ExecuteJSON(SQL, pUser: _JBearer, pAction: "READ", pControllerName: ControllerContext.ActionDescriptor.ControllerName.ToString(), pActionName: MethodBase.GetCurrentMethod().Name.ToString());
            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Loaded"",
                ""data"":" + _jsonData + @"
            }";

            return Content(_result, "application/json");
        }


        [HttpPost] //Create
        public IActionResult Save()
        {
            BearerClass _JBearer = _BearerClass.Header(Request);
            if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

            string _SQL = @"INSERT INTO [erp].[Group] (Code, Name, NameTH, CreateAt, CreateBy)
                VALUES('" + Request.Form["Code"].ToString() + @"'
                    ,  '" + Request.Form["Name"].ToString() + @"'
                    ,  '" + Request.Form["NameTH"].ToString() + @"'
                    , GETDATE()
                    , '" + _JBearer.UserCode.ToString() + @"'
                );
            ";
            _erpConnect.Execute(_SQL, pUser: _JBearer, pAction: "CREATE", pControllerName: ControllerContext.ActionDescriptor.ControllerName.ToString(), pActionName: MethodBase.GetCurrentMethod().Name.ToString());

            return this.Read();
        }


        [HttpPatch] //Update
        public IActionResult Save(int id = 0)
        {
            BearerClass _JBearer = _BearerClass.Header(Request);
            if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

            string _SQL = @"UPDATE [erp].[Group]
                SET Code = '" + Request.Form["Code"].ToString() + @"'
                    ,  Name = '" + Request.Form["Name"].ToString() + @"'
                    ,  NameTH = '" + Request.Form["NameTH"].ToString() + @"'
                    ,  UpdateAt = GETDATE()
                    ,  UpdateBy = '" + _JBearer.UserCode.ToString() + @"'
                WHERE _ID = '" + Request.Form["_ID"].ToString() + @"';
            ";
            _erpConnect.Execute(_SQL, pUser: _JBearer, pAction: "UPDATE", pControllerName: ControllerContext.ActionDescriptor.ControllerName.ToString(), pActionName: MethodBase.GetCurrentMethod().Name.ToString());

            return this.Read();
        }




        [HttpDelete]  //Delete
        public IActionResult Delete(int _id)
        {
            BearerClass _JBearer = _BearerClass.Header(Request);
            if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

            string _SQL = @"UPDATE [erp].[Group]
                SET isDelete = 1
                    ,  UpdateAt = GETDATE()
                    ,  UpdateBy = '" + _JBearer.UserCode.ToString() + @"'
                WHERE _ID = '" + _id + @"';
            ";
            _erpConnect.Execute(_SQL, pUser: _JBearer, pAction: "DELETE", pControllerName: ControllerContext.ActionDescriptor.ControllerName.ToString(), pActionName: MethodBase.GetCurrentMethod().Name.ToString());

            return this.Read();
        }

    }
}
