using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNMS004Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        public KBNMS004Controller(
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
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                _SQL = @" EXEC [exec].[spTB_MS_PartOrder] ";
                string _jsTB_Supplier = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_Factory"" : " + _jsTB_Factory + @",
                                ""TB_Supplier"" : " + _jsTB_Supplier + @"
                            }
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                //_BearerClass.Result
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
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                _json = JsonConvert.DeserializeObject(pData);


                _SQL = @" EXEC [exec].[spKBNMS004_SEARCH] '" + _BearerClass.Plant + "' ";
                string _jsonData = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);


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
            string _SQL = "";
            string[] _F;
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                _F = Request.Form["F_Supplier_Code"].ToString().Split("-");
                string _F_Supplier_Cd = (_F.Length > 0 ? _F[0] : "");
                string _F_Supplier_Plant = (_F.Length > 0 ? _F[1] : "");

                _F = Request.Form["F_Part_No"].ToString().Split("-");
                string _F_Part_No = (_F.Length > 0 ? _F[0] : "");
                string _F_Ruibetsu = (_F.Length > 0 ? _F[1] : "");

                TB_MS_PartSpecial TB_MS_PartSpecial = new TB_MS_PartSpecial();
                TB_MS_PartSpecial.F_Plant = _BearerClass.Plant;
                TB_MS_PartSpecial.F_Supplier_Cd = Request.Form["F_Supplier_Cd"].ToString();
                TB_MS_PartSpecial.F_Supplier_Plant = Request.Form["F_Supplier_Plant"].ToString();
                TB_MS_PartSpecial.F_Part_No = Request.Form["F_Part_No"].ToString().Substring(0,10);
                TB_MS_PartSpecial.F_Ruibetsu = Request.Form["F_Part_No"].ToString().Substring(11,2);
                TB_MS_PartSpecial.F_Kanban_No = Request.Form["F_Kanban_No"].ToString();
                TB_MS_PartSpecial.F_Store_Code = Request.Form["F_Store_Code"].ToString();
                TB_MS_PartSpecial.F_Start_Date = Request.Form["F_Start_Date"].ToString().Substring(0,4) + Request.Form["F_Start_Date"].ToString().Substring(5, 2) + Request.Form["F_Start_Date"].ToString().Substring(8, 2);
                TB_MS_PartSpecial.F_End_Date = Request.Form["F_End_Date"].ToString().Substring(0, 4) + Request.Form["F_End_Date"].ToString().Substring(5, 2) + Request.Form["F_End_Date"].ToString().Substring(8, 2);
                TB_MS_PartSpecial.F_Type_Special = Request.Form["F_Type_Special"].ToString();
                TB_MS_PartSpecial.F_Cycle = Request.Form["F_Cycle"].ToString().Replace("-", "");
                TB_MS_PartSpecial.F_Create_By = _BearerClass.UserCode.ToString();
                TB_MS_PartSpecial.F_Create_Date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                _KB3Context.TB_MS_PartSpecial.Add(TB_MS_PartSpecial);
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
            string _SQL = "";
            string[] _F;
            string _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data not found""
                    }";
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                


                _F = _BearerClass.Records.F_Supplier_Code.ToString().Split("-");
                string _F_Supplier_Cd = (_F.Length > 0 ? _F[0] : "");
                string _F_Supplier_Plant = (_F.Length > 0 ? _F[1] : "");

                _F = _BearerClass.Records.F_Part_No.ToString().Split("-");
                string _F_Part_No = (_F.Length > 0 ? _F[0] : "");
                string _F_Ruibetsu = (_F.Length > 0 ? _F[1] : "");

                _SQL = @"
                    UPDATE [dbo].[TB_MS_PartSpecial]
                    SET F_End_Date = '" + Request.Form["F_End_Date"].ToString().Replace("-", "") + @"'
                        ,F_Kanban_No = '" + Request.Form["F_Kanban_No"].ToString() + @"'
                        ,F_Store_Code = '" + Request.Form["F_Store_Code"].ToString() + @"'
                        ,F_Type_Special = '" + Request.Form["F_Type_Special"].ToString() + @"'
                        ,F_Cycle = '" + Request.Form["F_Cycle"].ToString().Replace("-","") + @"'
                        ,F_Update_By = '" + _BearerClass.UserCode + @"'
                        ,F_Update_Date = '" + DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")) + @"'
                    WHERE 1=1
                    AND F_Plant = '" + _BearerClass.Records.F_Plant.ToString() + @"'
                    AND F_Supplier_Cd = '" + _F_Supplier_Cd + @"'
                    AND F_Supplier_Plant = '" + _F_Supplier_Plant + @"'
                    AND F_Part_No = '" + _F_Part_No + @"'
                    AND F_Ruibetsu = '" + _F_Ruibetsu + @"'
                    AND F_Kanban_No = '" + _BearerClass.Records.F_Kanban_No.ToString() + @"'
                    AND F_Store_Code = '" + _BearerClass.Records.F_Store_Code.ToString() + @"'
                    AND F_Start_Date = '" + _BearerClass.Records.F_Start_Date.ToString().Replace("-", "") + @"'
                ";
                _KBCN.Execute(_SQL);

                //_BearerClass.Records.F_Start_Date.ToString().Trim().Replace("-", "")

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
            string _SQL = "";
            string[] _F;
            string _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Data not found""
                    }";
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                


                _F = _BearerClass.Records.F_Supplier_Code.ToString().Split("-");
                string _F_Supplier_Cd = (_F.Length > 0 ? _F[0] : "");
                string _F_Supplier_Plant = (_F.Length > 0 ? _F[1] : "");

                _F = _BearerClass.Records.F_Part_No.ToString().Split("-");
                string _F_Part_No = (_F.Length > 0 ? _F[0] : "");
                string _F_Ruibetsu = (_F.Length > 0 ? _F[1] : "");

                _SQL = @"
                    DELETE [dbo].[TB_MS_PartSpecial]
                    WHERE 1=1
                    AND F_Plant = '" + _BearerClass.Records.F_Plant.ToString() + @"'
                    AND F_Supplier_Cd = '" + _F_Supplier_Cd + @"'
                    AND F_Supplier_Plant = '" + _F_Supplier_Plant + @"'
                    AND F_Part_No = '" + _F_Part_No + @"'
                    AND F_Ruibetsu = '" + _F_Ruibetsu + @"'
                    AND F_Kanban_No = '" + _BearerClass.Records.F_Kanban_No.ToString() + @"'
                    AND F_Store_Code = '" + _BearerClass.Records.F_Store_Code.ToString() + @"'
                    AND F_Start_Date = '" + _BearerClass.Records.F_Start_Date.ToString().Replace("-", "") + @"'
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
