namespace KANBAN.Libs
{
    public class FileVersionHelper
    {

        public static string VersionedScriptPath(string filePath)
        {
            string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.Replace("~", "").TrimStart('/'));

            if (!Directory.GetCurrentDirectory().Contains("inetpub"))
            {
                if (File.Exists(absolutePath))
                {
                    var lastWriteTime = File.GetLastWriteTime(absolutePath).Ticks;
                    return $"{filePath}?v={lastWriteTime}";
                }
            }
            return $"{"/kanban" + filePath}?v={File.GetLastWriteTime(absolutePath).Ticks}";
        }

    }



}
