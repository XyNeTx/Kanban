using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Models.KB3.ImportData.Model;
using KANBAN.Models.KB3.ImportData.ViewModel;
using KANBAN.Services;
using KANBAN.Services.Import.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class KBNIM012MController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;        
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;

        private readonly KB3Context _KB3Context;
        private readonly IImportService _importService;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNIM012MController(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context,
            IImportService importService
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _importService = importService;

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

                if (pData != null) _json = JsonConvert.DeserializeObject(pData);

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

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



        //[HttpPost]
        //public IActionResult search([FromBody] string pData = null)
        //{
        //    dynamic _json = null;
        //    string _SQL = "";
        //    try
        //    {
        //        _BearerClass.Authentication();
        //        if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

        //        _json = JsonConvert.DeserializeObject(pData);


        //        _SQL = @" EXEC [exec].[spKBNMS001_SEARCH] '" + _json.F_Plant + "' ";
                
        //        string _jsonData = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);



        //        string _result = @"{
        //            ""status"":""200"",
        //            ""response"":""OK"",
        //            ""message"": ""Data Found"",
        //            ""data"": " + _jsonData + @"
        //        }";
        //        return Content(_result, "application/json");
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(e.Message.ToString(), "application/json");
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> FormLoad()
        {
            try
            {
                var import = await _importService.KBNIM012M.FormLoadKBNIM012M("ProcDb");
                var condition = await _importService.KBNIM012M.FormLoadKBNIM012M("KanBanDb");

                return Ok(new
                {
                    import = import,
                    condition = condition
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected Error!",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SupplierCode()
        {
            var supplierCode = await _importService.KBNIM012M.Supplier_DropDown();

            return Ok(new { data = supplierCode });
        }

        [HttpGet]
        public async Task<IActionResult> KanBanNo(string? supplierCode)
        {
            var result = await _importService.KBNIM012M.KanBan_DropDown(supplierCode);

            return Ok(new { data = result });

        }

        [HttpGet]
        public async Task<IActionResult> SupplierName(string? supplierCode)
        {
            var result = await _importService.KBNIM012M.SupplierName(supplierCode);

            string supplierName = result.F_short_name.Trim() + " : " + result.F_name;

            return Ok(new { data = supplierName });

        }

        [HttpGet]
        public async Task<IActionResult> StoreCode(string? supplierCode, string? kanbanNo)
        {
            var result = await _importService.KBNIM012M.StoreCode_DropDown(supplierCode, kanbanNo);
            return Ok(new { data = result });
        }

        [HttpGet]
        public async Task<IActionResult> PartNoList(string? supplierCode, string? kanbanNo, string? storeCode)
        {
            var result = await _importService.KBNIM012M.PartNoList(supplierCode, storeCode,kanbanNo);
            return Ok(new { data = result });
        }

        [HttpGet]
        public async Task<IActionResult> PartName(string partNo)
        {
           
            var result = await _importService.KBNIM012M.PartName(partNo);
            return Ok(new { data = result });

        }

        [HttpPost]
        public async Task<IActionResult> Search([FromBody] VM_KBNIM012M model)
        {
            try
            {
                
                var userID = User.FindFirst(ClaimTypes.UserData).Value;
                var plant_CTL = User.FindFirst(ClaimTypes.Locality).Value;
                var result = await _importService.KBNIM012M.Search(model);

                return Ok(new { data = result });

            }
            catch (Exception ex)
            {

                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveForeCast(List<TB_IMPORT_FORECAST_TEMP> model)
        {
            try
            {
                
                var userID = User.FindFirst(ClaimTypes.UserData).Value;
                var plant_CTL = User.FindFirst(ClaimTypes.Locality).Value;

                await _importService.KBNIM012M.Save(model);

                return Ok(new { data = "save Data Success!" });

            }
            catch (Exception ex)
            {

                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(VM_KBNIM012M model)
        {
            try
            {
                await _importService.KBNIM012M.Transfer(model);
                return Ok(new { data = "Transfer Data Completed" });
            }
            catch (Exception ex)
            {

                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Report(VM_KBNIM012M model)
        {
            try
            {
                
                await _importService.KBNIM012M.Report(model);
                return Ok(new { data = "Print Report Success!" });
            }
            catch (Exception ex)
            {

                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportData([FromForm] VM_KBNIM012M model)
        {
            try
            {
                
                await _importService.KBNIM012M.Import(model);
                return Ok(new { data = "Import Completed!" });
            }
            catch (Exception ex)
            {

                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
