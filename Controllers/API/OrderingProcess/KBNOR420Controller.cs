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
    public class KBNOR420Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        public KBNOR420Controller(
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

            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            try
            {
                _KBCN.Plant = _BearerClass.Plant;

                //_SQL = @"
                //    SELECT F_Plant
                //        ,F_Plant_Name
                //        ,F_Location
                //        ,F_Update_By
                //        ,F_Update_Date
                //    FROM [dbo].[TB_MS_Factory]
                //";
                //string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

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



        [HttpPost]
        public IActionResult calculateData([FromBody] string pPostData = null)
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
                _KBCN.Plant = _BearerClass.Plant;

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                _SQL = @" EXEC [exec].[spKBNOR420_CAL02] 
                    '" + _data.OrderType.ToString() + @"',
                    '" + _BearerClass.Plant + @"',
                    '" + _BearerClass.UserCode + @"',
                    '' ";
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

                        _SQL = @" EXEC [exec].[spKBNOR420_CAL02_S] 
                            '" + _data.OrderType.ToString() + @"',
                            '" + _BearerClass.Plant + @"',
                            '" + _BearerClass.UserCode + @"',
                            '" + _dtM1.Rows[i]["F_Store_Cd"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Supplier_Cd"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Supplier_Plant"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Delivery_Date"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Delivery_Trip"].ToString() + @"',
                            '' ";

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
                        _SQL = @" EXEC [exec].[spKBNOR420_CAL02_U] 
                            '" + _data.OrderType.ToString() + @"',
                            '" + _BearerClass.Plant + @"',
                            '" + _BearerClass.UserCode + @"',
                            '" + _dtM1.Rows[i]["F_Store_Cd"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Supplier_CD"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Supplier_Plant"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Delivery_Date"].ToString() + @"',
                            '" + _dtM1.Rows[i]["F_Delivery_Trip"].ToString() + @"',
                            '" + _Remark + @"',
                            '' ";
                        _KBCN.Execute(_SQL, pUser: _BearerClass,
                                        pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                        pActionName: ControllerContext.ActionDescriptor.ActionName
                                        );
                    }
                }


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
                _SQL = @" EXEC [exec].[spKBNOR420_EXCEPTION]
                    '" + _data.OrderType.ToString() + @"',
                    '" + _BearerClass.Plant + @"',
                    '" + _BearerClass.UserCode + @"' ";
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



        [HttpPost]
        public IActionResult updateDockCode([FromBody] string pPostData = null)
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
                _KBCN.Plant = _BearerClass.Plant;

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                _SQL = @" EXEC [exec].[spKBNOR420_CAL10] 
                    '" + _data.OrderType.ToString() + @"',
                    '" + _BearerClass.Plant + @"',
                    '" + _BearerClass.UserCode + @"',
                    '' ";
                DataTable _dtChk = _KBCN.ExecuteSQL(_SQL, pUser: _BearerClass,
                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                    pActionName: ControllerContext.ActionDescriptor.ActionName
                    );

                if (_dtChk.Rows.Count > 0)
                {
                    for (int i = 0; i < _dtChk.Rows.Count; i++)
                    {
                        String _Remark = "";

                        _SQL = @" EXEC [exec].[spKBNOR420_CAL10_S] 
                            '" + _data.OrderType.ToString() + @"',
                            '" + _BearerClass.Plant + @"',
                            '" + _BearerClass.UserCode + @"',
                            '" + _dtChk.Rows[i]["F_Supplier_Cd"].ToString() + @"',
                            '" + _dtChk.Rows[i]["F_Supplier_Plant"].ToString() + @"',
                            '" + _dtChk.Rows[i]["F_Delivery_Date"].ToString() + @"',
                            '" + _dtChk.Rows[i]["F_Delivery_Trip"].ToString() + @"',
                            '' ";

                        DataTable _daChkPOM = _KBCN.ExecuteSQL(_SQL, pUser: _BearerClass,
                            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                            pActionName: ControllerContext.ActionDescriptor.ActionName
                            );
                        if (_daChkPOM.Rows.Count > 0)
                        {
                            _SQL = @" EXEC [exec].[spKBNOR420_CAL10_U] 
                                '" + _data.OrderType.ToString() + @"',
                                '" + _BearerClass.Plant + @"',
                                '" + _BearerClass.UserCode + @"',
                                '" + _dtChk.Rows[i]["F_Supplier_Cd"].ToString() + @"',
                                '" + _dtChk.Rows[i]["F_Supplier_Plant"].ToString() + @"',
                                '" + _dtChk.Rows[i]["F_Delivery_Trip"].ToString() + @"',
                                '" + _dtChk.Rows[i]["F_Cycle_Time"].ToString() + @"',
                                '" + _daChkPOM.Rows[0]["F_Dock_Cd"].ToString() + @"',
                                '' ";
                            _KBCN.Execute(_SQL, pUser: _BearerClass,
                                            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                            pActionName: ControllerContext.ActionDescriptor.ActionName
                                            );                            
                        }
                    }
                }


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
                _SQL = @" EXEC [exec].[spKBNOR420_EXCEPTION]
                    '" + _data.OrderType.ToString() + @"',
                    '" + _BearerClass.Plant + @"',
                    '" + _BearerClass.UserCode + @"' ";
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



        [HttpPost]
        public IActionResult summaryRemark([FromBody] string pPostData = null)
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
                _KBCN.Plant = _BearerClass.Plant;

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                _SQL = @" EXEC [exec].[spKBNOR420_CAL11] 
                    '" + _data.OrderType.ToString() + @"',
                    '" + _BearerClass.Plant + @"',
                    '" + _BearerClass.UserCode + @"',
                    '' ";
                DataTable _dtChk = _KBCN.ExecuteSQL(_SQL, pUser: _BearerClass,
                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                    pActionName: ControllerContext.ActionDescriptor.ActionName
                    );

                if (_dtChk.Rows.Count > 0)
                {
                    String _Detail = "", _Key = "";
                    for (int i = 0; i < _dtChk.Rows.Count; i++)
                    {
                        if(i > 0 && _Key != _dtChk.Rows[i]["F_SUpplier_CD"].ToString() + _dtChk.Rows[i]["F_SUpplier_Plant"].ToString()
                                + _dtChk.Rows[i]["F_Store_Cd"].ToString()
                                + _dtChk.Rows[i]["F_Delivery_Date"].ToString() + _dtChk.Rows[i]["F_Delivery_Trip"].ToString())
                        {
                            //_Detail = Mid(_Detail, 1, Len(_Detail) - 1)

                            _SQL = @" EXEC [exec].[spKBNOR420_CAL11_U] 
                                '" + _data.OrderType.ToString() + @"',
                                '" + _BearerClass.Plant + @"',
                                '" + _BearerClass.UserCode + @"',
                                '" + _Detail + @"',
                                '" + _Key + @"',
                                '' ";
                            _KBCN.Execute(_SQL, pUser: _BearerClass,
                                            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                            pActionName: ControllerContext.ActionDescriptor.ActionName
                                            );
                            _Detail = "";
                        }
                        _Detail += _dtChk.Rows[i]["F_Remark"].ToString() + ",";
                        _Key = _dtChk.Rows[i]["F_SUpplier_CD"].ToString() + _dtChk.Rows[i]["F_SUpplier_Plant"].ToString()
                            + _dtChk.Rows[i]["F_Store_Cd"].ToString()
                            + _dtChk.Rows[i]["F_Delivery_Date"].ToString()+ _dtChk.Rows[i]["F_Delivery_Trip"].ToString();

                    }

                    //nDetail = Mid(nDetail, 1, Len(nDetail) - 1)
                    _SQL = @" EXEC [exec].[spKBNOR420_CAL11_U] 
                        '" + _data.OrderType.ToString() + @"',
                        '" + _BearerClass.Plant + @"',
                        '" + _BearerClass.UserCode + @"',
                        '" + _Detail + @"',
                        '" + _Key + @"',
                        '' ";
                    _KBCN.Execute(_SQL, pUser: _BearerClass,
                                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                    pActionName: ControllerContext.ActionDescriptor.ActionName
                                    );
                }


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
                _SQL = @" EXEC [exec].[spKBNOR420_EXCEPTION]
                    '" + _data.OrderType.ToString() + @"',
                    '" + _BearerClass.Plant + @"',
                    '" + _BearerClass.UserCode + @"' ";
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
