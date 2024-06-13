using Microsoft.AspNetCore.Http;
using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using HINOSystem.Libs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace HINOSystem.Controllers.API.erp
{
    public class HistoryLoginController : Controller
    {
        private readonly WarrantyClaimConnect _wrtConnect;
        private readonly KanbanConnection _KBCN;
        private readonly BearerClass _BearerClass;

        public HistoryLoginController(
            WarrantyClaimConnect wrtConnect,
            KanbanConnection kanbanConnection,
            BearerClass bearerClass)
        {
            _wrtConnect = wrtConnect;
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
        }



        [HttpPost]
        public IActionResult getHistory([FromBody] string pData = null)
        {
            string _sql = "", _result = "";
            dynamic _json = JsonConvert.DeserializeObject(pData);
            try
            {
                _KBCN.Plant = 3;
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");
                _KBCN.Plant = _BearerClass.Plant;

                _sql = @" EXEC [erp].[HistorySQL] '" + _json.Action + @"' ";
                string _HistorySQL = _KBCN.ExecuteJSON(_sql, skipLog: true);

                _sql = @" EXEC [erp].[HistoryAction] '" + _json.Action + @"', '" + _json.Date + @"', '" + _json.DateTo + @"', '" + (_json.Exclude == 1 ? "search" : "") + @"' ";
                string _HistoryAction = _KBCN.ExecuteJSON(_sql, skipLog: true);

                _sql = @" EXEC [erp].[HistoryFailed] '" + _json.Action + @"', '" + _json.FailedDate + @"', '" + _json.FailedDateTo + @"' ";
                string _HistoryFailed = _KBCN.ExecuteJSON(_sql, skipLog: true);

                _sql = @" EXEC [erp].[HistoryLogin] 'KB3', '" + _json.UserCode + @"', '" + _json.Date + @"' ";
                string _HistoryLogin = _KBCN.ExecuteJSON(_sql, skipLog: true);

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"": 
                        {
                            ""sql"" : "  + (_HistorySQL != "" && _HistorySQL != null ? _HistorySQL : @"""""") + @",
                            ""action"" : " + (_HistoryAction != "" && _HistoryAction != null ? _HistoryAction : @"""""") + @",
                            ""failed"" : " + (_HistoryFailed != "" && _HistoryFailed != null ? _HistoryFailed : @"""""" ) + @",
                            ""login"" : " + (_HistoryLogin != "" && _HistoryLogin != null ? _HistoryLogin : @"""""") + @"
                        }
                    }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                _result = @"{
                        ""status"":""200"",
                        ""response"":""ERROR"",
                        ""message"": " + e.Message.ToString() + @"
                    }";
                return Content(_result, "application/json");

            }
        }





        [HttpPost]
        public IActionResult getData([FromBody] string pData = null)
        {
            string _sql = "", _result = "";
            dynamic _json = JsonConvert.DeserializeObject(pData);
            try
            {
                _sql = @"
                    SELECT SystemName
                        , UserCode AS Code
                        , FORMAT(ActionAt, 'yyyy-MM-dd') AS ActionAt
	                    , u.Name, u.Surname
	                    , u.NameTH, u.SurnameTH
                    FROM [log].[Action] a
	                    LEFT JOIN [erp].[User] u ON u.Code = a.UserCode
                    WHERE 1=1
                    AND SystemName = '" + _json.System + @"' 
                    AND UserCode != ''
                    AND ActionType IN ('LOGIN','LOGOUT')

                    GROUP BY SystemName, UserCode, FORMAT(ActionAt, 'yyyy-MM-dd') 
	                    , u.Name, u.Surname
	                    , u.NameTH, u.SurnameTH

                    ORDER BY FORMAT(ActionAt, 'yyyy-MM-dd') DESC

                ";
                string _strSystem = _KBCN.ExecuteJSON(_sql);


                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":" + _strSystem + @"
                }";


                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                _result = @"{
                        ""status"":""200"",
                        ""response"":""ERROR"",
                        ""message"": " + e.Message.ToString() + @"
                    }";
                return Content(_result, "application/json");

            }
        }



        [HttpPost]
        public IActionResult getDetail([FromBody] string pData = null)
        {
            string _sql = "", _result = "";
            dynamic _json = JsonConvert.DeserializeObject(pData);
            try
            {
                _sql = @"

					SELECT *
					FROM (

                    SELECT Token
						, UserCode AS Code
						, FORMAT(MIN(ActionAt),'HH:mm:ss') StartAt, FORMAT(MAX(ActionAt),'HH:mm:ss') FinishAt
						, DATEDIFF(MINUTE, MIN (ActionAt), MAX (ActionAt)) AS ProcessTime
                    FROM [log].[Action] a
                    WHERE 1=1
                    AND SystemName = '" + _json.System + @"' 
                    AND UserCode != ''
					AND UserCode = '" + _json.UserCode + @"' 
					AND FORMAT(ActionAt,'yyyy-MM-dd') = '" + _json.Date + @"'

					GROUP BY Token
						, UserCode
						, FORMAT(ActionAt,'yyyy-MM-dd')
					) a

                    ORDER BY StartAt DESC

                ";
                string _strSystem = _KBCN.ExecuteJSON(_sql);


                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":" + _strSystem + @"
                }";


                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                _result = @"{
                        ""status"":""200"",
                        ""response"":""ERROR"",
                        ""message"": " + e.Message.ToString() + @"
                    }";
                return Content(_result, "application/json");

            }
        }



        [HttpPost]
        public IActionResult getDescription([FromBody] string pData = null)
        {
            string _sql = "", _result = "";
            dynamic _json = JsonConvert.DeserializeObject(pData);
            try
            {
                _sql = @"
					SELECT _ID
                        , UserCode
                        , Token
                        , ActionType
                        , FORMAT(ActionAt,'yyyy-MM-dd') AS ActionDate
                        , FORMAT(ActionAt,'HH:mm:ss') AS ActionAt
                        , SystemName
                        , ControllerName
                        , ActionName
                        , Result
                        , Message
                        , SQL
                    FROM [log].[Action] a
					WHERE 1=1
					AND Token = '" + _json.Session + @"'
					ORDER BY ActionAt ASC
                ";
                string _strSystem = _KBCN.ExecuteJSON(_sql);


                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":" + _strSystem + @"
                }";


                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                _result = @"{
                        ""status"":""200"",
                        ""response"":""ERROR"",
                        ""message"": " + e.Message.ToString() + @"
                    }";
                return Content(_result, "application/json");

            }
        }

    }
}
