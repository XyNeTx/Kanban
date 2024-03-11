using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.Web;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.Net.Http.Headers;
using System.Collections.Specialized;
using System.Net;
using System.DirectoryServices.ActiveDirectory;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Org.BouncyCastle.Asn1.Ocsp;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading.Tasks;

using HINOSystem.Libs;
using HINOSystem.Context;
using HINOSystem.Models.KB3.Master;
using NPOI.SS.Formula.Functions;
using System.Dynamic;

namespace HINOSystem.Controllers.API.Master
{
    public class EXECController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        public EXECController(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
            _KB3Context = kB3Context;

        }



        [HttpPost]
        public IActionResult xExecute([FromBody] string pPostData = null)
        {
            dynamic _bearer, _data = null;
            string _SQL, _resData = null;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Process not complete"",
                    ""rows"": null
                }";

            _bearer = _BearerClass.Header(Request);
            if (_bearer.Status == 401 || _bearer.Status == null) return Content(JsonConvert.SerializeObject(_bearer), "application/json");

            try
            {
                _KBCN.Plant = _bearer.Plant;

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                string _spName = "";
                string _spParameter = "";
                int i = 0;
                foreach (var entry in _data)
                {
                    if (i == 0) _spName = entry.Value.ToString();
                    if (i >= 1) _spParameter += "'" + entry.Value.ToString() + "', ";

                    i++;
                }
                _spParameter = _spParameter.Substring(0, _spParameter.Length - 2);

                _SQL = @" EXEC " + _spName + @" " + _spParameter;

                _KBCN.Execute(_SQL
                   , pUser: _bearer
                   , pAction: "EXECUTE WITH JQUERY"
                   , pControllerName: _bearer.ActionName
                   , pActionName: _spName
                    );

                _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Process complete"",
                        ""rows"": null
                    }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": """ + e.Message.ToString() + @""",
                        ""rows"": " + _resData + @"
                    }";
                return Content(_result, "application/json");
            }
        }

        [HttpPost]
        public IActionResult Execute([FromBody] string pPostData = null)
        {
            dynamic _bearer, _data = null;
            string _SQL, _resData = null;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Process not complete"",
                    ""rows"": null
                }";

            _bearer = _BearerClass.Header(Request);
            if (_bearer.Status == 401 || _bearer.Status == null) return Content(JsonConvert.SerializeObject(_bearer), "application/json");

            try
            {
                _KBCN.Plant = _bearer.Plant;

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                string _spName = "";
                string _spParameter = "";
                int i = 0;
                foreach (var entry in _data)
                {
                    if (i == 0) _spName = entry.Value.ToString();
                    if (i >= 1) _spParameter += "'" + entry.Value.ToString() + "', ";

                    i++;
                }

                _SQL = @" EXEC " + _spName + @" " + _spParameter + "''";

                _KBCN.Execute(_SQL
                   , pUser: _bearer
                    , pAction: "EXECUTE WITH JQUERY"
                   , pControllerName: _bearer.ActionName
                   , pActionName: _spName
                    );

                _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Process complete"",
                        ""rows"": null
                    }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": """ + e.Message.ToString() + @""",
                        ""rows"": " + _resData + @"
                    }";
                return Content(_result, "application/json");
            }
        }






        [HttpPost]
        public IActionResult xExecuteJSON([FromBody] string pPostData = null)
        {
            dynamic _bearer, _data = null;
            string _SQL, _resData = null;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""rows"": null
                }";

            _bearer = _BearerClass.Header(Request);
            if (_bearer.Status == 401 || _bearer.Status == null) return Content(JsonConvert.SerializeObject(_bearer), "application/json");

            try
            {
                _KBCN.Plant = _bearer.Plant;

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                string _spName = "";
                string _spParameter = "";
                int i = 0;
                foreach (var entry in _data)
                {
                    if (i == 0) _spName = entry.Value.ToString();
                    if (i >= 1) _spParameter += "'" + entry.Value.ToString() + "', ";

                    i++;
                }
                if (_spParameter != "") _spParameter = _spParameter.Substring(0, _spParameter.Length - 2);

                _SQL = @" EXEC " + _spName + @" " + _spParameter;

                _resData = _KBCN.ExecuteJSON(_SQL
                   , pUser: _bearer
                    , pAction: "EXECUTE JSON WITH JQUERY"
                   , pControllerName: _bearer.ActionName
                   , pActionName: _spName
                   );

                if (_resData != null && _resData != "[]")
                    _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data found"",
                        ""rows"": " + _resData + @"
                    }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": """ + e.Message.ToString() + @""",
                        ""rows"": " + _resData + @"
                    }";
                return Content(_result, "application/json");
            }
        }

        [HttpPost]
        public IActionResult ExecuteJSON([FromBody] string pPostData = null)
        {
            dynamic _bearer, _data = null;
            string _SQL, _resData = null;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""rows"": null
                }";

            _bearer = _BearerClass.Header(Request);
            if (_bearer.Status == 401 || _bearer.Status == null) return Content(JsonConvert.SerializeObject(_bearer), "application/json");

            try
            {
                _KBCN.Plant = _bearer.Plant;

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                string _spName = "";
                string _spParameter = "";
                int i = 0;
                foreach (var entry in _data)
                {
                    if (i == 0) _spName = entry.Value.ToString();
                    if (i >= 1) _spParameter += "'" + entry.Value.ToString() + "', ";

                    i++;
                }

                _SQL = @" EXEC " + _spName + @" " + _spParameter + "''";

                _resData = _KBCN.ExecuteJSON(_SQL
                    , pUser: _bearer
                    , pAction: "EXECUTE JSON WITH JQUERY"
                   , pControllerName: _bearer.ActionName
                   , pActionName: _spName
                    );


                if (_resData != null && _resData != "[]")
                    _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data found"",
                        ""rows"": " + _resData + @"
                    }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": """ + e.Message.ToString() + @""",
                        ""rows"": " + _resData + @"
                    }";
                return Content(_result, "application/json");
            }
        }



    }
}
