using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System.Globalization;

namespace KANBAN.Controllers.API.ReceiveProcess
{
    public class KBNCR220Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMContext _PPMContext;
        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNCR220Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            PPMContext pPMContext,
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
            _PPMContext = pPMContext;
        }

        public async Task<IActionResult> Initial([FromBody] string data)
        {
            try
            {
                Int32 now = Int32.Parse(DateTime.Now.ToString("yyyyMMdd"));
                string _result = "";
                //first load page
                if (data == null)
                {
                    var _supplierHead = await _KB3Context.TB_REC_HEADER.Select(x => new
                    {
                        x.F_OrderNo,
                        x.F_Delivery_Date,
                        x.F_Supplier_Code,
                        x.F_Supplier_Plant
                    }).Where(x => x.F_OrderNo.StartsWith("7Y") || x.F_OrderNo.StartsWith("7Z"))
                    .OrderBy(x => x.F_Supplier_Code).ToListAsync();

                    var supplierHead = _supplierHead.Where(x => Int32.Parse(x.F_Delivery_Date) == now && now == Int32.Parse(x.F_Delivery_Date))
                        .DistinctBy(x => new { x.F_Supplier_Code, x.F_Supplier_Plant });

                    List<string> supplierList = new();

                    foreach (var sup in supplierHead)
                    {
                        var T_Supplier = await _PPM3Context.T_Supplier_MS.Select(x => new
                        {
                            x.F_supplier_cd,
                            x.F_Plant_cd,
                            x.F_short_name,
                            x.F_TC_Str,
                            x.F_TC_End
                        }).Where(x => x.F_supplier_cd == sup.F_Supplier_Code && x.F_Plant_cd == sup.F_Supplier_Plant)
                        .OrderBy(x => x.F_supplier_cd).ToListAsync(); //Int32.Parse(x.F_TC_Str) <= now && now >= Int32.Parse(x.F_TC_End) &&

                        var singleSupplier = T_Supplier.OrderByDescending(x => x.F_TC_Str).DistinctBy(x => new
                        {
                            x.F_supplier_cd,
                            x.F_Plant_cd,
                            x.F_short_name
                        }).SingleOrDefault();
                        if (Int32.Parse(singleSupplier.F_TC_Str) <= now && Int32.Parse(singleSupplier.F_TC_End) >= now)
                        {
                            var _supplier = singleSupplier.F_supplier_cd + '-' + singleSupplier.F_Plant_cd + " : " + singleSupplier.F_short_name;
                            supplierList.Add(_supplier);
                        }
                    }
                    string _jsonData = JsonConvert.SerializeObject(supplierList);
                    _result = @"{
                                ""status"":""200"",
                                ""response"":""OK"",
                                ""message"": ""Data Found"",
                                ""data"": " + _jsonData + @"}";
                }
                else
                {
                    dynamic _json = JsonConvert.DeserializeObject(data);
                    string type = _json["type"];
                    string strDevDate = _json["devDate"];
                    string devDateRep = strDevDate.Replace("-", string.Empty);
                    string strToDate = _json["toDate"];
                    string toDateRep = strToDate.Replace("-", string.Empty);
                    int fromDate = Int32.Parse(devDateRep);
                    int toDate = Int32.Parse(toDateRep);

                    if (type == "All")
                    {
                        var _supplierHead = await _KB3Context.TB_REC_HEADER.Select(x => new
                        {
                            x.F_OrderNo,
                            x.F_Supplier_Code,
                            x.F_Supplier_Plant,
                            x.F_Delivery_Date
                        }).Where(x => x.F_OrderNo.StartsWith("7Z") || x.F_OrderNo.StartsWith("7Y"))
                            .OrderBy(x => x.F_Supplier_Code).ToListAsync();

                        var supplierHead = _supplierHead.Where(x => Int32.Parse(x.F_Delivery_Date) <= fromDate && Int32.Parse(x.F_Delivery_Date) <= toDate)
                            .DistinctBy(x => new { x.F_Supplier_Code, x.F_Supplier_Plant });

                        List<string> supplierList = new();

                        foreach (var sup in supplierHead)
                        {
                            var T_Supplier = await _PPM3Context.T_Supplier_MS.Select(x => new
                            {
                                x.F_supplier_cd,
                                x.F_Plant_cd,
                                x.F_short_name,
                                x.F_TC_Str,
                                x.F_TC_End
                            }).Where(x => x.F_supplier_cd == sup.F_Supplier_Code && x.F_Plant_cd == sup.F_Supplier_Plant)
                            .OrderBy(x => x.F_supplier_cd).ToListAsync(); //Int32.Parse(x.F_TC_Str) <= now && now >= Int32.Parse(x.F_TC_End) &&

                            var singleSupplier = T_Supplier.OrderByDescending(x => x.F_TC_Str).DistinctBy(x => new
                            {
                                x.F_supplier_cd,
                                x.F_Plant_cd,
                                x.F_short_name
                            }).SingleOrDefault();

                            if (Int32.Parse(singleSupplier.F_TC_Str) <= now && Int32.Parse(singleSupplier.F_TC_End) >= now)
                            {
                                var _supplier = singleSupplier.F_supplier_cd + '-' + singleSupplier.F_Plant_cd + " : " + singleSupplier.F_short_name;
                                supplierList.Add(_supplier);
                            }
                        }
                        string _jsonData = JsonConvert.SerializeObject(supplierList);
                        _result = @"{
                                ""status"":""200"",
                                ""response"":""OK"",
                                ""message"": ""Data Found"",
                                ""data"": " + _jsonData + @"}";
                    }
                    else if (type == "7Y")
                    {
                        var _supplierHead = await _KB3Context.TB_REC_HEADER.Select(x => new
                        {
                            x.F_OrderNo,
                            x.F_Delivery_Date,
                            x.F_Supplier_Code,
                            x.F_Supplier_Plant
                        }).Where(x => x.F_OrderNo.StartsWith("7Y"))
                        .OrderBy(x => x.F_Supplier_Code).ToListAsync();

                        var supplierHead = _supplierHead.Where(x => Int32.Parse(x.F_Delivery_Date) <= fromDate && Int32.Parse(x.F_Delivery_Date) <= toDate)
                            .DistinctBy(x => new { x.F_Supplier_Code, x.F_Supplier_Plant });

                        List<string> supplierList = new();

                        foreach (var sup in supplierHead)
                        {
                            var T_Supplier = await _PPM3Context.T_Supplier_MS.Select(x => new
                            {
                                x.F_supplier_cd,
                                x.F_Plant_cd,
                                x.F_short_name,
                                x.F_TC_Str,
                                x.F_TC_End
                            }).Where(x => x.F_supplier_cd == sup.F_Supplier_Code && x.F_Plant_cd == sup.F_Supplier_Plant)
                            .OrderBy(x => x.F_supplier_cd).ToListAsync();

                            var singleSupplier = T_Supplier.OrderByDescending(x => x.F_TC_Str).DistinctBy(x => new
                            {
                                x.F_supplier_cd,
                                x.F_Plant_cd,
                                x.F_short_name
                            }).SingleOrDefault();

                            if (Int32.Parse(singleSupplier.F_TC_Str) <= now && Int32.Parse(singleSupplier.F_TC_End) >= now)
                            {
                                var _supplier = singleSupplier.F_supplier_cd + '-' + singleSupplier.F_Plant_cd + " : " + singleSupplier.F_short_name;
                                supplierList.Add(_supplier);
                            }
                        }
                        string _jsonData = JsonConvert.SerializeObject(supplierList);
                        _result = @"{
                                ""status"":""200"",
                                ""response"":""OK"",
                                ""message"": ""Data Found"",
                                ""data"": " + _jsonData + @"}";
                    }
                    else
                    {
                        var _supplierHead = await _KB3Context.TB_REC_HEADER.Select(x => new
                        {
                            x.F_OrderNo,
                            x.F_Delivery_Date,
                            x.F_Supplier_Code,
                            x.F_Supplier_Plant
                        }).Where(x => x.F_OrderNo.StartsWith("7Z"))
                        .OrderBy(x => x.F_Supplier_Code).ToListAsync();

                        var supplierHead = _supplierHead.Where(x => Int32.Parse(x.F_Delivery_Date) >= fromDate && Int32.Parse(x.F_Delivery_Date) <= toDate)
                            .DistinctBy(x => new { x.F_Supplier_Code, x.F_Supplier_Plant });

                        List<string> supplierList = new();

                        foreach (var sup in supplierHead)
                        {
                            var T_Supplier = await _PPM3Context.T_Supplier_MS.Select(x => new
                            {
                                x.F_supplier_cd,
                                x.F_Plant_cd,
                                x.F_short_name,
                                x.F_TC_Str,
                                x.F_TC_End
                            }).Where(x => x.F_supplier_cd == sup.F_Supplier_Code && x.F_Plant_cd == sup.F_Supplier_Plant)
                            .OrderBy(x => x.F_supplier_cd).ToListAsync();

                            var singleSupplier = T_Supplier.OrderByDescending(x => x.F_TC_Str).DistinctBy(x => new
                            {
                                x.F_supplier_cd,
                                x.F_Plant_cd,
                                x.F_short_name
                            }).SingleOrDefault();
                            if (Int32.Parse(singleSupplier.F_TC_Str) <= now && Int32.Parse(singleSupplier.F_TC_End) >= now)
                            {
                                var _supplier = singleSupplier.F_supplier_cd + '-' + singleSupplier.F_Plant_cd + " : " + singleSupplier.F_short_name;
                                supplierList.Add(_supplier);
                            }
                        }
                        string _jsonData = JsonConvert.SerializeObject(supplierList);
                        _result = @"{
                                ""status"":""200"",
                                ""response"":""OK"",
                                ""message"": ""Data Found"",
                                ""data"": " + _jsonData + @"}";
                    }
                }
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

    }
}
