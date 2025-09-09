using HINOSystem.Context;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace HINOSystem.Controllers.API.Master
{
    public class LogController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly SerilogLibs _Serilog;
        private readonly KanbanConnection _KBCN;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly KB3Context _KB3Context;

        private string _logName = "";
        private string _DB = "";

        public LogController(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            KB3Context kB3Context,
            SerilogLibs serilogLibs,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
            _KB3Context = kB3Context;
            _Serilog = serilogLibs;
            _httpContextAccessor = httpContextAccessor;

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

            if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
            {
                return StatusCode(_BearerClass.Status, new
                {
                    status = _BearerClass.Status,
                    response = _BearerClass.Response,
                    message = _BearerClass.Message
                });
            }
            try
            {
                

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                //_Serilog.WriteLog(_data.message.ToString(), _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.WindowsDeviceClaim).Value);
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
                _logTemplate = _logTemplate.Replace("{USERNAME}", _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value);
                _logTemplate = _logTemplate.Replace("{DEVICE}", _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.WindowsDeviceClaim).Value);
                _logTemplate = _logTemplate.Replace("{IPADDRESS}", _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Dns).Value);
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
