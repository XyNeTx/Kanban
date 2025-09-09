//using Microsoft.Office.Interop.Excel;
using KANBAN.Services;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;

namespace HINOSystem.Libs
{
    public class BearerClass
    {
        private readonly IConfiguration _config;
        private readonly ERPConnection _erpConnection;
        private readonly KanbanConnection _KBCN;
        private readonly IHttpContextAccessor _http;
        private readonly SerilogLibs _log;

        protected IWorkbook workbook;
        protected IFormulaEvaluator formulaEvaluator;
        protected DataFormatter dataFormatter;

        private int FormatRow = 0;

        public int Status = 401;
        public string Token = "";
        public string UserCode = "";
        public string Device = "";
        public string Plant = "";
        public string IPAddress = "";
        public string ProcessDate = "";
        public string Shift = "";
        public string ControllerName = "";
        public string ActionName = "";
        public string Response = "";
        public string Message = "";
        public dynamic Records = null;
        public string LOV = "";
        public JObject Data = null;

        public JObject Result = null;

        public BearerClass(
            IConfiguration configuration,
            KanbanConnection kanbanConnection,
            ERPConnection erpConnection,
            IHttpContextAccessor http
            )
        {
            _config = configuration;
            _erpConnection = erpConnection;
            _KBCN = kanbanConnection;
            _http = http;

        }

        public dynamic Authentication()
        {
            this.Status = 200;
            this.Token = _http.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            this.UserCode = _http.HttpContext.Request.Headers["UserCode"].ToString();
            this.Device = _http.HttpContext.Request.Headers["Device"].ToString();
            this.IPAddress = _http.HttpContext.Request.Headers["IPAddress"].ToString();
            this.Plant = _http.HttpContext.Request.Headers["Plant"].ToString();
            this.ProcessDate = _http.HttpContext.Request.Headers["ProcessDate"].ToString();
            this.Shift = _http.HttpContext.Request.Headers["Shift"].ToString();
            this.ControllerName = _http.HttpContext.Request.Headers["Controller"].ToString();
            this.ActionName = _http.HttpContext.Request.Headers["Action"].ToString();

            return this;
        }

        public int CheckAuthen()
        {
            this.Authentication();

            return this.Status;
        }

        public async Task CheckAuthorize()
        {
            this.Authentication();
            if (this.Status == 401)
            {
                throw new CustomHttpException(401, "Unauthorized");
            }
            else if (this.Status == 403)
            {
                throw new CustomHttpException(403, "Forbidden");
            }

            await Task.CompletedTask;
        }

        public string StoreAccess()
        {
            return this.Plant switch
            {
                "1" => "1A",
                "2" => "2B",
                "3" => "3C",
                _ => "3C",
            };
        }

        public string versions()
        {
            string _version = @"KANBAN.dll";
            if (System.IO.File.Exists(_version))
            {
                DateTime _lastModified = System.IO.File.GetLastWriteTime(_version);
                _version = _lastModified.ToString("yy.MM.ddhhmm");
            }
            else
            {
                _version = DateTime.Now.ToString("yy.MM.ddhhmm");
            }

            return _version;
        }
    }
}