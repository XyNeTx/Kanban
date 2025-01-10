using Serilog;

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

        public string GetLogMessage()
        {
            _bearerClass.Authentication(_httpContextAccessor.HttpContext.Request);
            string Controller = _httpContextAccessor.HttpContext.Request.RouteValues["controller"].ToString();
            string Action = _httpContextAccessor.HttpContext.Request.RouteValues["action"].ToString();
            string UserName = _bearerClass.UserCode;
            string HostName = _bearerClass.Device.ToString();

            return $"Controller : {Controller} | Action : {Action}";
        }

        public void WriteLog(string Message, string UserName, string HostName)
        {
            try
            {
                string logMessage = GetLogMessage();
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
                Log.Information($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
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
                Log.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
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
                Log.Error($"{logMessage} | message : {Message} | username : {_bearerClass.UserCode} | hostname : {_bearerClass.Device}");
            }
            catch (Exception ex)
            {
                this.WriteErrorLogMsg($"message: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
