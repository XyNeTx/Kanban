using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;

namespace HINOSystem.Libs
{
    public class TextFileClass
    {
        private readonly IConfiguration _config;
        private readonly ERPConnection _erpConnection;
        private readonly KanbanConnection _KBCN;

        protected IWorkbook workbook;
        protected IFormulaEvaluator formulaEvaluator;
        protected DataFormatter dataFormatter;

        private readonly string StoragePath = @"wwwroot\Storage\DownloadTemp";

        private int FormatRow = 0;

        public JObject Data = null;

        public TextFileClass(
            IConfiguration configuration,
            KanbanConnection kanbanConnection,
            ERPConnection erpConnection
            )
        {
            _config = configuration;
            _erpConnection = erpConnection;
            _KBCN = kanbanConnection;

        }

        public async Task<bool> Write(string filePath = @"\file.txt", string text = @"")
        {

            try
            {
                string fullPath = Path.Combine(this.StoragePath, filePath);

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
                    await writer.WriteAsync(text);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> WriteLine(string filePath = @"\file.txt", string text = @"", string directory = @"")
        {
            try
            {
                string fullPath = this.StoragePath+ @"\" + DateTime.Now.ToString("yyyyMM") + @"\";
                //fullPath = Path.Combine(this.StoragePath, fullPath);
                
                if (!System.IO.Directory.Exists(fullPath))
                {
                    // Create the directory.
                    Directory.CreateDirectory(Directory.GetParent(fullPath).FullName);
                }

                fullPath = fullPath + filePath;

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
                    await writer.WriteLineAsync(text);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


    }
}