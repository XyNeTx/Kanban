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

        static SerilogLibs()
        {
            LogBL = new LoggerConfiguration()
                .WriteTo.Logger(log => log
                .WriteTo.File(
                    @"\\hmmta-ppm\Event_Log\New_KanbanF3\New_Kanban_F3_Cal_BL.json",
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Month))
            .CreateLogger();

            LogMaster = new LoggerConfiguration()
                .WriteTo.Logger(log => log
                .WriteTo.File(
                @"\\hmmta-ppm\Event_Log\New_KanbanF3\New_Kanban_F3_Master.json",
                outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Month))
                .CreateLogger();

            LogSpecial = new LoggerConfiguration()
                .WriteTo.Logger(log => log
                .WriteTo.File(
                @"\\hmmta-ppm\Event_Log\New_KanbanF3\New_Kanban_F3_Special.json",
                outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Month))
                .CreateLogger();

            LogImport = new LoggerConfiguration()
                .WriteTo.Logger(log => log
                .WriteTo.File(
                @"\\hmmta-ppm\Event_Log\New_KanbanF3\New_Kanban_F3_Import.json",
                outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Month))
                .CreateLogger();

        }

        public string Controller => _httpContextAccessor.HttpContext.Request.RouteValues["controller"].ToString();
        public string Action => _httpContextAccessor.HttpContext.Request.RouteValues["action"].ToString();

        public string GetLogMessage()
        {
            string Controller = _httpContextAccessor.HttpContext.Request.RouteValues["controller"].ToString();
            string Action = _httpContextAccessor.HttpContext.Request.RouteValues["action"].ToString();

            return $"Controller : {Controller} | Action : {Action}";
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



        //public void WriteLogBL(string Message)
        //{
        //    try
        //    {
        //        string logMessage = GetLogMessage();
        //        LogBL.Information($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
        //    }
        //    catch (Exception ex)
        //    {
        //        this.WriteErrorLogMsg($"message: {ex.Message}");
        //        Console.WriteLine(ex.StackTrace);
        //        Console.WriteLine(ex.Message);
        //        throw;
        //    }
        //}

        //public void WriteLogMaster(string Message)
        //{
        //    try
        //    {
        //        string logMessage = GetLogMessage();
        //        LogMaster.Information($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
        //    }
        //    catch (Exception ex)
        //    {
        //        this.WriteErrorLogMsg($"message: {ex.Message}");
        //        Console.WriteLine(ex.StackTrace);
        //        Console.WriteLine(ex.Message);
        //        throw;
        //    }
        //}

    }
}
