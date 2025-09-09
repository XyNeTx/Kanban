using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using Newtonsoft.Json;

using HINOSystem.Libs;
using HINOSystem.Context;
using HINOSystem.Models.KB3.Master;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNMS003Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        public KBNMS003Controller(
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
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

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
        public IActionResult search([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                
                _json = JsonConvert.DeserializeObject(pData);

                _SQL = @" EXEC [exec].[spKBNMS003_SEARCH] '" + User.FindFirst(ClaimTypes.Locality).Value + "' ";
                string _jsonData = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"": " + _jsonData + @"
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public IActionResult save()
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                string[] _F = Request.Form["F_Parent_Part_Name"].ToString().Split("-");
                string _F_Parent_Part = (_F.Length > 0 ? _F[0] : "");
                string _F_Ruibetsu = (_F.Length > 0 ? _F[1] : "");

                TB_MS_PartSet _TB_MS_PartSet = new TB_MS_PartSet();
                _TB_MS_PartSet.F_Plant = User.FindFirst(ClaimTypes.Locality).Value;
                _TB_MS_PartSet.F_Parent_Part = _F_Parent_Part;
                _TB_MS_PartSet.F_Ruibetsu = _F_Ruibetsu;
                _TB_MS_PartSet.F_Store_Cd = Request.Form["F_Store_Cd"].ToString();
                _TB_MS_PartSet.F_Start_Date = Request.Form["F_Start_Date"].ToString().Replace("-", "");
                _TB_MS_PartSet.F_End_Date = Request.Form["F_End_Date"].ToString().Replace("-", "");
                _TB_MS_PartSet.F_Update_By = User.FindFirst(ClaimTypes.UserData).Value.ToString();
                _TB_MS_PartSet.F_Update_Date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                _KB3Context.TB_MS_PartSet.Add(_TB_MS_PartSet);
                _KB3Context.SaveChanges();

                string _result = @"{
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




        [HttpPatch]
        public IActionResult save(int id = 0)
        {
            dynamic _json = null;
            string _SQL = "";
            string _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data not found""
                    }";
            try
            {
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                string[] _F = Request.Form["F_Parent_Part_Name"].ToString().Split("-");
                string _F_Parent_Part = (_F.Length > 0 ? _F[0] : "");
                string _F_Ruibetsu = (_F.Length > 0 ? _F[1] : "");

                _SQL = @"
                    UPDATE [dbo].[TB_MS_PartSet]
                    SET F_Plant= '" + User.FindFirst(ClaimTypes.Locality).Value.ToString().Trim() + @"'
                        , F_Parent_Part= '" + _F_Parent_Part + @"'
                        , F_Ruibetsu= '" + _F_Ruibetsu + @"'
                        , F_Store_Cd= '" + _BearerClass.Records.F_Store_Cd.ToString().Trim() + @"'
                        , F_Start_Date= '" + Request.Form["F_Start_Date"].ToString().Replace("-", "") + @"'
                        , F_End_Date= '" + Request.Form["F_End_Date"].ToString().Replace("-", "") + @"'
                        , F_Update_By= '" + User.FindFirst(ClaimTypes.UserData).Value + @"'
                        , F_Update_Date= '" + DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")) + @"'
                    WHERE 1=1
                    AND F_Plant= '" + _BearerClass.Records.F_Plant.ToString().Trim() + @"'
                    AND F_Parent_Part= '" + _BearerClass.Records.F_Parent_Part.ToString().Trim() + @"'
                    AND F_Ruibetsu= '" + _BearerClass.Records.F_Ruibetsu.ToString().Trim() + @"'
                    AND F_Store_Cd= '" + _BearerClass.Records.F_Store_Cd.ToString().Trim() + @"'
                    AND F_Start_Date= '" + _BearerClass.Records.F_Start_Date.ToString().Trim().Replace("-", "") + @"'
                ";
                _KBCN.Execute(_SQL);


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




        [HttpPost]
        public IActionResult delete(int id = 0)
        {
            dynamic _json = null;
            string _SQL = "";
            string _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data not found""
                    }";
            try
            {
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                _SQL = @"
                    DELETE FROM [dbo].[TB_MS_PartSet]
                    WHERE 1=1
                    AND F_Plant= '" + _BearerClass.Records.F_Plant.ToString().Trim() + @"'
                    AND F_Parent_Part= '" + _BearerClass.Records.F_Parent_Part.ToString().Trim() + @"'
                    AND F_Ruibetsu= '" + _BearerClass.Records.F_Ruibetsu.ToString().Trim() + @"'
                    AND F_Store_Cd= '" + _BearerClass.Records.F_Store_Cd.ToString().Trim() + @"'
                    AND F_Start_Date= '" + _BearerClass.Records.F_Start_Date.ToString().Trim().Replace("-", "") + @"'
                ";
                _KBCN.Execute(_SQL);


                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data has been delete""
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
