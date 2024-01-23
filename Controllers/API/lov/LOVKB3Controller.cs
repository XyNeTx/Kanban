using Microsoft.AspNetCore.Http;
using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using HINOSystem.Libs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.lov
{
    public class LOVKB3Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly KanbanConnection _KBCN;
        private readonly BearerClass _BearerClass;

        private readonly DBConnection _pseConnect;
        private readonly DBConnection _issConnect;

        public LOVKB3Controller(
            IConfiguration configuration,
            KanbanConnection kanbanConnection, 
            BearerClass bearerClass
            )
        {
            _configuration = configuration;
            _KBCN = kanbanConnection;
            _BearerClass = bearerClass;

            _pseConnect = new DBConnection(_configuration, "pse");
            _issConnect = new DBConnection(_configuration, "iss");
        }



        [HttpPost]
        public IActionResult PartList(string _id = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                BearerClass _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                _SQL = "SELECT '' AS RunningNo, Number AS Code, Name AS Description FROM [mst].[Part] WHERE isDelete=0 ORDER BY _ID ";
                string _jsPartList = _KBCN.ExecuteJSON(_SQL, pUser: _JBearer, skipLog: true);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": """",
                    ""data"":" + _jsPartList + @"
                }";

                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }




    }
}
