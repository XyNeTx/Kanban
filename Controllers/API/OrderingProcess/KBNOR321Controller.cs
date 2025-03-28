using HINOSystem.Context;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNOR321Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        public KBNOR321Controller(
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
        public IActionResult initial([FromBody] string pPostData = null)
        {
            dynamic _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";

            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            try
            {


                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                //_SQL = @" EXEC [exec].[spKBNOR310] '"
                //    + _BearerClass.Plant + @"','"
                //    + _BearerClass.UserCode + @"','"
                //    + _data.ProcessDate.ToString().Replace("-", "") + @"','"
                //    + _data.ProcessShift.ToString() + @"','' ";

                //_resData = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass,
                //    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                //    pActionName: ControllerContext.ActionDescriptor.ActionName
                //    );

                //_result = @"{
                //    ""status"":""200"",
                //    ""response"":""OK"",
                //    ""message"": ""Data Found"",
                //    ""data"": " + _resData + @"
                //}";

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"": null
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        //[HttpPost]
        //public IActionResult search([FromBody] string pPostData = null)
        //{
        //    dynamic _data = null;
        //    string _SQL, _resData;
        //    string _result = @"{
        //            ""status"":""200"",
        //            ""response"":""OK"",
        //            ""message"": ""No data found"",
        //            ""data"": null
        //        }";

        //    _BearerClass.Authentication(Request);
        //    if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

        //    try
        //    {
        //        

        //        if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

        //        _SQL = @" EXEC [exec].[spKBNOR310] '"
        //            + _BearerClass.Plant + @"','"
        //            + _BearerClass.UserCode + @"','"
        //            + _data.ProcessDate.ToString().Replace("-", "") + @"','"
        //            + _data.ProcessShift.ToString() + @"','' ";

        //        _resData = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass,
        //            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
        //            pActionName: ControllerContext.ActionDescriptor.ActionName
        //            );

        //        _result = @"{
        //            ""status"":""200"",
        //            ""response"":""OK"",
        //            ""message"": ""Data Found"",
        //            ""data"": " + _resData + @"
        //        }";
        //        return Content(_result, "application/json");
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(e.Message.ToString(), "application/json");
        //    }
        //}




    }
}
