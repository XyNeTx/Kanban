using HINOSystem.Context;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            dynamic _data = null;
            string _SQL, _resData = null;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Process not complete"",
                    ""rows"": null
                }";

            try
            {
                
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

                bool complete = _KBCN.Execute(_SQL
                   , pUser: _BearerClass
                   , pAction: "EXECUTE WITH JQUERY"
                   , pControllerName: _BearerClass.ActionName
                   , pActionName: _spName
                );

                if (!complete)
                {
                    return Ok(new
                    {
                        status = "400",
                        response = "Ok",
                        message = "Process not Complete",
                    });
                }

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
            dynamic _data = null;
            string _SQL, _resData = null;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Process not complete"",
                    ""rows"": null
                }";
            try
            {
                
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

                bool complete = _KBCN.Execute(_SQL
                   , pUser: _BearerClass
                    , pAction: "EXECUTE WITH JQUERY"
                   , pControllerName: _BearerClass.ActionName
                   , pActionName: _spName
                    );

                if (!complete)
                {
                    return Ok(new
                    {
                        status = "400",
                        response = "Ok",
                        message = "Process not Complete",
                    });
                }

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
            dynamic _data = null;
            string _SQL, _resData = null;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""rows"": null
                }";
            try
            {

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
                   , pUser: _BearerClass
                    , pAction: "EXECUTE JSON WITH JQUERY"
                   , pControllerName: _BearerClass.ActionName
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
            dynamic _data = null;
            string _SQL, _resData = null;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""rows"": null
                }";
            try
            {
                
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
                    , pUser: _BearerClass
                    , pAction: "EXECUTE JSON WITH JQUERY"
                   , pControllerName: _BearerClass.ActionName
                   , pActionName: _spName
                    );


                if (_resData != null && _resData != "[]")
                    _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data found"",
                        ""rows"": " + _resData + @"
                    }";

                if (_resData!.Contains("Error"))
                {
                    _result = @"{
                        ""status"":""500"",
                        ""response"":""OK"",
                        ""message"": ""Error found"",
                        ""error"": " + _resData + @"
                    }";
                }

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

        #region APIS 

        #endregion

    }
}
