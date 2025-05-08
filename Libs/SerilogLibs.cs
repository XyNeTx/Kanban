using Serilog;
using ILogger = Serilog.ILogger;

namespace HINOSystem.Libs
{
    public class SerilogLibs
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BearerClass _bearerClass;

        public SerilogLibs(IHttpContextAccessor httpContextAccessor, BearerClass bearerClass)
        {
            _httpContextAccessor = httpContextAccessor;
            _bearerClass = bearerClass;
        }

        private static readonly ILogger LogBL;
        private static readonly ILogger LogMaster;
        private static readonly ILogger LogSpecial;
        private static readonly ILogger LogImport;
        private static readonly ILogger LogError;
        private static readonly ILogger LogCKD;

        static SerilogLibs()
        {
            LogBL = new LoggerConfiguration()
                .WriteTo.Logger(log => log
                .WriteTo.File(
                    @"\\hmmta-ppm\Event_Log\New_KanbanF3\New_Kanban_F3_Cal_BL_.json",
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Month,
                    shared: true,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 524288000))
            .CreateLogger();

            LogMaster = new LoggerConfiguration()
                .WriteTo.Logger(log => log
                .WriteTo.File(
                    @"\\hmmta-ppm\Event_Log\New_KanbanF3\New_Kanban_F3_Master_.json",
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Month,
                    shared: true,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 524288000))
            .CreateLogger();

            LogSpecial = new LoggerConfiguration()
                .WriteTo.Logger(log => log
                .WriteTo.File(
                    @"\\hmmta-ppm\Event_Log\New_KanbanF3\New_Kanban_F3_Special_.json",
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Month,
                    shared: true,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 524288000))
            .CreateLogger();

            LogImport = new LoggerConfiguration()
                .WriteTo.Logger(log => log
                .WriteTo.File(
                    @"\\hmmta-ppm\Event_Log\New_KanbanF3\New_Kanban_F3_Import_.json",
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Month,
                    shared: true,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 524288000))
            .CreateLogger();

            LogCKD = new LoggerConfiguration()
                .WriteTo.Logger(log => log
                .WriteTo.File(
                    @"\\hmmta-ppm\Event_Log\New_KanbanF3\New_Kanban_F3_CKD_.json",
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Month,
                    shared: true,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 524288000))
            .CreateLogger();

            LogError = new LoggerConfiguration()
                .WriteTo.Logger(log => log
                .WriteTo.File(
                    @"\\hmmta-ppm\Event_Log\New_KanbanF3\New_Kanban_F3_Error_.json",
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Month,
                    shared: true,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 524288000))
            .CreateLogger();

        }

        public string Controller => _httpContextAccessor.HttpContext.Request.RouteValues["controller"].ToString();
        public string Action => _httpContextAccessor.HttpContext.Request.RouteValues["action"].ToString();
        public string Title => _httpContextAccessor.HttpContext.Request.Headers["title"].ToString();

        public string GetLogMessage()
        {
            return $"Controller : {Controller} : {Title} | Action : {Action}";
        }

        public void WriteLog(string Message, string UserName, string HostName)
        {
            try
            {
                //string logMessage = GetLogMessage();
                //Log.Information($"message : {logMessage} | {Message} | username : {UserName} | hostname : {HostName}");
                WriteLogMsg(Message);
            }
            catch (Exception ex)
            {
                this.WriteErrorLogMsg($"message: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void WriteLogMsg(string Message)
        {
            try
            {
                string logMessage = GetLogMessage();

                if (this.Controller.ToUpper().Contains("KBNMS"))
                {
                    LogMaster.Information($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                }
                else if (this.Action.ToUpper().Contains("CAL") && this.Controller.ToLower().Contains("kbnor121"))
                {
                    LogBL.Information($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                }
                else if (this.Controller.ToUpper().Contains("KBNOR2"))
                {
                    LogSpecial.Information($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                }
                else if (this.Controller.ToUpper().Contains("KBNIM"))
                {
                    LogImport.Information($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                }
                else if (this.Controller.ToUpper().Contains("KBNOR3"))
                {
                    LogCKD.Information($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                }
                else
                {
                    Log.Information($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                }
            }
            catch (Exception ex)
            {
                this.WriteErrorLogMsg($"message: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void WriteErrorLogMsg(string Message)
        {
            try
            {
                string logMessage = GetLogMessage();

                //if (this.Controller.ToUpper().Contains("KBNMS"))
                //{
                //    LogMaster.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                //}
                //else if (this.Action.ToUpper().Contains("CAL") && this.Controller.ToLower().Contains("kbnor121"))
                //{
                //    LogBL.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                //}
                //else if (this.Controller.ToUpper().Contains("KBNOR2"))
                //{
                //    LogSpecial.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                //}
                //else if (this.Controller.ToUpper().Contains("KBNIM"))
                //{
                //    LogImport.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                //}
                //else
                //{
                //    Log.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                //}
                LogError.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
            }
            catch (Exception ex)
            {
                this.WriteErrorLogMsg($"message: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void WriteErrorLog(string Message, string UserName, string HostName)
        {
            try
            {
                string logMessage = GetLogMessage();

                if (this.Controller.ToUpper().Contains("KBNMS"))
                {
                    LogMaster.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                }
                else if (this.Action.ToUpper().Contains("CAL") && this.Controller.ToLower().Contains("kbnor121"))
                {
                    LogBL.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                }
                else if (this.Controller.ToUpper().Contains("KBNOR2"))
                {
                    LogSpecial.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                }
                else if (this.Controller.ToUpper().Contains("KBNIM"))
                {
                    LogImport.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                }
                else if (this.Controller.ToUpper().Contains("KBNOR3"))
                {
                    LogCKD.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                }
                else
                {
                    Log.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
                }
            }
            catch (Exception ex)
            {
                this.WriteErrorLogMsg($"message: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public string SerializeErrObjHTML(object obj)
        {
            try
            {
                var listKey = obj.GetType().GetProperties();
                string serialized = "<br><br>";

                for (int i = 0; i < listKey.Length; i++)
                {
                    var property = listKey[i];
                    serialized += property.Name + " : " + (string)(property.GetValue(obj)) + "<br>";
                }

                return serialized;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
