using Microsoft.AspNetCore.Mvc;
using System.Data;

using Newtonsoft.Json;
using System.Globalization;

using HINOSystem.Context;
using HINOSystem.Controllers;
using HINOSystem.Libs;

namespace KANBAN.Controllers
{
    public class KBNOR440Controller : Controller
    {
        private readonly AuthenGuard _authenGuard;
        private readonly ILogger<KBNOR440Controller> _logger;

        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;


        public KBNOR440Controller(
            ILogger<KBNOR440Controller> logger,
            AuthenGuard authenGuard,
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            KB3Context kB3Context
            )
        {
            _authenGuard = authenGuard;
            _authenGuard.ComponentItem = "";
            _logger = logger;

            _configuration = configuration;
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
            _KB3Context = kB3Context;
        }



        public IActionResult calculateData()
        {
            dynamic _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";

            try
            {


                //    return _authenGuard.guard(ControllerContext, pViewPage: "OrderingProcess/KBNOR440.cshtml");
            }
            catch (Exception e)
            {
                //    //_SQL = @" EXEC [exec].[spKBNOR440_EXCEPTION]
                //    //        '" + _data.OrderType.ToString() + @"',
                //    //        '" + _BearerClass.Plant + @"',
                //    //        '" + _BearerClass.UserCode + @"' ";
                //    //_KBCN.Execute(_SQL, pUser: _BearerClass,
                //    //            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                //    //            pActionName: ControllerContext.ActionDescriptor.ActionName
                //    //            );


                //    _result = @"{
                //            ""status"":""200"",
                //            ""response"":""NO"",
                //            ""message"": ""Process Not Complete!!!""
                //        }";

                //    return _authenGuard.guard(ControllerContext, pViewPage: "OrderingProcess/KBNOR440.cshtml");
            }
            finally { _data = null; }
            {
                return _authenGuard.guard(ControllerContext, pViewPage: "OrderingProcess/KBNOR440.cshtml");

            }

        }
    }
}
