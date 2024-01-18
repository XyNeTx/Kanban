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
        private readonly WarrantyClaimConnect _wrtConnect;
        private readonly BearerClass _BearerClass;

        private readonly DBConnection _pseConnect;
        private readonly DBConnection _issConnect;

        public LOVKB3Controller(IConfiguration configuration, WarrantyClaimConnect wrtConnect, BearerClass bearerClass)
        {
            _configuration = configuration;
            _wrtConnect = wrtConnect;
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
                JObject _JBearer = _BearerClass.Authorization(Request.Headers.Authorization);
                if (_JBearer.GetValue("status").ToString() == "401") return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                _SQL = "SELECT '' AS RunningNo, Number AS Code, Name AS Description FROM [mst].[Part] WHERE isDelete=0 ORDER BY _ID ";
                string _jsPartList = _wrtConnect.executeSQLJSON(_SQL, pUser: _JBearer, skipLog: true);

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
