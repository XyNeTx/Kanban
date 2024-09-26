namespace KANBAN.Libs
{
    public class FileVersionHelper
    {

        public static string VersionedScriptPath(string filePath)
        {
            string absolutePath = string.Empty;

            if (!Directory.GetCurrentDirectory().Contains("inetpub"))
            {
                absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.Replace("~", "").TrimStart('/'));
                if (File.Exists(absolutePath))
                {
                    var lastWriteTime = File.GetLastWriteTime(absolutePath).Ticks;
                    return $"{filePath}?v={lastWriteTime}";
                }
            }

            //else
            //{
            //    absolutePath = Path.Combine(Directory.GetCurrentDirectory(),"kanban", "wwwroot", filePath.Replace("~", "").TrimStart('/'));
            //    if (File.Exists(absolutePath))
            //    {
            //        var lastWriteTime = File.GetLastWriteTime(absolutePath).Ticks;
            //        return $"{filePath}?v={lastWriteTime}";
            //    }
            //}

            return $"{"/kanban"+filePath}?v={File.GetLastWriteTime(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.Replace("~", "").TrimStart('/'))).Ticks}";
        }
    }
}
