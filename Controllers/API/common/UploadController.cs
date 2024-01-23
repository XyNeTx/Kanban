using System;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Http;
using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using HINOSystem.Libs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;


using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace HINOSystem.Controllers.API.common
{
    public class UploadController : Controller
    {
        private readonly WarrantyClaimConnect _wrtConnect;

        private readonly string StoragePath = "Storage\\uploads";
        
        public UploadController(WarrantyClaimConnect wrtConnect)
        {
            _wrtConnect = wrtConnect;
        }





        [HttpPost]
        public async Task<IActionResult> File(IFormFile file, string path = "")
        {
            string fileName = null;

            // Get original file name to get the extension from it.
            string orgFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Value;

            // Create a new file name to avoid existing files on the server with the same names.
            fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            fileName += "_" + Path.GetFileNameWithoutExtension(orgFileName).ToString().ToUpper().Replace(" ", "") + Path.GetExtension(orgFileName);

            string fullPath = this.StoragePath + "\\" + (path == "" ? "files" : path) + "\\" + DateTime.Now.ToString("yyyyMM") + "\\" + fileName;

            // Create the directory.
            Directory.CreateDirectory(Directory.GetParent(fullPath).FullName);

            // Save the file to the server.
            await using FileStream output = System.IO.File.Create(fullPath);
            await file.CopyToAsync(output);

            //var response = new { FileName = fileName };

            //return Ok(response);

            //string SQL = "SELECT * FROM erp.[User] WHERE _ID NOT IN (1,2) AND isDelete=0 ORDER BY Code ";
            //string _jsonData = _wrtConnect.ExecuteJSON(SQL);







            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Created"",
                ""data"": """ + fileName + @"""
            }";

            return Content(_result, "application/json");
        }






        [HttpPost]
        public async Task<IActionResult>  Files(IList<IFormFile> files, string path = "")
        {
            string fileName = null;

            foreach (IFormFile source in files)
            {
                // Get original file name to get the extension from it.
                string orgFileName = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Value;

                // Create a new file name to avoid existing files on the server with the same names.
                fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                fileName += "_" + Path.GetFileNameWithoutExtension(orgFileName).ToString().ToUpper().Replace(" ", "") + Path.GetExtension(orgFileName);

                string fullPath = this.StoragePath + "\\" + (path == "" ? "files" : path) + "\\" + DateTime.Now.ToString("yyyyMM") + "\\" + fileName;

                // Create the directory.
                Directory.CreateDirectory(Directory.GetParent(fullPath).FullName);

                // Save the file to the server.
                await using FileStream output = System.IO.File.Create(fullPath);
                await source.CopyToAsync(output);
            }

            //var response = new { FileName = fileName };

            //return Ok(response);

            //string SQL = "SELECT * FROM erp.[User] WHERE _ID NOT IN (1,2) AND isDelete=0 ORDER BY Code ";
            //string _jsonData = _wrtConnect.ExecuteJSON(SQL);







            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Created"",
                ""data"": """ + fileName + @"""
            }";

            return Content(_result, "application/json");
        }


    }
}
