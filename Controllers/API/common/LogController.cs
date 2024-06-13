using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.Web;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.Net.Http.Headers;
using System.Collections.Specialized;
using System.Net;
using System.DirectoryServices.ActiveDirectory;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Org.BouncyCastle.Asn1.Ocsp;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading.Tasks;

using HINOSystem.Libs;
using HINOSystem.Context;
using HINOSystem.Models.KB3.Master;
using NPOI.SS.Formula.Functions;
using System.Dynamic;
using static System.Net.Mime.MediaTypeNames;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace HINOSystem.Controllers.API.Master
{
    public class LogController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly SerilogLibs _Serilog;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        private string _logName = "";
        private string _DB = "";

        public LogController(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            KB3Context kB3Context,
            SerilogLibs serilogLibs
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
            _KB3Context = kB3Context;
            _Serilog = serilogLibs;

        }



        [HttpPost]
        public async Task<IActionResult> WriteLog([FromBody] string pPostData = null)
        {
            dynamic _data = null;
            string _SQL, _resData = null;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Process not complete"",
                    ""rows"": null
                }";

            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            try
            {
                _KBCN.Plant = _BearerClass.Plant;

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                //_Serilog.WriteLog(_data.message.ToString(), _BearerClass.UserCode, _BearerClass.Device);
                string _name = _data.name.ToString();
                string _level = "Information";

                string _logType = _configuration.GetValue<string>("Logs:Map:" + _name + ":type");
                string _logPath = _configuration.GetValue<string>("Logs:Map:" + _name + ":path");
                string _logFile = _configuration.GetValue<string>("Logs:Map:" + _name + ":file");
                Boolean _logCreatePath = _configuration.GetValue<Boolean>("Logs:Map:" + _name + ":createPath");
                string _logInterval = _configuration.GetValue<string>("Logs:Map:" + _name + ":rollingInterval");
                string _logTemplate = _configuration.GetValue<string>("Logs:Map:" + _name + ":outputTemplate");

                string _filePath = "", fullPath = _logPath + @"\";
                string _filePattern = DateTime.Now.ToString("yyyyMMdd");
                string _pathPattern = "";
                if (_logInterval.ToUpper() == "MONTH")
                {
                    _filePattern = DateTime.Now.ToString("yyyyMMdd");
                    _pathPattern = DateTime.Now.ToString("yyyyMM");
                }

                _logTemplate = _logTemplate.ToUpper();
                _logTemplate = _logTemplate.Replace("{USERNAME}", _BearerClass.UserCode);
                _logTemplate = _logTemplate.Replace("{DEVICE}", _BearerClass.Device);
                _logTemplate = _logTemplate.Replace("{IPADDRESS}", _BearerClass.IPAddress);
                _logTemplate = _logTemplate.Replace("{LEVEL}", _level);
                _logTemplate = _logTemplate.Replace("{TIMESTAMP}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                _logTemplate = _logTemplate.Replace("{MESSAGE}", _data.message.ToString());
                //if (_logInterval.ToUpper() == "MONTH") fullPath = _logPath + @"\" + DateTime.Now.ToString("yyyyMM") + @"\";
                //fullPath = Path.Combine(this.StoragePath, fullPath); 

                fullPath = _logPath + @"\" + (_pathPattern != "" ? _pathPattern + @"\" : "");

                if (!System.IO.Directory.Exists(fullPath) && _logCreatePath)
                {
                    // Create the directory.
                    Directory.CreateDirectory(Directory.GetParent(fullPath).FullName);
                }

                fullPath = fullPath + @"" + _logFile + _filePattern + @"." + _logType;

                if (!System.IO.File.Exists(fullPath))
                {
                    // Create the file if it does not exist
                    using (FileStream fs = System.IO.File.Create(fullPath))
                    {
                        // Leave the file open so StreamWriter can write to it
                    }
                }

                using (StreamWriter writer = new StreamWriter(fullPath, true))
                {
                    await writer.WriteLineAsync(_logTemplate);
                }

                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(_result, "application/json");
            }
        }

        

    }
}
