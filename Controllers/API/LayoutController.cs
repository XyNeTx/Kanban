using Microsoft.AspNetCore.Http;
using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using HINOSystem.Libs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;


using System;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;


using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HINOSystem.Controllers.API
{
    public class LayoutController : Controller
    {
        private readonly WarrantyClaimConnect _wrtConnect;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly string StoragePath = @"wwwroot\assets\img\avatars\private";

        public LayoutController(
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            WarrantyClaimConnect wrtConnect
            )
        {
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
            _wrtConnect = wrtConnect;
        }


        [HttpPatch] //Update profile
        public IActionResult Profile(int id = 0)
        {
            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            string _sql = @"UPDATE [erp].[User]
                SET Code = '" + Request.Form["Code"].ToString() + @"'
                    ,  Name = '" + Request.Form["Name"].ToString() + @"'
                    ,  Surname = '" + Request.Form["Surname"].ToString() + @"'
                    ,  NameTH = '" + Request.Form["NameTH"].ToString() + @"'
                    ,  SurnameTH = '" + Request.Form["SurnameTH"].ToString() + @"'
                    ,  Email = '" + Request.Form["Email"].ToString() + @"'
                    ,  UILanguage = '" + Request.Form["UILanguage"].ToString() + @"'
                WHERE _ID = '" + Request.Form["_ID"].ToString() + @"';

                SELECT * FROM [erp].[User] WHERE _ID NOT IN (1,2) AND isDelete=0 AND _ID = " + Request.Form["_ID"].ToString() + @" ORDER BY Code;
            ";
            string _jsonData = _KBCN.ExecuteJSON(_sql, pUser: _BearerClass, pAction: "CHANGE PROFILE", pControllerName: ControllerContext.ActionDescriptor.ControllerName.ToString(), pActionName: MethodBase.GetCurrentMethod().Name.ToString());
            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""CHANGE PROFILE"",
                ""data"":" + _jsonData + @"
            }";

            return Content(_result, "application/json");
        }


        
        [HttpPost]
        public async Task<IActionResult> ProfileImage(IFormFile file, string userid = "", string usercode = "")
        {
            string fileName = null;

            string orgFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Value;

            fileName = userid + "_" + usercode;
            fileName += "_" + Path.GetFileNameWithoutExtension(orgFileName).ToString().ToUpper().Replace(" ", "") + Path.GetExtension(orgFileName);

            string fullPath = this.StoragePath + @"\" + fileName;

            // Create the directory.
            Directory.CreateDirectory(Directory.GetParent(fullPath).FullName);

            // Save the file to the server.
            await using FileStream output = System.IO.File.Create(fullPath);
            await file.CopyToAsync(output);

            string _sql = @"UPDATE [erp].[User]
                SET Avatar = '" + fileName + @"'
                WHERE _ID = '" + userid + @"';

                SELECT * FROM [erp].[User] WHERE _ID NOT IN (1,2) AND isDelete=0 AND _ID = " + userid + @" ORDER BY Code;
            ";
            string _jsonData = _KBCN.ExecuteJSON(_sql, pAction: "CHANGE PROFILE IMAGE");

            fileName = "private/" + fileName;
            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""CHANGE PROFILE IMAGE"",
                ""data"": """ + fileName + @"""
            }";

            return Content(_result, "application/json");
        }


        //[HttpPost]
        //public string checkVersion()
        //{
        //    string strFilePath = @"KANBAN.dll";
        //    DateTime lastModified = System.IO.File.GetLastWriteTime(strFilePath);

        //    return "";
        //}


    }
}
