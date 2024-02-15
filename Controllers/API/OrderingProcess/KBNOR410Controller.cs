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

namespace HINOSystem.Controllers.API.Master
{
    public class KBNOR410Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        public KBNOR410Controller(
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
            dynamic _json = null;
            string _SQL = "";
            try
            {
                dynamic _JBearer = _BearerClass.Header(Request);
                if (_JBearer.Status == 401 || _JBearer.Status == null) return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                _KBCN.Plant = _JBearer.Plant;

                _SQL = @"
                    SELECT F_Plant
                        ,F_Plant_Name
                        ,F_Location
                        ,F_Update_By
                        ,F_Update_Date
                    FROM [dbo].[TB_MS_Factory]
                ";
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _JBearer, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_MS_Factory"" : " + _jsTB_MS_Factory + @"
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
        public IActionResult search([FromBody] string pPostData = null)
        {
            dynamic _bearer, _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";
            try
            {
                _bearer = _BearerClass.Header(Request);
                if (_bearer.Status == 401 || _bearer.Status == null) return Content(JsonConvert.SerializeObject(_bearer), "application/json");

                _KBCN.Plant = _bearer.Plant;

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                _SQL = @" Exec dbo.SP_DisplayUrgent '" + _data.OrderType.ToString() + @"','" + _bearer.Plant + @"','" + _bearer.UserCode + @"' ";

                _resData = _KBCN.ExecuteJSON(_SQL, pUser: _bearer,
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
        public IActionResult interfaceData([FromBody] string pData = null)
        {
            dynamic _bearer, _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";
            try
            {
                _bearer = _BearerClass.Header(Request);
                if (_bearer.Status == 401 || _bearer.Status == null) return Content(JsonConvert.SerializeObject(_bearer), "application/json");

                _KBCN.Plant = _bearer.Plant;

                _SQL = @"   
                        Delete From TB_ORDER 
                        Where F_OrderType = '" + _data.OrderType.ToString() + @"'
                        AND F_Update_By = '" + _bearer.UserCode + @"' 
                        AND F_Plant = '" + _bearer.Plant + @"' ";
                _KBCN.Execute(_SQL, pUser: _bearer,
                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                    pActionName: ControllerContext.ActionDescriptor.ActionName
                    );


                _SQL = @" EXEC [exec].[spKBNOR410_INTERFACE] 
                    '" + _data.OrderType.ToString() + @"',
                    '" + _bearer.Plant + @"',
                    '" + _bearer.UserCode + @"' ";
                DataTable _dtM1 = _KBCN.ExecuteSQL(_SQL, pUser: _bearer,
                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                    pActionName: ControllerContext.ActionDescriptor.ActionName
                    );

                if (_dtM1.Rows.Count > 0)
                {
                    for (int i = 0; i < _dtM1.Rows.Count; i++)
                    {
                        _SQL = @"
                            Select distinct F_Remark
                            From TB_Transaction 
                            WHERE  F_OrderType ='" + _data.OrderType.ToString() + @"' 
                            AND F_Reg_Flg = '1' 
                            AND F_Plant='" + _bearer.Plant + @"'
                            AND (F_Type_SPc ='9Y' OR F_Type_SPc ='T') 
                            AND case when F_Type_SPC ='9Y' then F_Delivery_Date else F_Adv_Deli_Date end ='" + _dtM1.Rows[i]["F_Delivery_Date"].ToString() + @"'
                            and F_Store_CD='" + _dtM1.Rows[i]["F_Store_Cd"].ToString() + @"'
                            and F_Supplier_Cd='" + _dtM1.Rows[i]["F_Supplier_Cd"].ToString() + @"'
                            and F_Supplier_Plant='" + _dtM1.Rows[i]["F_Supplier_Plant"].ToString() + @"'
                            and case when F_Type_SPC ='9Y' then F_Country else '' end ='" + _dtM1.Rows[i]["F_Country"].ToString() + @"'
                            and case when F_Type_SPC ='9Y' then SUBSTRING(F_PDS_NO,1,2) else '' end ='" + _dtM1.Rows[i]["F_Plant"].ToString() + @"'
                            and F_Update_By='" + _bearer.UserCode + @"' ";

                        DataTable _dtM2 = _KBCN.ExecuteSQL(_SQL, pUser: _bearer,
                            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                            pActionName: ControllerContext.ActionDescriptor.ActionName
                            );
                        if (_dtM2.Rows.Count > 0)
                        {
                            String _Remark = "";
                            for (int j = 0; j < _dtM2.Rows.Count; j++)
                            {
                                _Remark += _dtM2.Rows[i]["F_Remark"].ToString();
                            }
                        }
                    }
                }



                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data has been save""
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
