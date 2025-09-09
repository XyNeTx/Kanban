using HINOSystem.Context;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNOR460EXController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        private readonly string StoragePath = @"wwwroot\Storage\DownloadTemp";
        private readonly TextFileClass _textFileClass;


        public KBNOR460EXController(
            IConfiguration configuration,
            BearerClass bearerClass,
            TextFileClass textFileClass,
            KanbanConnection kanbanConnection,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _textFileClass = textFileClass;
            _KBCN = kanbanConnection;
            _KB3Context = kB3Context;

        }



        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";

            _BearerClass.Authentication();
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            try
            {
                

                _SQL = @" EXEC [exec].[spKBNOR460EX_INI_PDS] 
                    'U',
                    '" + User.FindFirst(ClaimTypes.Locality).Value + @"',
                    '" + User.FindFirst(ClaimTypes.UserData).Value + @"',
                    '' ";
                string _jsPDSNo = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                _SQL = @" EXEC [exec].[spKBNOR460EX_INI_SUPPLIER] 
                    'U',
                    '" + User.FindFirst(ClaimTypes.Locality).Value + @"',
                    '" + User.FindFirst(ClaimTypes.UserData).Value + @"',
                    '' ";
                string _jsSupplier = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""PDSNo"" : " + _jsPDSNo + @",
                                ""Supplier"" : " + _jsSupplier + @"
                            }
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public async Task<IActionResult> write([FromBody] string pPostData = null)
        {
            dynamic _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";

            _BearerClass.Authentication();
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            try
            {
                

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                await _textFileClass.WriteLine(
                    filePath: _data.File.ToString(), 
                    text: _data.Text.ToString()
                    );

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Export data complete"",
                    ""data"": """ + _data.File.ToString() + @"""
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
