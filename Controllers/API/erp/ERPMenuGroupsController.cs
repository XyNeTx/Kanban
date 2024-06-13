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
    public class ERPMenuGroupsController : Controller
    {
        private readonly WarrantyClaimConnect _wrtConnect;
        private readonly BearerClass _BearerClass;

        public ERPMenuGroupsController(WarrantyClaimConnect wrtConnect, BearerClass bearerClass)
        {
            _wrtConnect = wrtConnect;
            _BearerClass = bearerClass;
        }


        [HttpGet]  //Read
        public IActionResult Read(string _id = null)
        {
            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            string SQL = @"
                SELECT '' AS RunningNo
                    , m.*
                    , mp.Seq
                FROM [erp].[Menu] m
                    LEFT JOIN [erp].[MenuParent] mp ON mp.Menu_ID=m._ID
                WHERE 1=1
                AND m.isDelete=0 
                AND mp.Parent_ID IS NULL 
                ORDER BY mp.Seq, m._ID;
            ";
            if (_id != null) SQL = "SELECT * FROM [erp].[Menu] WHERE _ID = '" + _id + "' ";

            string _jsonData = _wrtConnect.ExecuteJSON(SQL, pUser: _BearerClass, pAction: "READ", pControllerName: ControllerContext.ActionDescriptor.ControllerName.ToString(), pActionName: MethodBase.GetCurrentMethod().Name.ToString());


            if (_id != null) SQL = "SELECT * FROM [erp].[Menu] WHERE _ID = '" + _id + "' ";

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
            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            string _SQL = @"INSERT INTO [erp].[Group] (Code, Name, NameTH, CreateAt, CreateBy)
                VALUES('" + Request.Form["Code"].ToString() + @"'
                    ,  '" + Request.Form["Name"].ToString() + @"'
                    ,  '" + Request.Form["NameTH"].ToString() + @"'
                    , GETDATE()
                    , '" + _BearerClass.UserCode.ToString() + @"'
                );
            ";
            _wrtConnect.Execute(_SQL, pUser: _BearerClass, pAction: "CREATE", pControllerName: ControllerContext.ActionDescriptor.ControllerName.ToString(), pActionName: MethodBase.GetCurrentMethod().Name.ToString());

            return this.Read();
        }


        [HttpPatch] //Update
        public IActionResult Save(int id = 0)
        {
            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            string _SQL = @"UPDATE [erp].[Group]
                SET Code = '" + Request.Form["Code"].ToString() + @"'
                    ,  Name = '" + Request.Form["Name"].ToString() + @"'
                    ,  NameTH = '" + Request.Form["NameTH"].ToString() + @"'
                    ,  UpdateAt = GETDATE()
                    ,  UpdateBy = '" + _BearerClass.UserCode.ToString() + @"'
                WHERE _ID = '" + Request.Form["_ID"].ToString() + @"';
            ";
            _wrtConnect.Execute(_SQL, pUser: _BearerClass, pAction: "UPDATE", pControllerName: ControllerContext.ActionDescriptor.ControllerName.ToString(), pActionName: MethodBase.GetCurrentMethod().Name.ToString());

            return this.Read();
        }




        [HttpPost]  //Delete
        public IActionResult Delete(int _id)
        {
            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            string _SQL = @"UPDATE [erp].[Group]
                SET isDelete = 1
                    ,  UpdateAt = GETDATE()
                    ,  UpdateBy = '" + _BearerClass.UserCode.ToString() + @"'
                WHERE _ID = '" + _id + @"';
            ";
            _wrtConnect.Execute(_SQL, pUser: _BearerClass, pAction: "DELETE", pControllerName: ControllerContext.ActionDescriptor.ControllerName.ToString(), pActionName: MethodBase.GetCurrentMethod().Name.ToString());

            return this.Read();
        }

    }
}
