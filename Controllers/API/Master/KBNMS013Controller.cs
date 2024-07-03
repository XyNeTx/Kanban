using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Models.KB3.Receive_Process;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNMS013Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly KB3Context _KB3Context;
        private readonly SerilogLibs _Log;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNMS013Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context,
            PPM3Context pPM3Context,
            SerilogLibs serilogLibs
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _PPM3Context = pPM3Context;
            _Log = serilogLibs;
        }

        public void setConString()
        {
            try
            {
                if (_KBCN.Plant.ToString() == "3")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB3Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPM3Connection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
                else if (_KBCN.Plant.ToString() == "2")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB2Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPMConnection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
                else if (_KBCN.Plant.ToString() == "1")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB1Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPMConnection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
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

                _KBCN.Plant = _BearerClass.Plant;

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

                _KBCN.Plant = _BearerClass.Plant;

                _json = JsonConvert.DeserializeObject(pData);

                _SQL = @" EXEC [exec].[spKBNMS013_SEARCH] '" + _BearerClass.Plant + "' ";
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
            dynamic _json = null;
            string _SQL = "";
            try
            {
                setConString();
                _BearerClass.Authentication(Request);

                if (_BearerClass.Status == 401) return Unauthorized(new
                {
                    status = "401",
                    response = "Unauthorized",
                    title = "Unauthorized",
                    message = "Please Login First"
                });


                KANBAN.Models.KB3.Receive_Process.TB_MS_PartOrder _TB_MS_PartOrder = new KANBAN.Models.KB3.Receive_Process.TB_MS_PartOrder
                {
                    F_Plant = _BearerClass.Plant[0],
                    F_Supplier_Cd = Request.Form["F_Supplier_Code"].ToString().Split("-")[0],
                    F_Supplier_Plant = Request.Form["F_Supplier_Code"].ToString().Split("-")[1][0],
                    F_Part_No = Request.Form["F_Part_No"].ToString().Split("-")[0],
                    F_Ruibetsu = Request.Form["F_Part_No"].ToString().Split("-")[1],
                    F_Kanban_No = Request.Form["F_Kanban_No"].ToString(),
                    F_Store_Code = Request.Form["F_Store_Code"].ToString(),
                    F_Start_Date = Request.Form["F_Start_Date"].ToString().Replace("-", string.Empty),
                    F_End_Date = Request.Form["F_End_Date"].ToString().Replace("-", string.Empty),
                    F_Type_Order = Request.Form["F_Type_Order"].ToString(),
                    F_Cycle = Request.Form["F_Cycle"].ToString().Replace("-", string.Empty),
                    F_Flg_ClearModule = 1,
                    F_PDS_Group = Request.Form["F_PDS_Group"].ToString(),
                    F_Create_Date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")),
                    F_Create_By = _BearerClass.UserCode.ToString(),
                    F_Update_Date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")),
                    F_Update_by = _BearerClass.UserCode.ToString(),
                    F_Check_Shift = 0,
                    F_Last_Check = string.Empty,
                    F_Next_Check = string.Empty,
                };

                _KB3Context.TB_MS_PartOrder.Add(_TB_MS_PartOrder);
                _KB3Context.SaveChanges();
                _Log.WriteLog($"Action : Save | Database : TB_MS_PartOrder | F_Part_No : {_TB_MS_PartOrder.F_Part_No} | " +
                    $"F_Ruibetsu : {_TB_MS_PartOrder.F_Ruibetsu} | F_Supplier_Cd : {_TB_MS_PartOrder.F_Supplier_Cd} | " +
                    $"F_Supplier_Plant : {_TB_MS_PartOrder.F_Supplier_Plant} | F_Kanban_No : {_TB_MS_PartOrder.F_Kanban_No} | " +
                    $"F_Store_Code : {_TB_MS_PartOrder.F_Store_Code}", _BearerClass.UserCode, _BearerClass.Device);

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
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                _KBCN.Plant = _BearerClass.Plant;

                _SQL = @"
                    UPDATE [dbo].[TB_MS_OldPart]
                    SET F_Part_Name = '" + Request.Form["F_Part_Name"].ToString().Replace("-", "") + @"'
                        ,F_End_Date = '" + Request.Form["F_End_Date"].ToString().Replace("-","") + @"'
                        ,F_Update_By = '" + _BearerClass.UserCode + @"'
                        ,F_Update_Date = '" + DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")) + @"'
                    WHERE 1=1
                    AND F_Plant = '" + Request.Form["F_Plant"].ToString() + @"'
                    AND F_Parent_Part = '" + Request.Form["F_Parent_Part"].ToString() + @"'
                    AND F_Ruibetsu = '" + Request.Form["F_Ruibetsu"].ToString().Replace("-", "") + @"'
                    AND F_Store_Cd = '" + Request.Form["F_Store_Cd"].ToString().Replace("-", "") + @"'
                    AND F_Start_Date = '" + Request.Form["F_Start_Date"].ToString().Replace("-", "") + @"'
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
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                _KBCN.Plant = _BearerClass.Plant;

                _SQL = @"
                    DELETE [dbo].[TB_MS_OldPart]
                    WHERE 1=1
                    AND F_Plant = '" + Request.Form["F_Plant"].ToString() + @"'
                    AND F_Parent_Part = '" + Request.Form["F_Parent_Part"].ToString() + @"'
                    AND F_Ruibetsu = '" + Request.Form["F_Ruibetsu"].ToString().Replace("-", "") + @"'
                    AND F_Store_Cd = '" + Request.Form["F_Store_Cd"].ToString().Replace("-", "") + @"'
                    AND F_Start_Date = '" + Request.Form["F_Start_Date"].ToString().Replace("-", "") + @"'
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
