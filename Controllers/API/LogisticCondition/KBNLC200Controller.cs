using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Models.KB3.LogisticCondition.ViewModel;
using KANBAN.Services;
using KANBAN.Services.Logistical.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNLC200Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;
        private readonly DBConnection _ppmConnect;
        private readonly KB3Context _KB3Context;
        private readonly ILogisticService _LogisticService;


        public KBNLC200Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            KB3Context kB3Context,
            ILogisticService service
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
            _KB3Context = kB3Context;

            _ppmConnect = new DBConnection(_configuration, "ppm");
            _LogisticService = service;
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
                string _jsMS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                _SQL = @" EXEC [exec].[spPPM_T_Supplier] ";
                string _jsPPM_T_Supplier = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_MS_Factory"" : " + _jsMS_Factory + @",
                                ""PPM_T_Supplier"" : " + _jsPPM_T_Supplier + @"
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
        public IActionResult report([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                _json = JsonConvert.DeserializeObject(pData);


                _SQL = @" EXEC [rpt].[spKBNLC200_Report] '"+ _json.Plant + @"'
                            , '"+ _json.Period + @"'
                            , '"+ _json.SupplierFrom + @"'
                            , '"+ _json.SupplierTo + @"'
                            , '" + _BearerClass.UserCode.ToString() + @"'
                            ";
                
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

        [HttpGet]
        public async Task<IActionResult> GetSupplier()
        {
            var result = await _LogisticService.IKBNLC200.Sup_DropDown();
            return Ok(new
            {
                status = "200",
                response = "Success",
                message = "Data has been retrieved",
                data = result
            });
        }


        [HttpPost]
        public async Task<IActionResult> Print(VM_REPORT_KBNLC200 model)
        {
            try
            {
                await _BearerClass.CheckAuthorize();
                await _LogisticService.IKBNLC200.Print(model);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Process Success"
                });
            }
            catch (Exception ex)
            {

                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

       

    }
}
