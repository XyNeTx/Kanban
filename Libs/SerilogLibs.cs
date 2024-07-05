using KANBAN.Models;
using Serilog;

namespace HINOSystem.Libs
{
    public class SerilogLibs
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SerilogLibs(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

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
                string logMessage = GetLogMessage();
                Log.Information($"message : {logMessage} | {Message} | username : {UserName} | hostname : {HostName}");
            }
            catch (Exception ex)
            {
                Log.Error($"message: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public void WriteErrorLog(string Message, string UserName, string HostName)
        {
            try
            {
                Log.Error($"message : {Message} | username : {UserName} | hostname : {HostName}");
            }
            catch (Exception ex)
            {
                Log.Error($"message: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
