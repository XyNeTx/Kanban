using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System.Linq;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT120Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNRT120Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            PPMInvenContext pPMInvenContext,
            PPM3Context pPM3Context,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _PPM3Context = pPM3Context;
            _PPMInvenContext = pPMInvenContext;
        }


        public async Task<IActionResult> Initial()
        {
            try
            {
                
                string _result = "";

                var supDB = await _KB3Context.TB_MS_PartOrder.Select(x => new
                {
                    Sup_CD = x.F_Supplier_Cd + '-' + x.F_Supplier_Plant,
                }).OrderBy(x => x.Sup_CD).Distinct().ToListAsync();

                var sebango = await _KB3Context.TB_MS_PartOrder
                .OrderByDescending(x => x.F_Supplier_Cd)
                .Select(x => new
                {
                    F_Sebango = x.F_Kanban_No
                }).Distinct().ToListAsync();

                var prtNo = await _KB3Context.TB_MS_PartOrder
                .OrderByDescending(x => x.F_Supplier_Cd)
                .Select(x => new
                {
                    Prt_No = x.F_Part_No.Trim() + '-' + x.F_Ruibetsu.Trim()
                }).Distinct().ToListAsync();

                var storeCD = await _KB3Context.TB_MS_PartOrder.OrderByDescending(x => x.F_Supplier_Cd).Select(x => new
                {
                    F_Store_CD = x.F_Store_Code
                }).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(supDB);
                string _jsonData2 = JsonConvert.SerializeObject(sebango);
                string _jsonData3 = JsonConvert.SerializeObject(prtNo);
                string _jsonData4 = JsonConvert.SerializeObject(storeCD);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @",
                                    ""data2"": " + _jsonData2 + @",
                                    ""data3"": " + _jsonData3 + @",
                                    ""data4"": " + _jsonData4 + @"
                                    }";
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<IActionResult> OnSupplierChange([FromBody] string data)
        {
            try
            {
                
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];

                var sebango = await _KB3Context.TB_MS_PartOrder
                .Where(x => (x.F_Supplier_Cd + '-' + x.F_Supplier_Plant).CompareTo(supFrom) >= 0 && (x.F_Supplier_Cd + '-' + x.F_Supplier_Plant).CompareTo(supTo) <= 0)
                .Select(x => new
                {
                    F_Sebango = x.F_Kanban_No
                }).Distinct().OrderBy(x => x.F_Sebango).ToListAsync();

                var prtNo = await _KB3Context.TB_MS_PartOrder
                .Where(x => (x.F_Supplier_Cd + '-' + x.F_Supplier_Plant).CompareTo(supFrom) >= 0 && (x.F_Supplier_Cd + '-' + x.F_Supplier_Plant).CompareTo(supTo) <= 0)
                .Select(x => new
                {
                    Prt_No = x.F_Part_No.Trim() + '-' + x.F_Ruibetsu.Trim()
                }).Distinct().OrderBy(x => x.Prt_No).ToListAsync();

                var storeCD = await _KB3Context.TB_MS_PartOrder.OrderByDescending(x => x.F_Supplier_Cd)
                .Where(x => (x.F_Supplier_Cd + '-' + x.F_Supplier_Plant).CompareTo(supFrom) >= 0 && (x.F_Supplier_Cd + '-' + x.F_Supplier_Plant).CompareTo(supTo) <= 0)
                .Select(x => new
                {
                    F_Store_CD = x.F_Store_Code
                }).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(sebango);
                string _jsonData2 = JsonConvert.SerializeObject(prtNo);
                string _jsonData3 = JsonConvert.SerializeObject(storeCD);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @",
                                    ""data2"": " + _jsonData2 + @",
                                    ""data3"": " + _jsonData3 + @"
                                    }";
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<IActionResult> OnKANBANChange([FromBody] string data)
        {
            try
            {
                
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string kbnFrom = _json["kbnFrom"];
                string kbnTo = _json["kbnTo"];

                var prtNo = await _KB3Context.TB_MS_PartOrder
                .Where(x => (x.F_Supplier_Cd + '-' + x.F_Supplier_Plant).CompareTo(supFrom) >= 0 && (x.F_Supplier_Cd + '-' + x.F_Supplier_Plant).CompareTo(supTo) <= 0)
                .Where(x => x.F_Kanban_No.CompareTo(kbnFrom) >= 0 && x.F_Kanban_No.CompareTo(kbnTo) <= 0)
                .Select(x => new
                {
                    Prt_No = x.F_Part_No.Trim() + '-' + x.F_Ruibetsu.Trim()
                }).Distinct().OrderBy(x => x.Prt_No).ToListAsync();

                var storeCD = await _KB3Context.TB_MS_PartOrder.OrderByDescending(x => x.F_Supplier_Cd)
                .Where(x => (x.F_Supplier_Cd + '-' + x.F_Supplier_Plant).CompareTo(supFrom) >= 0 && (x.F_Supplier_Cd + '-' + x.F_Supplier_Plant).CompareTo(supTo) <= 0)
                .Where(x => x.F_Kanban_No.CompareTo(kbnFrom) >= 0 && x.F_Kanban_No.CompareTo(kbnTo) <= 0)
                .Select(x => new
                {
                    F_Store_CD = x.F_Store_Code
                }).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(prtNo);
                string _jsonData2 = JsonConvert.SerializeObject(storeCD);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @",
                                    ""data2"": " + _jsonData2 + @"
                                    }";
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<IActionResult> OnPartChange([FromBody] string data)
        {
            try
            {
                
                string _result = "";
                dynamic _json = JsonConvert.DeserializeObject(data);
                string supFrom = _json["supFrom"];
                string supTo = _json["supTo"];
                string kbnFrom = _json["kbnFrom"];
                string kbnTo = _json["kbnTo"];
                string partFrom = _json["partFrom"];
                string partTo = _json["partTo"];


                var storeCD = await _KB3Context.TB_MS_PartOrder.OrderByDescending(x => x.F_Supplier_Cd)
                .Where(x => (x.F_Supplier_Cd + '-' + x.F_Supplier_Plant).CompareTo(supFrom) >= 0
                && (x.F_Supplier_Cd + '-' + x.F_Supplier_Plant).CompareTo(supTo) <= 0)
                .Where(x => x.F_Kanban_No.CompareTo(kbnFrom) >= 0 && x.F_Kanban_No.CompareTo(kbnTo) <= 0)
                .Where(x => (x.F_Part_No.Trim() + '-' + x.F_Ruibetsu.Trim()).CompareTo(partFrom) >= 0
                && (x.F_Part_No.Trim() + '-' + x.F_Ruibetsu.Trim()).CompareTo(partTo) <= 0)
                .Select(x => new
                {
                    F_Store_CD = x.F_Store_Code
                }).Distinct().ToListAsync();

                string _jsonData = JsonConvert.SerializeObject(storeCD);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @"
                                    }";
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}
