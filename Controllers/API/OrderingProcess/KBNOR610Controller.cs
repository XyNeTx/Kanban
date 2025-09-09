using Microsoft.AspNetCore.Mvc;
using System.Data;

using System.Security.Claims;

using Newtonsoft.Json;

using HINOSystem.Libs;
using HINOSystem.Context;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNOR610Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        public KBNOR610Controller(
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
            try
            {
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                _SQL = @" [exec].[spKBNOR600] '" + _data.OrderType.ToString() + @"','" + User.FindFirst(ClaimTypes.Locality).Value + @"','" + User.FindFirst(ClaimTypes.UserData).Value + @"' ";

                _resData = _KBCN.ExecuteJSON(_SQL
                    , pUser: _BearerClass
                    , pControllerName: ControllerContext.ActionDescriptor.ControllerName
                    , pActionName: ControllerContext.ActionDescriptor.ActionName
                    );

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                        ""data"": " + _resData + @"
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public IActionResult search([FromBody] string pPostData = null)
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
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                _SQL = @" Exec dbo.SP_DisplayUrgent '" + _data.OrderType.ToString() + @"','" + User.FindFirst(ClaimTypes.Locality).Value + @"','" + User.FindFirst(ClaimTypes.UserData).Value + @"' ";

                _resData = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass,
                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                    pActionName: ControllerContext.ActionDescriptor.ActionName
                    );

                if (_resData != null)
                    _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data Found"",
                        ""data"": " + _resData + @"
                    }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public IActionResult interfaceData([FromBody] string pPostData = null)
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
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                //Clear Old Data
                _SQL = @" EXEC [exec].[spKBNOR410_INTERFACE_D1] 
                    '" + _data.OrderType.ToString() + @"',
                    '" + User.FindFirst(ClaimTypes.Locality).Value + @"',
                    '" + User.FindFirst(ClaimTypes.UserData).Value + @"' ";
                _KBCN.Execute(_SQL, pUser: _BearerClass,
                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                    pActionName: ControllerContext.ActionDescriptor.ActionName
                    );


                _SQL = @" EXEC [exec].[spKBNOR410_INTERFACE_M1] 
                    '" + _data.OrderType.ToString() + @"',
                    '" + User.FindFirst(ClaimTypes.Locality).Value + @"',
                    '" + User.FindFirst(ClaimTypes.UserData).Value + @"' ";
                DataTable _dtM1 = _KBCN.ExecuteSQL(_SQL, pUser: _BearerClass,
                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                    pActionName: ControllerContext.ActionDescriptor.ActionName
                    );

                //Clear Data of Remark
                if (_dtM1.Rows.Count > 0)
                {
                    for (int i = 0; i < _dtM1.Rows.Count; i++)
                    {
                        String _Remark = "";

                        _SQL = @" EXEC [exec].[spKBNOR410_INTERFACE_M2] 
                            '" + _data.OrderType.ToString() + @"',
                            '" + User.FindFirst(ClaimTypes.Locality).Value + @"',
                            '" + User.FindFirst(ClaimTypes.UserData).Value + @"',
                            '" + _dtM1.Rows[i]["F_Delivery_Date"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Store_Cd"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Supplier_Cd"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Supplier_Plant"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Country"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Plant"].ToString() + @"' ";

                        DataTable _dtM2 = _KBCN.ExecuteSQL(_SQL, pUser: _BearerClass,
                            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                            pActionName: ControllerContext.ActionDescriptor.ActionName
                            );
                        if (_dtM2.Rows.Count > 0)
                        {
                            for (int j = 0; j < _dtM2.Rows.Count; j++)
                            {
                                _Remark += _dtM2.Rows[i]["F_Remark"].ToString();
                            }
                        }

                        //===Update Volume
                        _SQL = @" EXEC [exec].[spKBNOR410_INTERFACE_M3] 
                            '" + _data.OrderType.ToString() + @"',
                            '" + User.FindFirst(ClaimTypes.Locality).Value + @"',
                            '" + User.FindFirst(ClaimTypes.UserData).Value + @"',
                            '" + _dtM1.Rows[i]["F_Delivery_Date"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Store_Cd"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Supplier_Cd"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Supplier_Plant"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Country"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Plant"].ToString() + @"',
                            '" + _Remark + @"' ";
                        _KBCN.Execute(_SQL, pUser: _BearerClass,
                                        pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                        pActionName: ControllerContext.ActionDescriptor.ActionName
                                        );
                    }
                }
                // ======= End of Remark

                _SQL = @" EXEC [exec].[spKBNOR410_INTERFACE_M4] 
                            '" + _data.OrderType.ToString() + @"',
                            '" + User.FindFirst(ClaimTypes.Locality).Value + @"',
                            '" + User.FindFirst(ClaimTypes.UserData).Value + @"' ";
                _KBCN.Execute(_SQL, pUser: _BearerClass,
                            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                            pActionName: ControllerContext.ActionDescriptor.ActionName
                            );


                //Tacoma Only
                _SQL = @" EXEC [exec].[spKBNOR410_INTERFACE_TACOMA] 
                            '" + _data.OrderType.ToString() + @"',
                            '" + User.FindFirst(ClaimTypes.Locality).Value + @"',
                            '" + User.FindFirst(ClaimTypes.UserData).Value + @"' ";
                _KBCN.Execute(_SQL, pUser: _BearerClass,
                            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                            pActionName: ControllerContext.ActionDescriptor.ActionName
                            );


                //Update F_Reg_Flg=2
                _SQL = @" EXEC [exec].[spKBNOR410_INTERFACE_M5] 
                            '" + _data.OrderType.ToString() + @"',
                            '" + User.FindFirst(ClaimTypes.Locality).Value + @"',
                            '" + User.FindFirst(ClaimTypes.UserData).Value + @"' ";
                _KBCN.Execute(_SQL, pUser: _BearerClass,
                            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                            pActionName: ControllerContext.ActionDescriptor.ActionName
                            );



                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Process Data Complete""
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                //Delete TB_ORDER AND Update F_Reg_Flg=2==>1
                _SQL = @" EXEC [exec].[spKBNOR410_EXCEPTION]
                    '" + _data.OrderType.ToString() + @"',
                    '" + User.FindFirst(ClaimTypes.Locality).Value + @"',
                    '" + User.FindFirst(ClaimTypes.UserData).Value + @"' ";
                _KBCN.Execute(_SQL, pUser: _BearerClass,
                            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                            pActionName: ControllerContext.ActionDescriptor.ActionName
                            );


                _result = @"{
                    ""status"":""200"",
                    ""response"":""NO"",
                    ""message"": ""Process Not Complete!!!""
                }";
                return Content(_result, "application/json");
            }
        }



    }
}
