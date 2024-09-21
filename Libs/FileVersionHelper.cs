namespace KANBAN.Libs
{
    public class FileVersionHelper
    {
        public static string VersionedScriptPath(string filePath)
        {
            string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.Replace("~", "").TrimStart('/'));
            if (File.Exists(absolutePath))
            {
                var lastWriteTime = File.GetLastWriteTime(absolutePath).Ticks;
                return $"{filePath}?v={lastWriteTime}";
            }
            return filePath; // Return the original path if the file doesn't exist
        }
    }
}
