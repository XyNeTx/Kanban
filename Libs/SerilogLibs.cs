using Serilog;

namespace HINOSystem.Libs
{
    public class SerilogLibs
    {
        public void WriteLog(string Message, string UserName, string HostName)
        {
            try
            {
                Log.Information($"message : {Message} , username : {UserName} , hostname : {HostName}");
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
                Log.Error($"message : {Message} , username : {UserName} , hostname : {HostName}");
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
